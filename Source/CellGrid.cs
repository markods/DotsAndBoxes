using System;
using System.Drawing;

namespace DotsAndBoxes
{
    class CellGrid : ICloneable
    {
        private Cell[,] cell;   // cell matrix
        public uint rowcnt { get; }   // number of rows in matrix
        public uint colcnt { get; }   // number of columns in matrix

        private int max_pathnum; // number of paths (connected cells groups with at least 2 walls) in cell matrix
                                 // pathnum = -1 for circled cells
                                 // pathnum = 0 for non-circled cells that do not belong to any path
        private Path[] path;     // descriptions of paths
                                 // path[0].len = number of non-circled cells that do not belong to any path
        private uint[] circledcnt = { 0, 0 };   // number of circled cells in matrix (per player)
        private uint[] cellcnt_having_wallcnt = { 0, 0, 0, 0, 0 }; // number of cells in matrix (per number of walls)
        private uint moves2play = 0;  // number of moves left until game ends
        
        
        
        #region Initialization

        // create a CellGrid with given number of rows and columns
        public CellGrid(uint rowcnt, uint colcnt)
        {
            if (rowcnt == 0 || colcnt == 0)
                throw new ArgumentException(String.Format("All grid dimensions {0}x{1} should be nonzero", rowcnt, colcnt));

            // initializing grid
            this.rowcnt = rowcnt;
            this.colcnt = colcnt;
            cell = new Cell[rowcnt, colcnt];
            createCells(ref cell, rowcnt, colcnt);
            resetCounters();
        }

        // create a CellGrid with given number of rows and columns, but skips creation of individual cells
        private CellGrid(uint rowcnt, uint colcnt, bool method_for_cloning)   // dummy variable
        {
            if (rowcnt == 0 || colcnt == 0)
                throw new ArgumentException(String.Format("All grid dimensions {0}x{1} should be nonzero", rowcnt, colcnt));

            // initializing grid
            this.rowcnt = rowcnt;
            this.colcnt = colcnt;
            cell = new Cell[rowcnt, colcnt];
            resetCounters();
        }


        // initialize grid matrix with cells, also initializing those cells to their default values
        private static void createCells(ref Cell[,] matrix, uint rowcnt, uint colcnt)
        {
            for (uint i = 0, def_walls; i < rowcnt; i++)
            {
                for (int j = 0; j < colcnt; j++)
                {
                    def_walls = Cell.none;

                    // if( i == 0 ) def_walls |= Cell.top;
                    // if( j == 0 ) def_walls |= Cell.lef;
                    // if( i == rowcnt-1 ) def_walls |= Cell.bot;
                    // if( j == colcnt-1 ) def_walls |= Cell.rig;

                    matrix[i, j] = new Cell(def_walls);
                }
            }
        }

        // clone cells to matrix from given cell matrix of same size
        private static void cloneCells(ref Cell[,] matrix_to, ref Cell[,] matrix_fr, uint rowcnt, uint colcnt)
        {
            for (int i = 0; i < rowcnt; i++)
                for (int j = 0; j < colcnt; j++)
                    matrix_to[i, j] = (Cell)matrix_fr[i, j].Clone();
        }


        // clone entire cell grid
        public object Clone()
        {
            CellGrid copy = new CellGrid(rowcnt, colcnt, true);
            cloneCells(ref copy.cell, ref cell, rowcnt, colcnt);

            return copy;
        }

        // reset cells in matrix to their defaults
        public void reset()
        {
            // reset individual cells in grid
            for (int i = 0; i < rowcnt; i++)
                for (int j = 0; j < colcnt; j++)
                    cell[i, j].reset();

            resetCounters();
        }

        private void resetCounters()
        {
            max_pathnum = 0;
            path = new Path[rowcnt * colcnt + 1];

            for (long i = 0; i < rowcnt * colcnt + 1; i++)
                path[i] = new Path();
            path[0].len = rowcnt * colcnt;

            // reset circled cell count
            circledcnt[0] = 0;
            circledcnt[1] = 0;

            // reset move countners
            moves2play = 2 * rowcnt * colcnt + rowcnt + colcnt;
            // safe_moves2play = moves2play;

            // reset grid counters
            for (int i = 0; i <= Cell.wcnt; i++)
                cellcnt_having_wallcnt[i] = 0;
            cellcnt_having_wallcnt[0] = getCellCnt();
        }

        #endregion



        #region Getters and Setters

        public uint getCellCnt() { return rowcnt * colcnt; }
        public uint getCircledCnt() { return circledcnt[0] + circledcnt[1]; }
        public uint getOpenCnt() { return rowcnt * colcnt - getCircledCnt(); }
        public uint getCircledCnt4Player(int player_id) { return circledcnt[player_id & 1]; }
        public uint getCellcntHavingWallcnt(uint wallcnt) { return cellcnt_having_wallcnt[wallcnt % 5]; }
        public uint getMoves2Play() { return moves2play; }
        public uint getPathlen(int pathnum) { return path[pathnum].len; }

        // get the player_id of the winner
        public int getWinnerID()
        {
            // if all cells are circled, and players are not equal
            if (getOpenCnt() == 0 && circledcnt[0] != circledcnt[1])
                return (circledcnt[0] > circledcnt[1] ? 0 : 1);

            return Player.invalid_id;
        }

        // get number of safe moves
        public uint getSafeMoveCnt()
        {
            uint cnt = 0;
            for( uint r = 0; r < rowcnt; r++ )
                for( uint c = 0; c < colcnt; c++ )
                    for( uint i = 0; i < Cell.wcnt; i++ )
                        if( (Cell.wall[i] == Cell.top
                              ||  Cell.wall[i] == Cell.lef
                              || (Cell.wall[i] == Cell.bot && r == rowcnt - 1)
                              || (Cell.wall[i] == Cell.rig && c == colcnt - 1))
                            && isMoveSafe(r, c, Cell.wall[i]) )
                            cnt++;
            return cnt;
        }

        // find shortest path length
        public uint getMinPathlen()
        {
            uint minPathlen = rowcnt * colcnt;  // max possible value
            for( int m = 1; m <= max_pathnum; m++ )
                if( path[m].len < minPathlen )
                    minPathlen = path[m].len;

            return minPathlen;
        }

