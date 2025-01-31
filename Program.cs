using TPaintingCounter.Core;

static void Start()
{
    PaintingCounter.Reset();
    Console.WriteLine("Provide the path to the .wld file:");
    string? wldFileName = Console.ReadLine();
    if (wldFileName != null)
    {
        World w = World.Parse(wldFileName);
        if (w != null)
        {
            Console.WriteLine("Counting paintings...");
            var tiles = w.GetAllTiles().Where(t => PaintingCounter.PaintingTypes.Contains(t.Type));
            Console.WriteLine($"Total 2x3 painting tiles found: {tiles.Count(t => t.Type == PaintingCounter.Painting2x3)}");
            Console.WriteLine($"Total 3x2 painting tiles found: {tiles.Count(t => t.Type == PaintingCounter.Painting3x2)}");
            Console.WriteLine($"Total 3x3 painting tiles found: {tiles.Count(t => t.Type == PaintingCounter.Painting3x3)}");
            Console.WriteLine($"Total 6x4 painting tiles found: {tiles.Count(t => t.Type == PaintingCounter.Painting6x4)}");
            PaintingCounter.CountAllPaintings(tiles);
            foreach (var item in PaintingCounter.CountTotal)
            {
                Console.WriteLine($"{item.Key} : {item.Value}");
            }
            //Console.WriteLine($"This world has {tiles.Count(t => t.IsActive)} tiles.");
        }
        Start();
    }
    else
    {
        Start();
    }
}
Start();