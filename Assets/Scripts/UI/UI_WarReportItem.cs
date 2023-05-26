namespace AhmetsHub.ClashOfPirates
{
    using DevelopersHub.RealtimeNetworking.Client;
    using System.Collections;
    using System.Collections.Generic;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class UI_WarReportItem : MonoBehaviour
    {

        [SerializeField] private Button _detailsButton = null;
        [SerializeField] private TextMeshProUGUI _result = null;
        [SerializeField] private TextMeshProUGUI _stars = null;
        [SerializeField] private Image _background = null;

        Data.ClanWarData _data = null;

        private void Start()
        {
            _detailsButton.onClick.AddListener(Clicked);
        }

        public void Initialize(Data.ClanWarData data)
        {
            _data = data;
            if (data.winnerID <= 0)
            {
                _result.text = "Draw";
                _background.color = Color.gray;
            }
            else if (data.winnerID == Player.instanse.data.clanID)
            {
                _result.text = "Victory";
                _background.color = Color.green;
            }
            else
            {
                _result.text = "Defeat";
                _background.color = Color.red;
            }
            _stars.text = data.clan1Stars + " - " + data.clan2Stars;
            _detailsButton.gameObject.SetActive(data.hasReport);
        }

        private void Clicked()
        {
            _detailsButton.interactable = false;
            Packet packet = new Packet();
            packet.Write((int)Player.RequestsID.WARREPORT);
            packet.Write(_data.id);
            Sender.TCP_Send(packet);
        }

    }
}