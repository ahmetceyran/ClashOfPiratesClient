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
            AUTH = 1, SYNC = 2, BUILD = 3, REPLACE = 4, COLLECT = 5, PREUPGRADE = 6, UPGRADE = 7, INSTANTBUILD = 8, TRAIN = 9, CANCELTRAIN = 10, BATTLEFIND = 11, BATTLESTART = 12, BATTLEFRAME = 13, BATTLEEND = 14, OPENCLAN = 15, GETCLANS = 16, JOINCLAN = 17, LEAVECLAN = 18, EDITCLAN = 19, CREATECLAN = 20, OPENWAR = 21, STARTWAR = 22, CANCELWAR = 23, WARSTARTED = 24, WARATTACK = 25, WARREPORTLIST = 26, WARREPORT = 27, JOINREQUESTS = 28, JOINRESPONSE = 29, GETCHATS = 30, SENDCHAT = 31, SENDCODE = 32, CONFIRMCODE = 33, EMAILCODE = 34, EMAILCONFIRM = 35, LOGOUT = 36, KICKMEMBER = 37
        }

        public static readonly string username_key = "username";
        public static readonly string password_key = "password";

        private void Start()
        {
            RealtimeNetworking.OnPacketReceived += ReceivedPaket;
            RealtimeNetworking.OnDisconnectedFromServer += DisconnectedFromServer;
            string device = SystemInfo.deviceUniqueIdentifier;
            string password = "";
            string username = "";
            if (PlayerPrefs.HasKey(password_key))
            {
                password = PlayerPrefs.GetString(password_key);
            }
            if (PlayerPrefs.HasKey(password_key))
            {
                username = PlayerPrefs.GetString(username_key);
            }
            Packet packet = new Packet();
            packet.Write((int)RequestsID.AUTH);
            packet.Write(device);
            packet.Write(password);
            packet.Write(username);
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
                        PlayerPrefs.SetString(password_key, initializationData.password);
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
                    case RequestsID.OPENCLAN:
                        bool haveClan = packet.ReadBool();
                        Data.Clan clan = null;
                        List<Data.ClanMember> warMembers = null;
                        if (haveClan)
                        {
                            string clanData = packet.ReadString();
                            clan = Data.Desrialize<Data.Clan>(clanData);
                            if (clan.war != null && clan.war.id > 0)
                            {
                                string warData = packet.ReadString();
                                warMembers = Data.Desrialize<List<Data.ClanMember>>(warData);
                            }
                        }
                        UI_Clan.instanse.ClanOpen(clan, warMembers);
                        break;
                    case RequestsID.GETCLANS:
                        string clansData = packet.ReadString();
                        Data.ClansList clans = Data.Desrialize<Data.ClansList>(clansData);
                        UI_Clan.instanse.ClansListOpen(clans);
                        break;
                    case RequestsID.CREATECLAN:
                        response = packet.ReadInt();
                        UI_Clan.instanse.CreateResponse(response);
                        break;
                    case RequestsID.JOINCLAN:
                        response = packet.ReadInt();
                        UI_Clan.instanse.JoinResponse(response);
                        break;
                    case RequestsID.LEAVECLAN:
                        response = packet.ReadInt();
                        UI_Clan.instanse.LeaveResponse(response);
                        break;
                    case RequestsID.EDITCLAN:
                        response = packet.ReadInt();
                        UI_Clan.instanse.EditResponse(response);
                        break;
                    case RequestsID.OPENWAR:
                        string clanWarData = packet.ReadString();
                        Data.ClanWarData war = Data.Desrialize<Data.ClanWarData>(clanWarData);
                        UI_Clan.instanse.WarOpen(war);
                        break;
                    case RequestsID.STARTWAR:
                        response = packet.ReadInt();
                        UI_Clan.instanse.WarStartResponse(response);
                        break;
                    case RequestsID.CANCELWAR:
                        response = packet.ReadInt();
                        UI_Clan.instanse.WarSearchCancelResponse(response);
                        break;
                    case RequestsID.WARSTARTED:
                        databaseID = packet.ReadInt();
                        UI_Clan.instanse.WarStarted(databaseID);
                        break;
                    case RequestsID.WARATTACK:
                        databaseID = packet.ReadLong();
                        Data.OpponentData warOpponent = null;
                        if (databaseID > 0)
                        {
                            string d = packet.ReadString();
                            warOpponent = Data.Desrialize<Data.OpponentData>(d);
                        }
                        UI_Clan.instanse.AttackResponse(databaseID, warOpponent);
                        break;
                    case RequestsID.WARREPORTLIST:
                        string warReportsData = packet.ReadString();
                        List<Data.ClanWarData> warReports = Data.Desrialize<List<Data.ClanWarData>>(warReportsData);
                        UI_Clan.instanse.OpenWarHistoryList(warReports);
                        break;
                    case RequestsID.WARREPORT:
                        bool hasReport = packet.ReadBool();
                        Data.ClanWarData warReport = null;
                        if (hasReport)
                        {
                            string warReportData = packet.ReadString();
                            warReport = Data.Desrialize<Data.ClanWarData>(warReportData);
                        }
                        UI_Clan.instanse.WarOpen(warReport, true);
                        break;
                    case RequestsID.JOINREQUESTS:
                        string requstsData = packet.ReadString();
                        List<Data.JoinRequest> requests = Data.Desrialize<List<Data.JoinRequest>>(requstsData);
                        UI_Clan.instanse.OpenRequestsList(requests);
                        break;
                    case RequestsID.JOINRESPONSE:
                        response = packet.ReadInt();
                        if (UI_ClanJoinRequest.active != null)
                        {
                            UI_ClanJoinRequest.active.Response(response);
                            UI_ClanJoinRequest.active = null;
                        }
                        break;
                    case RequestsID.SENDCHAT:
                        response = packet.ReadInt();
                        UI_Chat.instanse.ChatSendResponse(response);
                        break;
                    case RequestsID.GETCHATS:
                        string chatsData = packet.ReadString();
                        List<Data.CharMessage> messages = Data.Desrialize<List<Data.CharMessage>>(chatsData);
                        int chatType = packet.ReadInt();
                        UI_Chat.instanse.ChatSynced(messages, (Data.ChatType)chatType);
                        break;
                    case RequestsID.EMAILCODE:
                        response = packet.ReadInt();
                        int expTime = packet.ReadInt();
                        UI_Settings.instanse.EmailSendResponse(response, expTime);
                        break;
                    case RequestsID.EMAILCONFIRM:
                        response = packet.ReadInt();
                        string confEmail = packet.ReadString();
                        UI_Settings.instanse.EmailConfirmResponse(response, confEmail);
                        break;
                    case RequestsID.KICKMEMBER:
                        databaseID = packet.ReadLong();
                        response = packet.ReadInt();
                        if (response == -1)
                        {
                            string kicker = packet.ReadString();
                            if (UI_Clan.instanse.isActive)
                            {
                                UI_Clan.instanse.Close();
                            }
                        }
                        else
                        {
                            UI_Clan.instanse.kickResponse(databaseID, response);
                        }
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

            gold = 0;
            maxGold = 0;

            elixir = 0;
            maxElixir = 0;

            darkElixir = 0;
            maxDarkElixir = 0;

            int gems = player.gems;

            if (player.buildings != null && player.buildings.Count > 0)
            {
                for (int i = 0; i < player.buildings.Count; i++)
                {
                    switch (player.buildings[i].id)
                    {
                        case Data.BuildingID.townhall:
                            maxGold += player.buildings[i].goldCapacity;
                            gold += player.buildings[i].goldStorage;
                            maxElixir += player.buildings[i].elixirCapacity;
                            elixir += player.buildings[i].elixirStorage;
                            maxDarkElixir += player.buildings[i].darkCapacity;
                            darkElixir += player.buildings[i].darkStorage;
                            break;
                        case Data.BuildingID.goldstorage:
                            maxGold += player.buildings[i].goldCapacity;
                            gold += player.buildings[i].goldStorage;
                            break;
                        case Data.BuildingID.elixirstorage:
                            maxElixir += player.buildings[i].elixirCapacity;
                            elixir += player.buildings[i].elixirStorage;
                            break;
                        case Data.BuildingID.darkelixirstorage:
                            maxDarkElixir += player.buildings[i].darkCapacity;
                            darkElixir += player.buildings[i].darkStorage;
                            break;
                    }
                }
            }

            for (int i = 0; i < player.units.Count; i++)
            {

            }

            UI_Main.instanse._goldText.text = gold + "/" + maxGold;
            UI_Main.instanse._elixirText.text = elixir + "/" + maxElixir;
            UI_Main.instanse._gemsText.text = gems.ToString();

            if (UI_Main.instanse.isActive && !UI_WarLayout.instanse.isActive)
            {
                UI_Main.instanse.DataSynced();
            }
            else if (UI_WarLayout.instanse.isActive)
            {
                UI_WarLayout.instanse.DataSynced();
            }
            else if (UI_Train.instanse.isOpen)
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
                RestartGame();
            }
        }

        public static void RestartGame()
        {
            SceneManager.LoadScene(0);
        }

    }
}