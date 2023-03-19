namespace AhmetsHub.ClashOfPirates
{
    using System.Xml.Serialization;
    using System.IO;
    using System.Collections.Generic;
    using System;

    public static class Data
    {
        public const int minGoldCollect = 10;
        public const int minFishCollect = 10;

        public class Player
        {
            public int gold = 0;
            public int fish = 0;
            public int diamonds = 0;
            public DateTime nowTime;
            public List<Building> buildings = new List<Building>();
        }
        public enum BuildingID
        {
            islandhall, goldmine, goldstorage, fisher, fishstorage, buildershut
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

    }
}