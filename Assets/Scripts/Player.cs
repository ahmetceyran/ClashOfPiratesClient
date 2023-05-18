namespace AhmetsHub.ClashOfPirates
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using DevelopersHub.RealtimeNetworking.Client;
    using UnityEngine.SceneManagement;

    public class Player : MonoBehaviour
    {

        public Data.Player data = new Data.Player();
        private static Player _instance = null; public static Player instanse { get { return _instance; } }
        public Data.InitializationData initializationData = new Data.InitializationData();
        private bool _inBattle = false; public static bool inBattle { get { return instanse._inBattle; } set { instanse._inBattle = value; } }

        public enum RequestsID
        {
            AUTH = 1, SYNC = 2, BUILD = 3, REPLACE = 4, COLLECT = 5, PREUPGRADE = 6, UPGRADE = 7, INSTANTBUILD = 8, TRAIN = 9, CANCELTRAIN = 10, BATTLEFIND = 11, BATTLESTART = 12, BATTLEFRAME = 13, BATTLEEND = 14
        }

        private void Start()
        {
            RealtimeNetworking.OnPacketReceived += ReceivedPaket;
            RealtimeNetworking.OnDisconnectedFromServer += DisconnectedFromServer;
            string device = SystemInfo.deviceUniqueIdentifier;
            Packet packet = new Packet();
            packet.Write((int)RequestsID.AUTH);
            packet.Write(device);
            Sender.TCP_Send(packet);
        }

        private void Awake()
        {
            _instance = this;
        }

        private bool connected = false;
        private float timer = 0;
        private bool updating = false;
        private float syncTime = 5;

        private void Update()
        {
            if (connected)
            {
                if (!_inBattle)
                {
                    if (timer <= 0)
                    {
                        if(updating == false)
                        {
                            updating = true;
                            timer = syncTime;
                            SendSyncRequest();
                        }
                    }
                    else
                    {
                        timer -= Time.deltaTime;
                    }
                }
                data.nowTime = data.nowTime.AddSeconds(Time.deltaTime);
            }
        }

        private void ReceivedPaket(Packet packet)
        {
            try
            {
                int id = packet.ReadInt();
                long databaseID = 0;
                int response = 0;

                switch ((RequestsID)id)
                {
                    case RequestsID.AUTH:
                        connected = true;
                        updating = true;
                        timer = 0;
                        string authData = packet.ReadString();
                        initializationData = Data.Desrialize<Data.InitializationData>(authData);


                        SendSyncRequest();
                        break;
                    case RequestsID.SYNC:
                        string playerData = packet.ReadString();
                        Data.Player playerSyncData = Data.Desrialize<Data.Player>(playerData);
                        SyncData(playerSyncData);
                        updating = false;
                        break;
                    case RequestsID.BUILD:
                        response = packet.ReadInt();
                        switch (response)
                        {
                            case 0:
                                Debug.Log("Unknown");
                                break;
                            case 1:
                                Debug.Log("Placed successfully");
                                RushSyncRequest();
                                break;
                            case 2:
                                Debug.Log("No resources");
                                break;
                            case 3:
                                Debug.Log("Max level");
                                break;
                            case 4:
                                Debug.Log("Place taken");
                                break;
                            case 5:
                                Debug.Log("No builder");
                                break;
                            case 6:
                                Debug.Log("Max limit reached");
                                break;
                        }
                        break;
                    case RequestsID.REPLACE:
                        int replaceResponse = packet.ReadInt();
                        int replaceX = packet.ReadInt();
                        int replaceY = packet.ReadInt();
                        long replaceID = packet.ReadLong();

                        for (int i = 0; i < UI_Main.instanse._grid.buildings.Count; i++)
                        {
                            if (UI_Main.instanse._grid.buildings[i].databaseID == replaceID)
                            {
                                switch (replaceResponse)
                                {
                                    case 0:
                                        Debug.Log("No building");
                                        break;
                                    case 1:
                                        Debug.Log("Replace successfully");
                                        UI_Main.instanse._grid.buildings[i].PlacedOnGrid(replaceX, replaceY);
                                        if (UI_Main.instanse._grid.buildings[i] != Building.selectedInstanse)
                                        {

                                        }
                                        RushSyncRequest();
                                        break;
                                    case 2:
                                        Debug.Log("Place taken");
                                        break;
                                }
                                UI_Main.instanse._grid.buildings[i].waitingReplaceResponse = false;
                                break;
                            }
                        }
                        break;
                    case RequestsID.COLLECT:
                        long db = packet.ReadLong();
                        int collected = packet.ReadInt();
                        for (int i = 0; i < UI_Main.instanse._grid.buildings.Count; i++)
                        {
                            if (db == UI_Main.instanse._grid.buildings[i].data.databaseID)
                            {
                                UI_Main.instanse._grid.buildings[i].collecting = false;
                                switch (UI_Main.instanse._grid.buildings[i].id)
                                {
                                    case Data.BuildingID.goldmine:
                                        UI_Main.instanse._grid.buildings[i].data.goldStorage -= collected;
                                        break;
                                    case Data.BuildingID.elixirmine:
                                        UI_Main.instanse._grid.buildings[i].data.elixirStorage -= collected;
                                        break;
                                    case Data.BuildingID.darkelixirmine:
                                        UI_Main.instanse._grid.buildings[i].data.darkStorage -= collected;
                                        break;
                                }
                                UI_Main.instanse._grid.buildings[i].AdjustUI();
                                break;
                            }
                        }
                        break;
                    case RequestsID.PREUPGRADE:
                        databaseID = packet.ReadLong();
                        string re = packet.ReadString();
                        Data.ServerBuilding sr = Data.Desrialize<Data.ServerBuilding>(re);
                        UI_BuildingUpgrade.instanse.Open(sr, databaseID);
                        break;
                    case RequestsID.UPGRADE:
                        response = packet.ReadInt();
                        switch (response)
                        {
                            case 0:
                                Debug.Log("Unknown");
                                break;
                            case 1:
                                Debug.Log("Upgrade started");
                                RushSyncRequest();
                                break;
                            case 2:
                                Debug.Log("No resources");
                                break;
                            case 3:
                                Debug.Log("Max level");
                                break;
                            case 5:
                                Debug.Log("No builder");
                                break;
                            case 6:
                                Debug.Log("Max limit reached");
                                break;
                        }
                        break;
                    case RequestsID.INSTANTBUILD:
                        response = packet.ReadInt();
                        if (response == 2)
                        {
                            Debug.Log("No gems.");
                        }
                        else if (response == 1)
                        {
                            Debug.Log("Instant built.");
                            RushSyncRequest();
                        }
                        else
                        {
                            Debug.Log("Nothing happend.");
                        }
                        break;
                    case RequestsID.TRAIN:
                        response = packet.ReadInt();
                        if (response == 4)
                        {
                            Debug.Log("Server unit not found.");
                        }
                        if (response == 3)
                        {
                            Debug.Log("No capacity.");
                        }
                        if (response == 2)
                        {
                            Debug.Log("No resources.");
                        }
                        else if (response == 1)
                        {
                            Debug.Log("Train started.");
                            RushSyncRequest();
                        }
                        else
                        {
                            Debug.Log("Nothing happend.");
                        }
                        break;
                    case RequestsID.CANCELTRAIN:
                        response = packet.ReadInt();
                        if (response == 1)
                        {
                            RushSyncRequest();
                        }
                        break;
                    case RequestsID.BATTLEFIND:
                        long target = packet.ReadLong();
                        Data.OpponentData opponent = null;
                        if (target > 0)
                        {
                            string d = packet.ReadString();
                            opponent = Data.Desrialize<Data.OpponentData>(d);
                        }
                        UI_Search.instanse.FindResponded(target, opponent);
                        break;
                    case RequestsID.BATTLESTART:
                        bool matched = packet.ReadBool();
                        bool attack = packet.ReadBool();
                        bool confirmed = matched && attack;
                        List<Data.BattleStartBuildingData> buildings = null;
                        int wt = 0;
                        int lt = 0;
                        if (confirmed)
                        {
                            wt = packet.ReadInt();
                            lt = packet.ReadInt();
                            string bsbd = packet.ReadString();
                            buildings = Data.Desrialize<List<Data.BattleStartBuildingData>>(bsbd);
                        }
                        UI_Battle.instanse.StartBattleConfirm(confirmed, buildings, wt, lt);
                        break;
                    case RequestsID.BATTLEEND:
                        int stars = packet.ReadInt();
                        int unitsDeployed = packet.ReadInt();
                        int lootedGold = packet.ReadInt();
                        int lootedElixir = packet.ReadInt();
                        int lootedDark = packet.ReadInt();
                        int trophies = packet.ReadInt();
                        int frame = packet.ReadInt();
                        UI_Battle.instanse.BattleEnded(stars, unitsDeployed, lootedGold, lootedElixir, lootedDark, trophies, frame);
                        break;
                }
            }
            catch (System.Exception ex)
            {
                Debug.Log(ex.Message);
            }
        }

        public void SendSyncRequest()
        {
            Packet p = new Packet();
            p.Write((int)RequestsID.SYNC);
            p.Write(SystemInfo.deviceUniqueIdentifier);
            Sender.TCP_Send(p);
        }

        [HideInInspector] public int gold = 0;
        [HideInInspector] public int maxGold = 0;

        [HideInInspector] public int elixir = 0;
        [HideInInspector] public int maxElixir = 0;

        [HideInInspector] public int darkElixir = 0;
        [HideInInspector] public int maxDarkElixir = 0;

        public void SyncData(Data.Player player)
        {
            data = player;

            if (_inBattle)
            {
                return;
            }

            gold = 0;
            maxGold = 0;

            elixir = 0;
            maxElixir = 0;

            darkElixir = 0;
            maxDarkElixir = 0;

            int gems = player.gems;

            if(player.buildings != null && player.buildings.Count > 0)
            {
                for (int i = 0; i < player.buildings.Count; i++)
                {
                    Building building = UI_Main.instanse._grid.GetBuilding(player.buildings[i].databaseID);
                    if(building != null)
                    {

                    }
                    else
                    {
                        Building prefab = UI_Main.instanse.GetBuildingPrefab(player.buildings[i].id);
                        if (prefab)
                        {
                            building = Instantiate(prefab, Vector3.zero, Quaternion.identity);
                            building.databaseID = player.buildings[i].databaseID;
                            building.PlacedOnGrid(player.buildings[i].x, player.buildings[i].y);
                            building._baseArea.gameObject.SetActive(false);

                            UI_Main.instanse._grid.buildings.Add(building);
                        }
                    }
                    
                    if (building.buildBar == null)
                    {
                        building.buildBar = Instantiate(UI_Main.instanse.barBuild, UI_Main.instanse.buttonsParent);
                        building.buildBar.gameObject.SetActive(false);
                    }

                    building.data = player.buildings[i];
                    switch (building.id)
                    {
                        case Data.BuildingID.townhall:
                            maxGold += building.data.goldCapacity;
                            gold += building.data.goldStorage;
                            maxElixir += building.data.elixirCapacity;
                            elixir += building.data.elixirStorage;
                            maxDarkElixir += building.data.darkCapacity;
                            darkElixir += building.data.darkStorage;
                            break;
                        case Data.BuildingID.goldmine:
                            if (building.collectButton == null)
                            {
                                building.collectButton = Instantiate(UI_Main.instanse.buttonCollectGold, UI_Main.instanse.buttonsParent);
                                building.collectButton.button.onClick.AddListener(building.Collect);
                                building.collectButton.gameObject.SetActive(false);
                            }
                            break;
                        case Data.BuildingID.goldstorage:
                            maxGold += building.data.goldCapacity;
                            gold += building.data.goldStorage;
                            break;
                        case Data.BuildingID.elixirmine:
                            if (building.collectButton == null)
                            {
                                building.collectButton = Instantiate(UI_Main.instanse.buttonCollectElixir, UI_Main.instanse.buttonsParent);
                                building.collectButton.button.onClick.AddListener(building.Collect);
                                building.collectButton.gameObject.SetActive(false);
                            }
                            break;
                        case Data.BuildingID.elixirstorage:
                            maxElixir += building.data.elixirCapacity;
                            elixir += building.data.elixirStorage;
                            break;
                        case Data.BuildingID.darkelixirmine:
                            if (building.collectButton == null)
                            {
                                building.collectButton = Instantiate(UI_Main.instanse.buttonCollectDarkElixir, UI_Main.instanse.buttonsParent);
                                building.collectButton.button.onClick.AddListener(building.Collect);
                                building.collectButton.gameObject.SetActive(false);
                            }
                            break;
                        case Data.BuildingID.darkelixirstorage:
                            maxDarkElixir += building.data.darkCapacity;
                            darkElixir += building.data.darkStorage;
                            break;
                    }
                    building.AdjustUI();
                }
            }

            for (int i = 0; i < player.units.Count; i++)
            {

            }

            UI_Main.instanse._goldText.text = gold + "/" + maxGold;
            UI_Main.instanse._elixirText.text = elixir + "/" + maxElixir;
            UI_Main.instanse._gemsText.text = gems.ToString();

            if (UI_Train.instanse.isOpen)
            {
                UI_Train.instanse.Sync();
            }
        }

        public void RushSyncRequest()
        {
            timer = 0;
        }

        private void DisconnectedFromServer()
        {
            ThreadDispatcher.instance.Enqueue(() => Desconnected());
        }

        private void Desconnected()
        {
            connected = false;
            RealtimeNetworking.OnDisconnectedFromServer -= DisconnectedFromServer;
            MessageBox.Open(0, 0.8f, false, MessageResponded, new string[] { "Failed to connect to server. Please check you internet connection and try again." }, new string[] { "Try Again" });
        }

        private void MessageResponded(int layoutIndex, int buttonIndex)
        {
            if (layoutIndex == 0)
            {
                SceneManager.LoadScene(0);
            }
        }

    }
}