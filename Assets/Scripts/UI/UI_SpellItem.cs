namespace AhmetsHub.ClashOfPirates
{
    using DevelopersHub.RealtimeNetworking.Client;
    using System.Collections;
    using System.Collections.Generic;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class UI_SpellItem : MonoBehaviour
    {

        [SerializeField] private Data.SpellID _id = Data.SpellID.healing; public Data.SpellID id { get { return _id; } }
        [SerializeField] private Button _button = null;
        [SerializeField] private TextMeshProUGUI _haveUSpellsText = null;
        [SerializeField] private TextMeshProUGUI _reqResourceText = null;
        private int count = 0; public int haveCount { get { return count; } set { count = value; _haveUSpellsText.text = count.ToString(); } }
        private bool canBrew = false;

        private void Start()
        {
            _button.onClick.AddListener(Clicked);
        }

        public void Initialize(Data.ServerSpell spell)
        {
            int spellFactoryLevel = 0;
            int darkSpellFactoryLevel = 0;

            for (int i = 0; i < Player.instanse.data.buildings.Count; i++)
            {
                if (Player.instanse.data.buildings[i].id == Data.BuildingID.spellfactory)
                {
                    spellFactoryLevel = Player.instanse.data.buildings[i].level;
                }
                else if (Player.instanse.data.buildings[i].id == Data.BuildingID.darkspellfactory)
                {
                    darkSpellFactoryLevel = Player.instanse.data.buildings[i].level;
                }
                if (spellFactoryLevel > 0 && darkSpellFactoryLevel > 0)
                {
                    break;
                }
            }

            canBrew = Data.IsSpellUnlocked(_id, spellFactoryLevel, darkSpellFactoryLevel);
            _button.interactable = canBrew;

            if (spell.requiredGold > 0)
            {
                _reqResourceText.text = "Gold: " + spell.requiredGold.ToString();
            }
            else if (spell.requiredElixir > 0)
            {
                _reqResourceText.text = "Elixir: " + spell.requiredElixir.ToString();
            }
            else if (spell.requiredGems > 0)
            {
                _reqResourceText.text = "Gems: " + spell.requiredGems.ToString();
            }
            else if (spell.requiredDarkElixir > 0)
            {
                _reqResourceText.text = "Dark: " + spell.requiredDarkElixir.ToString();
            }
            else
            {
                _reqResourceText.text = "Free";
            }
        }

        private void Clicked()
        {
            Packet paket = new Packet();
            paket.Write((int)Player.RequestsID.BREW);
            paket.Write(_id.ToString());
            Sender.TCP_Send(paket);
        }

        public void Sync()
        {
            count = 0;
            for (int i = 0; i < Player.instanse.data.spells.Count; i++)
            {
                if (Player.instanse.data.spells[i].id == _id && Player.instanse.data.spells[i].ready)
                {
                    count++;
                }
            }
            haveCount = count;
        }

    }
}