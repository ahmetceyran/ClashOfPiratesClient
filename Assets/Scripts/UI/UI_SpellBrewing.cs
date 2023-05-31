namespace AhmetsHub.ClashOfPirates
{
    using DevelopersHub.RealtimeNetworking.Client;
    using System.Collections;
    using System.Collections.Generic;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class UI_SpellBrewing : MonoBehaviour
    {


        [SerializeField] private TextMeshProUGUI _titleText = null;
        [SerializeField] private Image _bar = null;
        [SerializeField] private Button _buttonRemove = null;

        private Data.Spell _spell = null; public long databaseID { get { return _spell != null ? _spell.databaseID : 0; } }
        [HideInInspector] public Data.SpellID id = Data.SpellID.healing;

        [HideInInspector] public int index = -1;

        private void Start()
        {
            _buttonRemove.onClick.AddListener(Remove);
        }

        public void Initialize(Data.Spell spell)
        {
            _bar.fillAmount = 0;
            _spell = spell;
            _titleText.text = _spell.id.ToString();
        }

        public void Remove()
        {
            Packet paket = new Packet();
            paket.Write((int)Player.RequestsID.CANCELBREW);
            paket.Write(_spell.databaseID);
            Sender.TCP_Send(paket);
            UI_Train.instanse.RemoveTrainingItem(index);
        }

        private void Update()
        {
            if (_spell != null && index == 0)
            {
                if (_spell.brewTime <= 0 || _spell.brewedTime >= _spell.brewTime)
                {
                    _bar.fillAmount = 1f;
                    Player.instanse.RushSyncRequest();
                }
                else
                {
                    _spell.brewedTime += Time.deltaTime;
                    _bar.fillAmount = _spell.brewedTime / _spell.brewTime;
                }
            }
        }

    }
}