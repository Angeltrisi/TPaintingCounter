using TPaintingCounter.Core;
using TPaintingCounter.Core.DataStructures;

unsafe void Start()
{
    PaintingCounter.Reset();
    Console.WriteLine("Provide the name of the .wld file: (The file will automatically be searched for in tModLoader's Worlds folder and Terraria's Worlds folder)");
    string? wldFileName = args.Length == 0 ? Console.ReadLine() : args[0][..^4];
    if (wldFileName == null)
    {
        Start();
        return;
    }

    World w = World.Parse(wldFileName);
    if (w == null)
    {
        Start();
        return;
    }
    Console.WriteLine("Counting paintings...");
    Tile[] tiles = w.GetAllTiles();
    int length = tiles.Length;
    /*
    int painting2x3count = 0, painting3x2count = 0, painting3x3count = 0, painting6x4count = 0;

    for (int i = 0; i < length; i++)
    {
        ref Tile t = ref tiles[i];
        if (!PaintingCounter.PaintingTypes.Contains(t.Type))
            return;

        switch (t.Type)
        {
            case PaintingCounter.Painting2x3:
                painting2x3count++;
                continue;
            case PaintingCounter.Painting3x2:
                painting3x2count++;
                continue;
            case PaintingCounter.Painting3x3:
                painting3x3count++;
                continue;
            case PaintingCounter.Painting6x4:
                painting6x4count++;
                continue;
            default:
                continue;
        }
    }
    Console.WriteLine($"Total 2x3 painting tiles found: {painting2x3count}");
    Console.WriteLine($"Total 3x2 painting tiles found: {painting3x2count}");
    Console.WriteLine($"Total 3x3 painting tiles found: {painting3x3count}");
    Console.WriteLine($"Total 6x4 painting tiles found: {painting6x4count}");
    Console.WriteLine();
    */
    fixed (Tile* tilePtr = tiles)
    {
        PaintingCounter.CountAllPaintings(tilePtr, length);
    }
    foreach (var item in PaintingCounter.CountTotal)
    {
        Console.WriteLine($"{item.Key} : {item.Value}");
    }
    Console.WriteLine();
    Console.WriteLine($"Unique paintings: {PaintingCounter.CountTotal.Count}");

    Console.WriteLine("Type anything to try with another .wld file.");
    string? option = Console.ReadLine();
}
Start();