        // detect if the path is closed on both ends
        public bool isPathClosed(int pathnum)
        {
            if (pathnum < 0 || max_pathnum < pathnum)
                return false;

            return path[pathnum].iscircular
                || pathTerminatorCnt(pathnum) == Path.ClosedAtBothEnds;
        }

        // detect if the path is closed on at least one end
        public bool isPathClosedAtAtLeastOneEnd(int pathnum)
        {
            return pathTerminatorCnt( pathnum ) >= Path.ClosedAtOneEnd;

        }

        // get number of path ends that are closed
        public int pathTerminatorCnt(int pathnum)
        {
            if (pathnum < 0 || max_pathnum < pathnum)
                return Path.Wrong;

            if (path[pathnum].iscircular)
                return Path.Circular;

            int cnt = 0;
            if (cell[path[pathnum].begrow, path[pathnum].begcol].getWallCnt() == 3)
                cnt++;
            if (cell[path[pathnum].endrow, path[pathnum].endcol].getWallCnt() == 3)
                cnt++;

            return cnt;
        }

        #endregion



        #region Cells

        // indexer (for convinient access to cells in matrix)
        public Cell this[uint row, uint col]
        {
            get
            {
                if( row >= rowcnt || col >= colcnt )
                    throw new IndexOutOfRangeException(String.Format("Indexes ({0},{1}) out of range [{2},{3}] x [{2},{4}]", row, col, 0, rowcnt, colcnt));

                return cell[row, col];
            }
        }

        // return a cell that borders the current cell on given wall in matrix
        public Cell getAdjCell(uint row, uint col, uint wall)
        {
            if( row >= rowcnt || col >= colcnt )
                return null;

            wall &= Cell.all;

            if( wall == Cell.none ) return                    cell[row,   col  ];
            if( wall == Cell.top  ) return (row > 0)        ? cell[row-1, col  ] : null;
            if( wall == Cell.lef  ) return (col > 0)        ? cell[row,   col-1] : null;
            if( wall == Cell.bot  ) return (row < rowcnt-1) ? cell[row+1, col  ] : null;
            if( wall == Cell.rig  ) return (col < colcnt-1) ? cell[row,   col+1] : null;

            return null;
        }
        public bool areAdjacent(uint r1, uint c1, uint r2, uint c2)
        {
            if(    r1 < 0 || r1 >= rowcnt || c1 < 0 || c1 >= colcnt
                || r2 < 0 || r2 >= rowcnt || c2 < 0 || c2 >= colcnt)
                return false;

            if (   (r1 == r2 && (   (c1 + 1 == c2 && !cell[r1, c1].hasWall(Cell.rig))
                                 || (c1 == c2 + 1 && !cell[r1, c1].hasWall(Cell.lef))))
                || (c1 == c2 && (   (r1 + 1 == r2 && !cell[r1, c1].hasWall(Cell.bot))
                                 || (r1 == r2 + 1 && !cell[r1, c1].hasWall(Cell.top)))) )
                return true;

            return false;
        }

        public uint getAdjRow(uint row, uint wall)
        {
            // returns the same row if the adjacent cell does not exist
            if (wall == Cell.top) return (row > 0 ? row - 1 : row);          
            if (wall == Cell.bot) return (row < rowcnt - 1 ? row + 1 : row); 
            return row;
        }
        public uint getAdjCol(uint col, uint wall)
        {
            // returns the same col if the adjacent cell does not exist
            if (wall == Cell.lef) return (col > 0 ? col - 1 : col);
            if (wall == Cell.rig) return (col < colcnt - 1 ? col + 1 : col);
            return col;
        }

        #endregion



        #region Moves

        // checks if a move is safe to make a move
        // a move is safe if both adjacent cells have less than two walls
        public bool isMoveSafe(uint row, uint col, uint wall)
        {
            return (!cell[row, col].hasWall(wall)
                && cell[row, col].getWallCnt() < 2
                && ((wall == Cell.top && (row == 0 || cell[row - 1, col].getWallCnt() < 2))
                 || (wall == Cell.lef && (col == 0 || cell[row, col - 1].getWallCnt() < 2))
                 || (wall == Cell.bot && (row == rowcnt - 1 || cell[row + 1, col].getWallCnt() < 2))
                 || (wall == Cell.rig && (col == colcnt - 1 || cell[row, col + 1].getWallCnt() < 2))));
        }

