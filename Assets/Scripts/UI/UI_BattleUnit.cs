namespace AhmetsHub.ClashOfPirates
{
    using System.Collections;
    using System.Collections.Generic;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class UI_BattleUnit : MonoBehaviour
    {

        [SerializeField] private TextMeshProUGUI _titleText = null;
        [SerializeField] private TextMeshProUGUI _haveText = null;
        [SerializeField] private Button _buttonSelect = null;

        [HideInInspector] public Data.UnitID id = Data.UnitID.barbarian;
        public int count { get { return units.Count; } }
        private List<long> units = new List<long>();

        public void Initialize(Data.UnitID id)
        {
            _titleText.text = id.ToString();
            this.id = id;
        }

        public void Add(long id)
        {
            units.Add(id);
            _haveText.text = units.Count.ToString();
        }

        public long Get()
        {
            long value = -1;
            if (units.Count > 0)
            {
                value = units[0];
                units.RemoveAt(0);
            }
            _haveText.text = units.Count.ToString();
            return value;
        }

        private void Start()
        {
            _buttonSelect.onClick.AddListener(Select);
        }

        public void Select()
        {
            UI_Battle.instanse.UnitSelected(id);
        }

        public void Deselect()
        {
            
        }

    }
}