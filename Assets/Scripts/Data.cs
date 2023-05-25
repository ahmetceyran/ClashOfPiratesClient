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
        public const int minElixirCollect = 10;
        public const int minDarkElixirCollect = 10;
        public static readonly int battleDuration = 120;
        public static readonly int battlePrepDuration = 30;
        public static readonly int gridSize = 45;
        public static readonly float gridCellSize = 1;

        public static readonly float battleFrameRate = 0.1f;
        public static readonly int battleTilesWorthOfOneWall = 15;
        public static readonly int battleGroupWallAttackRadius = 5;
        public static readonly int battleGridOffset = 1;

        public static readonly int clanMaxMembers = 50;
        public static readonly int clansPerPage = 20;
        public static readonly int clanNameMinLength = 3;
        public static readonly int clanCreatePrice = 40000;
        public static readonly int clanWarAttacksPerPlayer = 2;
        public static readonly int clanWarPrepHours = 24;
        public static readonly int clanWarBattleHours = 24;
        public static readonly double clanWarMatchMinPercentage = 0.70d;

        public static readonly double clanWarMatchTownHallEffectPercentage = 0.60d;
        public static readonly double clanWarMatchSpellFactoryEffectPercentage = 0.5d;
        public static readonly double clanWarMatchDarkSpellFactoryEffectPercentage = 0.5d;
        public static readonly double clanWarMatchBarracksEffectPercentage = 0.5d;
        public static readonly double clanWarMatchDarkBarracksEffectPercentage = 0.5d;
        public static readonly double clanWarMatchCampsEffectPercentage = 0.20d;

        public static readonly int[] clanRanksWithEditPermission = { 1, 2 };
        public static readonly int[] clanRanksWithWarPermission = { 1, 2 };
        public static readonly int[] clanWarAvailableCounts = { 1, 5, 10, 15, 20, 30, 40, 50 };

        public enum ClanJoinType
        {
            AnyoneCanJoin = 0, NotAcceptingNewMembers = -1, TakingJoinRequests = 1
        }

        public class ClansList
        {
            public int page = 1;
            public int pagesCount = 1;
            public List<Data.Clan> clans = new List<Clan>();
        }

        public class ClanWarSearch
        {
            public long id = 0;
            public long clan = 0;
            public long player = 0;
            public DateTime time;
            public List<ClanWarSearchMember> members = null;
            public List<long> notMatch = new List<long>();
            public int match = -1;
            public bool handled = false;
        }

        public class ClanWarData
        {
            public long id = 0;
            public bool searching = false;
            public int count = 0;
            public string starter = "";
            public Clan clan1 = null;
            public Clan clan2 = null;
        }

        public class ClanWarAttack
        {
            public long id = 0;
            public DateTime start;
            public long attacker = 0;
            public long defender = 0;
            public int stars = 0;
            public int gold = 0;
            public int elixir = 0;
            public int dark = 0;
            public bool starsCounted = false;
        }

        public class ClanWarSearchMember
        {
            public int tempPosition = -1;
            public int warPosition = -1;
            public ClanMember data = new ClanMember();
            public List<Building> Buildings = new List<Building>();

            public int wallsPower = 0;
            public int defencePower = 0;

            public int townHall = 0;
            public int spellFactory = 0;
            public int darkSpellFactory = 0;
            public int barracks = 0;
            public int darkBarracks = 0;
            public int campsCapacity = 0;
        }

        public class Clan
        {
            public long id = 0;
            public string name = "Clan";
            public ClanJoinType joinType = ClanJoinType.AnyoneCanJoin;
            public int level = 1;
            public int xp = 0;
            public int rank = 0;
            public int trophies = 0;
            public int minTrophies = 0;
            public int minTownhallLevel = 0;
            public int pattern = 0;
            public int background = 0;
            public string patternColor = "";
            public string backgroundColor = "";
            public List<ClanMember> members = new List<ClanMember>();
            public ClanWar war = new ClanWar();
        }

        public class ClanWar
        {
            public long id = 0;
            public long clan1 = 0;
            public long clan2 = 0;
            public int stage = 0;
            public DateTime start;
            public List<ClanWarAttack> attacks = new List<ClanWarAttack>();
        }

        public class ClanMember
        {
            public long id = 0;
            public string name = "Player";
            public int level = 1;
            public int xp = 0;
            public int rank = 0;
            public int trophies = 0;
            public int townHallLevel = 1;
            public bool online = false;
            public long clanID = 0;
            public int clanRank = 0;
            public long warID = 0;
            public int warPos = 0;
        }

        public class Player
        {
            public long id = 0;
            public string name = "Player";
            public int gems = 0;
            public int trophies = 0;
            public DateTime nowTime;
            public DateTime shield;
            public int xp = 0;
            public int level = 1;
            public DateTime clanTimer;
            public long clanID = 0;
            public int clanRank = 0;
            public long warID = 0;
            public List<Building> buildings = new List<Building>();
            public List<Unit> units = new List<Unit>();
        }

        public enum UnitID
        {
            barbarian, archer, goblin, healer, wallbreaker, giant, miner, balloon, wizard, dragon, pekka, babydragon, electrodragon, yeti, dragonrider, electrotitan, minion, hogrider, valkyrie, golem, witch, lavahound, bowler, icegolem, headhunter
        }

        public static int GetClanWarGainedXP(int gainedStars, int enemyGainedStars, int maxStars, bool didWonFirstAttack)
        {
            int xp = 0;
            double percentage = (double)gainedStars / (double)enemyGainedStars;

            if (percentage >= 0.4d)
            {
                xp += 10;
            }

            if (percentage >= 0.6d)
            {
                xp += 25;
            }

            if (gainedStars > enemyGainedStars)
            {
                xp += 50;
            }

            if (didWonFirstAttack)
            {
                xp += 10;
            }

            return xp;
        }

        public static int GetClanNexLevelRequiredXp(int currentLevel)
        {
            switch (currentLevel)
            {
                case 1: return 0;
                case 2: return 500;
                case 3: return 1200;
                case 4: return 1900;
                case 5: return 3100;
                case 6: return 3800;
                case 7: return 4500;
                case 8: return 5200;
                case 9: return 5900;
                case 10: return 7900;
                case 11: return 8600;
                case 12: return 9300;
                case 13: return 10000;
                case 14: return 10700;
                case 15: return 15700;
                case 16: return 16400;
                case 17: return 17100;
                case 18: return 17800;
                case 19: return 18500;
                case 20: return 23500;
                case 21: return 24200;
                case 22: return 24900;
                case 23: return 25600;
                case 24: return 26300;
                case 25: return 31300;
                case 26: return 32000;
                case 27: return 32700;
                case 28: return 33400;
                case 29: return 34100;
                case 30: return 39100;
                case 31: return 39800;
                case 32: return 40500;
                case 33: return 41200;
                case 34: return 41900;
                case 35: return 46900;
                case 36: return 47600;
                case 37: return 48300;
                case 38: return 49000;
                case 39: return 49700;
                case 40: return 54700;
                case 41: return 55400;
                case 42: return 56100;
                case 43: return 56800;
                case 44: return 57500;
                case 45: return 62500;
                case 46: return 63200;
                case 47: return 63900;
                case 48: return 64600;
                case 49: return 65300;
                case 50: return 70300;
                case 51: return 71000;
                default: return 99999999;
            }
        }

        public static bool IsUnitUnlocked(UnitID id, int barracksLevel, int darkBarracksLevel)
        {
            switch (id)
            {
                case UnitID.barbarian: return barracksLevel >= 1;
                case UnitID.archer: return barracksLevel >= 2;
                case UnitID.goblin: return barracksLevel >= 4;
                case UnitID.healer: return barracksLevel >= 8;
                case UnitID.wallbreaker: return barracksLevel >= 5;
                case UnitID.giant: return barracksLevel >= 3;
                case UnitID.miner: return barracksLevel >= 12;
                case UnitID.balloon: return barracksLevel >= 6;
                case UnitID.wizard: return barracksLevel >= 7;
                case UnitID.dragon: return barracksLevel >= 9;
                case UnitID.pekka: return barracksLevel >= 10;
                case UnitID.babydragon: return barracksLevel >= 11;
                case UnitID.electrodragon: return barracksLevel >= 13;
                case UnitID.yeti: return barracksLevel >= 14;
                case UnitID.dragonrider: return barracksLevel >= 15;
                case UnitID.electrotitan: return barracksLevel >= 16;
                case UnitID.minion: return darkBarracksLevel >= 1;
                case UnitID.hogrider: return darkBarracksLevel >= 2;
                case UnitID.valkyrie: return darkBarracksLevel >= 3;
                case UnitID.golem: return darkBarracksLevel >= 4;
                case UnitID.witch: return darkBarracksLevel >= 5;
                case UnitID.lavahound: return darkBarracksLevel >= 6;
                case UnitID.bowler: return darkBarracksLevel >= 7;
                case UnitID.icegolem: return darkBarracksLevel >= 8;
                case UnitID.headhunter: return darkBarracksLevel >= 9;
                default: return false;
            }
        }

        public static int GetNexLevelRequiredXp(int currentLevel)
        {
            if (currentLevel == 1) { return 30; }
            else if (currentLevel <= 200) { return (currentLevel - 1) * 50; }
            else if (currentLevel <= 299) { return ((currentLevel - 200) * 500) + 9500; }
            else { return ((currentLevel - 300) * 1000) + 60000; }
        }

        public static int GetTotalXpEarned(int currentLevel)
        {
            if (currentLevel == 1) { return 0; }
            else if (currentLevel <= 201) { return ((currentLevel - 1) * (currentLevel - 2) * 25) + 30; }
            else if (currentLevel <= 299) { return ((currentLevel - 200) * (currentLevel - 200) * 250) + (9250 * (currentLevel - 200)) + 985530; }
            else { return ((currentLevel - 300) * (currentLevel - 300) * 500) + (59500 * (currentLevel - 300)) + 4410530; }
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
            townhall, goldmine, goldstorage, elixirmine, elixirstorage, darkelixirmine, darkelixirstorage, buildershut, armycamp, barracks, darkbarracks, wall, cannon, archertower, mortor, airdefense, wizardtower, hiddentesla, bombtower, xbow, infernotower, decoration, obstacle, boomb, springtrap, airbomb, giantbomb, seekingairmine, skeletontrap, clancastle, spellfactory, darkspellfactory
        }

        public static int GetStorageGoldAndElixirLoot(int townhallLevel, float storage)
        {
            double p = 0;
            switch (townhallLevel)
            {
                case 1: case 2: case 3: case 4: case 5: case 6: p = 0.2d; break;
                case 7: p = 0.18d; break;
                case 8: p = 0.16d; break;
                case 9: p = 0.14d; break;
                case 10: p = 0.12d; break;
                default: p = 0.1d; break;
            }
            return (int)Math.Floor(storage * p);
        }

        public static int GetStorageDarkElixirLoot(int townhallLevel, float storage)
        {
            double p = 0;
            switch (townhallLevel)
            {
                case 1: case 2: case 3: case 4: case 5: case 6: case 7: case 8: p = 0.06d; break;
                case 9: p = 0.05d; break;
                default: p = 0.04d; break;
            }
            return (int)Math.Floor(storage * p);
        }

        public static int GetMinesGoldAndElixirLoot(int townhallLevel, float storage)
        {
            return (int)Math.Floor(storage * 0.5d);
        }

        public static int GetMinesDarkElixirLoot(int townhallLevel, float storage)
        {
            return (int)Math.Floor(storage * 0.75d);
        }

        public static (int, int) GetBattleTrophies(int attackerTrophies, int defendderTrophies)
        {
            int win = 0;
            int lose = 0;
            if (attackerTrophies == defendderTrophies)
            {
                win = 30;
                lose = 20;
            }
            else
            {
                double delta = Math.Abs(attackerTrophies - defendderTrophies);
                if (attackerTrophies > defendderTrophies)
                {
                    win = 30 - (int)Math.Floor(delta * (28d / 600d));
                    lose = 20 + (int)Math.Floor(delta * (19d / 600d));
                    if (win < 2)
                    {
                        win = 2;
                    }
                }
                else
                {
                    win = 30 + (int)Math.Floor(delta * (28d / 600d));
                    lose = 20 - (int)Math.Floor(delta * (19d / 600d));
                    if (lose < 1)
                    {
                        lose = 1;
                    }
                }
            }
            return (win, lose);
        }

        public static (int, int) GetWarTrophies(int clan1Trophies, int clan2Trophies, int clan1Stars, int clan2Stars, int maxStars)
        {
            int clan1 = 0;
            int clan2 = 0;
            if (clan1Stars != clan2Stars && (clan1Stars > 0 || clan2Stars > 0))
            {
                double delta = Math.Abs(clan1Trophies - clan2Trophies);
                if (clan1Stars > clan2Stars)
                {
                    double percentage = (double)clan1Stars / (double)maxStars;
                    clan1 = (int)Math.Floor((20 + (clan1Trophies < clan2Trophies ? delta * 0.05d : 0)) * percentage);
                    clan2 = -clan1;
                }
                else
                {
                    double percentage = (double)clan2Stars / (double)maxStars;
                    clan2 = (int)Math.Floor((20 + (clan2Trophies < clan1Trophies ? delta * 0.05d : 0)) * percentage);
                    clan1 = -clan2;
                }
            }
            else
            {
                clan1 = -5;
                clan2 = -5;
            }
            return (clan1, clan2);
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
            public Unit unit = null;
        }

        public enum BattleType
        {
            normal = 1, war = 2
        }

        public class BattleData
        {
            public Battle battle = null;
            public BattleType type = BattleType.normal;
            public List<BattleFrame> savedFrames = new List<BattleFrame>();
            public List<BattleFrame> frames = new List<BattleFrame>();
        }

        public class OpponentData
        {
            public long id = 0;
            public List<Building> buildings = null;
        }

        public class BattleStartBuildingData
        {
            public BuildingID id = BuildingID.townhall;
            public long databaseID = 0;
            public int lootGoldStorage = 0;
            public int lootElixirStorage = 0;
            public int lootDarkStorage = 0;
        }

        public class InitializationData
        {
            public long accountID = 0;
            public List<ServerUnit> serverUnits = new List<ServerUnit>();
        }

        public class ServerUnit
        {
            public UnitID id = UnitID.barbarian;
            public int level = 0;
            public int requiredGold = 0;
            public int requiredElixir = 0;
            public int requiredGems = 0;
            public int requiredDarkElixir = 0;
            public int trainTime = 0;
            public int health = 0;
            public int housing = 0;
        }

        public class Unit
        {
            public UnitID id = UnitID.barbarian;
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
            public BuildingID id = BuildingID.townhall;
            public int level = 0;
            public long databaseID = 0;
            public int x = 0;
            public int y = 0;
            public int warX = -1;
            public int warY = -1;
            public int columns = 0;
            public int rows = 0;
            public int goldStorage = 0;
            public int elixirStorage = 0;
            public int darkStorage = 0;
            public DateTime boost;
            public int health = 100;
            public float damage = 0;
            public int capacity = 0;
            public int goldCapacity = 0;
            public int elixirCapacity = 0;
            public int darkCapacity = 0;
            public float speed = 0;
            public float radius = 0;
            public DateTime constructionTime;
            public bool isConstructing = false;
            public int buildTime = 0;
            public BuildingTargetType targetType = BuildingTargetType.none;
            public float blindRange = 0;
            public float splashRange = 0;
            public float rangedSpeed = 5;
            public double percentage = 0;
        }

        public class ServerBuilding
        {
            public string id = "";
            public int level = 0;
            public long databaseID = 0;
            public int requiredGold = 0;
            public int requiredElixir = 0;
            public int requiredGems = 0;
            public int requiredDarkElixir = 0;
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

        public static BuildingCount GetBuildingLimits(int townHallLevel, string globalID)
        {
            for (int i = 0; i < buildingAvailability[townHallLevel].buildings.Length; i++)
            {
                if (buildingAvailability[townHallLevel].buildings[i].id == globalID)
                {
                    return buildingAvailability[townHallLevel].buildings[i];
                }
            }
            return null;
        }

        public static BuildingCount GetTownHallLimits(int targetTownHallLevel)
        {
            return null;
        }

        public static int GetInstantBuildRequiredGems(int remainedSeconds)
        {
            int gems = 0;
            if (remainedSeconds > 0)
            {
                if (remainedSeconds <= 60)
                {
                    gems = 1;
                }
                else if (remainedSeconds <= 3600)
                {
                    gems = (int)(0.00537f * ((float)remainedSeconds - 60f)) + 1;
                }
                else if (remainedSeconds <= 86400)
                {
                    gems = (int)(0.00266f * ((float)remainedSeconds - 3600f)) + 20;
                }
                else
                {
                    gems = (int)(0.00143f * ((float)remainedSeconds - 86400f)) + 260;
                }
            }
            return gems;
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
                    new BuildingCount { id = "townhall", count = 1, maxLevel = 15},
                    new BuildingCount { id = "buildershut", count = 5, maxLevel = 1},
                    new BuildingCount { id = "goldmine", count = 1, maxLevel = 2},
                    new BuildingCount { id = "elixirmine", count = 1, maxLevel = 2},
                    new BuildingCount { id = "goldstorage", count = 1, maxLevel = 1},
                    new BuildingCount { id = "elixirstorage", count = 1, maxLevel = 1},
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
                    new BuildingCount { id = "townhall", count = 1, maxLevel = 15},
                    new BuildingCount { id = "buildershut", count = 5, maxLevel = 1},
                    new BuildingCount { id = "goldmine", count = 2, maxLevel = 4},
                    new BuildingCount { id = "elixirmine", count = 2, maxLevel = 4},
                    new BuildingCount { id = "goldstorage", count = 1, maxLevel = 3},
                    new BuildingCount { id = "elixirstorage", count = 1, maxLevel = 3},
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
                    new BuildingCount { id = "townhall", count = 1, maxLevel = 15},
                    new BuildingCount { id = "buildershut", count = 5, maxLevel = 1},
                    new BuildingCount { id = "goldmine", count = 3, maxLevel = 6},
                    new BuildingCount { id = "elixirmine", count = 3, maxLevel = 6},
                    new BuildingCount { id = "goldstorage", count = 2, maxLevel = 6},
                    new BuildingCount { id = "elixirstorage", count = 2, maxLevel = 6},
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
                    new BuildingCount { id = "townhall", count = 1, maxLevel = 15},
                    new BuildingCount { id = "buildershut", count = 5, maxLevel = 1},
                    new BuildingCount { id = "goldmine", count = 4, maxLevel = 8},
                    new BuildingCount { id = "elixirmine", count = 4, maxLevel = 8},
                    new BuildingCount { id = "goldstorage", count = 2, maxLevel = 8},
                    new BuildingCount { id = "elixirstorage", count = 2, maxLevel = 8},
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
                    new BuildingCount { id = "townhall", count = 1, maxLevel = 15},
                    new BuildingCount { id = "buildershut", count = 5, maxLevel = 1},
                    new BuildingCount { id = "goldmine", count = 5, maxLevel = 10},
                    new BuildingCount { id = "elixirmine", count = 5, maxLevel = 10},
                    new BuildingCount { id = "goldstorage", count = 2, maxLevel = 9},
                    new BuildingCount { id = "elixirstorage", count = 2, maxLevel = 9},
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
                    new BuildingCount { id = "townhall", count = 1, maxLevel = 15},
                    new BuildingCount { id = "buildershut", count = 5, maxLevel = 1},
                    new BuildingCount { id = "goldmine", count = 6, maxLevel = 10},
                    new BuildingCount { id = "elixirmine", count = 6, maxLevel = 10},
                    new BuildingCount { id = "goldstorage", count = 2, maxLevel = 10},
                    new BuildingCount { id = "elixirstorage", count = 2, maxLevel = 10},
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
                    new BuildingCount { id = "townhall", count = 1, maxLevel = 15},
                    new BuildingCount { id = "buildershut", count = 5, maxLevel = 1},
                    new BuildingCount { id = "goldmine", count = 6, maxLevel = 11},
                    new BuildingCount { id = "elixirmine", count = 6, maxLevel = 11},
                    new BuildingCount { id = "darkelixirmine", count = 1, maxLevel = 3},
                    new BuildingCount { id = "goldstorage", count = 2, maxLevel = 11},
                    new BuildingCount { id = "elixirstorage", count = 2, maxLevel = 11},
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
                    new BuildingCount { id = "townhall", count = 1, maxLevel = 15},
                    new BuildingCount { id = "buildershut", count = 5, maxLevel = 1},
                    new BuildingCount { id = "goldmine", count = 6, maxLevel = 12},
                    new BuildingCount { id = "elixirmine", count = 6, maxLevel = 12},
                    new BuildingCount { id = "darkelixirmine", count = 2, maxLevel = 3},
                    new BuildingCount { id = "goldstorage", count = 3, maxLevel = 11},
                    new BuildingCount { id = "elixirstorage", count = 3, maxLevel = 11},
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
                    new BuildingCount { id = "townhall", count = 1, maxLevel = 15},
                    new BuildingCount { id = "buildershut", count = 5, maxLevel = 1},
                    new BuildingCount { id = "goldmine", count = 7, maxLevel = 12},
                    new BuildingCount { id = "elixirmine", count = 7, maxLevel = 12},
                    new BuildingCount { id = "darkelixirmine", count = 3, maxLevel = 6},
                    new BuildingCount { id = "goldstorage", count = 4, maxLevel = 11},
                    new BuildingCount { id = "elixirstorage", count = 4, maxLevel = 11},
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
                    new BuildingCount { id = "townhall", count = 1, maxLevel = 15},
                    new BuildingCount { id = "buildershut", count = 5, maxLevel = 1},
                    new BuildingCount { id = "goldmine", count = 7, maxLevel = 13},
                    new BuildingCount { id = "elixirmine", count = 7, maxLevel = 13},
                    new BuildingCount { id = "darkelixirmine", count = 3, maxLevel = 7},
                    new BuildingCount { id = "goldstorage", count = 4, maxLevel = 11},
                    new BuildingCount { id = "elixirstorage", count = 4, maxLevel = 11},
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
                    new BuildingCount { id = "townhall", count = 1, maxLevel = 15},
                    new BuildingCount { id = "buildershut", count = 5, maxLevel = 1},
                    new BuildingCount { id = "goldmine", count = 7, maxLevel = 14},
                    new BuildingCount { id = "elixirmine", count = 7, maxLevel = 14},
                    new BuildingCount { id = "darkelixirmine", count = 3, maxLevel = 8},
                    new BuildingCount { id = "goldstorage", count = 4, maxLevel = 12},
                    new BuildingCount { id = "elixirstorage", count = 4, maxLevel = 12},
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
                    new BuildingCount { id = "townhall", count = 1, maxLevel = 15},
                    new BuildingCount { id = "buildershut", count = 5, maxLevel = 1},
                    new BuildingCount { id = "goldmine", count = 7, maxLevel = 15},
                    new BuildingCount { id = "elixirmine", count = 7, maxLevel = 15},
                    new BuildingCount { id = "darkelixirmine", count = 3, maxLevel = 9},
                    new BuildingCount { id = "goldstorage", count = 4, maxLevel = 13},
                    new BuildingCount { id = "elixirstorage", count = 4, maxLevel = 13},
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
                    new BuildingCount { id = "townhall", count = 1, maxLevel = 15},
                    new BuildingCount { id = "buildershut", count = 5, maxLevel = 1},
                    new BuildingCount { id = "goldmine", count = 7, maxLevel = 15},
                    new BuildingCount { id = "elixirmine", count = 7, maxLevel = 15},
                    new BuildingCount { id = "darkelixirmine", count = 3, maxLevel = 9},
                    new BuildingCount { id = "goldstorage", count = 4, maxLevel = 14},
                    new BuildingCount { id = "elixirstorage", count = 4, maxLevel = 14},
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
                    new BuildingCount { id = "townhall", count = 1, maxLevel = 15},
                    new BuildingCount { id = "buildershut", count = 5, maxLevel = 1},
                    new BuildingCount { id = "goldmine", count = 7, maxLevel = 15},
                    new BuildingCount { id = "elixirmine", count = 7, maxLevel = 15},
                    new BuildingCount { id = "darkelixirmine", count = 3, maxLevel = 9},
                    new BuildingCount { id = "goldstorage", count = 4, maxLevel = 15},
                    new BuildingCount { id = "elixirstorage", count = 4, maxLevel = 15},
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
                    new BuildingCount { id = "townhall", count = 1, maxLevel = 15},
                    new BuildingCount { id = "buildershut", count = 5, maxLevel = 1},
                    new BuildingCount { id = "goldmine", count = 7, maxLevel = 15},
                    new BuildingCount { id = "elixirmine", count = 7, maxLevel = 15},
                    new BuildingCount { id = "darkelixirmine", count = 3, maxLevel = 9},
                    new BuildingCount { id = "goldstorage", count = 4, maxLevel = 16},
                    new BuildingCount { id = "elixirstorage", count = 4, maxLevel = 16},
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