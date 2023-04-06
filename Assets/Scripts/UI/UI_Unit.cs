namespace AhmetsHub.ClashOfPirates
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    using DevelopersHub.RealtimeNetworking.Client;
    using TMPro;

    public class UI_Unit : MonoBehaviour
    {

        [SerializeField] private Data.UnitID _id = Data.UnitID.pirate; public Data.UnitID id { get { return _id; } }
        [SerializeField] private Button _button = null;
        [SerializeField] private TextMeshProUGUI _haveUnitsText = null;
        [SerializeField] private TextMeshProUGUI _reqResourceText = null;
        private int count = 0; public int haveCount { get { return count; } set { count = value; _haveUnitsText.text = count.ToString(); } } 

        private void Start()
        {

            _button.onClick.AddListener(Clicked);
        }

        public void Initialize(Data.ServerUnit unit)
    {
        if (unit.requiredGold > 0)
        {
            _reqResourceText.text = "Gold: " + unit.requiredGold.ToString();
        }
        else if (unit.requiredFish > 0)
        {
            _reqResourceText.text = "Elixir: " + unit.requiredFish.ToString();
        }
        else if (unit.requiredDiamonds > 0)
        {
            _reqResourceText.text = "Gems: " + unit.requiredDiamonds.ToString();
        }
        else
        {
            _reqResourceText.text = "Free";
        }
    }

        private void Clicked()
        {
            Packet paket = new Packet();
            paket.Write((int)Player.RequestsID.TRAIN);
            paket.Write(_id.ToString());
            Sender.TCP_Send(paket);
        }
    }
}