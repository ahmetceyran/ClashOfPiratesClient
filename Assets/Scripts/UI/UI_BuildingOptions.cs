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
        public RectTransform clanPanel = null;

        public Button infoButton = null;
        public Button upgradeButton = null;
        public Button instantButton = null;
        public Button trainButton = null;
        public Button clanButton = null;

        private void Awake()
        {
            _instance = this;
            _elements.SetActive(false);
        }

        public void SetStatus(bool status)
        {
            if(status && Building.selectedInstanse != null)
            {
                infoPanel.gameObject.SetActive(UI_Main.instanse.isActive);
                upgradePanel.gameObject.SetActive(Building.selectedInstanse.data.isConstructing == false && UI_Main.instanse.isActive);
                instantPanel.gameObject.SetActive(Building.selectedInstanse.data.isConstructing == true && UI_Main.instanse.isActive);
                trainPanel.gameObject.SetActive((Building.selectedInstanse.data.id == Data.BuildingID.armycamp || Building.selectedInstanse.data.id == Data.BuildingID.barracks) && UI_Main.instanse.isActive);
                clanPanel.gameObject.SetActive(Building.selectedInstanse.data.id == Data.BuildingID.clancastle && UI_Main.instanse.isActive);
            }
            _elements.SetActive(status);
        }

    }
}