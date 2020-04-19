using System;
using System.Drawing;

namespace DotsAndBoxes
{
    class Cell : ICloneable
    {
        // cell state
        private int player_id  = Player.invalid_id;    // id of player who won the cell
        private IAmblem amblem = null;                 // amblem of such player

        private uint walls;       // cell walls; cells share walls (duplication of data), but this is okay for performance reasons
        private uint def_walls;   // default walls that the cell was created with

        private int pathnum;      // path the cell belongs to (0 = cell does not belong to any path, -1 = cell is circled)
        private uint wallcnt;     // number of walls built in the cell

        public string[] lbl     = { "", "", "", "" };
        public string[] lbltemp = { "", "", "", "" };

        // public constants
        public static readonly uint top = 1 << 0;
        public static readonly uint lef = 1 << 1;
        public static readonly uint bot = 1 << 2;
        public static readonly uint rig = 1 << 3;

        public static readonly uint all = top | lef | bot | rig;
        public static readonly uint none = 0;

        public static readonly uint[] wall = { top, lef, bot, rig };
        public static readonly uint   wcnt = 4;

        public static readonly uint toppos = 0;
        public static readonly uint lefpos = 1;
        public static readonly uint botpos = 2;
        public static readonly uint rigpos = 3;


        #region Initialization

        private uint CalculateWallCnt()
        {
            return (walls & lef) / lef + (walls & rig) / rig + (walls & top) / top + (walls & bot) / bot;
        }

        public static uint getWallpos(uint wall)
        {
            if ((wall & (top | bot)) > 0 )
               return ((wall & top) == top ? toppos : botpos);

            return ((wall & lef) == lef ? lefpos : rigpos);
        }


        // create a cell, possibly with given default walls
        public Cell() { }
        public Cell(uint def_walls)
        {
            this.walls = this.def_walls = def_walls & all;
            pathnum = 0;
            wallcnt = CalculateWallCnt();

            for (int l = 0; l < wcnt; l++)
            {
                lbl[l] = "";
                lbltemp[l] = "";
            }
        }

        // clone cell
        public object Clone() { return MemberwiseClone(); }

        // restore the original cell walls (useful if the cell is touching the table edges)
        public void reset()
        {
            player_id = Player.invalid_id;
            amblem = null;
            walls = def_walls;
            pathnum = 0;
            wallcnt = CalculateWallCnt();

            for (int l = 0; l < wcnt; l++)
            {
                lbl[l] = "";
                lbltemp[l] = "";
            }
        }

        #endregion



        #region Moves

        // return the opposite wall to the given wall
        public static uint getOppWall(uint wall)
        {
            if( (wall & top) != none ) return bot;
            if( (wall & lef) != none ) return rig;
            if( (wall & bot) != none ) return top;
            if( (wall & rig) != none ) return lef;

            return none;
        }

        public uint getFreeWall()
        {
            if ((walls & lef) == none) return lef;
            if ((walls & top) == none) return top;
            if ((walls & rig) == none) return rig;
            if ((walls & bot) == none) return bot;

            return none;
        }

        public bool hasWall(uint wall) { return (walls & wall) != none; }   // check if cell has given wall
        public bool hasOppWall(uint wall) { return hasWall(getOppWall(wall)); }   // check if cell has given opposite wall

        public bool isCircled() { return walls == all; }   // check if cell has all four walls
        public int getPlayerId() { return player_id; }
        public uint getWalls() { return walls; }
        public int getPathnum() { return pathnum; }
        public uint getWallCnt() { return wallcnt; }

        public void setPathnum( int p ) { pathnum = p; }

        // return if the cell has become encircled by adding the given wall
        public bool addWall(Player player, uint wall)
        {
            // if the given wall is not empty
            if( (wall &= all) == none ) return false;

            // if the wall already exists
            // ...

            // if more than one wall will be added
            // ...

            // add the given wall
            walls |= wall;
            wallcnt = CalculateWallCnt();

            // incremental check if new paths were created or broken because of adding wall
            // if( wall == lef || wall == rig )
            // ...

                


            // check if the cell has become encircled
            if( walls == all && player_id == Player.invalid_id )
            {
                player_id = player.id;
                amblem = player.GetAmblem();
                return true;
            }

            return false;
        }
        
        // return if the cell has become encircled by adding the opposite wall to the given one
        public bool addOppWall(Player player, uint wall)
        {
            return addWall(player, getOppWall(wall));
        }

        // return if the cell has stopped being encircled by removing the given wall
        public bool remWall(uint wall)
        {
            // if the given wall is not empty
            if( (wall &= all) == none ) return false;

            // if the wall does not exist
            // ...

            // if more than one wall will be removed
            // ...

            // remove the given wall
            walls &= ~wall;
            wallcnt = CalculateWallCnt();

            // check if the cell has stopped being circled
            if( player_id != Player.invalid_id )
            {
                player_id = Player.invalid_id;
                amblem = null;
                return true;
            }

            return false;
        }
        
        // return if the cell has stopped being encircled by removing the opposite wall to the given one
        public bool remOppWall(uint wall)
        {
            return remWall(getOppWall(wall));
        }

        #endregion



        #region Drawing

