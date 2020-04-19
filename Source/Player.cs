using System.Drawing;
using System.Windows.Forms;

namespace DotsAndBoxes
{
    struct PlayerDrawables
    {
        public Panel player_p { get; set; }
        public TextBox name_t { get; set; }

        public Label cells_l { get; set; }
        public Label cell_cnt_l { get; set; }

        public Label score_l { get; set; }
        public Label score_cnt_l { get; set; }

        public IAmblem amblem { get; set; }

        public Color active_frgnd { get; set; }
        public Color active_bkgnd { get; set; }

        public static readonly Color inactive_frgnd = Color.LightGray;
        public static readonly Color inactive_bkgnd = Color.DarkGray;

        public Color getFrgnd(bool isactive) { return (isactive) ? active_frgnd : inactive_frgnd; }
        public Color getBkgnd(bool isactive) { return (isactive) ? active_bkgnd : inactive_bkgnd; }
}

    class Player
    {
        // player state
        public int id { get; }
        private uint score;
        private uint cells;

        private PlayerDrawables drawables;

        // public constants
        public const int invalid_id = -1;



        #region Initialization

        // create a player
        public Player(int id, PlayerDrawables drawables)
        {
            this.id = id;
            this.drawables = drawables;
        }
        
        #endregion



        #region Getters and Setters

        public IAmblem GetAmblem() { return drawables.amblem; }
        public uint getCircledCellCnt() { return cells; }
        public uint getScoreCnt() { return score; }

        // set player active status (if the player is currently making their move or waiting their turn)
        public void setActiveStatus(bool active)
        {
            Color frgnd = drawables.getFrgnd(active);
            Color bkgnd = drawables.getBkgnd(active);

            drawables.player_p.BackColor = bkgnd;
            drawables.name_t.BackColor = frgnd;

            drawables.cells_l.BackColor = bkgnd;
            drawables.cell_cnt_l.BackColor = bkgnd;

            drawables.score_l.BackColor = bkgnd;
            drawables.score_cnt_l.BackColor = bkgnd;

            drawables.player_p.Invalidate(true);
        }

        #endregion



        #region Moves

        // increase circled cell counter by given delta value
        public uint incCircledCellCnt(uint delta)
        {
            cells += delta;

            drawables.cell_cnt_l.Text = cells.ToString();
            drawables.cell_cnt_l.Invalidate();

            return cells;
        }

        // add points to score counter
        public uint incScore(uint delta)
        {
            score += delta;

            drawables.score_cnt_l.Text = score.ToString();
            drawables.score_cnt_l.Invalidate();

            return score;
        }

        // decrease circled cell counter by given delta value
        public uint decCircledCellCnt(uint delta)
        {
            if( delta >= cells ) delta = cells;
            cells -= delta;

            drawables.cell_cnt_l.Text = cells.ToString();
            drawables.cell_cnt_l.Invalidate();

            return cells;
        }
        
        // remove points from score counter
        public uint decScore(uint delta)
        {
            if( delta >= score ) delta = score;
            score -= delta;

            drawables.score_cnt_l.Text = score.ToString();
            drawables.score_cnt_l.Invalidate();

            return score;
        }

        // reset circled cell counter to zero
        public void rstCircledCellCnt() { decCircledCellCnt(cells); }
        
        // reset score counter to zero
        public void rstScore() { decScore(score); }

        #endregion
    }
}
