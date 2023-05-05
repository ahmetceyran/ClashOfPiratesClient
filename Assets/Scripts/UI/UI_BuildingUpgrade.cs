namespace AhmetsHub.ClashOfPirates
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    using TMPro;
    using DevelopersHub.RealtimeNetworking.Client;

    public class UI_BuildingUpgrade : MonoBehaviour
    {

        [SerializeField] public GameObject _elements = null;
        private static UI_BuildingUpgrade _instance = null; public static UI_BuildingUpgrade instanse { get { return _instance; } }

        [SerializeField] private Button _closeButton = null;
        [SerializeField] private TextMeshProUGUI reqGold = null;
        [SerializeField] private TextMeshProUGUI reqElixir = null;
        [SerializeField] private TextMeshProUGUI reqDark = null;
        [SerializeField] private TextMeshProUGUI reqGems = null;
        [SerializeField] private TextMeshProUGUI reqTime = null;
        [SerializeField] private Button _upgradeButton = null;

        private void Awake()
        {
            _instance = this;
            _elements.SetActive(false);
        }

        private long id = 0;

        private void Start()
        {
            _closeButton.onClick.AddListener(Close);
            _upgradeButton.onClick.AddListener(Upgrade);
        }

        public void Open(Data.ServerBuilding building, long databaseID)
        {
            id = databaseID;
            if (string.IsNullOrEmpty(building.id))
            {

            }
            else
            {
                reqGold.text = building.requiredGold.ToString();
                reqElixir.text = building.requiredElixir.ToString();
                reqDark.text = building.requiredDarkElixir.ToString();
                reqGems.text = building.requiredGems.ToString();
                reqTime.text = building.buildTime.ToString();
            }
            _elements.SetActive(true);
        }

        public void Close()
        {
            _elements.SetActive(false);
        }

        private void Upgrade()
        {
            Packet packet = new Packet();
            packet.Write((int)Player.RequestsID.UPGRADE);
            packet.Write(id);
            Sender.TCP_Send(packet);
            Close();
        }

    }
}