namespace AhmetsHub.ClashOfPirates
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    using TMPro;
    using DevelopersHub.RealtimeNetworking.Client;

    public class UI_UnitsTraining : MonoBehaviour
    {

        [SerializeField] private TextMeshProUGUI _titleText = null;
        [SerializeField] private Image _bar = null;
        [SerializeField] private Button _buttonRemove = null;

        private Data.Unit _unit = null; public long databaseID { get { return _unit != null ? _unit.databaseID : 0; } }
        [HideInInspector] public Data.UnitID id = Data.UnitID.pirate;

        [HideInInspector] public int index = -1;

        private void Start()
        {
            _buttonRemove.onClick.AddListener(Remove);
        }

        public void Initialize(Data.Unit unit)
        {
            _bar.fillAmount = 0;
            _unit = unit;
            _titleText.text = _unit.id.ToString();
        }

        public void Remove()
        {
            Packet paket = new Packet();
            paket.Write((int)Player.RequestsID.CANCELTRAIN);
            paket.Write(_unit.databaseID);
            Sender.TCP_Send(paket);
            //UI_Train.instanse.RemoveTrainingItem(index);
        }

        private void Update()
        {
            if (_unit != null && index == 0)
            {
                if (_unit.trainTime <= 0 || _unit.trainedTime >= _unit.trainTime)
                {
                    //UI_Train.instanse.RemoveTrainingItem(index);
                }
                else
                {
                    _unit.trainedTime += Time.deltaTime;
                    _bar.fillAmount = _unit.trainedTime / _unit.trainTime;
                }
            }
        }
        
    }
}