        // checks if a move is worth playing
        // TODO: split isMoveSensible in two parts 
        //    - one which will be used when IterateMove() steps into the new cell (independent of the wall)
        //    - one which will be used when IterateMove() tests each wall of already chosen cell
        public bool isMoveSensible(uint row, uint col, uint wall, uint minPathlen)
        {
            Cell c = cell[row, col];
            int p = c.getPathnum();

            // wall must be nonexistent
            if (c.hasWall(wall))
                return false;

            // wall of a cell that does not belong to any path
            if (  (p == 0)
                // since only top and left wall of a cell is examined, also check if a top or left neigbhouring cell does not belong to any path
                || (row > 0          && wall == Cell.top && !c.hasWall(Cell.top) && cell[row - 1, col].getPathnum() == 0)
                || (col > 0          && wall == Cell.lef && !c.hasWall(Cell.lef) && cell[row, col - 1].getPathnum() == 0)
             // || (row < rowcnt - 1 && wall == Cell.bot && !c.hasWall(Cell.bot) && cell[row + 1, col].getPathnum() == 0)
             // || (col < colcnt - 1 && wall == Cell.rig && !c.hasWall(Cell.rig) && cell[row, col + 1].getPathnum() == 0)
                )
                return true;

         // // optimization
         // Cell adjc = getAdjCell(row, col, wall);
         // int adjp = (adjc != null ? adjc.getPathnum() : p);
         // if (     (   p <= 0 || ! path[p].isRepresentative)
         //       && (adjp <= 0 || ! path[adjp].isRepresentative) )
         //     return false;

         // // optimization
         // // if 2-cells or shorter paths exist, ignore cell from a path longer than 2 cells
         // // if 4-cells or shorter paths exist, ignore cell from a path longer than 4 cells
         // // if only paths longer then 4-cells exist, use only the path with min length
         // Cell adjc = getAdjCell(row, col, wall);
         // int adjp = (adjc != null ? adjc.getPathnum() : p);
         // if ((p > 0 && !isPathClosedAtAtLeastOneEnd(p)
         //                  && ( // (minPathlen == 1 && path[p].len != 1) ||
         //                          (minPathlen <= 2 && path[p].len > 2) ||
         //                          (minPathlen <= 4 && path[p].len > 4) ||
         //                          (minPathlen > 4 && path[p].len > minPathlen)))
         //     || (adjp > 0 && !isPathClosedAtAtLeastOneEnd(adjp)
         //                  && ( // (minPathlen == 1 && path[adjp].len != 1) ||
         //                          (minPathlen <= 2 && path[adjp].len > 2) ||
         //                          (minPathlen <= 4 && path[adjp].len > 4) ||
         //                          (minPathlen > 4 && path[adjp].len > minPathlen))))
         //     return false;

         // // optimization
         // Cell adjc = getAdjCell(row, col, wall);
         // int adjp = (adjc != null ? adjc.getPathnum() : p);
         // if ((p > 0 && !isPathClosedAtAtLeastOneEnd(p)
         //                  && ( // (minPathlen == 1 && path[p].len != 1) ||
         //                       // (minPathlen <= 2 && path[p].len > 2) ||
         //                       // (minPathlen <= 4 && path[p].len > 4) ||
         //                       // (minPathlen <  4 && path[p].len >= 4 && ! isPathClosedAtAtLeastOneEnd(p) ) ||
         //                       // (minPathlen <= 7 && path[p].len >= 8 && isPathClosed(p)) ||
         //                          (minPathlen <= 7 && path[p].len > 7 ) ||
         //                          (minPathlen >  7 && path[p].len > minPathlen)))
         //     || (adjp > 0 && !isPathClosedAtAtLeastOneEnd(adjp)
         //                  && ( // (minPathlen == 1 && path[adjp].len != 1) ||
         //                       // (minPathlen <= 2 && path[adjp].len > 2) ||
         //                       // (minPathlen <= 4 && path[adjp].len > 4) ||
         //                       // (minPathlen <= 7 && path[adjp].len >= 4 && !isPathClosedAtAtLeastOneEnd(adjp)) ||
         //                       // (minPathlen <= 7 && path[adjp].len >= 8 && isPathClosed(adjp) ) ||
         //                          (minPathlen <= 7 && path[adjp].len > 7) ||
         //                          (minPathlen >  7 && path[adjp].len > minPathlen))))
         //     return false;

            // any wall of a cell that belongs to a 2-cells path (including a middle one of the 2-cell path) 
            if ( p > 0 && path[p].len == 2 )
                return true;

            // wall on the outer edge of a grid
            if (   (row == 0          && wall == Cell.top)
                || (row == rowcnt - 1 && wall == Cell.bot)
                || (col == 0          && wall == Cell.lef)
                || (col == colcnt - 1 && wall == Cell.rig))
                return true;

            // the 4th wall of a cell that begins/ends a path
            if (  // cell[row,col].getPathnum() > 0 
                  // && 
                  c.getWallCnt() == 3
                  // since only top and left walls of a cell are examined, also check if a top or left neigbhouring cell begins/ends the same path
                || (row > 0 && wall == Cell.top && !c.hasWall(Cell.top) && p == cell[row - 1, col].getPathnum() && cell[row - 1, col].getWallCnt() == 3)
                || (col > 0 && wall == Cell.lef && !c.hasWall(Cell.lef) && p == cell[row, col - 1].getPathnum() && cell[row, col - 1].getWallCnt() == 3)
             // || (row < rowcnt - 1 && wall == Cell.bot && !cell[row, col].hasWall(Cell.bot) && cell[row, col].getPathnum() == cell[row + 1, col].getPathnum() && cell[row + 1, col].getWallCnt() == 3)
             // || (col < colcnt - 1 && wall == Cell.rig && !cell[row, col].hasWall(Cell.rig) && cell[row, col].getPathnum() == cell[row, col + 1].getPathnum() && cell[row, col + 1].getWallCnt() == 3)
                )
                return true;

            // the opposite (non-neighbouring) wall of the cell whose neighbouring cell begins/ends the same 4-cells path
            if (  // cell[row, col].getPathnum() > 0
                  // && 
                  path[p].len == 4
               && c.getWallCnt() == 2
               && ((row > 0 && !c.hasWall(Cell.top) && p == cell[row - 1, col].getPathnum() && cell[row - 1, col].getWallCnt() == 3)
                || (col > 0 && !c.hasWall(Cell.lef) && p == cell[row, col - 1].getPathnum() && cell[row, col - 1].getWallCnt() == 3)
                || (row < rowcnt - 1 && !c.hasWall(Cell.bot) && p == cell[row + 1, col].getPathnum() && cell[row + 1, col].getWallCnt() == 3)
                || (col < colcnt - 1 && !c.hasWall(Cell.rig) && p == cell[row, col + 1].getPathnum() && cell[row, col + 1].getWallCnt() == 3)))
                return true;

            // wall between beg and end cell of the circular path
            if (path[p].iscircular 
                && (  (   row == path[p].begrow 
                       && col == path[p].begcol
                       && getAdjRow(row,wall) == path[p].endrow
                       && getAdjCol(col,wall) == path[p].endcol )
                    || (  row == path[p].endrow
                       && col == path[p].endcol
                       && getAdjRow(row, wall) == path[p].begrow
                       && getAdjCol(col, wall) == path[p].begcol) ) )
                return true;

            return false;
        }

