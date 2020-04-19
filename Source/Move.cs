namespace DotsAndBoxes
{
    class Move
    {
        // move state
        public int player_id { get; set; }
        public uint row { get; set; }
        public uint col { get; set; }
        public uint wall { get; set; }


        // create a move given the player id, row, col and orientation of the added wall
        public Move(int player_id, uint row, uint col, uint wall)
        {
            this.player_id = player_id;
            this.row = row;
            this.col = col;
            this.wall = wall;
        }

        // return string representation of move
        public override string ToString()
        {
            char a = ' ';
            long i = 0;

            if( wall == Cell.lef || wall == Cell.rig )
            {
                a = (char)('A' + row);
                i =              col + (wall == Cell.rig ? 1 : 0);
                return "" + a + i;
            }
            else
            {
                i =              row + (wall == Cell.bot ? 1 : 0);
                a = (char)('A' + col);
                return "" + i + a;
            }
        }
    }
}
