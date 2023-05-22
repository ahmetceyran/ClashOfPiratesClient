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
        [SerializeField] private Data.UnitID _id = Data.UnitID.barbarian; public Data.UnitID id { get { return _id; } }
        [SerializeField] private Button _button = null;
        [SerializeField] private TextMeshProUGUI _haveUnitsText = null;
        [SerializeField] private TextMeshProUGUI _reqResourceText = null;
        private int count = 0; public int haveCount { get { return count; } set { count = value; _haveUnitsText.text = count.ToString(); } }
        private bool canTrain = false;

        private void Start()
        {

            _button.onClick.AddListener(Clicked);
        }

        public void Initialize(Data.ServerUnit unit)
        {
            int barrackLevel = 0;
            int darkBarracksLevel = 0;

            for (int i = 0; i < Player.instanse.data.buildings.Count; i++)
            {
                if(Player.instanse.data.buildings[i].id == Data.BuildingID.barracks)
                {
                    barrackLevel = Player.instanse.data.buildings[i].level;
                }
                else if (Player.instanse.data.buildings[i].id == Data.BuildingID.darkbarracks)
                {
                    darkBarracksLevel = Player.instanse.data.buildings[i].level;
                }
                if(barrackLevel > 0 && darkBarracksLevel > 0)
                {
                    break;
                }
            }

            canTrain = Data.IsUnitUnlocked(_id, barrackLevel, darkBarracksLevel);
            _button.interactable = canTrain;

            if (unit.requiredGold > 0)
            {
                _reqResourceText.text = "Gold: " + unit.requiredGold.ToString();
            }
            else if (unit.requiredElixir > 0)
            {
                _reqResourceText.text = "Elixir: " + unit.requiredElixir.ToString();
            }
            else if (unit.requiredGems > 0)
            {
                _reqResourceText.text = "Gems: " + unit.requiredGems.ToString();
            }
            else if (unit.requiredDarkElixir > 0)
            {
                _reqResourceText.text = "Dark: " + unit.requiredDarkElixir.ToString();
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

        public void Sync()
        {
            count = 0;
            for (int i = 0; i < Player.instanse.data.units.Count; i++)
            {
                if (Player.instanse.data.units[i].id == _id && Player.instanse.data.units[i].ready)
                {
                    count++;
                }
            }
            haveCount = count;
        }

    }
}