namespace AhmetsHub.ClashOfPirates
{
    using DevelopersHub.RealtimeNetworking.Client;
    using System.Collections;
    using System.Collections.Generic;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class UI_ClanMember : MonoBehaviour
    {

        [SerializeField] private Button _kickButton = null;
        [SerializeField] private TextMeshProUGUI _nameText = null;
        [SerializeField] private TextMeshProUGUI _trophiesText = null;
        [SerializeField] private TextMeshProUGUI _levelText = null;
        [SerializeField] private TextMeshProUGUI _rankText = null;

        private Data.ClanMember _data = null; public long id { get { return _data.id; } }

        private void Start()
        {
            _kickButton.onClick.AddListener(Clicked);
        }

        public void Initialize(Data.ClanMember member, bool haveKickPermission)
        {
            _data = member;
            _nameText.text = _data.name;
            _trophiesText.text = _data.trophies.ToString();
            _levelText.text = _data.level.ToString();
            switch ((Data.ClanRank)_data.rank)
            {
                case Data.ClanRank.member:
                    _rankText.text = "Member";
                    break;
                case Data.ClanRank.leader:
                    _rankText.text = "Leader";
                    break;
                case Data.ClanRank.coleader:
                    _rankText.text = "Co-Leader";
                    break;
            }
            bool isInWar = false;
            bool canBeKicked = false;
            if (haveKickPermission)
            {
                if(Player.instanse.data.clanRank > _data.rank)
                {
                    canBeKicked = true;
                }
            }
            if (canBeKicked && member.warID >= 0)
            {
                isInWar = true;
            }
            _kickButton.gameObject.SetActive(canBeKicked);
            _kickButton.interactable = !isInWar;
        }

        private void Clicked()
        {
            _kickButton.interactable = false;
            Packet packet = new Packet();
            packet.Write((int)Player.RequestsID.KICKMEMBER);
            packet.Write(_data.id);
            Sender.TCP_Send(packet);
        }

        public void kickResponse(int response)
        {
            if(response == 1)
            {
                Destroy(gameObject);
            }
            else
            {
                _kickButton.interactable = true;
            }
        }

    }
}