namespace AhmetsHub.ClashOfPirates
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    public class UI_BuildingOptions : MonoBehaviour
    {

        [SerializeField] public GameObject _elements = null;

        private static UI_BuildingOptions _instance = null; public static UI_BuildingOptions instanse { get { return _instance; } }

        public RectTransform infoPanel = null;
        public RectTransform upgradePanel = null;
        public RectTransform instantPanel = null;
        public RectTransform trainPanel = null;

        public Button infoButton = null;
        public Button upgradeButton = null;
        public Button instantButton = null;
        public Button trainButton = null;

        private void Awake()
        {
            _instance = this;
            _elements.SetActive(false);
        }

        public void SetStatus(bool status)
        {
            if(status && Building.selectedInstanse != null)
            {
                infoPanel.gameObject.SetActive(true);
                upgradePanel.gameObject.SetActive(Building.selectedInstanse.data.isConstructing == false);
                instantPanel.gameObject.SetActive(Building.selectedInstanse.data.isConstructing == true);
                trainPanel.gameObject.SetActive(Building.selectedInstanse.data.id == Data.BuildingID.armycamp || Building.selectedInstanse.data.id == Data.BuildingID.barracks);
            }
            _elements.SetActive(status);
        }

    }
}