namespace AhmetsHub.ClashOfPirates
{
    using System.Collections;
    using System.Collections.Generic;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class UI_BattleSpell : MonoBehaviour
    {

        [SerializeField] private TextMeshProUGUI _titleText = null;
        [SerializeField] private TextMeshProUGUI _haveText = null;
        [SerializeField] private Button _buttonSelect = null;

        [HideInInspector] public Data.SpellID id = Data.SpellID.lightning;
        public int count { get { return spells.Count; } }
        private List<long> spells = new List<long>();

        public void Initialize(Data.SpellID id)
        {
            _titleText.text = id.ToString();
            this.id = id;
        }

        public void Add(long id)
        {
            spells.Add(id);
            _haveText.text = spells.Count.ToString();
        }

        public long Get()
        {
            long value = -1;
            if (spells.Count > 0)
            {
                value = spells[0];
                spells.RemoveAt(0);
            }
            _haveText.text = spells.Count.ToString();
            return value;
        }

        private void Start()
        {
            _buttonSelect.onClick.AddListener(Select);
        }

        public void Select()
        {
            UI_Battle.instanse.SpellSelected(id);
        }

        public void Deselect()
        {
            
        }

    }
}