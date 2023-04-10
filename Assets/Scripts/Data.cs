namespace AhmetsHub.ClashOfPirates
{
    using System.Xml.Serialization;
    using System.IO;
    using System.Collections.Generic;
    using System;
    using System.Threading.Tasks;

    public static class Data
    {
        public const int minGoldCollect = 10;
        public const int minFishCollect = 10;
        public static readonly int battleDuration = 120;
        public static readonly int gridSize = 45;
        public static readonly float gridCellSize = 1;
        public static readonly float battleFrameRate = 0.1f;
        public static readonly int battleTilesWorthOfOneWall = 15;
        public static readonly int battleGroupWallAttackRadius = 5;

        public class Player
        {
            public int gold = 0;
            public int fish = 0;
            public int diamonds = 0;
            public DateTime nowTime;
            public List<Building> buildings = new List<Building>();
            public List<Unit> units = new List<Unit>();
        }

        public enum UnitID
        {
            pirate, archer, goblin, healer, wallbreaker, giant, miner, balloon, wizard, dragon, pekka
        }

        public enum TargetPriority
        {
            none = 0, all = 1, defenses = 2, resources = 3, walls = 4
        }

        public enum BuildingTargetType
        {
            none = 0, ground = 1, air = 2, all = 3
        }

        public enum UnitMoveType
        {
            ground = 0, jump = 1, fly = 2, underground = 3
        }

        public enum BuildingID
        {
            islandhall, goldmine, goldstorage, fisher, fishstorage, buildershut, armycamp, barracks, wall, cannon, archertower, mortor, airdefense, wizardtower, hiddentesla, bombtower, xbow, infernotower, decoration, obstacle, boomb, springtrap, airbomb, giantbomb, seekingairmine, skeletontrap
        }

        public class BattleFrame
        {
            public int frame = 0;
            public List<BattleFrameUnit> units = new List<BattleFrameUnit>();
        }

        public class BattleFrameUnit
        {
            public long id = 0;
            public int x = 0;
            public int y = 0;
        }

        public class BattleData
        {
            public Battle battle = null;
            public List<BattleFrame> frames = new List<BattleFrame>();
        }

        public class OpponentData
        {
            public long id = 0;
            public List<Building> buildings = null;
        }

        public class InitializationData
        {
            public long accountID = 0;
            public List<ServerUnit> serverUnits = new List<ServerUnit>();
        }

        public class ServerUnit
        {
            public UnitID id = UnitID.pirate;
            public int level = 0;
            public int requiredGold = 0;
            public int requiredFish = 0;
            public int requiredDiamonds = 0;
            public int trainTime = 0;
            public int health = 0;
            public int housing = 0;
        }

        public class Unit
        {
            public UnitID id = UnitID.pirate;
            public int level = 0;
            public long databaseID = 0;
            public int hosing = 1;
            public bool trained = false;
            public bool ready = false;
            public int health = 0;
            public int trainTime = 0;
            public float trainedTime = 0;
            public float moveSpeed = 1;
            public float attackSpeed = 1;
            public float attackRange = 1;
            public float damage = 1;
            public float splashRange = 0;
            public float rangedSpeed = 5;
            public TargetPriority priority = TargetPriority.none;
            public UnitMoveType movement = UnitMoveType.ground;
            public float priorityMultiplier = 1;
        }

        public class Building
        {
            public BuildingID id = BuildingID.islandhall;
            public int level = 0;
            public long databaseID = 0;
            public int x = 0;
            public int y = 0;
            public int columns = 0;
            public int rows = 0;
            public int storage = 0;
            public DateTime boost;
            public int health = 100;
            public float damage = 0;
            public int capacity = 0;
            public float speed = 0;
            public float radius = 0;
            public DateTime constructionTime;
            public bool isConstructing = false;
            public int buildTime = 0;
            public BuildingTargetType targetType = BuildingTargetType.none;
            public float blindRange = 0;
            public float splashRange = 0;
            public float rangedSpeed = 5;
            public float percentage = 0;
        }

        public class ServerBuilding
        {
            public string id = "";
            public int level = 0;
            public long databaseID = 0;
            public int requiredGold = 0;
            public int requiredFish = 0;
            public int requiredDiamonds = 0;
            public int columns = 0;
            public int rows = 0;
            public int buildTime = 0;
            public int gainedXp = 0;
        }

        public static string Serialize<T>(this T target)
        {
            XmlSerializer xml = new XmlSerializer(typeof(T));
            StringWriter writer = new StringWriter();
            xml.Serialize(writer, target);
            return writer.ToString();
        }

        public static T Desrialize<T>(this string target)
        {
            XmlSerializer xml = new XmlSerializer(typeof(T));
            StringReader reader = new StringReader(target);
            return (T)xml.Deserialize(reader);
        }

        public async static Task<string> SerializeAsync<T>(this T target)
        {
            Task<string> task = Task.Run(() =>
            {
                XmlSerializer xml = new XmlSerializer(typeof(T));
                StringWriter writer = new StringWriter();
                xml.Serialize(writer, target);
                return writer.ToString();
            });
            return await task;
        }

        public async static Task<T> DesrializeAsync<T>(this string target)
        {
            Task<T> task = Task.Run(() =>
            {
                XmlSerializer xml = new XmlSerializer(typeof(T));
                StringReader reader = new StringReader(target);
                return (T)xml.Deserialize(reader);
            });
            return await task;
        }

        public static BuildingCount GetBuildingLimits(int islandHallLevel, string globalID)
        {
            for (int i = 0; i < buildingAvailability[islandHallLevel].buildings.Length; i++)
            {
                if (buildingAvailability[islandHallLevel].buildings[i].id == globalID)
                {
                    return buildingAvailability[islandHallLevel].buildings[i];
                }
            }
            return null;
        }

        public static BuildingCount GetIslandHallLimits(int targetIslandHallLevel)
        {
            return null;
        }
        
        public static int GetInstantBuildRequiredDiamonds(int remainedSeconds)
        {
            int diamonds = 0;
            if (remainedSeconds > 0)
            {
                if (remainedSeconds <= 60)
                {
                    diamonds = 1;
                }
                else if (remainedSeconds <= 3600)
                {
                    diamonds = (int)(0.00537f * ((float)remainedSeconds - 60f)) + 1;
                }
                else if (remainedSeconds <= 86400)
                {
                    diamonds = (int)(0.00266f * ((float)remainedSeconds - 3600f)) + 20;
                }
                else
                {
                    diamonds = (int)(0.00143f * ((float)remainedSeconds - 86400f)) + 260;
                }
            }
            return diamonds;
        }

        public class BuildingAvailability
        {
            public int level = 1;
            public BuildingCount[] buildings = null;
        }

        public class BuildingCount
        {
            public string id = "global_id";
            public int count = 0;
            public int maxLevel = 1;
        }

        public static BuildingAvailability[] buildingAvailability =
        {
            new BuildingAvailability
            {
                level = 0,
                buildings = {}
            },
            new BuildingAvailability
            {
                level = 1,
                buildings = new BuildingCount[]
                {
                    new BuildingCount { id = "islandhall", count = 1, maxLevel = 15},
                    new BuildingCount { id = "buildershut", count = 5, maxLevel = 1},
                    new BuildingCount { id = "goldmine", count = 1, maxLevel = 2},
                    new BuildingCount { id = "fisher", count = 1, maxLevel = 2},
                    new BuildingCount { id = "goldstorage", count = 1, maxLevel = 1},
                    new BuildingCount { id = "fishstorage", count = 1, maxLevel = 1},
                    new BuildingCount { id = "armycamp", count = 1, maxLevel = 1},
                    new BuildingCount { id = "barracks", count = 1, maxLevel = 3},
                    new BuildingCount { id = "cannon", count = 2, maxLevel = 2},
                }
            },
            new BuildingAvailability
            {
                level = 2,
                buildings = new BuildingCount[]
                {
                    new BuildingCount { id = "islandhall", count = 1, maxLevel = 15},
                    new BuildingCount { id = "buildershut", count = 5, maxLevel = 1},
                    new BuildingCount { id = "goldmine", count = 2, maxLevel = 4},
                    new BuildingCount { id = "fisher", count = 2, maxLevel = 4},
                    new BuildingCount { id = "goldstorage", count = 1, maxLevel = 3},
                    new BuildingCount { id = "fishstorage", count = 1, maxLevel = 3},
                    new BuildingCount { id = "armycamp", count = 1, maxLevel = 2},
                    new BuildingCount { id = "barracks", count = 1, maxLevel = 4},
                    new BuildingCount { id = "cannon", count = 2, maxLevel = 3},
                    new BuildingCount { id = "archertower", count = 1, maxLevel = 2},
                    new BuildingCount { id = "wall", count = 25, maxLevel = 2},
                }
            },
            new BuildingAvailability
            {
                level = 3,
                buildings = new BuildingCount[]
                {
                    new BuildingCount { id = "islandhall", count = 1, maxLevel = 15},
                    new BuildingCount { id = "buildershut", count = 5, maxLevel = 1},
                    new BuildingCount { id = "goldmine", count = 3, maxLevel = 6},
                    new BuildingCount { id = "fisher", count = 3, maxLevel = 6},
                    new BuildingCount { id = "goldstorage", count = 2, maxLevel = 6},
                    new BuildingCount { id = "fishstorage", count = 2, maxLevel = 6},
                    new BuildingCount { id = "clancastle", count = 1, maxLevel = 1},
                    new BuildingCount { id = "armycamp", count = 2, maxLevel = 3},
                    new BuildingCount { id = "barracks", count = 1, maxLevel = 5},
                    new BuildingCount { id = "laboratory", count = 1, maxLevel = 1},
                    new BuildingCount { id = "cannon", count = 2, maxLevel = 4},
                    new BuildingCount { id = "archertower", count = 1, maxLevel = 3},
                    new BuildingCount { id = "mortor", count = 1, maxLevel = 1},
                    new BuildingCount { id = "wall", count = 50, maxLevel = 3},
                    new BuildingCount { id = "boomb", count = 2, maxLevel = 2},
                }
            },
            new BuildingAvailability
            {
                level = 4,
                buildings = new BuildingCount[]
                {
                    new BuildingCount { id = "islandhall", count = 1, maxLevel = 15},
                    new BuildingCount { id = "buildershut", count = 5, maxLevel = 1},
                    new BuildingCount { id = "goldmine", count = 4, maxLevel = 8},
                    new BuildingCount { id = "fisher", count = 4, maxLevel = 8},
                    new BuildingCount { id = "goldstorage", count = 2, maxLevel = 8},
                    new BuildingCount { id = "fishstorage", count = 2, maxLevel = 8},
                    new BuildingCount { id = "clancastle", count = 1, maxLevel = 2},
                    new BuildingCount { id = "armycamp", count = 2, maxLevel = 4},
                    new BuildingCount { id = "barracks", count = 1, maxLevel = 6},
                    new BuildingCount { id = "laboratory", count = 1, maxLevel = 2},
                    new BuildingCount { id = "cannon", count = 2, maxLevel = 5},
                    new BuildingCount { id = "archertower", count = 2, maxLevel = 4},
                    new BuildingCount { id = "mortor", count = 1, maxLevel = 2},
                    new BuildingCount { id = "airdefense", count = 1, maxLevel = 2},
                    new BuildingCount { id = "wall", count = 75, maxLevel = 4},
                    new BuildingCount { id = "boomb", count = 2, maxLevel = 2},
                    new BuildingCount { id = "springtrap", count = 2, maxLevel = 1},
                }
            },
            new BuildingAvailability
            {
                level = 5,
                buildings = new BuildingCount[]
                {
                    new BuildingCount { id = "islandhall", count = 1, maxLevel = 15},
                    new BuildingCount { id = "buildershut", count = 5, maxLevel = 1},
                    new BuildingCount { id = "goldmine", count = 5, maxLevel = 10},
                    new BuildingCount { id = "fisher", count = 5, maxLevel = 10},
                    new BuildingCount { id = "goldstorage", count = 2, maxLevel = 9},
                    new BuildingCount { id = "fishstorage", count = 2, maxLevel = 9},
                    new BuildingCount { id = "clancastle", count = 1, maxLevel = 2},
                    new BuildingCount { id = "armycamp", count = 3, maxLevel = 5},
                    new BuildingCount { id = "barracks", count = 1, maxLevel = 7},
                    new BuildingCount { id = "laboratory", count = 1, maxLevel = 3},
                    new BuildingCount { id = "spellfactory", count = 1, maxLevel = 1},
                    new BuildingCount { id = "cannon", count = 3, maxLevel = 6},
                    new BuildingCount { id = "archertower", count = 3, maxLevel = 6},
                    new BuildingCount { id = "mortor", count = 1, maxLevel = 3},
                    new BuildingCount { id = "airdefense", count = 1, maxLevel = 3},
                    new BuildingCount { id = "wizardtower", count = 1, maxLevel = 2},
                    new BuildingCount { id = "wall", count = 100, maxLevel = 5},
                    new BuildingCount { id = "boomb", count = 4, maxLevel = 3},
                    new BuildingCount { id = "springtrap", count = 2, maxLevel = 1},
                    new BuildingCount { id = "airbomb", count = 2, maxLevel = 2},
                }
            },
            new BuildingAvailability
            {
                level = 6,
                buildings = new BuildingCount[]
                {
                    new BuildingCount { id = "islandhall", count = 1, maxLevel = 15},
                    new BuildingCount { id = "buildershut", count = 5, maxLevel = 1},
                    new BuildingCount { id = "goldmine", count = 6, maxLevel = 10},
                    new BuildingCount { id = "fisher", count = 6, maxLevel = 10},
                    new BuildingCount { id = "goldstorage", count = 2, maxLevel = 10},
                    new BuildingCount { id = "fishstorage", count = 2, maxLevel = 10},
                    new BuildingCount { id = "clancastle", count = 1, maxLevel = 3},
                    new BuildingCount { id = "armycamp", count = 3, maxLevel = 6},
                    new BuildingCount { id = "barracks", count = 1, maxLevel = 8},
                    new BuildingCount { id = "laboratory", count = 1, maxLevel = 4},
                    new BuildingCount { id = "spellfactory", count = 1, maxLevel = 2},
                    new BuildingCount { id = "cannon", count = 3, maxLevel = 7},
                    new BuildingCount { id = "archertower", count = 3, maxLevel = 7},
                    new BuildingCount { id = "mortor", count = 2, maxLevel = 4},
                    new BuildingCount { id = "airdefense", count = 2, maxLevel = 4},
                    new BuildingCount { id = "wizardtower", count = 2, maxLevel = 3},
                    new BuildingCount { id = "airsweeper", count = 1, maxLevel = 2},
                    new BuildingCount { id = "wall", count = 125, maxLevel = 6},
                    new BuildingCount { id = "boomb", count = 4, maxLevel = 3},
                    new BuildingCount { id = "springtrap", count = 4, maxLevel = 1},
                    new BuildingCount { id = "airbomb", count = 2, maxLevel = 2},
                    new BuildingCount { id = "giantbomb", count = 1, maxLevel = 2},
                }
            },
            new BuildingAvailability
            {
                level = 7,
                buildings = new BuildingCount[]
                {
                    new BuildingCount { id = "islandhall", count = 1, maxLevel = 15},
                    new BuildingCount { id = "buildershut", count = 5, maxLevel = 1},
                    new BuildingCount { id = "goldmine", count = 6, maxLevel = 11},
                    new BuildingCount { id = "fisher", count = 6, maxLevel = 11},
                    new BuildingCount { id = "darkelixirmine", count = 1, maxLevel = 3},
                    new BuildingCount { id = "goldstorage", count = 2, maxLevel = 11},
                    new BuildingCount { id = "fishstorage", count = 2, maxLevel = 11},
                    new BuildingCount { id = "darkelixirstorage", count = 1, maxLevel = 2},
                    new BuildingCount { id = "clancastle", count = 1, maxLevel = 3},
                    new BuildingCount { id = "armycamp", count = 4, maxLevel = 6},
                    new BuildingCount { id = "barracks", count = 1, maxLevel = 9},
                    new BuildingCount { id = "darkbarracks", count = 1, maxLevel = 2},
                    new BuildingCount { id = "laboratory", count = 1, maxLevel = 5},
                    new BuildingCount { id = "spellfactory", count = 1, maxLevel = 3},
                    new BuildingCount { id = "kingaltar", count = 1, maxLevel = 1},
                    new BuildingCount { id = "cannon", count = 5, maxLevel = 8},
                    new BuildingCount { id = "archertower", count = 4, maxLevel = 8},
                    new BuildingCount { id = "mortor", count = 3, maxLevel = 5},
                    new BuildingCount { id = "airdefense", count = 3, maxLevel = 5},
                    new BuildingCount { id = "wizardtower", count = 2, maxLevel = 4},
                    new BuildingCount { id = "airsweeper", count = 1, maxLevel = 3},
                    new BuildingCount { id = "hiddentesla", count = 2, maxLevel = 3},
                    new BuildingCount { id = "wall", count = 175, maxLevel = 6},
                    new BuildingCount { id = "boomb", count = 6, maxLevel = 4},
                    new BuildingCount { id = "springtrap", count = 4, maxLevel = 2},
                    new BuildingCount { id = "airbomb", count = 2, maxLevel = 3},
                    new BuildingCount { id = "giantbomb", count = 2, maxLevel = 2},
                    new BuildingCount { id = "seekingairmine", count = 1, maxLevel = 1},
                }
            },
            new BuildingAvailability
            {
                level = 8,
                buildings = new BuildingCount[]
                {
                    new BuildingCount { id = "islandhall", count = 1, maxLevel = 15},
                    new BuildingCount { id = "buildershut", count = 5, maxLevel = 1},
                    new BuildingCount { id = "goldmine", count = 6, maxLevel = 12},
                    new BuildingCount { id = "fisher", count = 6, maxLevel = 12},
                    new BuildingCount { id = "darkelixirmine", count = 2, maxLevel = 3},
                    new BuildingCount { id = "goldstorage", count = 3, maxLevel = 11},
                    new BuildingCount { id = "fishstorage", count = 3, maxLevel = 11},
                    new BuildingCount { id = "darkelixirstorage", count = 1, maxLevel = 4},
                    new BuildingCount { id = "clancastle", count = 1, maxLevel = 4},
                    new BuildingCount { id = "armycamp", count = 4, maxLevel = 6},
                    new BuildingCount { id = "barracks", count = 1, maxLevel = 10},
                    new BuildingCount { id = "darkbarracks", count = 1, maxLevel = 4},
                    new BuildingCount { id = "laboratory", count = 1, maxLevel = 6},
                    new BuildingCount { id = "spellfactory", count = 1, maxLevel = 3},
                    new BuildingCount { id = "darkspellfactory", count = 1, maxLevel = 2},
                    new BuildingCount { id = "kingaltar", count = 1, maxLevel = 1},
                    new BuildingCount { id = "cannon", count = 5, maxLevel = 10},
                    new BuildingCount { id = "archertower", count = 5, maxLevel = 10},
                    new BuildingCount { id = "mortor", count = 4, maxLevel = 6},
                    new BuildingCount { id = "airdefense", count = 3, maxLevel = 6},
                    new BuildingCount { id = "wizardtower", count = 3, maxLevel = 6},
                    new BuildingCount { id = "airsweeper", count = 1, maxLevel = 4},
                    new BuildingCount { id = "hiddentesla", count = 3, maxLevel = 6},
                    new BuildingCount { id = "bombtower", count = 1, maxLevel = 2},
                    new BuildingCount { id = "wall", count = 225, maxLevel = 8},
                    new BuildingCount { id = "boomb", count = 6, maxLevel = 5},
                    new BuildingCount { id = "springtrap", count = 6, maxLevel = 3},
                    new BuildingCount { id = "airbomb", count = 4, maxLevel = 3},
                    new BuildingCount { id = "giantbomb", count = 3, maxLevel = 3},
                    new BuildingCount { id = "seekingairmine", count = 2, maxLevel = 1},
                    new BuildingCount { id = "skeletontrap", count = 2, maxLevel = 2},
                }
            },
            new BuildingAvailability
            {
                level = 9,
                buildings = new BuildingCount[]
                {
                    new BuildingCount { id = "islandhall", count = 1, maxLevel = 15},
                    new BuildingCount { id = "buildershut", count = 5, maxLevel = 1},
                    new BuildingCount { id = "goldmine", count = 7, maxLevel = 12},
                    new BuildingCount { id = "fisher", count = 7, maxLevel = 12},
                    new BuildingCount { id = "darkelixirmine", count = 3, maxLevel = 6},
                    new BuildingCount { id = "goldstorage", count = 4, maxLevel = 11},
                    new BuildingCount { id = "fishstorage", count = 4, maxLevel = 11},
                    new BuildingCount { id = "darkelixirstorage", count = 1, maxLevel = 6},
                    new BuildingCount { id = "clancastle", count = 1, maxLevel = 5},
                    new BuildingCount { id = "armycamp", count = 4, maxLevel = 7},
                    new BuildingCount { id = "barracks", count = 1, maxLevel = 11},
                    new BuildingCount { id = "darkbarracks", count = 1, maxLevel = 6},
                    new BuildingCount { id = "laboratory", count = 1, maxLevel = 7},
                    new BuildingCount { id = "spellfactory", count = 1, maxLevel = 4},
                    new BuildingCount { id = "darkspellfactory", count = 1, maxLevel = 4},
                    new BuildingCount { id = "kingaltar", count = 1, maxLevel = 1},
                    new BuildingCount { id = "qeenaltar", count = 1, maxLevel = 1},
                    new BuildingCount { id = "cannon", count = 5, maxLevel = 11},
                    new BuildingCount { id = "archertower", count = 6, maxLevel = 11},
                    new BuildingCount { id = "mortor", count = 4, maxLevel = 7},
                    new BuildingCount { id = "airdefense", count = 4, maxLevel = 7},
                    new BuildingCount { id = "wizardtower", count = 4, maxLevel = 7},
                    new BuildingCount { id = "airsweeper", count = 2, maxLevel = 5},
                    new BuildingCount { id = "hiddentesla", count = 4, maxLevel = 7},
                    new BuildingCount { id = "bombtower", count = 1, maxLevel = 3},
                    new BuildingCount { id = "xbow", count = 2, maxLevel = 3},
                    new BuildingCount { id = "wall", count = 250, maxLevel = 10},
                    new BuildingCount { id = "boomb", count = 6, maxLevel = 6},
                    new BuildingCount { id = "springtrap", count = 6, maxLevel = 4},
                    new BuildingCount { id = "airbomb", count = 4, maxLevel = 4},
                    new BuildingCount { id = "giantbomb", count = 4, maxLevel = 3},
                    new BuildingCount { id = "seekingairmine", count = 4, maxLevel = 2},
                    new BuildingCount { id = "skeletontrap", count = 2, maxLevel = 3},
                }
            },
            new BuildingAvailability
            {
                level = 10,
                buildings = new BuildingCount[]
                {
                    new BuildingCount { id = "islandhall", count = 1, maxLevel = 15},
                    new BuildingCount { id = "buildershut", count = 5, maxLevel = 1},
                    new BuildingCount { id = "goldmine", count = 7, maxLevel = 13},
                    new BuildingCount { id = "fisher", count = 7, maxLevel = 13},
                    new BuildingCount { id = "darkelixirmine", count = 3, maxLevel = 7},
                    new BuildingCount { id = "goldstorage", count = 4, maxLevel = 11},
                    new BuildingCount { id = "fishstorage", count = 4, maxLevel = 11},
                    new BuildingCount { id = "darkelixirstorage", count = 1, maxLevel = 6},
                    new BuildingCount { id = "clancastle", count = 1, maxLevel = 6},
                    new BuildingCount { id = "armycamp", count = 4, maxLevel = 8},
                    new BuildingCount { id = "barracks", count = 1, maxLevel = 12},
                    new BuildingCount { id = "darkbarracks", count = 1, maxLevel = 7},
                    new BuildingCount { id = "laboratory", count = 1, maxLevel = 8},
                    new BuildingCount { id = "spellfactory", count = 1, maxLevel = 5},
                    new BuildingCount { id = "darkspellfactory", count = 1, maxLevel = 5},
                    new BuildingCount { id = "kingaltar", count = 1, maxLevel = 1},
                    new BuildingCount { id = "qeenaltar", count = 1, maxLevel = 1},
                    new BuildingCount { id = "cannon", count = 6, maxLevel = 13},
                    new BuildingCount { id = "archertower", count = 7, maxLevel = 13},
                    new BuildingCount { id = "mortor", count = 4, maxLevel = 8},
                    new BuildingCount { id = "airdefense", count = 4, maxLevel = 8},
                    new BuildingCount { id = "wizardtower", count = 4, maxLevel = 9},
                    new BuildingCount { id = "airsweeper", count = 2, maxLevel = 6},
                    new BuildingCount { id = "hiddentesla", count = 4, maxLevel = 8},
                    new BuildingCount { id = "bombtower", count = 2, maxLevel = 4},
                    new BuildingCount { id = "xbow", count = 3, maxLevel = 4},
                    new BuildingCount { id = "infernotower", count = 2, maxLevel = 3},
                    new BuildingCount { id = "wall", count = 275, maxLevel = 11},
                    new BuildingCount { id = "boomb", count = 6, maxLevel = 7},
                    new BuildingCount { id = "springtrap", count = 6, maxLevel = 5},
                    new BuildingCount { id = "airbomb", count = 5, maxLevel = 4},
                    new BuildingCount { id = "giantbomb", count = 5, maxLevel = 4},
                    new BuildingCount { id = "seekingairmine", count = 5, maxLevel = 3},
                    new BuildingCount { id = "skeletontrap", count = 3, maxLevel = 4},
                }
            },
            new BuildingAvailability
            {
                level = 11,
                buildings = new BuildingCount[]
                {
                    new BuildingCount { id = "islandhall", count = 1, maxLevel = 15},
                    new BuildingCount { id = "buildershut", count = 5, maxLevel = 1},
                    new BuildingCount { id = "goldmine", count = 7, maxLevel = 14},
                    new BuildingCount { id = "fisher", count = 7, maxLevel = 14},
                    new BuildingCount { id = "darkelixirmine", count = 3, maxLevel = 8},
                    new BuildingCount { id = "goldstorage", count = 4, maxLevel = 12},
                    new BuildingCount { id = "fishstorage", count = 4, maxLevel = 12},
                    new BuildingCount { id = "darkelixirstorage", count = 1, maxLevel = 6},
                    new BuildingCount { id = "clancastle", count = 1, maxLevel = 7},
                    new BuildingCount { id = "armycamp", count = 4, maxLevel = 9},
                    new BuildingCount { id = "barracks", count = 1, maxLevel = 13},
                    new BuildingCount { id = "darkbarracks", count = 1, maxLevel = 8},
                    new BuildingCount { id = "laboratory", count = 1, maxLevel = 9},
                    new BuildingCount { id = "spellfactory", count = 1, maxLevel = 6},
                    new BuildingCount { id = "darkspellfactory", count = 1, maxLevel = 5},
                    new BuildingCount { id = "kingaltar", count = 1, maxLevel = 1},
                    new BuildingCount { id = "qeenaltar", count = 1, maxLevel = 1},
                    new BuildingCount { id = "wardenaltar", count = 1, maxLevel = 1},
                    new BuildingCount { id = "cannon", count = 7, maxLevel = 15},
                    new BuildingCount { id = "archertower", count = 8, maxLevel = 15},
                    new BuildingCount { id = "mortor", count = 4, maxLevel = 10},
                    new BuildingCount { id = "airdefense", count = 4, maxLevel = 9},
                    new BuildingCount { id = "wizardtower", count = 5, maxLevel = 10},
                    new BuildingCount { id = "airsweeper", count = 2, maxLevel = 7},
                    new BuildingCount { id = "hiddentesla", count = 4, maxLevel = 9},
                    new BuildingCount { id = "bombtower", count = 2, maxLevel = 6},
                    new BuildingCount { id = "xbow", count = 4, maxLevel = 5},
                    new BuildingCount { id = "infernotower", count = 2, maxLevel = 5},
                    new BuildingCount { id = "eagleartillery", count = 1, maxLevel = 2},
                    new BuildingCount { id = "wall", count = 300, maxLevel = 12},
                    new BuildingCount { id = "boomb", count = 6, maxLevel = 8},
                    new BuildingCount { id = "springtrap", count = 6, maxLevel = 5},
                    new BuildingCount { id = "airbomb", count = 5, maxLevel = 5},
                    new BuildingCount { id = "giantbomb", count = 5, maxLevel = 5},
                    new BuildingCount { id = "seekingairmine", count = 5, maxLevel = 3},
                    new BuildingCount { id = "skeletontrap", count = 3, maxLevel = 4},
                    new BuildingCount { id = "tornadotrap", count = 1, maxLevel = 2},
                }
            },
            new BuildingAvailability
            {
                level = 12,
                buildings = new BuildingCount[]
                {
                    new BuildingCount { id = "islandhall", count = 1, maxLevel = 15},
                    new BuildingCount { id = "buildershut", count = 5, maxLevel = 1},
                    new BuildingCount { id = "goldmine", count = 7, maxLevel = 15},
                    new BuildingCount { id = "fisher", count = 7, maxLevel = 15},
                    new BuildingCount { id = "darkelixirmine", count = 3, maxLevel = 9},
                    new BuildingCount { id = "goldstorage", count = 4, maxLevel = 13},
                    new BuildingCount { id = "fishstorage", count = 4, maxLevel = 13},
                    new BuildingCount { id = "darkelixirstorage", count = 1, maxLevel = 7},
                    new BuildingCount { id = "clancastle", count = 1, maxLevel = 8},
                    new BuildingCount { id = "armycamp", count = 4, maxLevel = 10},
                    new BuildingCount { id = "barracks", count = 1, maxLevel = 14},
                    new BuildingCount { id = "darkbarracks", count = 1, maxLevel = 9},
                    new BuildingCount { id = "laboratory", count = 1, maxLevel = 10},
                    new BuildingCount { id = "spellfactory", count = 1, maxLevel = 6},
                    new BuildingCount { id = "darkspellfactory", count = 1, maxLevel = 5},
                    new BuildingCount { id = "workshop", count = 1, maxLevel = 3},
                    new BuildingCount { id = "kingaltar", count = 1, maxLevel = 1},
                    new BuildingCount { id = "qeenaltar", count = 1, maxLevel = 1},
                    new BuildingCount { id = "wardenaltar", count = 1, maxLevel = 1},
                    new BuildingCount { id = "cannon", count = 7, maxLevel = 17},
                    new BuildingCount { id = "archertower", count = 8, maxLevel = 17},
                    new BuildingCount { id = "mortor", count = 4, maxLevel = 12},
                    new BuildingCount { id = "airdefense", count = 4, maxLevel = 10},
                    new BuildingCount { id = "wizardtower", count = 5, maxLevel = 11},
                    new BuildingCount { id = "airsweeper", count = 2, maxLevel = 7},
                    new BuildingCount { id = "hiddentesla", count = 5, maxLevel = 10},
                    new BuildingCount { id = "bombtower", count = 2, maxLevel = 7},
                    new BuildingCount { id = "xbow", count = 4, maxLevel = 6},
                    new BuildingCount { id = "infernotower", count = 3, maxLevel = 6},
                    new BuildingCount { id = "eagleartillery", count = 1, maxLevel = 3},
                    new BuildingCount { id = "wall", count = 300, maxLevel = 13},
                    new BuildingCount { id = "boomb", count = 6, maxLevel = 8},
                    new BuildingCount { id = "springtrap", count = 8, maxLevel = 5},
                    new BuildingCount { id = "airbomb", count = 6, maxLevel = 6},
                    new BuildingCount { id = "giantbomb", count = 6, maxLevel = 5},
                    new BuildingCount { id = "seekingairmine", count = 6, maxLevel = 3},
                    new BuildingCount { id = "skeletontrap", count = 3, maxLevel = 4},
                    new BuildingCount { id = "tornadotrap", count = 1, maxLevel = 3},
                }
            },
            new BuildingAvailability
            {
                level = 13,
                buildings = new BuildingCount[]
                {
                    new BuildingCount { id = "islandhall", count = 1, maxLevel = 15},
                    new BuildingCount { id = "buildershut", count = 5, maxLevel = 1},
                    new BuildingCount { id = "goldmine", count = 7, maxLevel = 15},
                    new BuildingCount { id = "fisher", count = 7, maxLevel = 15},
                    new BuildingCount { id = "darkelixirmine", count = 3, maxLevel = 9},
                    new BuildingCount { id = "goldstorage", count = 4, maxLevel = 14},
                    new BuildingCount { id = "fishstorage", count = 4, maxLevel = 14},
                    new BuildingCount { id = "darkelixirstorage", count = 1, maxLevel = 8},
                    new BuildingCount { id = "clancastle", count = 1, maxLevel = 9},
                    new BuildingCount { id = "armycamp", count = 4, maxLevel = 11},
                    new BuildingCount { id = "barracks", count = 1, maxLevel = 15},
                    new BuildingCount { id = "darkbarracks", count = 1, maxLevel = 9},
                    new BuildingCount { id = "laboratory", count = 1, maxLevel = 11},
                    new BuildingCount { id = "spellfactory", count = 1, maxLevel = 7},
                    new BuildingCount { id = "darkspellfactory", count = 1, maxLevel = 5},
                    new BuildingCount { id = "workshop", count = 1, maxLevel = 5},
                    new BuildingCount { id = "kingaltar", count = 1, maxLevel = 1},
                    new BuildingCount { id = "qeenaltar", count = 1, maxLevel = 1},
                    new BuildingCount { id = "wardenaltar", count = 1, maxLevel = 1},
                    new BuildingCount { id = "championaltar", count = 1, maxLevel = 1},
                    new BuildingCount { id = "cannon", count = 7, maxLevel = 19},
                    new BuildingCount { id = "archertower", count = 8, maxLevel = 19},
                    new BuildingCount { id = "mortor", count = 4, maxLevel = 13},
                    new BuildingCount { id = "airdefense", count = 4, maxLevel = 11},
                    new BuildingCount { id = "wizardtower", count = 5, maxLevel = 13},
                    new BuildingCount { id = "airsweeper", count = 2, maxLevel = 7},
                    new BuildingCount { id = "hiddentesla", count = 5, maxLevel = 12},
                    new BuildingCount { id = "bombtower", count = 2, maxLevel = 8},
                    new BuildingCount { id = "xbow", count = 4, maxLevel = 8},
                    new BuildingCount { id = "infernotower", count = 3, maxLevel = 7},
                    new BuildingCount { id = "eagleartillery", count = 1, maxLevel = 4},
                    new BuildingCount { id = "scattershot", count = 2, maxLevel = 2},
                    new BuildingCount { id = "wall", count = 300, maxLevel = 14},
                    new BuildingCount { id = "boomb", count = 7, maxLevel = 9},
                    new BuildingCount { id = "springtrap", count = 9, maxLevel = 5},
                    new BuildingCount { id = "airbomb", count = 6, maxLevel = 8},
                    new BuildingCount { id = "giantbomb", count = 6, maxLevel = 7},
                    new BuildingCount { id = "seekingairmine", count = 7, maxLevel = 4},
                    new BuildingCount { id = "skeletontrap", count = 3, maxLevel = 4},
                    new BuildingCount { id = "tornadotrap", count = 1, maxLevel = 3},
                }
            },
            new BuildingAvailability
            {
                level = 14,
                buildings = new BuildingCount[]
                {
                    new BuildingCount { id = "islandhall", count = 1, maxLevel = 15},
                    new BuildingCount { id = "buildershut", count = 5, maxLevel = 1},
                    new BuildingCount { id = "goldmine", count = 7, maxLevel = 15},
                    new BuildingCount { id = "fisher", count = 7, maxLevel = 15},
                    new BuildingCount { id = "darkelixirmine", count = 3, maxLevel = 9},
                    new BuildingCount { id = "goldstorage", count = 4, maxLevel = 15},
                    new BuildingCount { id = "fishstorage", count = 4, maxLevel = 15},
                    new BuildingCount { id = "darkelixirstorage", count = 1, maxLevel = 9},
                    new BuildingCount { id = "clancastle", count = 1, maxLevel = 10},
                    new BuildingCount { id = "armycamp", count = 4, maxLevel = 11},
                    new BuildingCount { id = "barracks", count = 1, maxLevel = 16},
                    new BuildingCount { id = "darkbarracks", count = 1, maxLevel = 9},
                    new BuildingCount { id = "laboratory", count = 1, maxLevel = 12},
                    new BuildingCount { id = "spellfactory", count = 1, maxLevel = 7},
                    new BuildingCount { id = "darkspellfactory", count = 1, maxLevel = 5},
                    new BuildingCount { id = "workshop", count = 1, maxLevel = 6},
                    new BuildingCount { id = "pethouse", count = 1, maxLevel = 4},
                    new BuildingCount { id = "kingaltar", count = 1, maxLevel = 1},
                    new BuildingCount { id = "qeenaltar", count = 1, maxLevel = 1},
                    new BuildingCount { id = "wardenaltar", count = 1, maxLevel = 1},
                    new BuildingCount { id = "championaltar", count = 1, maxLevel = 1},
                    new BuildingCount { id = "cannon", count = 7, maxLevel = 20},
                    new BuildingCount { id = "archertower", count = 8, maxLevel = 20},
                    new BuildingCount { id = "mortor", count = 4, maxLevel = 14},
                    new BuildingCount { id = "airdefense", count = 4, maxLevel = 12},
                    new BuildingCount { id = "wizardtower", count = 5, maxLevel = 14},
                    new BuildingCount { id = "airsweeper", count = 2, maxLevel = 7},
                    new BuildingCount { id = "hiddentesla", count = 5, maxLevel = 13},
                    new BuildingCount { id = "bombtower", count = 2, maxLevel = 9},
                    new BuildingCount { id = "xbow", count = 4, maxLevel = 9},
                    new BuildingCount { id = "infernotower", count = 3, maxLevel = 8},
                    new BuildingCount { id = "eagleartillery", count = 1, maxLevel = 5},
                    new BuildingCount { id = "scattershot", count = 2, maxLevel = 3},
                    new BuildingCount { id = "wall", count = 325, maxLevel = 15},
                    new BuildingCount { id = "boomb", count = 8, maxLevel = 10},
                    new BuildingCount { id = "springtrap", count = 9, maxLevel = 5},
                    new BuildingCount { id = "airbomb", count = 7, maxLevel = 9},
                    new BuildingCount { id = "giantbomb", count = 7, maxLevel = 8},
                    new BuildingCount { id = "seekingairmine", count = 8, maxLevel = 4},
                    new BuildingCount { id = "skeletontrap", count = 4, maxLevel = 4},
                    new BuildingCount { id = "tornadotrap", count = 1, maxLevel = 3},
                }
            },
            new BuildingAvailability
            {
                level = 15,
                buildings = new BuildingCount[]
                {
                    new BuildingCount { id = "islandhall", count = 1, maxLevel = 15},
                    new BuildingCount { id = "buildershut", count = 5, maxLevel = 1},
                    new BuildingCount { id = "goldmine", count = 7, maxLevel = 15},
                    new BuildingCount { id = "fisher", count = 7, maxLevel = 15},
                    new BuildingCount { id = "darkelixirmine", count = 3, maxLevel = 9},
                    new BuildingCount { id = "goldstorage", count = 4, maxLevel = 16},
                    new BuildingCount { id = "fishstorage", count = 4, maxLevel = 16},
                    new BuildingCount { id = "darkelixirstorage", count = 1, maxLevel = 10},
                    new BuildingCount { id = "clancastle", count = 1, maxLevel = 11},
                    new BuildingCount { id = "armycamp", count = 4, maxLevel = 12},
                    new BuildingCount { id = "barracks", count = 1, maxLevel = 16},
                    new BuildingCount { id = "darkbarracks", count = 1, maxLevel = 9},
                    new BuildingCount { id = "laboratory", count = 1, maxLevel = 13},
                    new BuildingCount { id = "spellfactory", count = 1, maxLevel = 7},
                    new BuildingCount { id = "darkspellfactory", count = 1, maxLevel = 5},
                    new BuildingCount { id = "workshop", count = 1, maxLevel = 7},
                    new BuildingCount { id = "pethouse", count = 1, maxLevel = 8},
                    new BuildingCount { id = "kingaltar", count = 1, maxLevel = 1},
                    new BuildingCount { id = "qeenaltar", count = 1, maxLevel = 1},
                    new BuildingCount { id = "wardenaltar", count = 1, maxLevel = 1},
                    new BuildingCount { id = "championaltar", count = 1, maxLevel = 1},
                    new BuildingCount { id = "cannon", count = 7, maxLevel = 21},
                    new BuildingCount { id = "archertower", count = 8, maxLevel = 21},
                    new BuildingCount { id = "mortor", count = 4, maxLevel = 15},
                    new BuildingCount { id = "airdefense", count = 4, maxLevel = 13},
                    new BuildingCount { id = "wizardtower", count = 5, maxLevel = 15},
                    new BuildingCount { id = "airsweeper", count = 2, maxLevel = 7},
                    new BuildingCount { id = "hiddentesla", count = 5, maxLevel = 13},
                    new BuildingCount { id = "bombtower", count = 2, maxLevel = 10},
                    new BuildingCount { id = "xbow", count = 4, maxLevel = 10},
                    new BuildingCount { id = "infernotower", count = 3, maxLevel = 9},
                    new BuildingCount { id = "eagleartillery", count = 1, maxLevel = 5},
                    new BuildingCount { id = "scattershot", count = 2, maxLevel = 3},
                    new BuildingCount { id = "spelltower", count = 2, maxLevel = 3},
                    new BuildingCount { id = "monolith", count = 1, maxLevel = 2},
                    new BuildingCount { id = "wall", count = 325, maxLevel = 16},
                    new BuildingCount { id = "boomb", count = 8, maxLevel = 11},
                    new BuildingCount { id = "springtrap", count = 9, maxLevel = 5},
                    new BuildingCount { id = "airbomb", count = 7, maxLevel = 10},
                    new BuildingCount { id = "giantbomb", count = 7, maxLevel = 8},
                    new BuildingCount { id = "seekingairmine", count = 8, maxLevel = 4},
                    new BuildingCount { id = "skeletontrap", count = 4, maxLevel = 4},
                    new BuildingCount { id = "tornadotrap", count = 1, maxLevel = 3},
                }
            },
        };

    }
}