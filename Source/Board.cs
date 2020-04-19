using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace DotsAndBoxes
{
    struct BoardDrawables
    {
        // should be set before creating board
        public Color background { get; set; }
        public Color active_line { get; set; }
        public Color inactive_line { get; set; }

        // doesn't matter if these variables are set since they are recalculated automatically
        public int width  { get; set; }
        public int height { get; set; }

        public int padding_wid { get; set; }
        public int padding_hei { get; set; }

        public float cell_wid { get; set; }
        public int pen_wid { get; set; }
    }



    class Board
    {
        // board tools
        private CellGrid grid;
        private Stack<Move> move;

        private Panel panel;
        private TextBox rowcnt_t;
        private TextBox colcnt_t;

        private BoardDrawables drawables;
        private SolidBrush brush;
        private Pen pen;

        private PaintEventHandler panel_paint_h;
        private EventHandler panel_resize_h;


        // public constants
        public const uint min_rowcnt = 1;
     // public const uint def_rowcnt = 3;
        public const uint max_rowcnt = 20;

        public const uint min_colcnt = 1;
     // public const uint def_colcnt = 3;
        public const uint max_colcnt = 20;



        #region Initialization

        // create a board
        public Board(Panel panel, TextBox rowcnt_t, TextBox colcnt_t, BoardDrawables drawables)
        {
            // initializing board components
            this.panel = panel;
            this.rowcnt_t = rowcnt_t;
            this.colcnt_t = colcnt_t;

            // initializing drawables
            panel.BackColor = Color.Transparent;
            this.drawables = drawables;
            brush = new SolidBrush(drawables.background);
            pen = new Pen(drawables.inactive_line);

            // initializing board
            initialize();

            // setting panel event handlers
            panel.Paint += panel_paint_h = new PaintEventHandler(panel_Paint);
            panel.Resize += panel_resize_h = new EventHandler(panel_Resize);
        }

        // reset the board with/without structural changes
        public void initialize()
        {
            // calculate board dimensions from text boxes
            if( !uint.TryParse(rowcnt_t.Text, out uint rowcnt) ) rowcnt = grid.rowcnt;
            if( !uint.TryParse(colcnt_t.Text, out uint colcnt) ) colcnt = grid.colcnt;

            // fit board dimensions to their intended intervals
            rowcnt = Math.Min(rowcnt, max_rowcnt);
            rowcnt = Math.Max(min_rowcnt, rowcnt);

            colcnt = Math.Min(colcnt, max_colcnt);
            colcnt = Math.Max(min_colcnt, colcnt);

            // invalidate text boxes with recalculated board dimensions
            rowcnt_t.Text = rowcnt.ToString();
            colcnt_t.Text = colcnt.ToString();
            rowcnt_t.Invalidate();
            colcnt_t.Invalidate();


            // if the grid is the same size as before, just reset it
            if (grid == null || grid.rowcnt != rowcnt || grid.colcnt != colcnt) grid = new CellGrid(rowcnt, colcnt);
            else grid.reset();

            // if the move list exists just clear it
            if (move == null) move = new Stack<Move>();
            else move.Clear();

            // update drawables and invalidate panel
            updateDrawables();
            panel.Invalidate();
        }

        // destroy the board
        ~Board()
        {
            if (panel != null)
            {
                panel.Paint -= panel_paint_h;
                panel.Resize -= panel_resize_h;
                panel = null;
            }

            if (move != null)
            {
                move.Clear();
                move = null;
            }
        }

        #endregion



        #region Getters and Setters

        public void setRowcnt_t(string rc_t) { rowcnt_t.Text = rc_t; }
        public void setColcnt_t(string cc_t) { colcnt_t.Text = cc_t; }

        public ref CellGrid getGridRef() { return ref grid; }


        // indexer (for convinient access to cells in matrix)
        public Cell this[uint row, uint col]
        {
            get
            {
                return grid[row, col];
            }
        }

        #endregion



        #region Moves

        // return (row, col, wall) of nearest wall from point (x,y)
        public Tuple<uint, uint, uint> getNearestWall(float x, float y)
        {
            float a = drawables.cell_wid;
            x -= drawables.padding_wid;
            y -= drawables.padding_hei;
            uint row = (uint)(y / a);
            uint col = (uint)(x / a);

            if (row >= grid.rowcnt || col >= grid.colcnt)
                return new Tuple<uint, uint, uint>(row, col, Cell.none);   // no such wall could be found


            // unfortunately has to be hardcoded
            float[] dist = new float[Cell.wcnt];   // currently, four

            dist[0] = y - row * a;   // dist from point (x,y) to top wall of cell
            dist[1] = x - col * a;   //                          left
            dist[2] = a - dist[0];   //                          bottom
            dist[3] = a - dist[1];   //                          right

            // finding wall idx with minimal distance from point (x,y)
            uint min = 0;
            for (uint i = 1; i < Cell.wcnt; i++)
                if (dist[i] < dist[min])
                    min = i;

            if (dist[min] <= drawables.cell_wid / 4)   // tolerance for mouse clicks
                return new Tuple<uint, uint, uint>(row, col, Cell.wall[min]);

            return new Tuple<uint, uint, uint>(row, col, Cell.none);   // no near wall found
        }

        // return if the move is successful and the number of cells that the player won
        public Tuple<bool, uint> doMove(Player player, uint row, uint col, uint wall)
        {
            // try the move
            Tuple<bool, uint> t1 = grid.doMove(player, row, col, wall);

            // save the results
            bool is_valid = t1.Item1;
            uint circled_cnt = t1.Item2;

            // if the move is not valid, do nothing
            if (!is_valid)
                return new Tuple<bool, uint>(false, 0);

            // save the move to the move history
            Move m = new Move(player.id, row, col, wall);
            move.Push(m);

            // redraw the panel, return the result
            panel.Invalidate();
            return new Tuple<bool, uint>(true, circled_cnt);
        }

        // return if the undo move is successful (player id who made the move) and the number of cells that the player lost
        public Tuple<int, uint> undoMove()
        {
            // no moves to undo
            if (move.Count == 0)
                return new Tuple<int, uint>(Player.invalid_id, 0);

            // pop the most recent move from the move history
            Move m = move.Pop();

            // undo the move and save the number of cells that the player won
            uint uncircled_cnt = grid.undoMove( m.player_id, m.row, m.col, m.wall);

            // redraw the panel, return the result
            panel.Invalidate();
            return new Tuple<int, uint>(m.player_id, uncircled_cnt);
        }

        // return string representation of move history
        public override string ToString()
        {
            string s = "";
            foreach (Move m in move)
            {
                s = m.ToString() + (s == "" ? "" : "\n") + s;    
            }
            return "" + grid.rowcnt + " " + grid.colcnt + "\n" + s;
        }
        
        #endregion



        #region Drawing

        // draw everything on board
        public void draw(Graphics g)
        {
            // copy draw context
            Color active_line, inactive_line;
            int width, height, offset_x, offset_y, pen_wid;
            float cell_wid;

            lock( this )
            {
                // copy draw context to stack variables
                active_line   = drawables.active_line;
                inactive_line = drawables.inactive_line;

                width  = drawables.width;
                height = drawables.height;

                offset_x = drawables.padding_wid;
                offset_y = drawables.padding_hei;

                cell_wid = drawables.cell_wid;
                pen_wid  = drawables.pen_wid;
            }

            // drawing panel background color
            g.FillRectangle(brush, 0, 0, drawables.width, drawables.height);

            // drawing player amblems
            for (uint i = 0; i < grid.rowcnt; i++)
                for (uint j = 0; j < grid.colcnt; j++)
                    grid[i, j].drawAmblem(g, offset_x + j * cell_wid, offset_y + i * cell_wid, cell_wid, pen_wid);

            // drawing cell grid
            pen.Color = inactive_line;
            pen.Width = pen_wid;
            grid.draw(g, pen, offset_x, offset_y, cell_wid);

            // drawing active walls on top of cell grid
            pen.Color = active_line;
            for (uint i = 0; i < grid.rowcnt; i++)
                for (uint j = 0; j < grid.colcnt; j++)
                    grid[i, j].drawActiveWalls(g, pen, offset_x + j * cell_wid, offset_y + i * cell_wid, cell_wid);

        }

        // update drawables in accordance to panel size
        private void updateDrawables()
        {
            lock( this )
            {
                // copying panel width and height
                drawables.width  = panel.ClientRectangle.Width;
                drawables.height = panel.ClientRectangle.Height;

                // calculating cell width for current panel size
                drawables.cell_wid = Math.Min( drawables.width *95/100 / grid.colcnt,
                                               drawables.height*95/100 / grid.rowcnt );

                // calculating padding for current panel size
                drawables.padding_wid = (drawables.width  - (int) (grid.colcnt * drawables.cell_wid))/2;
                drawables.padding_hei = (drawables.height - (int) (grid.rowcnt * drawables.cell_wid))/2;

                // calculating pen width for current panel size
                drawables.pen_wid  = Math.Min( drawables.width  / 100,
                                               drawables.height / 100 );
            }
        }

        #endregion



        #region Panel Event Handlers

        // handle panel paint event
        private void panel_Paint(object sender, PaintEventArgs e)
        {
            draw(e.Graphics);
        }

        // handle panel resize event
        private void panel_Resize(object sender, EventArgs e)
        {
            updateDrawables();
            panel.Invalidate();
        }

        #endregion





    }
}
