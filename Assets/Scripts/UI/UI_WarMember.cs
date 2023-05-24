namespace AhmetsHub.ClashOfPirates
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    using TMPro;

    public class UI_WarMember : MonoBehaviour
    {

        [SerializeField] private Image _image = null;
        [SerializeField] private TextMeshProUGUI _name = null;
        public GameObject selectedEffects = null;

        [HideInInspector] public Data.ClanMember _data = null;

        private void Start()
        {
            Button button = GetComponentInChildren<Button>();
            if (button)
            {
                button.onClick.AddListener(Select);
            }
        }

        public void Initialize(Data.ClanMember data)
        {
            selectedEffects.SetActive(false);
            _data = data;
            _name.text = (_data.warPos + 1).ToString() + ". " + data.name;
        }

        private void Select()
        {
            UI_Clan.instanse.SelectWarMember(this);
        }

    }
}