        // return if the move is valid and the number of cells that the player has encircled with their move
        public Tuple<bool, uint> doMove(Player player, uint row, uint col, uint wall)
        {
            // fix user input
            wall &= Cell.all;

            // if the position is not valid or a wall to be added already exists, do nothing
            if( row >= rowcnt || col >= colcnt || wall == Cell.none || cell[row, col].hasWall(wall) )
                return new Tuple<bool, uint>(false, 0);

            // get current cell and adjacent cell that share the same given wall
            Cell cur = cell[row, col];
            Cell adj = getAdjCell(row, col, wall);
            uint circled_cnt = 0;

            // if the current cell will become encircled, remove it from the path before adding the wall
            if( cur.getWallCnt() == 3 )
                removeFromPath(row, col);

            // if the adjacent cell will become encircled, remove it from the path before adding the wall
            if( adj != null && adj.getWallCnt() == 3 )
                removeFromPath(getAdjRow(row, wall), getAdjCol(col, wall));

            // add the wall, and test if the current cell has been encircled
            if( cur.addWall(player, wall) )
                circled_cnt++;

            // add the wall, test if the adjacent cell has been encircled
            if( adj != null && adj.addOppWall(player, wall) )
                circled_cnt++;

            // check if the move has broken a path (if both current and adjacent cells are belonging to the same path)
            if( adj != null
               && cur.getPathnum() > 0
               && cur.getPathnum() == adj.getPathnum() )
                breakPath(row, col, getAdjRow(row, wall), getAdjCol(col, wall));

            // check if current cell is a candidate for path
            checkPathMerging(row, col /*, wall */);

            // check if adjacent cell is a candidate for path
            if( adj != null )
                checkPathMerging(getAdjRow(row, wall), getAdjCol(col, wall));

            // update cell counters and return number of encircled cells
            moves2play--;

            // if (cur.getWallCnt() == 1)
            //     safe_moves2play--;
            // if (cur.getWallCnt() == 2)
            // {
            //     if (!cur.hasWall(Cell.top) && (row == 0 || cell[row - 1, col].getWallCnt() < 2))
            //         safe_moves2play--;
            //     if (!cur.hasWall(Cell.lef) && (col == 0 || cell[row, col - 1].getWallCnt() < 2))
            //         safe_moves2play--;
            //     if (!cur.hasWall(Cell.bot) && (row == rowcnt-1 || cell[row + 1, col].getWallCnt() < 2))
            //         safe_moves2play--;
            //     if (!cur.hasWall(Cell.rig) && (col == colcnt-1 || cell[row, col + 1].getWallCnt() < 2))
            //         safe_moves2play--;
            // }
            // if (adj != null)
            // {
            //     uint adjrow = getAdjRow(row, wall);
            //     uint adjcol = getAdjCol(col, wall);
            //     if (adj.getWallCnt() == 2)
            //     {
            //         if (!adj.hasWall(Cell.top) && (adjrow == 0 || cell[adjrow - 1, adjcol].getWallCnt() < 2))
            //             safe_moves2play--;
            //         if (!adj.hasWall(Cell.lef) && (adjcol == 0 || cell[adjrow, adjcol - 1].getWallCnt() < 2))
            //             safe_moves2play--;
            //         if (!adj.hasWall(Cell.bot) && (adjrow == rowcnt - 1 || cell[adjrow + 1, adjcol].getWallCnt() < 2))
            //             safe_moves2play--;
            //         if (!adj.hasWall(Cell.rig) && (adjcol == colcnt - 1 || cell[adjrow, adjcol + 1].getWallCnt() < 2))
            //             safe_moves2play--;
            //     }
            // }

            circledcnt[player.id] += circled_cnt;
            // if( cellcnt_having_wallcnt[cur.getWallCnt() - 1] > 0 )
            cellcnt_having_wallcnt[cur.getWallCnt() - 1]--;
            cellcnt_having_wallcnt[cur.getWallCnt()]++;
            if( adj != null )
            {
                // if (cellcnt_having_wallcnt[adj.getWallCnt() - 1] > 0)
                cellcnt_having_wallcnt[adj.getWallCnt() - 1]--;
                cellcnt_having_wallcnt[adj.getWallCnt()]++;
            }


            // // debugging
            // for( int p = 1; p < max_pathnum; p ++ )
            //     if (   !isPathTerminator(path[p].begrow, path[p].begcol)
            //         || !isPathTerminator(path[p].endrow, path[p].endcol))
            //         ; // not possible under normal circumstances


            return new Tuple<bool, uint>(true, circled_cnt);
        }

        // return if the undo is valid and the number of cells that the player has lost by undoing their move
        public uint undoMove(int player_id, uint row, uint col, uint wall)
        {
            // fix user input
            wall &= Cell.all;

            // if the position is not valid or a wall to be removed does not exist, do nothing
            if( row >= rowcnt || col >= colcnt || wall == Cell.none || !cell[row, col].hasWall(wall) )
                return 0;

            // get current cell and adjacent cell that share the same given wall
            Cell cur = cell[row, col];
            Cell adj = getAdjCell(row, col, wall);
            uint uncircled_cnt = 0;

            // if the current cell will become removed from the path, do so before removing the wall
            if( cur.getWallCnt() == 2 )
                removeFromPath(row, col);

            // if the adjacent cell will become removed from the path, do so before removing the wall
            if( adj != null && adj.getWallCnt() == 2 )
                removeFromPath(getAdjRow(row, wall), getAdjCol(col, wall));

            // remove the wall, and test if the current cell has been uncircled
            if( cur.remWall(wall) )
                uncircled_cnt++;

            // remove the wall, and test if the adjacent cell has been uncircled
            if( adj != null && adj.remOppWall(wall) )
                uncircled_cnt++;

            // if both the current and adjacent cell have been uncircled at once, allocate the new path
            if( uncircled_cnt == 2 )
            {
                // // debugging
                // if (max_pathnum == path.GetUpperBound(0))
                //     ; // not possible under normal circumstances

                max_pathnum++;
                cur.setPathnum(max_pathnum);
                adj.setPathnum(max_pathnum);
                path[max_pathnum].len = 2;
                path[max_pathnum].begrow = row;
                path[max_pathnum].begcol = col;
                path[max_pathnum].endrow = getAdjRow(row, wall);
                path[max_pathnum].endcol = getAdjCol(col, wall);
                // path[0].len -= 2;
            }

            if( adj != null && cur.getPathnum() > 0 && path[cur.getPathnum()].len > 2 && cur.getPathnum() == adj.getPathnum() )
                path[cur.getPathnum()].iscircular = true;
            else
            {
                // check if the path of the current cell should be merged
                checkPathMerging(row, col);

                // check if the path of the adjacent cell should be merged
                if( adj != null )
                    checkPathMerging(getAdjRow(row, wall), getAdjCol(col, wall));
            }

            // update cell counters and return number of encircled cells
            moves2play++;

            // //debugging
            // if (circledcnt[player_id] < uncircled_cnt)
            //     ; // circledcnt[player_id] must be nonzero
            circledcnt[player_id] -= uncircled_cnt;
            // if (cellcnt_having_wallcnt[cur.getWallCnt() + 1] > 0)
            cellcnt_having_wallcnt[cur.getWallCnt() + 1]--;
            cellcnt_having_wallcnt[cur.getWallCnt()]++;
            if( adj != null )
            {
                // if (cellcnt_having_wallcnt[adj.getWallCnt() + 1] > 0)
                cellcnt_having_wallcnt[adj.getWallCnt() + 1]--;
                cellcnt_having_wallcnt[adj.getWallCnt()]++;
            }

            // // debugging
            // for (int p = 1; p < max_pathnum; p++)
            //     if (!isPathTerminator(path[p].begrow, path[p].begcol)
            //         || !isPathTerminator(path[p].endrow, path[p].endcol))
            //         ; // not possible under normal circumstances

            return uncircled_cnt;
        }

