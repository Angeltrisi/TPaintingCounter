﻿using TPaintingCounter.Core.DataStructures;

namespace TPaintingCounter.Core
{
    public static class PaintingCounter
    {
        public static readonly Dictionary<PaintingType2x3, int> Count2x3 = [];
        public static readonly Dictionary<PaintingType3x2, int> Count3x2 = [];
        public static readonly Dictionary<PaintingType3x3, int> Count3x3 = [];
        public static readonly Dictionary<PaintingType6x4, int> Count6x4 = [];
        public static readonly Dictionary<string, int> CountTotal = [];
        public const ushort Painting2x3 = 245;
        public const ushort Painting3x2 = 246;
        public const ushort Painting3x3 = 240;
        public const ushort Painting6x4 = 242;
        public static readonly ushort[] PaintingTypes = [Painting2x3, Painting3x2, Painting3x3, Painting6x4];
        // 2x3's spritesheet doesn't wrap
        public const byte Painting2x3AmountPerRow = 255;
        // 3x2 only has one painting per row in the spritesheet
        public const byte Painting3x2AmountPerRow = 1;
        // 3x3 has 36 paintings per row in the spritesheet
        public const byte Painting3x3AmountPerRow = 36;
        // 6x4's spritesheet is ordered differently to the other two: first it goes from top to bottom, then goes to the next column. it has 27 paintings per column
        public const byte Painting6x4AmountPerColumn = 27;
        public static void Reset()
        {
            CountTotal.Clear();
            Count2x3.Clear();
            Count3x2.Clear();
            Count3x3.Clear();
            Count6x4.Clear();
        }
        public static void CountAllPaintings(IEnumerable<Tile> tiles)
        {
            PaintingType2x3[] possible2x3Paintings = Enum.GetValues<PaintingType2x3>();
            PaintingType3x2[] possible3x2Paintings = Enum.GetValues<PaintingType3x2>();
            PaintingType3x3[] possible3x3Paintings = Enum.GetValues<PaintingType3x3>();
            PaintingType6x4[] possible6x4Paintings = Enum.GetValues<PaintingType6x4>();

            foreach (var tile in tiles)
            {
                if (tile.Type == Painting2x3)
                {
                    int xSize = 36;
                    if (tile.FrameX % xSize == 0)
                    {
                        int painting = tile.FrameX / xSize; // this gives us the painting type. no extra calcs needed
                        PaintingType2x3 paintingType = (PaintingType2x3)painting;
                        if (possible2x3Paintings.Contains(paintingType))
                        {
                            if (Count2x3.TryGetValue(paintingType, out int value))
                            {
                                CountTotal[paintingType.ToString()] = Count2x3[paintingType] = ++value;
                                continue;
                            }
                            CountTotal[paintingType.ToString()] = Count2x3[paintingType] = 1;
                        }
                    }
                    continue;
                }
                if (tile.Type == Painting3x2)
                {
                    int ySize = 36;
                    if (tile.FrameY % ySize == 0)
                    {
                        int painting = tile.FrameY / ySize; // this once again gives us the painting type. no extra calcs needed
                        PaintingType3x2 paintingType = (PaintingType3x2)painting;
                        if (possible3x2Paintings.Contains(paintingType))
                        {
                            if (Count3x2.TryGetValue(paintingType, out int value))
                            {
                                CountTotal[paintingType.ToString()] = Count3x2[paintingType] = ++value;
                                continue;
                            }
                            CountTotal[paintingType.ToString()] = Count3x2[paintingType] = 1;
                        }
                    }
                    continue;
                }
                if (tile.Type == Painting3x3)
                {
                    int xSize = 54;
                    int ySize = 54;
                    if (tile.FrameX % xSize == 0 && tile.FrameY % ySize == 0)
                    {
                        int xPos = tile.FrameX / xSize;
                        int yPos = tile.FrameY / ySize;
                        int painting = xPos + (yPos * Painting3x3AmountPerRow); // e. g. 0 + (1 * 36) = 36, which gives us the mourning wood trophy (correct)
                        PaintingType3x3 paintingType = (PaintingType3x3)painting;
                        if (possible3x3Paintings.Contains(paintingType))
                        {
                            if (Count3x3.TryGetValue(paintingType, out int value))
                            {
                                CountTotal[paintingType.ToString()] = Count3x3[paintingType] = ++value;
                                continue;
                            }
                            CountTotal[paintingType.ToString()] = Count3x3[paintingType] = 1;
                        }
                    }
                    continue;
                }
                if (tile.Type == Painting6x4)
                {
                    int xSize = 108;
                    int ySize = 72;
                    if (tile.FrameX % xSize == 0 && tile.FrameY % ySize == 0)
                    {
                        int xPos = tile.FrameX / xSize;
                        int yPos = tile.FrameY / ySize;
                        int painting = yPos + (xPos * Painting6x4AmountPerColumn);
                        PaintingType6x4 paintingType = (PaintingType6x4)painting;
                        if (possible6x4Paintings.Contains(paintingType))
                        {
                            if (Count6x4.TryGetValue(paintingType, out int value))
                            {
                                CountTotal[paintingType.ToString()] = Count6x4[paintingType] = ++value;
                                continue;
                            }
                            CountTotal[paintingType.ToString()] = Count6x4[paintingType] = 1;
                        }
                    }
                    continue;
                }
            }
        }
    }
}
