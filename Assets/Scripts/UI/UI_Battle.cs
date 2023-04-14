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
        [SerializeField] public TextMeshProUGUI _timerText = null;
        [SerializeField] public TextMeshProUGUI _percentageText = null;
        [SerializeField] private UI_Bar healthBarPrefab = null;
        [SerializeField] private RectTransform healthBarGrid = null;
        [SerializeField] private BattleUnit[] battleUnits = null;
        [SerializeField] private Button _findButton = null;
        [SerializeField] private Button _closeButton = null;
        private List<BattleUnit> unitsOnGrid = new List<BattleUnit>();
        public List<BuildingOnGrid> buildingsOnGrid = new List<BuildingOnGrid>();
        private DateTime baseTime;
        private List<UnitToAdd> toAdd = new List<UnitToAdd>();
        private long target = 0;

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
        }

        private void Close()
        {
            Player.instanse.SyncData(Player.instanse.data);
            isStarted = false;
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
            _findButton.gameObject.SetActive(true);
            _closeButton.gameObject.SetActive(true);
        }

        public void Display(List<Data.Building> buildings, long defender)
        {
            target = defender;
            startbuildings = buildings;
            battleBuildings.Clear();
            for (int i = 0; i < buildings.Count; i++)
            {
                Battle.Building building = new Battle.Building();
                building.building = buildings[i];
                battleBuildings.Add(building);
            }

            _timerText.text = TimeSpan.FromSeconds(Data.battleDuration).ToString(@"mm\:ss");

            ClearBuildingsOnGrid();
            ClearUnitsOnGrid();
            ClearUnits();

            //UI_Main.instanse._grid.Clear();
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
            }

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

            _findButton.gameObject.SetActive(true);
            _closeButton.gameObject.SetActive(true);
            SetStatus(true);

            toAdd.Clear();
            battle = new Battle();
            battle.Initialize(battleBuildings, DateTime.Now, BuildingAttackCallBack, BuildingDestroyedCallBack, BuildingDamageCallBack);

            _percentageText.text = (battle.percentage * 100f).ToString("F2") + "%";

            readyToStart = true;
            isStarted = false;
        }

        private void StartBattle()
        {
            _findButton.gameObject.SetActive(false);
            _closeButton.gameObject.SetActive(false);
            readyToStart = false;
            baseTime = DateTime.Now;
            Packet packet = new Packet();
            packet.Write((int)Player.RequestsID.BATTLESTART);
            Data.OpponentData opponent = new Data.OpponentData();
            opponent.id = target;
            opponent.buildings = startbuildings;
            string data = Data.Serialize<Data.OpponentData>(opponent);
            packet.Write(data);
            Sender.TCP_Send(packet);
        }

        public void StartBattleConfirm(bool confirmed)
        {
            if (confirmed)
            {
                isStarted = true;
            }
            else
            {
                Debug.Log("Battle is not confirmed by the server.");
            }
        }

        [SerializeField] private GameObject _elements = null;
        [SerializeField] private RectTransform unitsGrid = null;
        [SerializeField] private UI_BattleUnit unitsPrefab = null;
        private static UI_Battle _instance = null; public static UI_Battle instanse { get { return _instance; } }
        private bool _active = true; public bool isActive { get { return _active; } }

        public int selectedUnit = -1;

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
            //Player.inBattle = status;
            _active = status;
            _elements.SetActive(status);
        }

        private void Update()
        {
            if(battle != null)
            {
                if (isStarted)
                {
                    TimeSpan span = DateTime.Now - baseTime;

                    if(_timerText != null)
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
                    }

                    if (span.TotalSeconds >= Data.battleDuration)
                    {
                        // Battle time is over
                        isStarted = false;
                    }
                }
                
                UpdateUnits();
                UpdateBuildings();
            }
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
                    Vector3 position = new Vector3(battle._units[unitsOnGrid[i].index].position.x, 0, battle._units[unitsOnGrid[i].index].position.y);
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
                    unit.transform.localPosition = new Vector3(battle._units[u].position.x, 0, battle._units[u].position.y);
                    unit.Initialize(u, battle._units[u].unit.databaseID);

                    unit.healthBar = Instantiate(healthBarPrefab, healthBarGrid);
                    unit.healthBar.bar.fillAmount = 1;
                    unit.healthBar.gameObject.SetActive(false);

                    unitsOnGrid.Add(unit);
                }
            }
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
            Debug.Log("Unit killed.");
        }

        public void UnitDamageCallBack(long id, float damage)
        {
            Debug.Log("Unit took damage: " + damage);
        }

        public void UnitHealCallBack(long id, float health)
        {

        }

        public void BuildingAttackCallBack(long id, Battle.BattleVector2 target)
        {

        }

        public void BuildingDamageCallBack(long id, float damage)
        {
            Debug.Log("Building took damage: " + damage);
        }

        public void BuildingDestroyedCallBack(long id, float percentage)
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
            Debug.Log("Building destroyed.");
        }
        #endregion

    }
}