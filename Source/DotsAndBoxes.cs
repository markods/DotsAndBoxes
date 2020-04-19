using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace DotsAndBoxes
{
    public partial class DotsAndBoxes : Form
    {
        private Board board;
        private Player[] player;

        private const int player_cnt = 2;
        private int curr_player_id = 0;

        // visualization
        public static uint show = 0;
        public uint visualizationDepth = 2;

        private Timer timer = new Timer();   // timer for waking up AI player

        // stack for remembering consecutive moves played during AI analysis
        Stack<Move> consecutivemoves = new Stack<Move>();


        // public constants
        public const uint skip2           = (1 << 0);
        public const uint infinitywins    = (1 << 1);
        public const uint predict3        = (1 << 2);
        public const uint skipunsafe      = (1 << 3);   // TODO: apply skipunsafe only when grid.getSafecnt() > 0
        public const uint skipunsensible  = (1 << 4);
        public const uint takeconsecutive = (1 << 5);
        public const uint noattr          = 0;

        public const uint show_game_info            = 1 << 0;
        public const uint show_path_numbers         = 1 << 1;
        public const uint show_cell_coordinates     = 1 << 2;
        public const uint show_ai_moves             = 1 << 3;
        public const uint show_almost_circled_cells = 1 << 4;



        #region Initialization

        // create a windows form and initialize game
        public DotsAndBoxes()
        {
            InitializeComponent();
            initializeGame();

            timer.Enabled = false;
            timer.Interval = 500; // AI wakeup time in ms
            timer.Tick += new EventHandler(PlayAI);
        }

        // initialize game stuff
        private void initializeGame()
        {
            // create board
            BoardDrawables board_dc = new BoardDrawables
            {
                active_line   = Color.Black,
                inactive_line = Color.LightGray,
                background    = Color.LemonChiffon
            };

            board = new Board(board_p, rowcnt_t, colcnt_t, board_dc);
            player = new Player[player_cnt];


            // create player 1
            PlayerDrawables player1_dc = new PlayerDrawables
            {
                player_p = player1_p,
                name_t   = player1_name_t,

                cells_l    = player1_cells_l,
                cell_cnt_l = player1_cell_cnt_l,

                score_l     = player1_score_l,
                score_cnt_l = player1_score_cnt_l,

                amblem = new AmblemX(Color.PowderBlue, Color.Black),

                active_frgnd = Color.PowderBlue,
                active_bkgnd = Color.RoyalBlue
            };
            player[0] = new Player(0, player1_dc);


            // create player 2
            PlayerDrawables player2_dc = new PlayerDrawables
            {
                player_p = player2_p,
                name_t   = player2_name_t,

                cells_l    = player2_cells_l,
                cell_cnt_l = player2_cell_cnt_l,

                score_l     = player2_score_l,
                score_cnt_l = player2_score_cnt_l,

                amblem = new AmblemO(Color.Orange, Color.Black),

                active_frgnd = Color.Orange,
                active_bkgnd = Color.Coral
            };
            player[1] = new Player(1, player2_dc);


            // set player active statuses
            player[0].setActiveStatus(true);
            player[1].setActiveStatus(false);
        }

        #endregion



        #region Players

        public int nextPlayerId(int curr_player)
        {
            return (curr_player + 1) % player_cnt;
        }

        private bool isCurrPlayerAI()
        {
            if (curr_player_id == 0 && 2 <= player1_name_t.Text.Length && player1_name_t.Text.Substring(0, 2) == "AI") return true;
            if (curr_player_id == 1 && 2 <= player2_name_t.Text.Length && player2_name_t.Text.Substring(0, 2) == "AI") return true;
            return false;
        }

        private char getCurrPlayerAILevel()
        {
            String playername;
            if (curr_player_id == 0)
                playername = player1_name_t.Text;
            else // if (curr_player_id == 1)
                playername = player2_name_t.Text;

            if (   playername[0] == 'A' && playername[1] == 'I' 
                && 3 <= playername.Length 
                && (playername[2] == '1' || playername[2] == '2' || playername[2] == '3'))
                return (playername[2]);

            return ' ';
        }

        private uint getCurrPlayerAIDepth()
        {
            string playername;
            if (curr_player_id == 0)
                playername = player1_name_t.Text;
            else // if (curr_player_id == 1)
                playername = player2_name_t.Text;

            if (     playername[0] == 'A' && playername[1] == 'I'
                  && 5 <= playername.Length 
                  && !('0' <= playername[3] && playername[3] <= '9')
                  && Int32.TryParse( playername.Substring(4), out int AIDepth ) 
                  && AIDepth >= 0 )
                return (uint) AIDepth;

            return 6; // default AIDepth
        }

        // return id of winning player (invalid id if there is no such player)
        private int getWinnerId()
        {
            CellGrid grid = board.getGridRef();

            // if the board still has uncircled cells, do nothing
            if( grid.getCircledCnt() != grid.getCellCnt() )
                return Player.invalid_id;

            int id_best = 0;      // id of winning player(s)
            uint ccc_best = 0;     // circled cell count of winning player(s)
            uint winner_cnt = 0;   // number of winning players, should be one if there is a clear winner

            // for all players
            for( uint id = 0, ccc; id < player_cnt; id++ )
            {
                // save current player circle cell count
                ccc = player[id].getCircledCellCnt();

                // if current player beats previous players
                if( ccc_best < ccc )
                {
                    // update best player variable, and reset the winning players counter
                    id_best = (int) id;
                    ccc_best = ccc;
                    winner_cnt = 1;
                }
                else if( ccc_best == ccc )
                {
                    // if the current player has the same circled cell count as the best player, increase the winning players counter
                    winner_cnt++;
                }
            }

            // if there are more winning players, then the game is a tie
            if( winner_cnt > 1 )
                id_best = Player.invalid_id;

            // return winning player id
            return id_best;
        }

        #endregion



        #region Moves

        // start new game
        private void newGame()
        {
            for( uint i = 0; i < player_cnt; i++ )
                player[i].rstCircledCellCnt();

            board.initialize();
            consecutivemoves.Clear();
            curr_player_id = 1; // begin with the player 1, since doTurn() follows

            // initiate timer to wake up AI player
            doTurn( true ); // do wakeupAI
        }

        // reset player scores
        private void resetScore()
        {
            for( uint i = 0; i < player_cnt; i++ )
                player[i].rstScore();
        }

        // do move given the x and y coordinates of mouse click in reference to board_p origin point (top left point)
        private bool doGameMoveXY(float x, float y)
        {
            // try doing the move
            Tuple<uint, uint, uint> t1 = board.getNearestWall(x, y);
            Tuple<bool, uint> m = doGameMove(t1.Item1, t1.Item2, t1.Item3, true);  // do wakeupAI
            return m.Item1;
        }

        // do move given the row, col and wall values; return if the move is sucessful and the number of encircled cells
        public Tuple<bool, uint> doGameMove(uint row, uint col, uint wall, bool wakeupAI )
        {
            // try doing the move
            Tuple<bool, uint> t1 = board.doMove(player[curr_player_id], row, col, wall);

            // results
            bool is_successful_move = t1.Item1;
            uint circled_cnt = t1.Item2;

            // if the move is not successful, do nothing
            if (!is_successful_move) 
                return new Tuple<bool, uint>(false, 0);

            // increase player cell count
            player[curr_player_id].incCircledCellCnt(circled_cnt);

            // increase player score count if there are no uncircled cells left
            int id = getWinnerId();
            if (id != Player.invalid_id)
                player[id].incScore(1);

            // give turn if the player has not encircled any cell with their move
            if (circled_cnt == 0) 
               doTurn( wakeupAI );

            // the move is successful
            return new Tuple<bool, uint>(true, circled_cnt);
        }

        // undo most recent move, return if successful
        public bool undoGameMove()
        {
            // ckeck if player score should be decreased (if the board has no uncircled cells)
            int id = getWinnerId();

            // try undoing the most recent move
            Tuple<int, uint> t1 = board.undoMove();

            // results
            int prev_player_id = t1.Item1;
            uint uncircled_cnt = t1.Item2;

            // if the player id is not a valid player (meaning that there are no moves to undo), do nothing
            if( prev_player_id == Player.invalid_id ) return false;

            // decrease player cell count
            player[curr_player_id].decCircledCellCnt(uncircled_cnt);
            undoTurn(prev_player_id);

            // decreasing score
            if( id != Player.invalid_id )
                player[id].decScore(1);

            // the undo is successful
            return true;
        }


        // give turn to the next player
        private void doTurn( bool wakeupAI )
        {
            player[curr_player_id].setActiveStatus(false);
            curr_player_id = (curr_player_id+1) % player_cnt;
            player[curr_player_id].setActiveStatus(true);

            // initiate timer to wake up AI player
            if (wakeupAI && isCurrPlayerAI())
                timer.Start();
        }

        // undo turn of current player and allow previous player to redo their move
        private void undoTurn(int prev_player_id)
        {
            timer.Stop();

            if( prev_player_id == Player.invalid_id ) return;

            player[curr_player_id].setActiveStatus(false);
            curr_player_id = prev_player_id;
            player[curr_player_id].setActiveStatus(true);
        }

        #endregion



        #region AI

        // calculate AI player next move (called by a AI player wakeup timer)
        private void PlayAI(Object myObject, EventArgs myEventArgs)
        {
            timer.Stop();
            Random rnd = new Random();  //TODO: maybe should be moved outside PlayAI?

            uint w = Cell.none;
            uint r = 0;
            uint c = 0;
            CellGrid grid = board.getGridRef();

            eraseLabels();

            Tuple<bool, uint> m;    // move result
            bool m_success = false; // was a move successful
            uint m_circled_cnt = 0; // has a move circled some boxes

            char AIlevel = getCurrPlayerAILevel();
            if (AIlevel != ' ' && AIlevel != '1' && AIlevel != '2' && AIlevel != '3')  // wrong AI level would cause a dead-loop
                return;

            uint AIdepth = getCurrPlayerAIDepth();
         // if (AIdepth < 1 || AIdepth > 20)  // wrong AI depth would cause problems
         //     return;

            // if there is any wall left to add
            while (!m_success && grid.getOpenCnt() > 0)
            {
                // allways take the 3-wall cell
                if ((AIlevel == '1' || AIlevel == '2' || AIlevel == '3') && w == Cell.none)
                {
                    // search for the first cell (from top left corner of the grid) having 3 walls (ready to circle)
                    uint pathlen = 0;
                    bool pathclosed = false;
                    for (uint row = 0; row < grid.rowcnt && w == Cell.none; row++)
                        for (uint col = 0; col < grid.colcnt && w == Cell.none; col++)
                            if (board[row, col].getWallCnt() == 3) 
                            {
                                if (AIlevel == '3' && grid.getSafeMoveCnt() == 0)
                                {
                                    // immediately (without thinking) take all 3-walled cells only 
                                    // if they are alone in path, or in 2-cell long closed path
                                    // or if they belong to 5-cells or longer closed path, or 3-cells or longer open path
                                    pathlen = grid.getPathlen( grid[row, col].getPathnum() );
                                    pathclosed = grid.isPathClosed( grid[row, col].getPathnum() );
                                    if ( (pathclosed ? 2 : 1) < pathlen && pathlen < (pathclosed ? 5 : 3))
                                        continue;
                                }
                                r = row;
                                c = col;
                                w = grid[r, c].getFreeWall();
                            }
                }

                // default AI level is ' '
                if (AIlevel == ' ' && w == Cell.none)
                {
                    // choose the first free wall, starting from the top left corner of the grid
                    do
                    {
                        w = board[r, c].getFreeWall();
                        if (w != Cell.none)
                            break;

                        if (++c >= grid.colcnt)
                        {
                            c = 0;
                            if (++r >= grid.rowcnt)
                                break;
                        }
                    }
                    while (w == Cell.none);
                }

                if (AIlevel == '2' && w == Cell.none)
                {
                    // choose a wall based on minimax algorithm
                    Tuple<Move, int> res = minimax(ref grid, AIdepth, skip2);
                    if (res.Item1 != null)
                    {
                        r = res.Item1.row;
                        c = res.Item1.col;
                        w = res.Item1.wall;
                    }
                }

                if (AIlevel == '3' && w == Cell.none)
                {
                 // if (grid.getSafeMoveCnt() <= AI3depth)
                 // if (grid.getMoves2Play() <= AI3depth + 1 || grid.getSafeMoveCnt() < AI3depth/2 )
                    if (grid.getMoves2Play() <= AIdepth + 1 || grid.getSafeMoveCnt() == 0)
                    {
                        // choose a wall based on minimax algorithm
                        Tuple<Move, int> res = minimax(ref grid, AIdepth, /*infinitywins | */ /* predict3 | */ skipunsensible | takeconsecutive );
                        if (res.Item1 != null)
                        {
                            r = res.Item1.row;
                            c = res.Item1.col;
                            w = res.Item1.wall;
                        }
                    }
                }

                if ((AIlevel == '1' || AIlevel == '2' || AIlevel == '3') && w == Cell.none)
                {
                    // choose a safe move
                    uint sm = grid.getSafeMoveCnt();
                    if (sm > 0)
                    {
                        // choose which occurance of a safe move will be played
                        int smo = rnd.Next(0, (int)sm);

                        // search for the chosen occurance of a safe move
                        w = Cell.none;
                        for (uint row = 0; row < grid.rowcnt && w == Cell.none; row++)
                            for (uint col = 0; col < grid.colcnt && w == Cell.none; col++)
                                for (int i = 0; i < Cell.wcnt && w == Cell.none; i++)
                                    if (grid.isMoveSafe(row, col, Cell.wall[i]))
                                    {
                                        if (smo == 0)
                                        {
                                            r = row;
                                            c = col;
                                            w = Cell.wall[i];
                                        }
                                        else
                                           smo--;
                                   }
                    }
                    else
                    {
                        // TODO: choose a wall which gives the shortest path to the opponent!!!!
                        //...

                        // choose a wall randomly, without checking if it is free or not
                        r = (uint)rnd.Next(0, (int)grid.rowcnt);
                        c = (uint)rnd.Next(0, (int)grid.colcnt);
                        w = (uint)(1 << rnd.Next(1, (int)(Cell.wcnt + 1)));
                    }
                }

                // make a move (add chosen wall)
                if (w != Cell.none)
                {
                    m = doGameMove(r, c, w, true); // wakeupAI
                    m_success = m.Item1;
                    m_circled_cnt = m.Item2;

                    w = Cell.none; // allow the next move
                }
            }

            // same AI plays again if his last move closed a box
            if (m_circled_cnt > 0)
                timer.Start();
        }

        // iterate through available moves on given grid
        private bool iterateMove(ref CellGrid grid, ref Move m, uint attr, uint minPathlen)
        {
            // search for the next sensible move starting from the last move
            do
            {
                // get the next wall of the cell, in the following order:  top -> lef -> bot -> rig
                // initially m.wall will result in Cell.none (because it has initial value Cell.none)
                do
                {
                    m.wall = ((m.wall << 1) & Cell.all);
                }
                while ( // (attr & skipunsensible) == 0 && // if unsensible should be skipped, check all walls of a cell
                         (   (m.wall == Cell.bot && m.row != grid.rowcnt - 1)    // skip bot wall except if the cell is in the bottom row
                          || (m.wall == Cell.rig && m.col != grid.colcnt - 1))); // skip rig wall except if the cell is in the right column

                // if all walls of the cell were exausted, move to the next cell
                if (m.wall == Cell.none)
                {
                    do
                    {
                        // move to the next cell
                        if (++m.col >= grid.colcnt)  // m.col will initially be 0 (because it has initial value uint.MaxValue)
                        {
                            m.col = 0;
                            if (++m.row >= grid.rowcnt)
                            {
                                // m.row = 1000;  //TEST

                                m = null; // there are no moves left to play
                                return false;
                            }
                        }
                    }
                    while (grid[m.row, m.col].getWallCnt() == 4   // skip cells having all walls
                       || ((attr & skip2) == skip2 && grid[m.row, m.col].getWallCnt() == 2));  // if required, skip cells having two walls

                    // the first wall of the cell to check is at the top
                    m.wall = Cell.top;
                }
            }
            while (grid[m.row, m.col].hasWall(m.wall) // repeat the search until the chosen wall does not exist in the chosen cell, then try that move
                || ((attr & skip2) == skip2   // if cells already having two walls should be skipped, do so
                    && ((m.wall == Cell.lef && m.col > 0 && grid[m.row, m.col - 1].getWallCnt() == 2)
                     || (m.wall == Cell.rig && m.col < grid.colcnt - 1 && grid[m.row, m.col + 1].getWallCnt() == 2)
                     || (m.wall == Cell.top && m.row > 0 && grid[m.row - 1, m.col].getWallCnt() == 2)
                     || (m.wall == Cell.bot && m.row < grid.rowcnt - 1 && grid[m.row + 1, m.col].getWallCnt() == 2)))
                || ((attr & skipunsafe) == skipunsafe && !grid.isMoveSafe(m.row, m.col, m.wall)) 
                || ((attr & skipunsensible) == skipunsensible && !grid.isMoveSensible(m.row, m.col, m.wall, minPathlen)) );
            return true;
        }

        // HACK: makes the application wait for given number of miliseconds
        public void Wait(int ms)
        {
            DateTime start = DateTime.Now;
            while ((DateTime.Now - start).TotalMilliseconds < ms)
                Application.DoEvents();
        }

        // calculate the best move for the current player and its win ratio (the first player is always the maximising player)
        private Tuple<Move, int> minimax(ref CellGrid grid, uint max_depth, uint attr)
        {
            return _minimax(curr_player_id, curr_player_id, ref grid, max_depth, 0, int.MinValue, int.MaxValue, attr);
        }

        // minimax helper function
        private Tuple<Move, int> _minimax(int root_player_id, int player_id, ref CellGrid grid, uint max_depth, uint depth, int alpha, int beta, uint attr)
        {
            // allow user actions while AI playing
            if( depth <= 2 )
                Application.DoEvents();

            // best move found so far
            Move best_move = null;

            // is any move possible?
            bool moved = false;

            int consecutivemoves_cnt = 0;

            // if the max depth of minimax tree hasn't been reached, and
            // if there are some possible moves
            // try to play a move
            if (depth < max_depth && grid.getOpenCnt() > 0)
            {

                // initiate iteration through all valid moves
                //    - m.wall has initial value Cell.none, so increasing it (during the first iteration) will start from Cell.top
                //    - m.col has initial value uint.MaxValue, so increasing it (during the first iteration) will give 0
                Move m = new Move(player_id, 0, uint.MaxValue, Cell.none);

                // win ratio of the current move
                int ratio = 0;

                // optimization
                // uint minPathlen = grid.getMinPathlen();
                   uint minPathlen = 0;  // optimizations are not used
                // grid.markRepresentativePaths(); 
                // findSensibleMoves();

                // try every valid sensible move
                while (iterateMove(ref grid, ref m, attr, minPathlen) )
                {
                    moved = true;

                    // the first move is assumed to be the best one
                    if (depth == 0 && best_move == null)
                        best_move = new Move(m.player_id, m.row, m.col, m.wall);

                    if( (show & DotsAndBoxes.show_ai_moves ) != 0 )
                    {
                        if (depth == 0)
                        {
                            setLabel(m.row, m.col, m.wall, "[?]");
                            // Wait(10);
                         // this.Invalidate(true);
                            this.Refresh();
                        }
                        else if (depth < visualizationDepth)
                        {
                            setTempLabel(m.row, m.col, m.wall, grid[m.row, m.col].lbl[Cell.getWallpos(m.wall)] + " d" + depth + "/" + max_depth);
                         // this.Invalidate(true);
                            this.Refresh();
                        }
                    }

                    // do the move 
                    bool lastmove_is_valid = false;
                    uint lastmove_circled_cnt = 0;
                    Tuple<bool, uint> t1 = grid.doMove(player[m.player_id], m.row, m.col, m.wall);
                    lastmove_is_valid = t1.Item1;
                    lastmove_circled_cnt = t1.Item2;

                    // automatically take consecutive cells from the same path, until the path becomes 
                    // 4-cells long (if the path is closed on the other end) 
                    // or 2-cells long (if the path is opened on the other end)
                    int ap = 0;
                    if ((attr & takeconsecutive) == takeconsecutive)
                    {
                        uint ar = grid.rowcnt;
                        uint ac = grid.colcnt;
                        while (lastmove_circled_cnt == 1)
                        {
                            ar = grid.getAdjRow(m.row, m.wall);
                            ac = grid.getAdjCol(m.col, m.wall);
                            if (ar == m.row && ac == m.col) 
                                break;  // adjacent cell does not exist
                            if (grid[ar, ac].getWallCnt() != 3)
                                break;  // adjacent cell does not have 3 walls (it is not ready to be taken by the same player)

                            ap = grid[ar, ac].getPathnum();  // path number of adjacent cell
                            if (   grid.isPathClosed( ap ) 
                                && grid.getPathlen(ap) <= 4)
                                break;  // adjacent cell belongs to a too short closed path
                            if (grid.getPathlen(ap) <= 2)
                                break;  // adjacent cell belongs to a too short path

                            consecutivemoves.Push(m);  // remember the previous move, so it can be undone
                            consecutivemoves_cnt++;

                            // m.row = ar;
                            // m.col = ac;
                            // m.wall = grid[ar, ac].getFreeWall();
                            m = new Move(player_id, ar, ac, grid[ar, ac].getFreeWall());

                            t1 = grid.doMove(player[m.player_id], m.row, m.col, m.wall );
                            lastmove_is_valid = t1.Item1;
                            lastmove_circled_cnt = t1.Item2;
                        }
                    }

                    ////////////////////////////////////////////////////

                    if ((show & DotsAndBoxes.show_ai_moves) != 0)
                    {
                        if (depth < visualizationDepth)
                        {
                            // this.Invalidate(true);
                            this.Refresh();
                        }
                    }

                    // who plays the next move?
                    int next_player_id = (lastmove_circled_cnt > 0 ? m.player_id : nextPlayerId(m.player_id));

                    // calculate the win ratio for the next player
                    Tuple<Move, int> res = _minimax(root_player_id, next_player_id, ref grid, max_depth, depth + 1, alpha, beta, attr);
                    ratio = res.Item2;

                    // maximising player turn - try to increase alpha as much as possible (but it has to be less than beta)
                    bool better_move_found = false;
                    if (m.player_id == root_player_id)
                    {
                        if (ratio > alpha)
                        {
                            alpha = ratio;
                            if (depth == 0)
                            {
                                better_move_found = true;

                                best_move.player_id = m.player_id;
                                best_move.row = m.row;
                                best_move.col = m.col;
                                best_move.wall = m.wall;
                            }
                        }
                    }
                    // minimising player turn - try to decrease beta as much as possible (but it has to be bigger than alpha)
                    else
                    {
                        if (ratio < beta)
                        {
                            beta = ratio;
                            if (depth == 0)
                            {
                                better_move_found = true;

                                best_move.player_id = m.player_id;
                                best_move.row = m.row;
                                best_move.col = m.col;
                                best_move.wall = m.wall;
                            }
                        }
                    }

                    grid.undoMove(m.player_id, m.row, m.col, m.wall);

                    // undo all consecutive moves
                    if ((attr & takeconsecutive) == takeconsecutive)
                    {
                        while (consecutivemoves_cnt > 0)
                        {
                            consecutivemoves_cnt--;
                            m = consecutivemoves.Pop();
                            grid.undoMove(m.player_id, m.row, m.col, m.wall);
                        }
                    }

                    if ((show & DotsAndBoxes.show_ai_moves) != 0)
                    {
                        if (depth == 0)
                        {
                            string str = "";

                            if (!better_move_found)
                                str += "≤";

                            if (alpha == int.MaxValue)
                                str += "+∞";
                            else if (alpha == int.MinValue)
                                str += "-∞";
                            else
                                str += alpha;
                         // str += ",";
                         // if (beta == int.MaxValue)
                         //     str += "+∞";
                         // else if (beta == int.MinValue)
                         //     str += "-∞";
                         // else
                         //     str += "" + beta;

                            setLabel(m.row, m.col, m.wall, str);
                         // this.Invalidate(true);
                            this.Refresh();

                            str = null;
                        }
                        else if (depth < visualizationDepth)
                        {
                            setTempLabel(m.row, m.col, m.wall, "");
                         // this.Invalidate(true);
                            this.Refresh();
                        }
                    }

                    // this interval should not be inverted if the move is possible (given that both players play optimally)
                    // (with information obtained in preorder fashion from minimax tree)
                 // if (alpha == beta)
                 // {
                 //     best_move.player_id = Player.invalid_id;
                 //     best_move.row = 0;
                 //     best_move.col = 0;
                 //     best_move.wall = Cell.none;
                 // }
                    if (alpha >= beta)
                    {
                        // alpha = int.MinValue;
                        // beta = int.MaxValue;
                        break;
                    }
                }
            }

            // if no move could be played 
            // then evaluate current grid state and return its win ratio
            if ( ! moved )
                return new Tuple<Move, int>(null, evalGridState(ref grid, player_id, root_player_id, attr));

            // return the best move and its best ratio found for the current player
            return new Tuple<Move, int>(best_move, ((player_id == root_player_id) ? alpha : beta) );
        }

        // evaluate current cell grid state, and return player win ratio (integer between -inf and +inf)
        // always evaluate in favour of the root player
        private int evalGridState(ref CellGrid grid, int player_id, int root_player_id, uint attr)
        {
            // return 0;   // TEST

            // Wait(200);

            int benefit = 0;
         
            if ((attr & infinitywins) == infinitywins)
            {
                // if the game is finished
                if (grid.getOpenCnt() == 0)
                {
                    // return 0 in case of draw
                    if (grid.getWinnerID() == Player.invalid_id)
                        return 0;

                    // return max or min, depending on who would win
                    return (grid.getWinnerID() == root_player_id ? int.MaxValue : int.MinValue);
                }
            }

            // take into account the difference in number of circled cells of the current player and the opponent
            benefit += (int)grid.getCircledCnt4Player(root_player_id) - (int)grid.getCircledCnt4Player(nextPlayerId(root_player_id));

         // // take into account the number of cells with 3 walls
         // //   add if the player is root_player_id, subtract otherwise
         // int wall3cnt = 0;
         // for (uint r = 0; r < grid.rowcnt; r++)
         //     for (uint c = 0; c < grid.colcnt; c++)
         //         if (grid[r, c].getWallCnt() == 3)
         //             wall3cnt++;
         // benefit += (player_id == root_player_id ? wall3cnt : -wall3cnt);

         if( (attr & predict3) == predict3 )
         // benefit += (player_id == root_player_id ? (int) grid.getCellcntHavingWallcnt(3) : - (int) grid.getCellcntHavingWallcnt(3));
            benefit += (player_id == root_player_id ? (int)grid.getCellcntHavingWallcnt(3) : -(int)grid.getCellcntHavingWallcnt(3));

            // return the calculated mark of the GridState
            return benefit;


            // TODO: depending on who is the next player from the current grid state
            //   - subtract/add the number of cells with 0/1 walls (subtract if the next player is root_player_id, add otherwise)
        }

        #endregion



        #region Visualization
        // TODO: fix this entire region (temporary interface hack)

        // erase wall labels in all cells
        private void eraseLabels()
        {
            CellGrid grid = board.getGridRef();
            for( uint r = 0; r < grid.rowcnt; r++ )
                for( uint c = 0; c < grid.colcnt; c++ )
                    for( int l = 0; l < Cell.wcnt; l++ )
                    {
                        board[r, c].lbl[l]     = "";
                        board[r, c].lbltemp[l] = "";
                    }
        }

        private void setLabel(uint row, uint col, uint wall, string str)
        {
            if( wall == Cell.lef ) { board[row, col].lbl[Cell.lefpos] = str; return; }
            if( wall == Cell.rig ) { board[row, col].lbl[Cell.rigpos] = str; return; }
            if( wall == Cell.top ) { board[row, col].lbl[Cell.toppos] = str; return; }
            if( wall == Cell.bot ) { board[row, col].lbl[Cell.botpos] = str; return; }
        }

        private void setTempLabel(uint row, uint col, uint wall, string str)
        {
            if( wall == Cell.lef ) { board[row, col].lbltemp[Cell.lefpos] = str; return; }
            if( wall == Cell.rig ) { board[row, col].lbltemp[Cell.rigpos] = str; return; }
            if( wall == Cell.top ) { board[row, col].lbltemp[Cell.toppos] = str; return; }
            if( wall == Cell.bot ) { board[row, col].lbltemp[Cell.botpos] = str; return; }
        }

        #endregion



        #region Event Handlers

        // handle 'do move' request
        private void board_p_MouseClick(object sender, MouseEventArgs e)
        {
            if( ! isCurrPlayerAI() )  // playing using mouse is allowed only if the current player is a human
                doGameMoveXY(e.X, e.Y);
        }

        // handle 'undo move' request
        private void undo_b_Click(object sender, EventArgs e)
        {
            undoGameMove();
            eraseLabels();

        }

        // handle 'new game' request
        private void new_game_b_Click(object sender, EventArgs e)
        {
            newGame();
        }

        // handle 'reset score' request
        private void reset_score_b_Click(object sender, EventArgs e)
        {
            resetScore();
        }

        // handle 'import' request
        private void menu_file_import_Click(object sender, EventArgs e)
        {
            timer.Stop();

            // allow user to choose file
            OpenFileDialog openFileDialog = new OpenFileDialog  
	        {  
	         // InitialDirectory = @"D:\",  
	            Title = "Choose a board file to import",  
	            CheckFileExists = true,  
	            CheckPathExists = true,  
	            DefaultExt = "txt",  
	            Filter = "txt files (*.txt)|*.txt",  
	            FilterIndex = 2,  
	            RestoreDirectory = true,  
	            ReadOnlyChecked = true,  
	            ShowReadOnly = true  
	         };
            if (openFileDialog.ShowDialog() != DialogResult.OK)  // if user hasn't choosen the file, abort importing
                return;

         // string filepath = "board.txt";
            string filepath = openFileDialog.FileName; // the chosen file
            if (!System.IO.File.Exists(filepath))
                return;


            using (System.IO.StreamReader sr = System.IO.File.OpenText(filepath))
            {
                // first line must exist
                string ln = "";
                if ( (ln = sr.ReadLine()) == null)
                    return;

                // number of rows is given in the left part of the first line, number of columns in the right part 
                int i = 0;
                while (i < ln.Length && ln[i] != ' ')
                    i++;
                if (i < ln.Length && ln[i] == ' ')
                {
                    board.setRowcnt_t (ln.Substring(0, i));
                    board.setColcnt_t (ln.Substring(i + 1, ln.Length - (i+1)));
                }

                newGame();
                
                CellGrid grid = board.getGridRef();

                // other lines contain coordinates of a line chosen by a player
                while ((ln = sr.ReadLine()) != null)
                {
                    if (ln.Length < 2)
                        continue;
               
                    // line format is "Xn"
                    if ('A' <= ln[0] && ln[0] <= 'Z')
                    {
                        uint r = (uint) ln[0] - (uint) 'A';
                        if( !uint.TryParse(ln.Substring(1, ln.Length - 1), out uint c) )
                            continue;  // ignore errors

                        if (c < grid.colcnt )
                            doGameMove(r, c, Cell.lef, false); // do not wakeupAI
                        else
                            doGameMove(r, c-1, Cell.rig, false); // do not wakeupAI
                    }
                    // line format is "nX"
                    else if ('A' <= ln[ln.Length-1] && ln[ln.Length-1] <= 'Z')
                    {
                        uint c = (uint)ln[ln.Length-1] - (uint)'A';
                        if( !uint.TryParse(ln.Substring(0, ln.Length - 1), out uint r) )
                            continue;  // ignore errors

                        if (r < grid.rowcnt)
                            doGameMove(r, c, Cell.top, false); // do not wakeupAI
                        else
                            doGameMove(r - 1, c, Cell.bot, false); // do not wakeupAI
                    }
                    else
                    {
                        continue; // ignore errors
                    }
                }
            }
        }

        // handle 'export' request
        private void menu_file_export_Click(object sender, EventArgs e)
        {

            // allow user to choose file
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                // InitialDirectory = @"D:\",  
                Title = "Choose a board file to export",
             // CheckFileExists = true,
                CheckPathExists = true,
                DefaultExt = "txt",
                Filter = "txt files (*.txt)|*.txt",
                FilterIndex = 2,
                RestoreDirectory = true,
            };
            if (saveFileDialog.ShowDialog() != DialogResult.OK)  // if user hasn't chosen the file, abort exporting
                return;

         // string filepath = "board.txt";
            string filepath = saveFileDialog.FileName; // the chosen file

            using (System.IO.StreamWriter sw = System.IO.File.CreateText(filepath))
            {
                string s = board.ToString();
                sw.Write(s);
            }
        }

        // handle 'show game info' request
        private void game_info_mi_CheckStateChanged(object sender, EventArgs e)
        {
            show &= ~DotsAndBoxes.show_game_info;
            if ( game_info_mi.Checked )
                 show |= show_game_info;
            this.Refresh();
        }

        // handle 'show path numbers' request
        private void path_numbers_mi_CheckStateChanged(object sender, EventArgs e)
        {
            show &= ~DotsAndBoxes.show_path_numbers;
            if (path_numbers_mi.Checked)
                show |= show_path_numbers;
            this.Refresh();
        }

        // handle 'show cell coordinates' request
        private void cell_coordinates_mi_CheckStateChanged(object sender, EventArgs e)
        {
            show &= ~DotsAndBoxes.show_cell_coordinates;
            if (cell_coordinates_mi.Checked)
                show |= show_cell_coordinates;
            this.Refresh();
        }

        // handle 'show ai moves' request
        private void ai_moves_mi_CheckStateChanged(object sender, EventArgs e)
        {
            show &= ~DotsAndBoxes.show_ai_moves;
            if (ai_moves_mi.Checked)
                show |= show_ai_moves;
            this.Refresh();
        }

        // handle 'show almost circled cells' request
        private void almost_circled_cells_mi_CheckStateChanged(object sender, EventArgs e)
        {
            show &= ~DotsAndBoxes.show_almost_circled_cells;
            if( almost_circled_cells_mi.Checked )
                show |= show_almost_circled_cells;
            this.Refresh();
        }

        #endregion

    }
}