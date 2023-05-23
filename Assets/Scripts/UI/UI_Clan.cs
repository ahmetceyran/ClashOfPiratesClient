namespace AhmetsHub.ClashOfPirates
{
    using DevelopersHub.RealtimeNetworking.Client;
    using System.Collections;
    using System.Collections.Generic;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class UI_Clan : MonoBehaviour
    {

        [SerializeField] private GameObject _elements = null;
        [SerializeField] private GameObject _profilePanel = null;
        [SerializeField] private GameObject _listPanel = null;
        [SerializeField] private GameObject _createPanel = null;
        [SerializeField] private Button _closeButton = null;

        [Header("Clans List")]
        [SerializeField] private UI_ClanItem _clansPrefab = null;
        [SerializeField] private RectTransform _clansParent = null;
        [SerializeField] private Button _createButton = null;
        [SerializeField] private Button _nextButton = null;
        [SerializeField] private Button _lastButton = null;
        [SerializeField] private Button _prevButton = null;
        [SerializeField] private Button _firstButton = null;
        [SerializeField] private TextMeshProUGUI _pageText = null;

        [Header("Clans Create")]
        [SerializeField] private Button _createConfirm = null;
        [SerializeField] private Button _createCancel = null;
        [SerializeField] private Image _createBackground = null;
        [SerializeField] private Image _createIcon = null;
        [SerializeField] private TMP_InputField _createName = null;
        [SerializeField] private TextMeshProUGUI _createJoinText = null;
        [SerializeField] private TextMeshProUGUI _createTrophiesText = null;
        [SerializeField] private TextMeshProUGUI _createHallText = null;
        [SerializeField] private Button _createJoinNext = null;
        [SerializeField] private Button _createJoinPrev = null;
        [SerializeField] private Button _createTrophiesNext = null;
        [SerializeField] private Button _createTrophiesPrev = null;
        [SerializeField] private Button _createHallNext = null;
        [SerializeField] private Button _createHallPrev = null;
        [SerializeField] private Button _createPatternNext = null;
        [SerializeField] private Button _createPatternPrev = null;

        [Header("Clan Profile")]
        [SerializeField] private Button _profileJoin = null;
        [SerializeField] private Button _profileLeave = null;
        [SerializeField] private Button _profileClans = null;
        [SerializeField] private Button _profileEdit = null;
        [SerializeField] private TextMeshProUGUI _profileName = null;
        [SerializeField] private Image _profileBackground = null;
        [SerializeField] private Image _profileIcon = null;

        [Header("Other")]
        public Sprite[] patterns = null;

        private List<UI_ClanItem> clanItems = new List<UI_ClanItem>();
        private bool editingProfile = false;

        private static UI_Clan _instance = null; public static UI_Clan instanse { get { return _instance; } }
        private bool _active = true; public bool isActive { get { return _active; } }

        private Data.Clan profileClan = null;
        private Data.Clan clanToSave = null;

        private void Awake()
        {
            _instance = this;
            _elements.SetActive(false);
        }

        private void Start()
        {
            _closeButton.onClick.AddListener(Close);
            _createButton.onClick.AddListener(CreateOpen);
            _createConfirm.onClick.AddListener(CreateConfirm);
            _createCancel.onClick.AddListener(CreateCancel);
            _createJoinNext.onClick.AddListener(CreateJoinNext);
            _createJoinPrev.onClick.AddListener(CreateJoinPrev);
            _createTrophiesNext.onClick.AddListener(CreateTrophiesNext);
            _createTrophiesPrev.onClick.AddListener(CreateTrophiesPrev);
            _createHallNext.onClick.AddListener(CreateHallNext);
            _createHallPrev.onClick.AddListener(CreateHallPrev);
            _createPatternNext.onClick.AddListener(CreatePatternNext);
            _createPatternPrev.onClick.AddListener(CreatePatternPrev);
            _profileJoin.onClick.AddListener(Join);
            _profileLeave.onClick.AddListener(Leave);
            _profileClans.onClick.AddListener(Clans);
            _profileEdit.onClick.AddListener(Edit);
        }

        public void Open()
        {
            _active = true;
            _profilePanel.SetActive(false);
            _listPanel.SetActive(false);
            _createPanel.SetActive(false);
            Packet packet = new Packet();
            if (Player.instanse.data.clanID > 0)
            {
                packet.Write((int)Player.RequestsID.OPENCLAN);
                long id = 0;
                packet.Write(id);
                packet.Write(id);
            }
            else
            {
                profileClan = null;
                packet.Write((int)Player.RequestsID.GETCLANS);
                packet.Write(0);
            }
            Sender.TCP_Send(packet);
            _elements.SetActive(true);
        }

        public void Close()
        {
            ClearClanItems();
            _active = false;
            _elements.SetActive(false);
        }

        public void ClansListOpen(Data.ClansList clans)
        {
            ClearClanItems();
            if (clans.clans.Count > 0)
            {
                for (int i = 0; i < clans.clans.Count; i++)
                {
                    UI_ClanItem item = Instantiate(_clansPrefab, _clansParent);
                    item.Initialize(clans.clans[i]);
                    clanItems.Add(item);
                }
            }
            _createButton.gameObject.SetActive(Player.instanse.data.clanID <= 0);
            _nextButton.interactable = (clans.page != clans.pagesCount && clans.clans.Count > 0);
            _lastButton.interactable = (clans.page != clans.pagesCount && clans.clans.Count > 0);
            _prevButton.interactable = (clans.page != 1 && clans.clans.Count > 0);
            _firstButton.interactable = (clans.page != 1 && clans.clans.Count > 0);
            _pageText.text = clans.page.ToString() + "/" + clans.pagesCount.ToString();
            _listPanel.SetActive(true);
            _profilePanel.SetActive(false);
            _createPanel.SetActive(false);
        }

        public void ClanOpen(Data.Clan clan, List<Data.ClanMember> warMembers)
        {
            profileClan = clan;
            _profileJoin.gameObject.SetActive(Player.instanse.data.clanID <= 0 && Player.instanse.data.clanID != clan.id);
            _profileLeave.gameObject.SetActive(Player.instanse.data.clanID > 0 && Player.instanse.data.clanID == clan.id);
            _profileEdit.gameObject.SetActive(Player.instanse.data.clanID > 0 && Player.instanse.data.clanID == clan.id);
            _profileJoin.interactable = (clan.members.Count < Data.clanMaxMembers);
            _profileName.text = profileClan.name;
            _profileIcon.sprite = patterns[profileClan.pattern];
            _profileBackground.color = Tools.HexToColor(profileClan.backgroundColor);
            _profileIcon.color = Tools.HexToColor(profileClan.patternColor);
            _listPanel.SetActive(false);
            _createPanel.SetActive(false);
            _profilePanel.SetActive(true);
        }

        private void Join()
        {
            Packet packet = new Packet();
            packet.Write((int)Player.RequestsID.JOINCLAN);
            packet.Write(profileClan.id);
            Sender.TCP_Send(packet);
        }

        private void Leave()
        {
            Packet packet = new Packet();
            packet.Write((int)Player.RequestsID.LEAVECLAN);
            Sender.TCP_Send(packet);
        }

        private void Clans()
        {
            Packet packet = new Packet();
            packet.Write((int)Player.RequestsID.GETCLANS);
            packet.Write(0);
            Sender.TCP_Send(packet);
        }

        private void Edit()
        {
            CreateOpen(true);
        }

        private void CreateOpen()
        {
            CreateOpen(false);
        }

        private void CreateOpen(bool edit)
        {
            editingProfile = edit;
            clanToSave = new Data.Clan();
            if (profileClan != null)
            {
                clanToSave.name = profileClan.name;
                clanToSave.minTrophies = profileClan.minTrophies;
                clanToSave.minTownhallLevel = profileClan.minTownhallLevel;
                clanToSave.pattern = profileClan.pattern;
                clanToSave.background = profileClan.background;
                clanToSave.patternColor = profileClan.patternColor;
                clanToSave.backgroundColor = profileClan.backgroundColor;
                clanToSave.joinType = profileClan.joinType;
            }
            else
            {
                clanToSave.name = "";
                clanToSave.minTrophies = 0;
                clanToSave.minTownhallLevel = 1;
                clanToSave.pattern = 0;
                clanToSave.background = 0;
                clanToSave.patternColor = Tools.ColorToHex(Color.black);
                clanToSave.backgroundColor = Tools.ColorToHex(Color.white);
                clanToSave.joinType = Data.ClanJoinType.AnyoneCanJoin;
            }
            _createName.text = clanToSave.name;
            _createTrophiesText.text = clanToSave.minTrophies.ToString();
            _createHallText.text = clanToSave.minTownhallLevel.ToString();
            _createIcon.sprite = patterns[clanToSave.pattern];
            _createBackground.color = Tools.HexToColor(clanToSave.backgroundColor);
            _createIcon.color = Tools.HexToColor(clanToSave.patternColor);
            UpdateCreateJoinType();
            _createPanel.SetActive(true);
            _listPanel.SetActive(false);
            _profilePanel.SetActive(false);
        }

        private void UpdateCreateJoinType()
        {
            switch (clanToSave.joinType)
            {
                case Data.ClanJoinType.AnyoneCanJoin:
                    _createJoinText.text = "Anyone Can Join";
                    break;
                case Data.ClanJoinType.NotAcceptingNewMembers:
                    _createJoinText.text = "Closed";
                    break;
                case Data.ClanJoinType.TakingJoinRequests:
                    _createJoinText.text = "Request Only";
                    break;
            }
        }

        private void CreateConfirm()
        {
            string name = _createName.text.Trim();
            if (!string.IsNullOrEmpty(name) && name.Length >= Data.clanNameMinLength)
            {
                Packet packet = new Packet();
                if (editingProfile)
                {
                    packet.Write((int)Player.RequestsID.EDITCLAN);
                }
                else
                {
                    packet.Write((int)Player.RequestsID.CREATECLAN);
                }
                packet.Write(name);
                packet.Write(clanToSave.minTrophies);
                packet.Write(clanToSave.minTownhallLevel);
                packet.Write(clanToSave.pattern);
                packet.Write(clanToSave.background);
                packet.Write(clanToSave.patternColor);
                packet.Write(clanToSave.backgroundColor);
                packet.Write((int)clanToSave.joinType);
                Sender.TCP_Send(packet);
            }
            else
            {
                MessageBox.Open(1, 0.8f, true, ErrorConfirm, new string[] { "Clan name should not be less than " + Data.clanNameMinLength.ToString() + " characters." }, new string[] { "OK" });
            }
        }

        public void CreateResponse(int response)
        {
            switch (response)
            {
                case 2:
                    MessageBox.Open(1, 0.8f, true, ErrorConfirm, new string[] { "You already are in a clan." }, new string[] { "OK" });
                    break;
                case 3:
                    MessageBox.Open(1, 0.8f, true, ErrorConfirm, new string[] { "You can only join a clan every 24 hours." }, new string[] { "OK" });
                    break;
                case 4:
                    MessageBox.Open(1, 0.8f, true, ErrorConfirm, new string[] { "You don't have enough resources." }, new string[] { "OK" });
                    break;
                default:
                    MessageBox.Open(1, 0.8f, true, ErrorConfirm, new string[] { "Failed to create the clan." }, new string[] { "OK" });
                    break;
            }
        }

        public void EditResponse(int response)
        {
            switch (response)
            {
                case 2:
                    MessageBox.Open(1, 0.8f, true, ErrorConfirm, new string[] { "You don't have permission to edit the clan." }, new string[] { "OK" });
                    break;
                default:
                    MessageBox.Open(1, 0.8f, true, ErrorConfirm, new string[] { "Failed to edit the clan." }, new string[] { "OK" });
                    break;
            }
        }

        public void JoinResponse(int response)
        {
            switch (response)
            {
                case 1:
                    Player.instanse.RushSyncRequest();
                    Close();
                    break;
                case 2:
                    MessageBox.Open(1, 0.8f, true, ErrorConfirm, new string[] { "You already are in a clan." }, new string[] { "OK" });
                    break;
                case 3:
                    MessageBox.Open(1, 0.8f, true, ErrorConfirm, new string[] { "You can only join a clan every 24 hours." }, new string[] { "OK" });
                    break;
                case 4:
                    MessageBox.Open(1, 0.8f, true, ErrorConfirm, new string[] { "You don't have minimum requirements." }, new string[] { "OK" });
                    break;
                case 5:
                    MessageBox.Open(1, 0.8f, true, ErrorConfirm, new string[] { "Can not find the clan." }, new string[] { "OK" });
                    break;
                case 6:
                    MessageBox.Open(1, 0.8f, true, ErrorConfirm, new string[] { "Clan capacity is full." }, new string[] { "OK" });
                    break;
                case 7:
                    MessageBox.Open(1, 0.8f, true, ErrorConfirm, new string[] { "Clan is not accepting new members." }, new string[] { "OK" });
                    break;
                case 8:
                case 9:
                    MessageBox.Open(1, 0.8f, true, ErrorConfirm, new string[] { "Request to join the clan has been sent. You have to wait for them to accept your request." }, new string[] { "OK" });
                    break;
                default:
                    MessageBox.Open(1, 0.8f, true, ErrorConfirm, new string[] { "Failed to join the clan." }, new string[] { "OK" });
                    break;
            }
        }
        
        public void LeaveResponse(int response)
        {
            switch (response)
            {
                case 1:
                    Close();
                    break;
                case 2:
                    MessageBox.Open(1, 0.8f, true, ErrorConfirm, new string[] { "You can not leave during war." }, new string[] { "OK" });
                    break;
                default:
                    MessageBox.Open(1, 0.8f, true, ErrorConfirm, new string[] { "Failed to leave the clan." }, new string[] { "OK" });
                    break;
            }
        }

        private void ErrorConfirm(int layoutIndex, int buttonIndex)
        {
            if (layoutIndex == 1)
            {
                MessageBox.Close();
            }
        }

        private void CreateCancel()
        {
            Open();
        }

        private void CreateJoinNext()
        {
            int i = (int)clanToSave.joinType + 1;
            if(i > 1) { i = -1; }
            if (i < -1) { i = 1; }
            clanToSave.joinType = (Data.ClanJoinType)i;
            UpdateCreateJoinType();
        }

        private void CreateJoinPrev()
        {
            int i = (int)clanToSave.joinType - 1;
            if (i > 1) { i = -1; }
            if (i < -1) { i = 1; }
            clanToSave.joinType = (Data.ClanJoinType)i;
            UpdateCreateJoinType();
        }

        private void CreateTrophiesNext()
        {
            clanToSave.minTrophies += 100;
            if (clanToSave.minTrophies > 5500) { clanToSave.minTrophies = 0; }
            if (clanToSave.minTrophies < 0) { clanToSave.minTrophies = 5500; }
            _createTrophiesText.text = clanToSave.minTrophies.ToString();
        }

        private void CreateTrophiesPrev()
        {
            clanToSave.minTrophies -= 100;
            if (clanToSave.minTrophies > 5500) { clanToSave.minTrophies = 0; }
            if (clanToSave.minTrophies < 0) { clanToSave.minTrophies = 5500; }
            _createTrophiesText.text = clanToSave.minTrophies.ToString();
        }

        private void CreateHallNext()
        {
            clanToSave.minTownhallLevel += 1;
            if (clanToSave.minTownhallLevel > 15) { clanToSave.minTownhallLevel = 1; }
            if (clanToSave.minTownhallLevel < 1) { clanToSave.minTownhallLevel = 15; }
            _createHallText.text = clanToSave.minTownhallLevel.ToString();
        }

        private void CreateHallPrev()
        {
            clanToSave.minTownhallLevel -= 1;
            if (clanToSave.minTownhallLevel > 15) { clanToSave.minTownhallLevel = 1; }
            if (clanToSave.minTownhallLevel < 1) { clanToSave.minTownhallLevel = 15; }
            _createHallText.text = clanToSave.minTownhallLevel.ToString();
        }

        private void CreatePatternNext()
        {
            clanToSave.pattern += 1;
            if (clanToSave.pattern >= patterns.Length) { clanToSave.pattern = 0; }
            if (clanToSave.pattern < 0) { clanToSave.pattern = patterns.Length - 1; }
            _createIcon.sprite = patterns[clanToSave.pattern];
        }

        private void CreatePatternPrev()
        {
            clanToSave.pattern -= 1;
            if (clanToSave.pattern >= patterns.Length) { clanToSave.pattern = 0; }
            if (clanToSave.pattern < 0) { clanToSave.pattern = patterns.Length - 1; }
            _createIcon.sprite = patterns[clanToSave.pattern];
        }

        private void ClearClanItems()
        {
            for (int i = 0; i < clanItems.Count; i++)
            {
                if (clanItems[i])
                {
                    Destroy(clanItems[i].gameObject);
                }
            }
            clanItems.Clear();
        }

    }
}