        // draw only the active walls (inactive walls will be drawn in CellGrid)
        public void drawActiveWalls(Graphics g, Pen pen, float x, float y, float a)
        {
            float d = pen.Width/2;

         // // draw active lines of a cell
         // if( (walls & top) != none ) g.DrawLine(pen, x-d,   y,     x+a+d, y    );
         // if( (walls & lef) != none ) g.DrawLine(pen, x,     y+a+d, x,     y-d  );
         // if( (walls & bot) != none ) g.DrawLine(pen, x+a+d, y+a,   x+d,   y+a  );
         // if( (walls & rig) != none ) g.DrawLine(pen, x+a,   y-d,   x+a,   y+a+d);

            // draw active lines of a cell
            Pen nextmovePen = new Pen(Color.Red);
            Pen tempPen     = new Pen(Color.Blue);
            nextmovePen.Width = pen.Width;
            tempPen.Width     = pen.Width;
            if ((walls & top) != none) g.DrawLine( (lbltemp[toppos] != "" ? tempPen : (lbl[toppos] == "[?]" ? nextmovePen : pen)), x-d,   y,     x+a+d, y    );
            if ((walls & lef) != none) g.DrawLine( (lbltemp[lefpos] != "" ? tempPen : (lbl[lefpos] == "[?]" ? nextmovePen : pen)), x,     y+a+d, x,     y-d  );
            if ((walls & bot) != none) g.DrawLine( (lbltemp[botpos] != "" ? tempPen : (lbl[botpos] == "[?]" ? nextmovePen : pen)), x+a+d, y+a,   x-d,   y+a  );
            if ((walls & rig) != none) g.DrawLine( (lbltemp[rigpos] != "" ? tempPen : (lbl[rigpos] == "[?]" ? nextmovePen : pen)), x+a,   y-d,   x+a,   y+a+d);



            Rectangle rect1 = new Rectangle((int)x, (int)y, (int)(a), (int)(a));

            if ((DotsAndBoxes.show & DotsAndBoxes.show_almost_circled_cells) != 0)
            {
                // mark cell which is ready to be circled (in the next move)
                Rectangle rect2 = new Rectangle((int)(x+a/4), (int)(y+a/4), (int)(a/2), (int)(a/2));
                if (wallcnt == 3)
                {
                 // g.DrawRectangle(Pens.Black, rect2);
                    SolidBrush brush = new SolidBrush(Color.LimeGreen);
                    g.FillRectangle(brush, rect2);
                }
            }

            StringFormat strFormat = new StringFormat();
            strFormat.Alignment = StringAlignment.Center;
            strFormat.LineAlignment = StringAlignment.Center;

            if ((DotsAndBoxes.show & DotsAndBoxes.show_path_numbers) != 0)
            {
                // draw pathnum
                if (pathnum > 0)
                {
                    Font pathnumFont = new Font("Arial", a / 2);
                    SolidBrush pathnumBrush = new SolidBrush(Color.LightGray);
                    string pathnum_s = "" + pathnum;
                    g.DrawString(pathnum_s, pathnumFont, pathnumBrush, rect1, strFormat);
                }

                // draw wallcnt
                if (wallcnt > 0)
                {
                    Font wallcntFont = new Font("Arial", a / 16);
                    SolidBrush wallcntBrush = new SolidBrush(Color.Gray);
                    string wallcnt_s = "" + wallcnt;
                    g.DrawString(wallcnt_s, wallcntFont, wallcntBrush, rect1, strFormat);
                }
            }

            if ((DotsAndBoxes.show & DotsAndBoxes.show_game_info) != 0)
            {
                // draw line labels
                Font lblFont = new Font("Arial", a / 10);
                SolidBrush lblBrush = new SolidBrush(Color.Red);

                // draw top line label
                strFormat.Alignment = StringAlignment.Center;
                strFormat.LineAlignment = StringAlignment.Near;
                if (lbltemp[toppos] != null && lbltemp[toppos].Length > 0)
                    g.DrawString(lbltemp[toppos], lblFont, lblBrush, rect1, strFormat);
                else if (lbl[toppos] != null && lbl[toppos].Length > 0)
                    g.DrawString(lbl[toppos], lblFont, lblBrush, rect1, strFormat);

                // draw bottom line label
                strFormat.Alignment = StringAlignment.Center;
                strFormat.LineAlignment = StringAlignment.Far;
                if (lbltemp[botpos] != null && lbltemp[botpos].Length > 0)
                    g.DrawString(lbltemp[botpos], lblFont, lblBrush, rect1, strFormat);
                else if (lbl[botpos] != null && lbl[botpos].Length > 0)
                    g.DrawString(lbl[botpos], lblFont, lblBrush, rect1, strFormat);

                // draw left line label
                strFormat.Alignment = StringAlignment.Near;
                strFormat.LineAlignment = StringAlignment.Center;
                if (lbltemp[lefpos] != null && lbltemp[lefpos].Length > 0)
                    g.DrawString(lbltemp[lefpos], lblFont, lblBrush, rect1, strFormat);
                else if (lbl[lefpos] != null && lbl[lefpos].Length > 0)
                    g.DrawString(lbl[lefpos], lblFont, lblBrush, rect1, strFormat);

                // draw right line label
                strFormat.Alignment = StringAlignment.Far;
                strFormat.LineAlignment = StringAlignment.Center;
                if (lbltemp[rigpos] != null && lbltemp[rigpos].Length > 0)
                    g.DrawString(lbltemp[rigpos], lblFont, lblBrush, rect1, strFormat);
                else if (lbl[rigpos] != null && lbl[rigpos].Length > 0)
                    g.DrawString(lbl[rigpos], lblFont, lblBrush, rect1, strFormat);
            }
        }

        // draw player amblem
        public void drawAmblem(Graphics g, float x, float y, float a, int d)
        {
            amblem?.draw(g, x, y, a, d);
        }

        #endregion


    }
}
