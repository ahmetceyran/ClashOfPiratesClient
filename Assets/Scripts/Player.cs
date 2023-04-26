namespace AhmetsHub.ClashOfPirates
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using DevelopersHub.RealtimeNetworking.Client;

    public class Player : MonoBehaviour
    {

        public Data.Player data = new Data.Player();
        private static Player _instance = null; public static Player instanse {get {return _instance; }}
        public Data.InitializationData initializationData = new Data.InitializationData();
        private bool _inBattle = false; public static bool inBattle { get { return instanse._inBattle; } set { instanse._inBattle = value; } }

        public enum RequestsID
        {
            AUTH = 1, SYNC = 2, BUILD = 3, REPLACE = 4, COLLECT = 5, PREUPGRADE = 6, UPGRADE = 7, INSTANTBUILD = 8, TRAIN = 9, CANCELTRAIN = 10, BATTLEFIND = 11, BATTLESTART = 12, BATTLEFRAME = 13
        }

        private void Start()
        {
            RealtimeNetworking.OnPacketReceived += ReceivedPaket;
            ConnectToServer();
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
            if(connected)
            {
                if(!_inBattle)
                {
                    if(timer <= 0)
                    {
                        updating = true;
                        timer = syncTime;
                        SendSyncRequest();
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
                            Debug.Log("Max Level");
                            break;
                        case 4:
                            Debug.Log("Place taken");
                            break;
                        case 5:
                            Debug.Log("No Builder");
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

                    for(int i = 0; i < UI_Main.instanse._grid.buildings.Count; i++)
                    {
                        if(UI_Main.instanse._grid.buildings[i].databaseID == replaceID)
                        {
                            switch (replaceResponse)
                            {
                                case 0:
                                    Debug.Log("No building");
                                    break;
                                case 1:
                                    Debug.Log("Replaced successfully");
                                    UI_Main.instanse._grid.buildings[i].PlacedOnGrid(replaceX, replaceY);
                                    if(UI_Main.instanse._grid.buildings[i] != Building.selectedInstanse)
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
                    Debug.Log("Collected " + collected);
                    for(int i = 0; i < UI_Main.instanse._grid.buildings.Count; i++)
                    {
                        if(db == UI_Main.instanse._grid.buildings[i].data.databaseID)
                        {
                            UI_Main.instanse._grid.buildings[i].collecting = false;
                            UI_Main.instanse._grid.buildings[i].data.storage -= collected;
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
                            Debug.Log("Max Level");
                            break;
                        case 5:
                            Debug.Log("No Builder");
                            break;
                        case 6:
                            Debug.Log("Max limit reached");
                            break;
                    }
                    break;
                case RequestsID.INSTANTBUILD:
                    response = packet.ReadInt();
                    if(response == 2)
                    {
                        Debug.Log("No diamonds.");
                    }
                    else if(response == 1)
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
                    if(response == 4)
                    {
                        Debug.Log("Server unit not found.");
                    }
                    else if(response == 3)
                    {
                        Debug.Log("No capacity.");
                    }
                    else if(response == 2)
                    {
                        Debug.Log("No resources.");
                    }
                    else if(response == 1)
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
                    if(response == 1)
                    {
                        RushSyncRequest();
                    }
                    break;
                case RequestsID.BATTLEFIND:
                    long target = packet.ReadLong();
                    Data.OpponentData opponent = null;
                    if(target > 0)
                    {
                        string d = packet.ReadString();
                        opponent = Data.Desrialize<Data.OpponentData>(d);
                    }
                    UI_Search.instanse.FindResponded(target, opponent);
                    break;
                case RequestsID.BATTLESTART:
                    bool matched = packet.ReadBool();
                    bool attack = packet.ReadBool();
                    UI_Battle.instanse.StartBattleConfirm(matched && attack);
                    break;
            }
        }

        public void SendSyncRequest()
        {
            Packet p = new Packet();
            p.Write((int)RequestsID.SYNC);
            p.Write(SystemInfo.deviceUniqueIdentifier);
            Sender.TCP_Send(p);
        }

        public void SyncData(Data.Player player)
        {
            data = player;

            if(_inBattle)
            {
                return;
            }

            int gold = 0;
            int maxGold = 0;

            int fish = 0;
            int maxFish = 0;

            int diamonds = player.diamonds;

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

                    if(building.buildBar == null)
                    {
                        building.buildBar = Instantiate(UI_Main.instanse.barBuild, UI_Main.instanse.buttonsParent);
                        building.buildBar.gameObject.SetActive(false);
                    }

                    building.data = player.buildings[i];
                    switch(building.id)
                    {
                        case Data.BuildingID.islandhall:
                            break;
                        case Data.BuildingID.fisher:
                            if(building.collectButton == null)
                            {
                                building.collectButton = Instantiate(UI_Main.instanse.buttonCollectFish, UI_Main.instanse.buttonsParent);
                                building.collectButton.button.onClick.AddListener(building.Collect);
                                building.collectButton.gameObject.SetActive(false);
                            }
                            break;
                        case Data.BuildingID.fishstorage:
                            maxFish += building.data.capacity;
                            fish += building.data.storage;
                            break;
                        case Data.BuildingID.goldmine:
                            if(building.collectButton == null)
                            {
                                building.collectButton = Instantiate(UI_Main.instanse.buttonCollectGold, UI_Main.instanse.buttonsParent);
                                building.collectButton.button.onClick.AddListener(building.Collect);
                                building.collectButton.gameObject.SetActive(false);
                            }
                            break;
                        case Data.BuildingID.goldstorage:
                            maxGold += building.data.capacity;
                            gold += building.data.storage;
                            break;
                    }
                    building.AdjustUI();
                }
            }

            for(int i = 0; i < player.units.Count; i++)
            {

            }

            UI_Main.instanse._goldText.text = " : " + gold + " / " + maxGold;
            UI_Main.instanse._fishText.text = " : " + fish + " / " + maxFish;
            UI_Main.instanse._diamondsText.text = " : " + diamonds.ToString();

            if(UI_Train.instanse.isOpen)
            {
                UI_Train.instanse.Sync();
            }
        }

        public void RushSyncRequest()
        {
            timer = 0;
        }

        private void ConnectionResponse(bool successful)
        {
            if (successful)
            {
                RealtimeNetworking.OnDisconnectedFromServer += DisconnectedFromServer;
                string device = SystemInfo.deviceUniqueIdentifier;
                Packet packet = new Packet();
                packet.Write((int)RequestsID.AUTH);
                packet.Write(device);
                Sender.TCP_Send(packet);
            }
            else
            {
                // TODO: Connection failed message box with retry button.
            }
            RealtimeNetworking.OnConnectingToServerResult -= ConnectionResponse;
        }

        public void ConnectToServer()
        {
            RealtimeNetworking.OnConnectingToServerResult += ConnectionResponse;
            RealtimeNetworking.Connect();
        }

        private void DisconnectedFromServer()
        {
            connected = false;
            RealtimeNetworking.OnDisconnectedFromServer -= DisconnectedFromServer;
            // TODO: Connection failed message box with retry button.
        }

    }
}