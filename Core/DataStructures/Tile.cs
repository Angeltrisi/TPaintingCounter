namespace TPaintingCounter.Core.DataStructures
{
    public struct Tile()
    {
        public Tile(Tile t) : this()
        {
            Type = t.Type;
            WallType = t.WallType;
            FrameX = t.FrameX;
            FrameY = t.FrameY;
            IsActive = t.IsActive;
            Color = t.Color;
            WallColor = t.WallColor;
            LiquidAmount = t.LiquidAmount;
            LiquidType = t.LiquidType;
            WireType = t.WireType;
            SlopeType = t.SlopeType;
            IsActuated = t.IsActuated;
            HasActuator = t.HasActuator;
        }
        public ushort Type;
        public ushort WallType;
        public short FrameX;
        public short FrameY;
        public bool IsActive;
        public byte Color;
        public byte WallColor;
        public byte LiquidAmount;
        public LiquidType LiquidType;
        public WireType WireType;
        public SlopeType SlopeType;
        public bool IsActuated;
        public bool HasActuator;
    }
}
