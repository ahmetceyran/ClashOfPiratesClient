namespace AhmetsHub.ClashOfPirates
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    public class UI_Train : MonoBehaviour
    {
        
        [SerializeField] public GameObject _elements = null;
        [SerializeField] private Button _closeButton = null;

        [SerializeField] private UI_UnitsTraining _trainPrefab = null;
        [SerializeField] private RectTransform _trainGrid = null;

        private static UI_Train _instance = null; public static UI_Train instanse { get { return _instance; } }

        private List<UI_UnitsTraining> trainigItems = new List<UI_UnitsTraining>();
        [SerializeField] private List<UI_Unit> uiUnits = new List<UI_Unit>();
       
        private void Awake()
        {
            _instance = this;
            _elements.SetActive(false);
        }

        [HideInInspector] public bool isOpen = false;

        private void ClearTrainingItems()
        {
            for (int i = 0; i < trainigItems.Count; i++)
            {
                if (trainigItems[i])
                {
                    Destroy(trainigItems[i].gameObject);
                }
            }
            trainigItems.Clear();
        }

        public void Initialize()
    {
        for (int i = 0; i < uiUnits.Count; i++)
        {
            for (int j = 0; j < Player.instanse.initializationData.serverUnits.Count; j++)
            {
                if (uiUnits[i].id == Player.instanse.initializationData.serverUnits[j].id)
                {
                    uiUnits[i].Initialize(Player.instanse.initializationData.serverUnits[j]);
                    break;
                }
            }
        }
    }

        private void Start()
        {
            _closeButton.onClick.AddListener(Close);
        }

        public void SetStatus(bool status)
        {
            ClearTrainingItems();
            if (status)
            {
                Sync();
            }
            _elements.SetActive(status);
            isOpen = status;
        }

        private void UpdateTrainingList()
        {
            for (int i = 0; i < Player.instanse.data.units.Count; i++)
            {
                if (Player.instanse.data.units[i].ready == false)
                {
                    int x = -1;
                    for (int j = 0; j < trainigItems.Count; j++)
                    {
                        if (trainigItems[j] && Player.instanse.data.units[i].databaseID == trainigItems[j].databaseID)
                        {
                            x = j;
                            break;
                        }
                    }
                    if (x >= 0)
                    {

                    }
                    else
                    {
                        UI_UnitsTraining unit = Instantiate(_trainPrefab, _trainGrid.transform);
                        unit.Initialize(Player.instanse.data.units[i]);
                        trainigItems.Add(unit);
                    }
                }
            }
            ResetTrainingItemsIndex();
        }

        private void Close()
        {
            SetStatus(false);
            UI_Main.instanse.SetStatus(true);
        }

        public void Sync()
        {
            for (int i = 0; i < uiUnits.Count; i++)
            {
                uiUnits[i].Sync();
            }

            for (int i = trainigItems.Count - 1; i >= 0; i--)
            {
                if (trainigItems[i])
                {
                    int x = -1;
                    for (int j = 0; j < Player.instanse.data.units.Count; j++)
                    {
                        if (Player.instanse.data.units[j].databaseID == trainigItems[i].databaseID)
                        {
                            x = j;
                            break;
                        }
                    }
                    if (x >= 0)
                    {
                        if (Player.instanse.data.units[x].ready)
                        {
                            RemoveTrainingItem(i);
                        }
                    }
                    else
                    {
                        RemoveTrainingItem(i);
                    }
                }
                else
                {
                    RemoveTrainingItem(i);
                }
            }
            UpdateTrainingList();
        }

        public void RemoveTrainingItem(int i)
        {
            if (i < 0 || i >= trainigItems.Count)
            {
                return;
            }
            if (trainigItems[i])
            {
                Destroy(trainigItems[i].gameObject);
            }
            trainigItems.RemoveAt(i);
            ResetTrainingItemsIndex();
        }

        public void ResetTrainingItemsIndex()
        {
            for (int j = 0; j < trainigItems.Count; j++)
            {
                if (trainigItems[j])
                {
                    trainigItems[j].index = j;
                }
            }
        }
    }
}
