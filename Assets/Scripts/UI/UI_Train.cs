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
            _elements.SetActive(status);
        }

        private void Close()
        {
            SetStatus(false);
            UI_Main.instanse.SetStatus(true);
        }
    }
}