        #endregion



        #region Paths

        public void markRepresentativePaths()
        {
            for (int p = 1; p <= max_pathnum; p++)
                path[p].isRepresentative = true;

            for (int p = 1; p <= max_pathnum - 1; p++)
            {
                for (int n = p+1; n <= max_pathnum; n++)
                {
                    if (  path[n].len                    == path[p].len
                       && path[n].iscircular             == path[p].iscircular
                       && isPathClosed(n)                == isPathClosed(p)
                       && isPathClosedAtAtLeastOneEnd(n) == isPathClosedAtAtLeastOneEnd(p)
                       )
                    {
                        path[n].isRepresentative = false;
                    }
                }
            }
        }

        private void renumMaxPath(int new_pathnum)
        {
            // new_pathnum must not exist
            if (new_pathnum > 0 && path[new_pathnum].len > 0)
                return;

            // renum the bigest pathnum
            if (new_pathnum < max_pathnum)
            {
                // start from any end of the max path
                uint r = path[max_pathnum].begrow;
                uint c = path[max_pathnum].begcol;
                Tuple<bool, uint, uint> res;
                bool NextExists = false;
                do
                {
                    // set the new pathnum to the chosen part of the path
                    cell[r, c].setPathnum(new_pathnum);

                    // move to next cell in the chosen part of the path
                    res = getNextInPath(r, c, max_pathnum);
                    NextExists = res.Item1;
                    r = res.Item2;
                    c = res.Item3;
                }
                while (NextExists);

                path[new_pathnum].len = path[max_pathnum].len;
                path[new_pathnum].iscircular = path[max_pathnum].iscircular;
                path[new_pathnum].begrow = path[max_pathnum].begrow;
                path[new_pathnum].begcol = path[max_pathnum].begcol;
                path[new_pathnum].endrow = path[max_pathnum].endrow;
                path[new_pathnum].endcol = path[max_pathnum].endcol;

                path[max_pathnum].len = 0;
                path[max_pathnum].iscircular = false;

                // debugging
                path[max_pathnum].begrow = 0;
                path[max_pathnum].begcol = 0;
                path[max_pathnum].endrow = 0;
                path[max_pathnum].endcol = 0;
            }

         // // debugging
         // if (max_pathnum == 0)
         //     ; // not possible under normal circumstances

            max_pathnum--;
        }

        // replaces pathnum of the old path with the pathnum of the new path
        private void renumPath(int old_pathnum, int new_pathnum)
        {
            if (old_pathnum <= 0)  // path does not exist
                return;

            if (new_pathnum <= 0)  // path does not exist
                return;

            if (old_pathnum == new_pathnum)
                return;

            if (old_pathnum > max_pathnum)
                return;

            uint r = 0;
            uint c = 0;

            // prepare to remove the path with bigger pathnum
            if (old_pathnum < new_pathnum)
            {
                int tmp     = old_pathnum;
                old_pathnum = new_pathnum;
                new_pathnum = tmp;
            }

         // // debugging
         // if (path[old_pathnum].len == 1)
         //     ; //check merging the path with a single cell
         //
         // // debugging
         // if (path[new_pathnum].len == 2)
         //     ; //check adding of the third cell in path


            ///////////////

            // remove the old path
            // start from the end of the old path which is adjacent to the new path
            r = path[old_pathnum].begrow;
            c = path[old_pathnum].begcol;
            if (    !areAdjacent(r, c, path[new_pathnum].begrow, path[new_pathnum].begcol) 
                 && !areAdjacent(r, c, path[new_pathnum].endrow, path[new_pathnum].endcol) )
            {
                r = path[old_pathnum].endrow;
                c = path[old_pathnum].endcol;
            }
            Tuple<bool, uint, uint> res;
            bool NextExists = false;
            do
            {
                if (areAdjacent(r, c, path[new_pathnum].begrow, path[new_pathnum].begcol))
                {
                    path[new_pathnum].begrow = r;
                    path[new_pathnum].begcol = c;
                }
                else
                {
                    path[new_pathnum].endrow = r;
                    path[new_pathnum].endcol = c;
                }

                // set the new pathnum to the chosen part of the path
                cell[r, c].setPathnum(new_pathnum);
                path[new_pathnum].len++;

             // // debugging
             // if (path[old_pathnum].len == 0)
             //     ; //length must be nonzero
                path[old_pathnum].len--;

                // move to next cell in the chosen part of the path
                res = getNextInPath(r, c, old_pathnum);
                NextExists = res.Item1;
                r = res.Item2;
                c = res.Item3;
            }
            while (NextExists);

            //////////////



            // path[new_pathnum].len       += path[old_pathnum].len;
            path[new_pathnum].iscircular = path[old_pathnum].iscircular;
            // path[new_pathnum].begrow     = path[old_pathnum].begrow;
            // path[new_pathnum].begcol     = path[old_pathnum].begcol;
            // path[new_pathnum].endrow     = path[old_pathnum].endrow;
            // path[new_pathnum].endcol     = path[old_pathnum].endcol;


            // path[old_pathnum].len = 0;
            path[old_pathnum].iscircular = false;
            renumMaxPath(old_pathnum);


         // // debugging
         // if (   !isPathTerminator(path[new_pathnum].begrow, path[new_pathnum].begcol)
         //     || !isPathTerminator(path[new_pathnum].endrow, path[new_pathnum].endcol))
         //     ; // not possible under normal circumstances
         //
         // // debugging
         // if (   !isPathTerminator(path[old_pathnum].begrow, path[old_pathnum].begcol)
         //     || !isPathTerminator(path[old_pathnum].endrow, path[old_pathnum].endcol))
         //     ; // not possible under normal circumstances
        }

