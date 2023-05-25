namespace AhmetsHub.ClashOfPirates
{
    using DevelopersHub.RealtimeNetworking.Client;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class UI_Battle : MonoBehaviour
    {

        Battle battle = null;
        private bool isStarted = false;
        private bool readyToStart = false;
        [SerializeField] private GameObject _endPanel = null;
        [SerializeField] public TextMeshProUGUI _timerText = null;
        [SerializeField] public TextMeshProUGUI _percentageText = null;
        [SerializeField] public TextMeshProUGUI _lootGoldText = null;
        [SerializeField] public TextMeshProUGUI _lootElixirText = null;
        [SerializeField] public TextMeshProUGUI _lootDarkText = null;
        [SerializeField] private UI_Bar healthBarPrefab = null;
        [SerializeField] private RectTransform healthBarGrid = null;
        [SerializeField] private BattleUnit[] battleUnits = null;
        [SerializeField] private Button _findButton = null;
        [SerializeField] private Button _closeButton = null;
        [SerializeField] private Button _okButton = null;
        [SerializeField] private Button _surrenderButton = null;
        private List<BattleUnit> unitsOnGrid = new List<BattleUnit>();
        public List<BuildingOnGrid> buildingsOnGrid = new List<BuildingOnGrid>();
        private DateTime baseTime;
        private List<UnitToAdd> toAdd = new List<UnitToAdd>();
        private long target = 0;
        private bool surrender = false;
        private Data.BattleType _battleType = Data.BattleType.normal;

        public class BuildingOnGrid
        {
            public long id = 0;
            public int index = -1;
            public Building building = null;
            public UI_Bar healthBar = null;
        }

        private class UnitToAdd
        {
            public UnitToAdd(long id, int x, int y)
            {
                this.id = id;
                this.x = x;
                this.y = y;
            }
            public long id;
            public int x;
            public int y;
        }

        private void Start()
        {
            _closeButton.onClick.AddListener(Close);
            _findButton.onClick.AddListener(Find);
            _okButton.onClick.AddListener(CloseEndPanel);
            _surrenderButton.onClick.AddListener(Surrender);
        }

        private void CloseEndPanel()
        {
            Close();
        }

        private void MessageResponded(int layoutIndex, int buttonIndex)
        {
            if (layoutIndex == 1)
            {
                MessageBox.Close();
            }
        }

        private void Close()
        {
            Player.instanse.SyncData(Player.instanse.data);
            isStarted = false;
            readyToStart = false;
            SetStatus(false);
            UI_Main.instanse.SetStatus(true);
        }

        public void Find()
        {
            readyToStart = false;
            _findButton.gameObject.SetActive(false);
            _closeButton.gameObject.SetActive(false);
            UI_Search.instanse.Find();
        }

        List<Data.Building> startbuildings = new List<Data.Building>();
        List<Battle.Building> battleBuildings = new List<Battle.Building>();

        public void NoTarget()
        {
            Close();
            MessageBox.Open(1, 0.8f, true, MessageResponded, new string[] { "There is no target to attack at this moment. Please try again later." }, new string[] { "OK" });
        }

        public bool Display(List<Data.Building> buildings, long defender, Data.BattleType battleType)
        {
            ClearUnits();
            for (int i = 0; i < Player.instanse.data.units.Count; i++)
            {
                if (!Player.instanse.data.units[i].ready)
                {
                    continue;
                }
                int k = -1;
                for (int j = 0; j < units.Count; j++)
                {
                    if (units[j].id == Player.instanse.data.units[i].id)
                    {
                        k = j;
                        break;
                    }
                }
                if (k < 0)
                {
                    k = units.Count;
                    UI_BattleUnit bu = Instantiate(unitsPrefab, unitsGrid);
                    bu.Initialize(Player.instanse.data.units[i].id);
                    units.Add(bu);
                }
                units[k].Add(Player.instanse.data.units[i].databaseID);
            }

            if (units.Count <= 0)
            {
                MessageBox.Open(1, 0.8f, true, MessageResponded, new string[] { "You do not have any units for battle.." }, new string[] { "OK" });
                return false;
            }

            _battleType = battleType;
            int townhallLevel = 1;
            for (int i = 0; i < buildings.Count; i++)
            {
                if (buildings[i].id == Data.BuildingID.townhall)
                {
                    townhallLevel = buildings[i].level;
                    if (_battleType == Data.BattleType.normal)
                    {
                        break;
                    }
                }
                if (_battleType == Data.BattleType.war)
                {
                    buildings[i].x = buildings[i].warX;
                    buildings[i].y = buildings[i].warY;
                }
            }

            target = defender;
            startbuildings = buildings;
            battleBuildings.Clear();

            for (int i = 0; i < buildings.Count; i++)
            {
                Battle.Building building = new Battle.Building();
                building.building = buildings[i];
                switch (building.building.id)
                {
                    case Data.BuildingID.townhall:
                        building.lootGoldStorage = Data.GetStorageGoldAndElixirLoot(townhallLevel, building.building.goldStorage);
                        building.lootElixirStorage = Data.GetStorageGoldAndElixirLoot(townhallLevel, building.building.elixirStorage);
                        building.lootDarkStorage = Data.GetStorageDarkElixirLoot(townhallLevel, building.building.darkStorage);
                        break;
                    case Data.BuildingID.goldmine:
                        building.lootGoldStorage = Data.GetMinesGoldAndElixirLoot(townhallLevel, building.building.goldStorage);
                        break;
                    case Data.BuildingID.goldstorage:
                        building.lootGoldStorage = Data.GetStorageGoldAndElixirLoot(townhallLevel, building.building.goldStorage);
                        break;
                    case Data.BuildingID.elixirmine:
                        building.lootElixirStorage = Data.GetMinesGoldAndElixirLoot(townhallLevel, building.building.elixirStorage);
                        break;
                    case Data.BuildingID.elixirstorage:
                        building.lootElixirStorage = Data.GetStorageGoldAndElixirLoot(townhallLevel, building.building.elixirStorage);
                        break;
                    case Data.BuildingID.darkelixirmine:
                        building.lootDarkStorage = Data.GetMinesDarkElixirLoot(townhallLevel, building.building.darkStorage);
                        break;
                    case Data.BuildingID.darkelixirstorage:
                        building.lootDarkStorage = Data.GetStorageDarkElixirLoot(townhallLevel, building.building.darkStorage);
                        break;
                }
                battleBuildings.Add(building);
            }

            _timerText.text = TimeSpan.FromSeconds(Data.battlePrepDuration).ToString(@"mm\:ss");

            ClearBuildingsOnGrid();
            ClearUnitsOnGrid();

            UI_Main.instanse._grid.Clear();
            for (int i = 0; i < battleBuildings.Count; i++)
            {
                Building prefab = UI_Main.instanse.GetBuildingPrefab(battleBuildings[i].building.id);
                if (prefab)
                {
                    BuildingOnGrid building = new BuildingOnGrid();
                    building.building = Instantiate(prefab, Vector3.zero, Quaternion.identity);
                    building.building.databaseID = battleBuildings[i].building.databaseID;
                    building.building.PlacedOnGrid(battleBuildings[i].building.x, battleBuildings[i].building.y);
                    building.building._baseArea.gameObject.SetActive(false);

                    building.healthBar = Instantiate(healthBarPrefab, healthBarGrid);
                    building.healthBar.bar.fillAmount = 1;
                    building.healthBar.gameObject.SetActive(false);

                    building.id = battleBuildings[i].building.databaseID;
                    building.index = i;
                    buildingsOnGrid.Add(building);
                }

                battleBuildings[i].building.x += Data.battleGridOffset;
                battleBuildings[i].building.y += Data.battleGridOffset;
            }

            _findButton.gameObject.SetActive(_battleType == Data.BattleType.normal);
            _closeButton.gameObject.SetActive(true);
            _surrenderButton.gameObject.SetActive(false);
            baseTime = DateTime.Now;
            SetStatus(true);

            toAdd.Clear();
            battle = new Battle();
            battle.Initialize(battleBuildings, DateTime.Now, BuildingAttackCallBack, BuildingDestroyedCallBack, BuildingDamageCallBack, StarGained);

            _percentageText.text = (battle.percentage * 100f).ToString("F2") + "%";
            UpdateLoots();

            surrender = false;
            readyToStart = true;
            isStarted = false;

            return true;
        }

        private void UpdateLoots()
        {
            var looted = battle.GetlootedResources();
            _lootGoldText.text = looted.Item1 + "/" + looted.Item4;
            _lootElixirText.text = looted.Item2 + "/" + looted.Item5;
            _lootDarkText.text = looted.Item3 + "/" + looted.Item6;
        }

        private void StartBattle()
        {
            _timerText.text = TimeSpan.FromSeconds(Data.battleDuration).ToString(@"mm\:ss");
            _findButton.gameObject.SetActive(false);
            _closeButton.gameObject.SetActive(false);
            _surrenderButton.gameObject.SetActive(true);
            readyToStart = false;
            baseTime = DateTime.Now;
            Packet packet = new Packet();
            packet.Write((int)Player.RequestsID.BATTLESTART);
            Data.OpponentData opponent = new Data.OpponentData();
            opponent.id = target;
            opponent.buildings = startbuildings;
            string data = Data.Serialize<Data.OpponentData>(opponent);
            packet.Write(data);
            packet.Write((int)_battleType);
            Sender.TCP_Send(packet);
        }

        public void BattleEnded(int stars, int unitsDeployed, int lootedGold, int lootedElixir, int lootedDark, int trophies, int frame)
        {
            _findButton.gameObject.SetActive(false);
            _closeButton.gameObject.SetActive(false);
            _surrenderButton.gameObject.SetActive(false);
            var looted = battle.GetlootedResources();
            Debug.Log("Battle Ended.");
            Debug.Log("Frame -> Client:" + battle.frameCount + " Server:" + frame);
            Debug.Log("Stars -> Client:" + battle.stars + " Server:" + stars);
            Debug.Log("Units Deployed -> Client:" + battle.unitsDeployed + " Server:" + unitsDeployed);
            Debug.Log("Looted Gold -> Client:" + looted.Item1 + " Server:" + lootedGold);
            Debug.Log("Looted Elixir -> Client:" + looted.Item2 + " Server:" + lootedElixir);
            Debug.Log("Looted Dark Elixir -> Client:" + looted.Item3 + " Server:" + lootedDark);
            Debug.Log("Trophies -> Client:" + battle.GetTrophies() + " Server:" + trophies);
            _endPanel.SetActive(true);
        }

        public void StartBattleConfirm(bool confirmed, List<Data.BattleStartBuildingData> buildings, int winTrophies, int loseTrophies)
        {
            if (confirmed)
            {
                battle.winTrophies = winTrophies;
                battle.loseTrophies = loseTrophies;
                for (int i = 0; i < battle._buildings.Count; i++)
                {
                    for (int j = 0; j < buildings.Count; j++)
                    {
                        if (battle._buildings[i].building.databaseID == buildings[j].databaseID)
                        {
                            battle._buildings[i].lootGoldStorage = buildings[j].lootGoldStorage;
                            battle._buildings[i].lootElixirStorage = buildings[j].lootElixirStorage;
                            battle._buildings[i].lootDarkStorage = buildings[j].lootDarkStorage;
                        }
                    }
                }
                isStarted = true;
            }
            else
            {
                Debug.Log("Battle is not confirmed by the server.");
            }
        }

        private void Surrender()
        {
            surrender = true;
        }

        public void EndBattle(bool surrender, int surrenderFrame)
        {
            _findButton.gameObject.SetActive(false);
            _closeButton.gameObject.SetActive(false);
            _surrenderButton.gameObject.SetActive(false);
            battle.end = true;
            battle.surrender = surrender;
            isStarted = false;
            Packet packet = new Packet();
            packet.Write((int)Player.RequestsID.BATTLEEND);
            packet.Write(surrender);
            packet.Write(surrenderFrame);
            Sender.TCP_Send(packet);
        }

        [SerializeField] private GameObject _elements = null;
        [SerializeField] private RectTransform unitsGrid = null;
        [SerializeField] private UI_BattleUnit unitsPrefab = null;
        private static UI_Battle _instance = null; public static UI_Battle instanse { get { return _instance; } }
        private bool _active = false; public bool isActive { get { return _active; } }

        [HideInInspector] public int selectedUnit = -1;

        private List<UI_BattleUnit> units = new List<UI_BattleUnit>();

        public void UnitSelected(Data.UnitID id)
        {
            if(selectedUnit >= 0)
            {
                // deselect
            }
            for (int i = 0; i < units.Count; i++)
            {
                if (units[i].id == id)
                {
                    selectedUnit = i;
                    break;
                }
            }
            if (selectedUnit >= 0 && units[selectedUnit].count <= 0)
            {
                selectedUnit = -1;
            }
        }

        public void PlaceUnit(int x, int y)
        {
            if (battle != null && selectedUnit >= 0 && units[selectedUnit].count > 0 && battle.CanAddUnit(x, y))
            {
                if (!isStarted)
                {
                    if (!readyToStart)
                    {
                        return;
                    }
                    StartBattle();
                }
                long id = units[selectedUnit].Get();
                if(id >= 0)
                {
                    if (units[selectedUnit].count <= 0)
                    {
                        selectedUnit = -1;
                    }
                    toAdd.Add(new UnitToAdd(id, x, y));
                }
            }
        }

        private void Awake()
        {
            _instance = this;
            _elements.SetActive(false);
        }

        private void ClearUnits()
        {
            for (int i = 0; i < units.Count; i++)
            {
                if (units[i])
                {
                    Destroy(units[i].gameObject);
                }
            }
            units.Clear();
        }

        public void SetStatus(bool status)
        {
            if (!status)
            {
                ClearBuildingsOnGrid();
                ClearUnitsOnGrid();
                ClearUnits();
            }
            else
            {
                _endPanel.SetActive(false);
            }
            Player.inBattle = status;
            _active = status;
            _elements.SetActive(status);
        }

        private void Update()
        {
            if (battle != null && battle.end == false)
            {
                if (isStarted)
                {
                    TimeSpan span = DateTime.Now - baseTime;

                    if (_timerText != null)
                    {
                        _timerText.text = TimeSpan.FromSeconds(Data.battleDuration - span.TotalSeconds).ToString(@"mm\:ss");
                    }

                    int frame = (int)Math.Floor(span.TotalSeconds / Data.battleFrameRate);
                    if (frame > battle.frameCount)
                    {
                        if (toAdd.Count > 0)
                        {
                            Data.BattleFrame battleFrame = new Data.BattleFrame();
                            battleFrame.frame = battle.frameCount + 1;
                            for (int i = toAdd.Count - 1; i >= 0; i--)
                            {
                                for (int j = 0; j < Player.instanse.data.units.Count; j++)
                                {
                                    if (Player.instanse.data.units[j].databaseID == toAdd[i].id)
                                    {
                                        battle.AddUnit(Player.instanse.data.units[j], toAdd[i].x, toAdd[i].y, UnitSpawnCallBack, UnitAttackCallBack, UnitDiedCallBack, UnitDamageCallBack, UnitHealCallBack);
                                        Data.BattleFrameUnit bfu = new Data.BattleFrameUnit();
                                        bfu.id = Player.instanse.data.units[j].databaseID;
                                        bfu.x = toAdd[i].x;
                                        bfu.y = toAdd[i].y;
                                        battleFrame.units.Add(bfu);
                                        break;
                                    }
                                }
                                toAdd.RemoveAt(i);
                            }
                            Packet packet = new Packet();
                            packet.Write((int)Player.RequestsID.BATTLEFRAME);
                            string frameData = Data.Serialize<Data.BattleFrame>(battleFrame);
                            packet.Write(frameData);
                            Sender.TCP_Send(packet);
                        }
                        battle.ExecuteFrame();
                        if ((float)battle.frameCount * Data.battleFrameRate >= battle.duration || Math.Abs(battle.percentage - 1d) <= 0.0001d)
                        {
                            EndBattle(false, battle.frameCount);
                        }
                        else if (surrender || (!battle.IsAliveUnitsOnGrid() && !HaveUnitLeftToPlace()))
                        {
                            EndBattle(true, battle.frameCount);
                        }
                    }
                }
                else if (readyToStart)
                {
                    TimeSpan span = DateTime.Now - baseTime;
                    if(span.TotalSeconds >= Data.battlePrepDuration)
                    {
                        StartBattle();
                    }
                    else
                    {
                        _timerText.text = TimeSpan.FromSeconds(Data.battlePrepDuration - span.TotalSeconds).ToString(@"mm\:ss");
                    }
                }
                UpdateUnits();
                UpdateBuildings();
            }
        }

        private bool HaveUnitLeftToPlace()
        {
            if (units.Count > 0)
            {
                for (int i = 0; i < units.Count; i++)
                {
                    if (units[i].count > 0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private BattleUnit GetUnitPrefab(Data.UnitID id)
        {
            for (int i = 0; i < battleUnits.Length; i++)
            {
                if(battleUnits[i].id == id)
                {
                    return battleUnits[i];
                }
            }
            return null;
        }

        public void ClearUnitsOnGrid()
        {
            for (int i = 0; i < unitsOnGrid.Count; i++)
            {
                if (unitsOnGrid[i])
                {
                    Destroy(unitsOnGrid[i].gameObject);
                }
            }
            unitsOnGrid.Clear();
        }

        public void ClearBuildingsOnGrid()
        {
            for (int i = 0; i < buildingsOnGrid.Count; i++)
            {
                if (buildingsOnGrid[i].building != null)
                {
                    Destroy(buildingsOnGrid[i].building.gameObject);
                }
            }
            buildingsOnGrid.Clear();
        }

        #region Events
        private void UpdateUnits()
        {
            for (int i = 0; i < unitsOnGrid.Count; i++)
            {
                if (battle._units[unitsOnGrid[i].index].health > 0)
                {
                    Vector3 position = new Vector3(battle._units[unitsOnGrid[i].index].positionOnGrid.x, 0, battle._units[unitsOnGrid[i].index].positionOnGrid.y);
                    unitsOnGrid[i].transform.localPosition = position;
                  
                    if(battle._units[unitsOnGrid[i].index].health < battle._units[unitsOnGrid[i].index].unit.health)
                    {
                        unitsOnGrid[i].healthBar.gameObject.SetActive(true);
                        unitsOnGrid[i].healthBar.bar.fillAmount = battle._units[unitsOnGrid[i].index].health / battle._units[unitsOnGrid[i].index].unit.health;
                        unitsOnGrid[i].healthBar.rect.anchoredPosition = GetUnitBarPosition(unitsOnGrid[i].transform.position);
                    }
                }
            }
        }

        private void UpdateBuildings()
        {
            for (int i = 0; i < buildingsOnGrid.Count; i++)
            {
                if (battle._buildings[buildingsOnGrid[i].index].health > 0)
                {
                    if (battle._buildings[buildingsOnGrid[i].index].health < battle._buildings[buildingsOnGrid[i].index].building.health)
                    {
                        buildingsOnGrid[i].healthBar.gameObject.SetActive(true);
                        buildingsOnGrid[i].healthBar.bar.fillAmount = battle._buildings[buildingsOnGrid[i].index].health / battle._buildings[buildingsOnGrid[i].index].building.health;
                        buildingsOnGrid[i].healthBar.rect.anchoredPosition = GetUnitBarPosition(UI_Main.instanse._grid.GetEndPosition(buildingsOnGrid[i].building));
                    }
                }
            }
        }

        private Vector2 GetUnitBarPosition(Vector3 position)
        {
            Vector3 planDownLeft = CameraController.instanse.planDownLeft;
            Vector3 planTopRight = CameraController.instanse.planTopRight;

            float w = planTopRight.x - planDownLeft.x;
            float h = planTopRight.z - planDownLeft.z;

            float endW = position.x - planDownLeft.x;
            float endH = position.z - planDownLeft.z;

            return new Vector2(endW / w * Screen.width, endH / h * Screen.height);
        }

        public void UnitSpawnCallBack(long id)
        {
            int u = -1;
            for (int i = 0; i < battle._units.Count; i++)
            {
                if (battle._units[i].unit.databaseID == id)
                {
                    u = i;
                    break;
                }
            }
            if (u >= 0)
            {
                BattleUnit prefab = GetUnitPrefab(battle._units[u].unit.id);
                if (prefab)
                {
                    BattleUnit unit = Instantiate(prefab, UI_Main.instanse._grid.transform);
                    unit.transform.localPosition = new Vector3(battle._units[u].positionOnGrid.x, 0, battle._units[u].positionOnGrid.y);
                    unit.Initialize(u, battle._units[u].unit.databaseID);

                    unit.healthBar = Instantiate(healthBarPrefab, healthBarGrid);
                    unit.healthBar.bar.fillAmount = 1;
                    unit.healthBar.gameObject.SetActive(false);

                    unitsOnGrid.Add(unit);
                }
            }
        }

        public void StarGained()
        {

        }

        public void UnitAttackCallBack(long id, Battle.BattleVector2 target)
        {

        }

        public void UnitDiedCallBack(long id)
        {
            for (int i = 0; i < unitsOnGrid.Count; i++)
            {
                if(unitsOnGrid[i].databaseID == id)
                {
                    Destroy(unitsOnGrid[i].healthBar.gameObject);
                    Destroy(unitsOnGrid[i].gameObject);
                    unitsOnGrid.RemoveAt(i);
                    break;
                }
            }
        }

        public void UnitDamageCallBack(long id, float damage)
        {
            
        }

        public void UnitHealCallBack(long id, float health)
        {

        }

        public void BuildingAttackCallBack(long id, Battle.BattleVector2 target)
        {

        }

        public void BuildingDamageCallBack(long id, float damage)
        {
            UpdateLoots();
        }

        public void BuildingDestroyedCallBack(long id, double percentage)
        {
            if(percentage > 0)
            {
                _percentageText.text = (battle.percentage * 100f).ToString("F2") + "%";
            }
            for (int i = 0; i < buildingsOnGrid.Count; i++)
            {
                if (buildingsOnGrid[i].id == id)
                {
                    Destroy(buildingsOnGrid[i].healthBar.gameObject);
                    Destroy(buildingsOnGrid[i].building.gameObject);
                    buildingsOnGrid.RemoveAt(i);
                    break;
                }
            }
        }
        #endregion

    }
}