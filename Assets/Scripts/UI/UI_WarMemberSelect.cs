namespace AhmetsHub.ClashOfPirates
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    using TMPro;

    public class UI_WarMemberSelect : MonoBehaviour
    {

        [SerializeField] private TextMeshProUGUI _name = null;
        [SerializeField] private TextMeshProUGUI _level = null;
        [SerializeField] private Button _in = null;
        [SerializeField] private Button _out = null;

        private Data.ClanMember _data = null;

        public void Initialize(Data.ClanMember data)
        {
            _data = data;
            _name.text = data.name;
            _level.text = data.level.ToString();
        }

        private void Start()
        {
            _in.onClick.AddListener(In);
            _out.onClick.AddListener(Out);
            _out.gameObject.SetActive(false);
        }

        private void In()
        {
            _in.gameObject.SetActive(false);
            _out.gameObject.SetActive(true);
            UI_Clan.instanse.WarMemberStatus(_data.id, true);
        }

        private void Out()
        {
            _in.gameObject.SetActive(true);
            _out.gameObject.SetActive(false);
            UI_Clan.instanse.WarMemberStatus(_data.id, false);
        }

    }
}