        // make a new path that contains a single cell
        private int makePath(uint r, uint c)
        {
            // path will not be created if cell already belongs to a path
            int old_p = cell[r, c].getPathnum();
            if ( old_p > 0 )
                return -2;  

            // remove cell from the list of non-path cells
            if (old_p == 0)
            {
             // // debugging
             // if (path[0].len == 0)
             //     ;  // length must be nonzero

                path[0].len--;  
            }

         // // debugging
         // if (max_pathnum == path.GetUpperBound(0))
         //     ; // not possible under normal circumstances


            // create a new path
            max_pathnum++;


            // attach a cell to a path
            cell[r, c].setPathnum(max_pathnum);


         // // debugging
         // for (int pn = 1; pn <= max_pathnum; pn++)
         // {
         //     bool found = false;
         //     for (uint _r = 0; _r < rowcnt && !found; _r++)
         //         for (uint _c = 0; _c < colcnt && !found; _c++)
         //             if (cell[_r, _c].getPathnum() == pn)
         //                 found = true;
         //     if (!found)
         //         ; // not posssible under normal circumstances
         // }


            // set path attributes
            path[max_pathnum].len = 1;
            path[max_pathnum].iscircular = false;
            path[max_pathnum].begrow = r;
            path[max_pathnum].begcol = c;
            path[max_pathnum].endrow = r;
            path[max_pathnum].endcol = c;

            return max_pathnum;
        }

        // extend a path by adding one cell
        private bool extendPath(int p, uint r, uint c)
        {
            // path will be extended by a cell only if:
            //    - a cell does not belong to a path, and it is uncircled
            //    - a cell is adjacent to the beginning or ending cell of the path
            int old_p = cell[r, c].getPathnum();

         // // debugging
         // if (old_p == -1)
         //     ;  // possible under normal circumstances

         // // debugging
         // if (path[p].iscircular)
         //     ; // a cell can't be added to a circular path

            bool adjacentBeg = areAdjacent(r, c, path[p].begrow, path[p].begcol);
            bool adjacentEnd = areAdjacent(r, c, path[p].endrow, path[p].endcol);
            if ( old_p > 0 || !(adjacentBeg || adjacentEnd))
                return false;

            // remove cell from the list of non-path cells
            if (old_p == 0)
            {
             // // debugging
             // if (path[0].len == 0)
             //     ; // length must be nonzero

                path[0].len--;
            }

            // attach cell to the path
            cell[r, c].setPathnum(p);

            // adjust path attributes
            path[p].len++;
            if (adjacentEnd)
            {
                path[p].endrow = r;
                path[p].endcol = c;
            }
            else
            {
                path[p].begrow = r;
                path[p].begcol = c;
            }

         // // debugging
         // if (   !isPathTerminator(path[p].begrow, path[p].begcol)
         //     || !isPathTerminator(path[p].endrow, path[p].endcol))
         //     ; // not possible under normal circumstances

            // check if path has become circular
            if ( !path[p].iscircular && path[p].len >= 4 && areAdjacent(path[p].begrow, path[p].begcol, path[p].endrow, path[p].endcol) )
            {
                path[p].iscircular = true;
            }

            return true;
        }

        // merge two paths
        private void mergePaths(uint r1, uint c1, uint r2, uint c2)
        {
            if (r1 == c1 && r2 == c2)
                return; // nothing to do

            int p1 = cell[r1, c1].getPathnum();
            int p2 = cell[r2, c2].getPathnum();
            if (p1 <= 0 && p2 <= 0)
            {
                int new_p = makePath(r1, c1);

             // // debugging
             // if (new_p <= 0)
             //     ; // not possible under normal circumstances

                extendPath(new_p, r2, c2);
            }
            else if (p1 <= 0)
            {
                extendPath(p2, r1, c1);
            }
            else if (p2 <= 0)
            {
                extendPath(p1, r2, c2);
            }
            else
                renumPath(p1, p2);
        }

        private void checkPathMerging(uint row, uint col)
        {
            Cell c = cell[row, col];

            if (c.getWallCnt() == 4) // a cell with all walls is not a candidate for path
                return;

            if (c.getWallCnt() >= 2)
            {
                if (c.getPathnum() <= 0)
                    makePath(row, col);

                uint sur_cnt = 0;                 // number of surrounding paths to merge with
                uint[] sur_row = { 0, 0, 0, 0 };  // max 2 expected, 4 defined just in case of error
                uint[] sur_col = { 0, 0, 0, 0 };
                if (row > 0          && !c.hasWall(Cell.top) && c.getPathnum() != cell[row - 1, col].getPathnum() && cell[row - 1, col].getWallCnt() >= 2)  { sur_row[sur_cnt] = row - 1; sur_col[sur_cnt] = col;     sur_cnt++; }
                if (row < rowcnt - 1 && !c.hasWall(Cell.bot) && c.getPathnum() != cell[row + 1, col].getPathnum() && cell[row + 1, col].getWallCnt() >= 2)  { sur_row[sur_cnt] = row + 1; sur_col[sur_cnt] = col;     sur_cnt++; }
                if (col > 0          && !c.hasWall(Cell.lef) && c.getPathnum() != cell[row, col - 1].getPathnum() && cell[row, col - 1].getWallCnt() >= 2)  { sur_row[sur_cnt] = row;     sur_col[sur_cnt] = col - 1; sur_cnt++; }
                if (col < colcnt - 1 && !c.hasWall(Cell.rig) && c.getPathnum() != cell[row, col + 1].getPathnum() && cell[row, col + 1].getWallCnt() >= 2)  { sur_row[sur_cnt] = row;     sur_col[sur_cnt] = col + 1; sur_cnt++; }

             // // debugging
             // if (sur_cnt > 2)
             //     ; // not possible under normal circumstances

                if (sur_cnt == 2 )
                {
                    // if two surrounding paths exist and have the same pathnum, merging with the new cell will result in circular path
                    if (c.getPathnum() != cell[sur_row[0], sur_col[0]].getPathnum() 
                                       && cell[sur_row[0], sur_col[0]].getPathnum() == cell[sur_row[1], sur_col[1]].getPathnum())
                       path[c.getPathnum()].iscircular = true;
                    else
                       mergePaths(row, col, sur_row[1], sur_col[1]);
                }
                if( sur_cnt > 0 )
                    mergePaths(row, col, sur_row[0], sur_col[0]);
            }
        }

