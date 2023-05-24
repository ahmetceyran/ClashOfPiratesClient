namespace AhmetsHub.ClashOfPirates
{
    using System.Collections;
    using System.Collections.Generic;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class UI_WarLayoutBuilding : MonoBehaviour
    {

        [SerializeField] private Button _button = null;
        [SerializeField] private TextMeshProUGUI _name = null;
        private long _id = 0;

        private void Start()
        {
            _button.onClick.AddListener(Clicked);
        }
        
        public void Initialized(Data.Building building)
        {
            _id = building.databaseID;
            _name.text = building.id.ToString();
        }

        private void Clicked()
        {
            if (UI_WarLayout.instanse.placingItem != null)
            {
                UI_WarLayout.instanse.placingItem.SetActive(true);
                UI_WarLayout.instanse.placingItem = null;
                UI_Build.instanse.Cancel();
            }
            int n = -1;
            for (int i = 0; i < Player.instanse.data.buildings.Count; i++)
            {
                if(Player.instanse.data.buildings[i].databaseID == _id)
                {
                    n = i;
                    break;
                }
            }
            if(n >= 0)
            {
                Building prefab = UI_Main.instanse.GetBuildingPrefab(Player.instanse.data.buildings[n].id);
                if (prefab)
                {
                    UI_WarLayout.instanse.placingID = Player.instanse.data.buildings[n].databaseID;
                    Vector3 position = Vector3.zero;
                    Building building = Instantiate(prefab, position, Quaternion.identity);
                    building.PlacedOnGrid(20, 20); // todo: best avalibale pos
                    building._baseArea.gameObject.SetActive(true);
                    Building.buildInstanse = building;
                    CameraController.instanse.isPlacingBuilding = true;
                    UI_WarLayout.instanse.placingItem = gameObject;
                    UI_WarLayout.instanse.placingItem.SetActive(false);
                    UI_Build.instanse.SetStatus(true);
                }
            }
        }

    }
}