using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using TPaintingCounter.Core.DataStructures;

namespace TPaintingCounter.Core
{
    public class World(int sizeX, int sizeY)
    {
        public Tile[,] tileMap = new Tile[sizeX, sizeY];
        public int maxTilesX = sizeX;
        public int maxTilesY = sizeY;

        public static World Parse(string filename)
        {
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string worldPath = Path.Combine(documentsPath, "My Games", "Terraria", "tModLoader", "Worlds");
            string vanillaWorldPath = Path.Combine(documentsPath, "My Games", "Terraria", "Worlds");

            string fullPath = Path.Combine(worldPath, $"{filename}.wld");
            string fullVanillaPath = Path.Combine(vanillaWorldPath, $"{filename}.wld");
            byte[] fileBytes = null;
            if (File.Exists(fullPath))
            {
                fileBytes = File.ReadAllBytes(fullPath);
                Console.WriteLine($"Read {fileBytes.Length} bytes from {fullPath}");
            }
            else if (File.Exists(fullVanillaPath))
            {
                fileBytes = File.ReadAllBytes(fullVanillaPath);
                Console.WriteLine($"Read {fileBytes.Length} bytes from {fullVanillaPath}");
            }
            else
            {
                Console.WriteLine($"File not found: {fullPath}");
                return null;
            }

            using MemoryStream stream = new(fileBytes);
            using BinaryReader reader = new(stream);
            reader.BaseStream.Position = 0L;

            // world version
            int worldVersion = reader.ReadInt32();
            Console.WriteLine($"World file version is {worldVersion}.");

            // magic id
            byte[] magicIdentifier = reader.ReadBytes(7);
            if (Encoding.UTF8.GetString(magicIdentifier) != "relogic")
                throw new Exception("This is not a valid Re-Logic file!");

            // filetype
            FileType fileType = (FileType)reader.ReadByte();
            Console.WriteLine($"File type is {fileType}.");

            // file revision
            uint fileRevision = reader.ReadUInt32();
            Console.WriteLine($"File revision is {fileRevision}");

            // is this a favorite world?
            bool isFavorite = (reader.ReadUInt64() & 1) == 1;
            string add = isFavorite ? string.Empty : "not ";
            Console.WriteLine($"This world is {add}marked as favorite.");

            // stream positions of different things in the world file
            short posSize = reader.ReadInt16();
            int[] positions = new int[posSize];
            for (int i = 0; i < posSize; i++)
            {
                positions[i] = reader.ReadInt32();
            }

            // tiles with important framing
            ushort importanceSize = reader.ReadUInt16();
            bool[] importance = new bool[importanceSize];
            byte by = 0;
            byte by2 = 128;
            for (int i = 0; i < importanceSize; i++)
            {
                if (by2 == 128)
                {
                    by = reader.ReadByte();
                    by2 = 1;
                }
                else
                {
                    by2 <<= 1;
                }
                if ((by & by2) == by2)
                {
                    importance[i] = true;
                }
            }

            // world name
            string worldName = reader.ReadString();
            Console.WriteLine($"World Name: {worldName}");

            string seed = string.Empty;
            Guid guid;
            if (worldVersion >= 179)
            {
                seed = (worldVersion != 179) ? reader.ReadString() : reader.ReadInt32().ToString();
                ulong worldGenVersion = reader.ReadUInt64();
                if (worldVersion >= 181)
                    guid = new Guid(reader.ReadBytes(16));
            }
            Console.WriteLine($"Seed: {seed}");

            // load a bunch of world info
            int worldID = reader.ReadInt32();
            int leftWorld = reader.ReadInt32();
            int rightWorld = reader.ReadInt32();
            int topWorld = reader.ReadInt32();
            int bottomWorld = reader.ReadInt32();
            int maxTilesY = reader.ReadInt32();
            int maxTilesX = reader.ReadInt32();

            // world special seed and mode data
            GameMode gameMode;
            bool drunkWorld = false;
            bool forTheWorthyWorld = false;
            bool celebrationMk10World = false;
            bool theConstantWorld = false;
            bool notTheBeesWorld = false;
            bool dontDigUpWorld = false;
            bool noTrapsWorld = false;
            bool getFixedBoiWorld = false;

            if (worldVersion >= 209)
            {
                gameMode = (GameMode)reader.ReadInt32();

                if (worldVersion >= 222)
                    drunkWorld = reader.ReadBoolean();

                if (worldVersion >= 227)
                    forTheWorthyWorld = reader.ReadBoolean();

                if (worldVersion >= 238)
                    celebrationMk10World = reader.ReadBoolean();

                if (worldVersion >= 239)
                    theConstantWorld = reader.ReadBoolean();

                if (worldVersion >= 241)
                    notTheBeesWorld = reader.ReadBoolean();

                if (worldVersion >= 249)
                    dontDigUpWorld = reader.ReadBoolean();

                if (worldVersion >= 266)
                    noTrapsWorld = reader.ReadBoolean();

                if (worldVersion >= 267)
                    getFixedBoiWorld = reader.ReadBoolean();
                else
                    getFixedBoiWorld = dontDigUpWorld && drunkWorld;
            }
            else
            {
                if (worldVersion >= 112)
                    gameMode = reader.ReadBoolean() ? GameMode.Expert : GameMode.Classic;
                else
                    gameMode = GameMode.Classic;

                if (worldVersion == 208 && reader.ReadBoolean())
                    gameMode = GameMode.Master;
            }

            // world creation date
            DateTime worldCreation;
            if (worldVersion >= 141)
                worldCreation = DateTime.FromBinary(reader.ReadInt64());

            // a bunch of useless stuff (this will be tedious)

            // moon type
            byte moonType = reader.ReadByte();

            // idk what this is other than tree
            int treeX0 = reader.ReadInt32();
            int treeX1 = reader.ReadInt32();
            int treeX2 = reader.ReadInt32();

            // tree styles?? idk what that means either
            int treeStyle0 = reader.ReadInt32();
            int treeStyle1 = reader.ReadInt32();
            int treeStyle2 = reader.ReadInt32();
            int treeStyle3 = reader.ReadInt32();

            // cave backgrounds
            int caveBackX0 = reader.ReadInt32();
            int caveBackX1 = reader.ReadInt32();
            int caveBackX2 = reader.ReadInt32();

            // cave style apparently
            int caveStyle0 = reader.ReadInt32();
            int caveStyle1 = reader.ReadInt32();
            int caveStyle2 = reader.ReadInt32();
            int caveStyle3 = reader.ReadInt32();

            // biome background styles
            int iceBackStyle = reader.ReadInt32();
            int jungleBackStyle = reader.ReadInt32();
            int underworldBackStyle = reader.ReadInt32();

            // spawn tile position
            int spawnTileX = reader.ReadInt32();
            int spawnTileY = reader.ReadInt32();

            // world layers
            double worldSurface = reader.ReadDouble();


            /*


            double rockLayer = reader.ReadDouble();

            // time-related stuff
            double time = reader.ReadDouble();
            bool dayTime = reader.ReadBoolean();
            int moonPhase = reader.ReadInt32();
            bool bloodMoon = reader.ReadBoolean();
            bool eclipse = reader.ReadBoolean();

            // dungeon position
            int dungeonX = reader.ReadInt32();
            int dungeonY = reader.ReadInt32();

            // flags

            // worldgen
            bool crimson = reader.ReadBoolean();

            // downed bosses
            bool downedEyeOfCthulhu = reader.ReadBoolean();
            bool downedEvilBoss = reader.ReadBoolean();
            bool downedSkeletron = reader.ReadBoolean();
            bool downedQueenBee = reader.ReadBoolean();
            bool downedDestroyer = reader.ReadBoolean();
            bool downedTheTwins = reader.ReadBoolean();
            bool downedSkeletronPrime = reader.ReadBoolean();
            bool downedAnyMechBoss = reader.ReadBoolean(); // redcode ahh, why isn't this just downedDestoyer || downedTheTwins || downedSkeletronPrime
            bool downedPlantera = reader.ReadBoolean();
            bool downedGolem = reader.ReadBoolean();
            bool downedKingSlime = false;
            if (worldVersion >= 118)
                downedKingSlime = reader.ReadBoolean();

            // saved NPCs
            bool savedGoblinTinkerer = reader.ReadBoolean();
            bool savedWizard = reader.ReadBoolean();
            bool savedMechanic = reader.ReadBoolean();

            // downed special
            bool downedGoblinArmy = reader.ReadBoolean();
            bool downedClown = reader.ReadBoolean();
            bool downedFrostLegion = reader.ReadBoolean();
            bool downedPirateInvasion = reader.ReadBoolean();

            // more world flags
            bool shadowOrbSmashes = reader.ReadBoolean();
            bool shouldSpawnMeteor = reader.ReadBoolean();
            byte shadowOrbCount = reader.ReadByte();
            int evilAltarCount = reader.ReadInt32();

            bool hardMode = reader.ReadBoolean();

            //ReadUselessStuff(reader, worldVersion);


            */


            reader.BaseStream.Position = positions[1];

            World world = new(maxTilesX, maxTilesY);
            for (int i = 0; i < maxTilesX; i++)
            {
                for (int j = 0; j < maxTilesY; j++)
                {
                    int num2 = -1;
                    byte b2;
                    byte b;
                    byte b3 = (b2 = (b = 0));
                    Tile tile = world.tileMap[i, j];
                    byte b4 = reader.ReadByte();
                    bool flag = false;
                    if ((b4 & 1) == 1)
                    {
                        flag = true;
                        b3 = reader.ReadByte();
                    }
                    bool flag2 = false;
                    if (flag && (b3 & 1) == 1)
                    {
                        flag2 = true;
                        b2 = reader.ReadByte();
                    }
                    if (flag2 && (b2 & 1) == 1)
                    {
                        b = reader.ReadByte();
                    }
                    byte b5;
                    if ((b4 & 2) == 2)
                    {
                        tile.IsActive = true;
                        if ((b4 & 0x20) == 32)
                        {
                            b5 = reader.ReadByte();
                            num2 = reader.ReadByte();
                            num2 = (num2 << 8) | b5;
                        }
                        else
                        {
                            num2 = reader.ReadByte();
                        }
                        tile.Type = (ushort)num2;
                        if (importance[num2])
                        {
                            tile.FrameX = reader.ReadInt16();
                            tile.FrameY = reader.ReadInt16();
                            if (tile.Type == 144)
                            {
                                tile.FrameY = 0;
                            }
                        }
                        else
                        {
                            tile.FrameX = -1;
                            tile.FrameY = -1;
                        }
                        if ((b2 & 8) == 8)
                        {
                            tile.Color = reader.ReadByte();
                        }
                    }
                    if ((b4 & 4) == 4)
                    {
                        tile.WallType = reader.ReadByte();
                        if ((b2 & 0x10) == 16)
                        {
                            tile.WallColor = reader.ReadByte();
                        }
                    }
                    b5 = (byte)((b4 & 0x18) >> 3);
                    if (b5 != 0)
                    {
                        tile.LiquidAmount = reader.ReadByte();
                        if ((b2 & 0x80) == 128)
                        {
                            tile.LiquidType = LiquidType.Shimmer;
                        }
                        else if (b5 > 1)
                        {
                            if (b5 == 2)
                            {
                                tile.LiquidType = LiquidType.Lava;
                            }
                            else
                            {
                                tile.LiquidType = LiquidType.Honey;
                            }
                        }
                    }
                    if (b3 > 1)
                    {
                        if ((b3 & 2) == 2)
                        {
                            tile.WireType |= WireType.Red;;
                        }
                        if ((b3 & 4) == 4)
                        {
                            tile.WireType |= WireType.Blue;
                        }
                        if ((b3 & 8) == 8)
                        {
                            tile.WireType |= WireType.Green;
                        }
                        b5 = (byte)((b3 & 0x70) >> 4);
                        if (b5 != 0) //&& (Main.tileSolid[tile.type] || TileID.Sets.NonSolidSaveSlopes[tile.type]))
                        {
                            if (b5 == 1)
                            {
                                tile.SlopeType = SlopeType.HalfBlock;
                            }
                            else
                            {
                                tile.SlopeType = (SlopeType)(b5 - 1);
                            }
                        }
                    }
                    if (b2 > 1)
                    {
                        if ((b2 & 2) == 2)
                        {
                            tile.HasActuator = true;
                        }
                        if ((b2 & 4) == 4)
                        {
                            tile.IsActuated = true;
                        }
                        if ((b2 & 0x20) == 32)
                        {
                            tile.WireType |= WireType.Yellow;
                        }
                        if ((b2 & 0x40) == 64)
                        {
                            b5 = reader.ReadByte();
                            tile.WallType = (ushort)((b5 << 8) | tile.WallType);
                        }
                    }
                    if (b > 1)
                    {
                        if ((b & 2) == 2)
                        {
                            //tile.invisibleBlock(invisibleBlock: true);
                        }
                        if ((b & 4) == 4)
                        {
                            //tile.invisibleWall(invisibleWall: true);
                        }
                        if ((b & 8) == 8)
                        {
                            //tile.fullbrightBlock(fullbrightBlock: true);
                        }
                        if ((b & 0x10) == 16)
                        {
                            //tile.fullbrightWall(fullbrightWall: true);
                        }
                    }
                    int num3 = (byte)((b4 & 0xC0) >> 6) switch
                    {
                        0 => 0,
                        1 => reader.ReadByte(),
                        _ => reader.ReadInt16(),
                    };
                    if (num2 != -1)
                    {
                        if ((double)j <= worldSurface)
                        {
                            if ((double)(j + num3) <= worldSurface)
                            {
                                //WorldGen.tileCounts[num2] += (num3 + 1) * 5;
                            }
                            else
                            {
                                int num4 = (int)(worldSurface - (double)j + 1.0);
                                int num5 = num3 + 1 - num4;
                                //WorldGen.tileCounts[num2] += num4 * 5 + num5;
                            }
                        }
                        else
                        {
                            //WorldGen.tileCounts[num2] += num3 + 1;
                        }
                    }
                    // i have no idea what RLE is but i'll do some research afterwards.
                    // i will thank the robots (chyattCBT) for fixing the logic here.
                    // the original while loop was causing tiles to be overwritten
                    for (int k = 0; k <= num3; k++)
                    {
                        int yPos = j + k;
                        if (yPos < maxTilesY)
                        {
                            world.tileMap[i, yPos] = tile;
                        }
                    }
                    j += num3;

                }
            }
            return world;
        }
        public static void ReadUselessStuff(BinaryReader reader, int worldVersion)
        {

            // special seed flags
            bool afterPartyOfDoom = false;
            if (worldVersion >= 257)
                afterPartyOfDoom = reader.ReadBoolean();

            // invasion stuff
            int invasionDelay = reader.ReadInt32();
            int invasionSize = reader.ReadInt32();
            int invasionType = reader.ReadInt32();
            double invasionX = reader.ReadDouble();

            double slimeRainTime;
            if (worldVersion >= 118)
                slimeRainTime = reader.ReadDouble();

            // more special stuff
            byte sundialCooldown;
            if (worldVersion >= 113)
                sundialCooldown = reader.ReadByte();

            // rain
            bool raining = reader.ReadBoolean();
            int rainTime = reader.ReadInt32();
            float maxRain = reader.ReadSingle();

            // hardmode ores
            int cobalt = reader.ReadInt32();
            int mythril = reader.ReadInt32();
            int adamantite = reader.ReadInt32();

            // background styles (i still don't know wtf any of the background stuff is or why it needs to be stored in the world file)
            byte bg0 = reader.ReadByte();
            byte bg1 = reader.ReadByte();
            byte bg2 = reader.ReadByte();
            byte bg3 = reader.ReadByte();
            byte bg4 = reader.ReadByte();
            byte bg5 = reader.ReadByte();
            byte bg6 = reader.ReadByte();
            byte bg7 = reader.ReadByte();

            int cloudBgActive = reader.ReadInt32();
            short numClouds = reader.ReadInt16();

            // wind
            float windSpeedTarget = reader.ReadSingle();

            // angler
            List<string> anglerWhoFishedToday = [];
            bool savedAngler = false;
            int anglerQuest;
            if (worldVersion >= 95)
            {
                for (int i = reader.ReadInt32(); i > 0; i--)
                {
                    anglerWhoFishedToday.Add(reader.ReadString());
                }
            }
            if (worldVersion >= 99)
            {
                savedAngler = reader.ReadBoolean();
            }
            if (worldVersion >= 101)
            {
                anglerQuest = reader.ReadInt32();
            }
            // other NPCs
            bool savedStylist = false;
            if (worldVersion >= 104)
            {
                savedStylist = reader.ReadBoolean();
            }
            bool savedTaxCollector = false;
            if (worldVersion >= 129)
            {
                savedTaxCollector = reader.ReadBoolean();
            }
            bool savedGolfer = false;
            if (worldVersion >= 201)
            {
                savedGolfer = reader.ReadBoolean();
            }
            // misc random stuff
            int invasionSizeStart;
            if (worldVersion >= 107)
            {
                invasionSizeStart = reader.ReadInt32();
            }
            int cultistDelay;
            if (worldVersion >= 108)
            {
                cultistDelay = reader.ReadInt32();
            }
            if (worldVersion >= 109)
            {
                int killCountAmount = reader.ReadInt16();
                for (int i = 0; i < killCountAmount; i++)
                {
                    // i don't care about NPCs so i'm not making stuff for NPCs. still have to read stuff though.
                    reader.ReadInt32();
                }
            }
            bool fastForwardToDawn = false;
            if (worldVersion >= 128)
            {
                fastForwardToDawn = reader.ReadBoolean();
            }
            // wow! more downed flags!
            bool downedDukeFishron = reader.ReadBoolean();
            bool downedMartianInvasion = reader.ReadBoolean();
            bool downedLunaticCultist = reader.ReadBoolean();
            bool downedMoonLord = reader.ReadBoolean();
            bool downedPumpking = reader.ReadBoolean();
            bool downedMourningWood = reader.ReadBoolean();
            bool downedIceQueen = reader.ReadBoolean();
            bool downedSantaNK1 = reader.ReadBoolean();
            bool downedEverscream = reader.ReadBoolean();
            bool downedSolarPillar = false;
            bool downedVortexPillar = false;
            bool downedNebulaPillar = false;
            bool downedStardustPillar = false;
            bool solarPillarActive = false;
            bool vortexPillarActive = false;
            bool nebulaPillarActive = false;
            bool stardustPillarActive = false;
            bool pillarInvasionActive = false;
            if (worldVersion >= 140)
            {
                downedSolarPillar = reader.ReadBoolean();
                downedVortexPillar = reader.ReadBoolean();
                downedNebulaPillar = reader.ReadBoolean();
                downedStardustPillar = reader.ReadBoolean();
                solarPillarActive = reader.ReadBoolean();
                vortexPillarActive = reader.ReadBoolean();
                nebulaPillarActive = reader.ReadBoolean();
                stardustPillarActive = reader.ReadBoolean();
                pillarInvasionActive = reader.ReadBoolean();
            }
            // party stuff
            bool partyManual = false;
            bool partyGenuine = false;
            int partyCooldown;
            if (worldVersion >= 170)
            {
                partyManual = reader.ReadBoolean();
                partyGenuine = reader.ReadBoolean();
                partyCooldown = reader.ReadInt32();
                int partyCelebratingNPCs = reader.ReadInt32();
                for (int i = 0; i < partyCelebratingNPCs; i++)
                {
                    reader.ReadInt32();
                }
            }
            // sandstorm stuff
            bool sandstormHappening = false;
            int sandstormTimeLeft = 0;
            float sandstormSeverity = 0f;
            float sandstormIntendedSeverity = 0f;

            if (worldVersion >= 174)
            {
                sandstormHappening = reader.ReadBoolean();
                sandstormTimeLeft = reader.ReadInt32();
                sandstormSeverity = reader.ReadSingle();
                sandstormIntendedSeverity = reader.ReadSingle();
            }

            // ooa stuff
            bool savedBarkeep = false;
            bool downedOOAt1 = false;
            bool downedOOAt2 = false;
            bool downedOOAt3 = false;

            if (worldVersion >= 178)
            {
                savedBarkeep = reader.ReadBoolean();
                downedOOAt1 = reader.ReadBoolean();
                downedOOAt2 = reader.ReadBoolean();
                downedOOAt3 = reader.ReadBoolean();
            }

            // more backgrounds wow
            byte bg8;
            if (worldVersion > 194)
                bg8 = reader.ReadByte();
            byte bg9;
            if (worldVersion >= 215)
                bg9 = reader.ReadByte();
            byte bg10;
            byte bg11;
            byte bg12;
            if (worldVersion > 195)
            {
                bg10 = reader.ReadByte();
                bg11 = reader.ReadByte();
                bg12 = reader.ReadByte();
            }

            // more more flags
            bool advancedCombatTechniquesUsed = false;
            if (worldVersion >= 204)
                advancedCombatTechniquesUsed = reader.ReadBoolean();

            // lantern night
            int lanternNightCooldown;
            bool lanternNightGenuine = false;
            bool lanternNightManual = false;
            bool lanternNightNextNightIsGenuine = false;

            if (worldVersion >= 207)
            {
                lanternNightCooldown = reader.ReadInt32();
                lanternNightGenuine = reader.ReadBoolean();
                lanternNightManual = reader.ReadBoolean();
                lanternNightNextNightIsGenuine = reader.ReadBoolean();
            }

            // load treetops? (wtf is this gameeee)
            // i'll leave this here for later. for now i'll just skip
        }
        public IEnumerable<Tile> GetAllTiles()
        {
            for (int i = 0; i < maxTilesX; i++)
            {
                for (int j = 0; j < maxTilesY; j++)
                {
                    yield return tileMap[i, j];
                }
            }
        }
    }
}