        // return row and col of the first passable adjacent cell having NextPathnum as pathnum
        private Tuple<bool,uint,uint> getNextInPath(uint row, uint col, int NextPathnum)
        {
            Cell c = cell[row, col];

            // path does not exist if wall count is less then 2
            if (c.getWallCnt() < 2)
                return new Tuple<bool, uint, uint>(false,row, col);

            if (col > 0 && !c.hasWall(Cell.lef) && cell[row, col - 1].getPathnum() == NextPathnum)
                return new Tuple<bool, uint, uint>(true, row, col - 1);
            if (col < colcnt - 1 && !c.hasWall(Cell.rig) && cell[row, col + 1].getPathnum() == NextPathnum)
                return new Tuple<bool, uint, uint>(true, row, col + 1);
            if (row > 0 && !c.hasWall(Cell.top) && cell[row - 1, col].getPathnum() == NextPathnum)
                return new Tuple<bool, uint, uint>(true, row - 1, col);
            if (row < rowcnt - 1 && !c.hasWall(Cell.bot) && cell[row + 1, col].getPathnum() == NextPathnum)
                return new Tuple<bool, uint, uint>(true, row + 1, col);

            return new Tuple<bool, uint, uint>(false, row,col);
        }

        // remove the cell from path, and remove the complete path if it contains no more cells
        // 
        // prerequisite: the wall (which is causing the cell to become removed from the path) must NOT already be removed!!!!
        // prerequisite: the wall (which is causing the cell to become fully circled) must NOT already be added!!!
        //
        private void removeFromPath( uint row, uint col )
        {
            // detect all accessible surrounding cells having the same pathnum (max 2 expected, defined 4 just in case of error)
            int old_pathnum = cell[row, col].getPathnum();
            uint[] r = { 0, 0, 0, 0 };
            uint[] c = { 0, 0, 0, 0 };
            uint samepathCellCnt = 0;
            if( old_pathnum > 0 )
            {
                if (row > 0          && !cell[row, col].hasWall(Cell.top) && cell[row - 1, col].getPathnum() == old_pathnum) { r[samepathCellCnt] = row - 1; c[samepathCellCnt] = col; samepathCellCnt++; }
                if (row < rowcnt - 1 && !cell[row, col].hasWall(Cell.bot) && cell[row + 1, col].getPathnum() == old_pathnum) { r[samepathCellCnt] = row + 1; c[samepathCellCnt] = col; samepathCellCnt++; }
                if (col > 0          && !cell[row, col].hasWall(Cell.lef) && cell[row, col - 1].getPathnum() == old_pathnum) { r[samepathCellCnt] = row; c[samepathCellCnt] = col - 1; samepathCellCnt++; }
                if (col < colcnt - 1 && !cell[row, col].hasWall(Cell.rig) && cell[row, col + 1].getPathnum() == old_pathnum) { r[samepathCellCnt] = row; c[samepathCellCnt] = col + 1; samepathCellCnt++; }
            }

            // get the next cell in the same path (must be done before removing the cell from the path)
            Tuple<bool, uint, uint> res;
            res = getNextInPath(row, col, old_pathnum);
            bool NextExists = res.Item1;
            uint next_row = res.Item2;
            uint next_col = res.Item3;

            // remove the cell from the path (actually, from any path)
            if (cell[row, col].getWallCnt() == 3)  // cell that will be fully circled
                cell[row, col].setPathnum(-1);
            else
            {
                cell[row, col].setPathnum(0);
                path[0].len++;
            }

         // // debugging
         // if (path[old_pathnum].len == 0)
         //     ; // length must be nonzero

            path[old_pathnum].len--;

            if (samepathCellCnt == 0)
                renumMaxPath(old_pathnum);          // the cell is the last one in the path, remove the whole path
            else if (samepathCellCnt == 1)
            {
                // the cell is on the path end

                if (row == path[old_pathnum].begrow && col == path[old_pathnum].begcol)
                {
                    path[old_pathnum].begrow = next_row;
                    path[old_pathnum].begcol = next_col;
                }
                else
                {
                    path[old_pathnum].endrow = next_row;
                    path[old_pathnum].endcol = next_col;
                }


             // // debugging
             // if (   !isPathTerminator(path[old_pathnum].begrow, path[old_pathnum].begcol)
             //     || !isPathTerminator(path[old_pathnum].endrow, path[old_pathnum].endcol))
             //     ; // not possible under normal circumstances

            }
            else if (samepathCellCnt == 2)
                breakPath(r[0], c[0], r[1], c[1]);  // the cell is in the middle of the path, so the path must be broken (one part should be renum)

         // // debugging
         // else
         //     ; // not possible to have more than 2 surrounding cells in the same path     
        }

        // break path in two
        //     - starting from the beg cell, choose and follow one part of the path, and update the pathnum to the max
        //     - if the path was circular (if it finishes on the wrong end cell), undo the whole operation
        //
        // prerequisite: "beg" cell and "wrongEnd" cell must be separated (either by another cell or by common wall)
        // 
        private void breakPath(uint begrow, uint begcol, uint wrongEndrow, uint wrongEndcol)
        {
            uint r = begrow;
            uint c = begcol;
            int old_pathnum = cell[r, c].getPathnum();

            if (old_pathnum <= 0)  // path does not exist
                return;

         // // debugging
         // if (max_pathnum == path.GetUpperBound(0))
         //     ; // not possible under normal circumstances

            if (path[old_pathnum].iscircular)
            {
             // path[old_pathnum].len
                path[old_pathnum].iscircular = false;
                path[old_pathnum].begrow = begrow;
                path[old_pathnum].begcol = begcol;
                path[old_pathnum].endrow = wrongEndrow;
                path[old_pathnum].endcol = wrongEndcol;
                return;
            }

            max_pathnum++;
            int new_pathnum = max_pathnum;
            path[new_pathnum].len = 0;

            path[new_pathnum].begrow = r;
            path[new_pathnum].begcol = c;

            Tuple<bool, uint, uint> res;
            bool NextExists = false;
            do
            {
                // set the new pathnum to the chosen part of the path
                cell[r, c].setPathnum(new_pathnum);
                path[new_pathnum].len++;

             // // debugging
             // if (path[old_pathnum].len == 0)
             //     ; // length must be nonzero

                path[old_pathnum].len--;

                path[new_pathnum].endrow = r;
                path[new_pathnum].endcol = c;

                // move to next cell in the chosen part of the path
                res = getNextInPath(r, c, old_pathnum);
                NextExists = res.Item1;
                r = res.Item2;
                c = res.Item3;
            }
            while (NextExists);

            if (path[old_pathnum].begrow == r && path[old_pathnum].begcol == c)
            {
                path[old_pathnum].begrow = wrongEndrow;
                path[old_pathnum].begcol = wrongEndcol;
            }
            else
            {
                path[old_pathnum].endrow = wrongEndrow;
                path[old_pathnum].endcol = wrongEndcol;
            }

            // the path was circular if it finished on the wrong end cell, so renumbering was a mistake, and should be restored
            if (r == wrongEndrow && c == wrongEndcol)
            {
                renumPath(new_pathnum, old_pathnum); // restore old numbering
                path[cell[r, c].getPathnum()].iscircular = false;
            }

         // // debugging
         // if (!isPathTerminator(path[old_pathnum].begrow, path[old_pathnum].begcol)
         //     || !isPathTerminator(path[old_pathnum].endrow, path[old_pathnum].endcol))
         //     ; // not possible under normal circumstances

        }

