namespace DotsAndBoxes
{
    class Path
    {
        // path state
        public uint len { get; set; } = 0;
        public bool iscircular { get; set; } = false;
        public bool isRepresentative { get; set; }

        // coordinates of the beg cell in a path
        public uint begrow { get; set; }  
        public uint begcol { get; set; }

        // coordinates of the end cell in a path
        public uint endrow { get; set; }
        public uint endcol { get; set; }

        // public constants
        public static readonly int Wrong            = -2;
        public static readonly int Circular         = -1;
        public static readonly int Open             =  0;
        public static readonly int ClosedAtOneEnd   =  1;
        public static readonly int ClosedAtBothEnds =  2;
    }
}