        #endregion



        #region Drawing

        // draw inactive wall grid
        public void draw(Graphics g, Pen pen, float x, float y, float a)
        {
            // draw horizontal and vertical lines of a grid
            float d = pen.Width/2;
            for( uint i = 0; i <= rowcnt; i++ ) g.DrawLine(pen, x - d,   y + i*a, x + colcnt*a + d, y + i*a         );   // horizontal lines
            for( uint i = 0; i <= colcnt; i++ ) g.DrawLine(pen, x + i*a, y - d,   x + i*a,          y + rowcnt*a + d);   // vertical lines

            if ((DotsAndBoxes.show & (DotsAndBoxes.show_cell_coordinates | DotsAndBoxes.show_path_numbers)) != 0)
            {
                Font rcFont = new Font("Arial", 7);
                SolidBrush rcBrush = new SolidBrush(Color.Blue);
                StringFormat rcFormat = new StringFormat();
                rcFormat.Alignment = StringAlignment.Near;
                rcFormat.LineAlignment = StringAlignment.Near;
                Rectangle rcRect = new Rectangle(0, 0, (int)(a - 2 * d), (int)(a - 2 * d));
                Pen penBeg = new Pen(Color.LightGray);
                Pen penEnd = new Pen(Color.LightGray);
                for (uint r = 0; r < rowcnt; r++)
                {
                    for (uint c = 0; c < colcnt; c++)
                    {
                        rcRect.X = (int)(x + d + c * a);
                        rcRect.Y = (int)(y + d + r * a);
                        rcRect.Width = (int)(a - 2 * d);
                        rcRect.Height = (int)(a - 2 * d);

                        if ((DotsAndBoxes.show & DotsAndBoxes.show_cell_coordinates) != 0)
                        {
                            // draw cell coordinates in top left corner of each cell
                            g.DrawString("" + r + "," + c, rcFont, rcBrush, rcRect, rcFormat);
                        }

                        if ((DotsAndBoxes.show & DotsAndBoxes.show_path_numbers) != 0)
                        {
                            // draw inner circle if a cell is at a path begining
                            // draw small rectangle a cell is at a path end
                            int p = cell[r, c].getPathnum();
                            if (p > 0)
                            {
                                rcRect.Inflate((int)(-a / 8), (int)(-a / 8));
                                if (r == path[p].begrow && c == path[p].begcol)
                                    g.DrawEllipse(penBeg, rcRect);
                                rcRect.Inflate((int)(-a / 16), (int)(-a / 16));
                                if (r == path[p].endrow && c == path[p].endcol)
                                    g.DrawRectangle(penEnd, rcRect);
                            }
                        }
                    }
                }
            }


            if ((DotsAndBoxes.show & DotsAndBoxes.show_game_info) != 0)
            {
                // display some statistical data
                StringFormat strFormat = new StringFormat();
                strFormat.Alignment = StringAlignment.Near;
                strFormat.LineAlignment = StringAlignment.Near;
                Rectangle rect1 = new Rectangle(0, 0, 1000, 1000);
                Font strFont = new Font("Arial", 10);
                SolidBrush strBrush = new SolidBrush(Color.Red);
                string str = "";
                str += "------------------------------------------\n";
                str += "row count = " + rowcnt + "\n";
                str += "col count = " + colcnt + "\n";
                str += "------------------------------------------\n";
                str += "circled cells by player0 = " + circledcnt[0] + "\n";
                str += "circled cells by player1 = " + circledcnt[1] + "\n";
                str += "------------------------------------------\n";
                str += "cells having 0 walls = " + cellcnt_having_wallcnt[0] + "\n";
                str += "cells having 1 wall  = " + cellcnt_having_wallcnt[1] + "\n";
                str += "cells having 2 walls = " + cellcnt_having_wallcnt[2] + "\n";
                str += "cells having 3 walls = " + cellcnt_having_wallcnt[3] + "\n";
                str += "cells having 4 walls = " + cellcnt_having_wallcnt[4] + "\n";
                str += "------------------------------------------\n";
                str += "moves left = " + moves2play + "\n";
                // str += "safe_moves_left = " + safe_moves2play + "\n";
                // str += "safecell_cnt =" + (cellcnt_having_wallcnt[0]+ cellcnt_having_wallcnt[1]) + "\n";
                str += "safe moves left = " + getSafeMoveCnt() + "\n";
                str += "------------------------------------------\n";
                str += "max_pathnum = " + max_pathnum + "\n";
                str += "min pathlen = " + getMinPathlen() + "\n";
                str += "no-path cells = " + path[0].len + "\n";
                for (int i = 1; i <= max_pathnum; i++)
                {
                    str += "path[" + i + "]\t len = " + path[i].len + "\t beg = " + path[i].begrow + ',' + path[i].begcol + "\t end = " + path[i].endrow + ',' + path[i].endcol;
                    if (path[i].iscircular)
                        str += "\t circular";
                    str += "\n";
                }
                str += "------------------------------------------\n";

                g.DrawString(str, strFont, strBrush, rect1, strFormat);
            }
        }

        #endregion


    }
}
