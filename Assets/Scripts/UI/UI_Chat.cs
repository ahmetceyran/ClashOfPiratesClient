namespace AhmetsHub.ClashOfPirates
{
    using DevelopersHub.RealtimeNetworking.Client;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    using TMPro;

    public class UI_Chat : MonoBehaviour
    {

        [SerializeField] private GameObject _elements = null;
        [SerializeField] private RectTransform _panel = null;
        // [SerializeField] private Button _buttonOpen = null; in ui main
        [SerializeField] private Button _buttonClose = null;
        [SerializeField] private Button _buttonSend = null;
        [SerializeField] private Button _buttonClan = null;
        [SerializeField] private Button _buttonGlobal = null;
        [SerializeField] private TMP_InputField _inputMessage = null;
        [SerializeField] private UI_ChatItem _chatPrefab = null;
        [SerializeField] private RectTransform _chatGridClan = null;
        [SerializeField] private RectTransform _chatGridGlobal = null;
        //[SerializeField] private GameObject _loadingPrefab = null;

        private static UI_Chat _instance = null; public static UI_Chat instanse { get { return _instance; } }
        private bool _active = false; public bool isActive { get { return _active; } }

        private List<UI_ChatItem> clanChats = new List<UI_ChatItem>();
        private List<UI_ChatItem> globalChats = new List<UI_ChatItem>();

        private bool updating = false;
        private float timer = 0;
        private Data.ChatType type = Data.ChatType.global;
        private Vector2 closePosition = Vector2.zero;
        private Vector2 openPosition = Vector2.zero;
        private float transitionDuration = 1f;
        private float transitionTimer = 1f;
        // private GameObject loading = null;
        private bool sending = false;

        private void Awake()
        {
            _instance = this;
            _elements.SetActive(false);
        }

        private void Start()
        {
            _panel.anchorMax = Vector2.zero;
            _panel.anchorMin = Vector2.zero;
            _panel.pivot = Vector2.zero;
            _panel.sizeDelta = new Vector2(Screen.width * 0.25f, Screen.height);
            closePosition = new Vector2(-_panel.sizeDelta.x, 0);
            openPosition = Vector2.zero;
            _panel.anchoredPosition = closePosition;
            _buttonSend.onClick.AddListener(Send);
            _buttonClose.onClick.AddListener(Close);
            _buttonClan.onClick.AddListener(Clan);
            _buttonGlobal.onClick.AddListener(Global);
        }

        private void Update()
        {
            if (_active)
            {
                if(timer < Data.chatSyncPeriod)
                {
                    timer += Time.deltaTime;
                }
                else
                {
                    if (!updating)
                    {
                        Sync();
                    }
                }
            }
        }

        public void ChatSynced(List<Data.CharMessage> messages, Data.ChatType chatType)
        {
            if (!_active) { return; }
            for (int i = 0; i < messages.Count; i++)
            {
                UI_ChatItem chat = Instantiate(_chatPrefab, messages[i].type == Data.ChatType.clan ? _chatGridClan : _chatGridGlobal);
                LayoutRebuilder.ForceRebuildLayoutImmediate(messages[i].type == Data.ChatType.clan ? _chatGridClan : _chatGridGlobal);
                chat.Inirialize(messages[i]);
                if (messages[i].type == Data.ChatType.clan)
                {
                    clanChats.Add(chat);
                }
                else
                {
                    globalChats.Add(chat);
                }
            }
            if(clanChats.Count > Data.clanChatArchiveMaxMessages)
            {
                for (int i = 0; i < clanChats.Count - Data.clanChatArchiveMaxMessages; i++)
                {
                    Destroy(clanChats[i].gameObject);
                }
            }
            if (globalChats.Count > Data.globalChatArchiveMaxMessages)
            {
                for (int i = 0; i < globalChats.Count - Data.globalChatArchiveMaxMessages; i++)
                {
                    Destroy(globalChats[i].gameObject);
                }
            }
            /*
            if(!sending && loading != null)
            {
                Destroy(loading);
                loading = null;
            }
            */
            updating = false;
        }

        private void Send()
        {
            string message = _inputMessage.text.Trim();
            if (!string.IsNullOrEmpty(message))
            {
                sending = true;
                _inputMessage.interactable = false;
                _buttonSend.interactable = false;
                Packet packet = new Packet();
                packet.Write((int)Player.RequestsID.SENDCHAT);
                packet.Write(message);
                packet.Write((int)type);
                long target = 0;
                if (type == Data.ChatType.clan)
                {
                    target = Player.instanse.data.clanID;
                }
                else
                {
                    target = 0;
                }
                packet.Write(target);
                Sender.TCP_Send(packet);
            }
        }

        public void ChatSendResponse(int response)
        {
            sending = false;
            _inputMessage.text = "";
            _inputMessage.interactable = true;
            _buttonSend.interactable = true;
        }

        private void Clan()
        {
            type = Data.ChatType.clan;
            _chatGridGlobal.gameObject.SetActive(false);
            _chatGridClan.gameObject.SetActive(true);
            _buttonClan.interactable = false;
            _buttonGlobal.interactable = true;
            AddLoading();
            Sync();
        }

        private void Global()
        {
            type = Data.ChatType.global;
            _chatGridGlobal.gameObject.SetActive(true);
            _chatGridClan.gameObject.SetActive(false);
            _buttonClan.interactable = true;
            _buttonGlobal.interactable = false;
            AddLoading();
            Sync();
        }

        private void AddLoading()
        {/*
            if(loading != null)
            {
                Destroy(loading);
                loading = null;
            }
            loading = Instantiate(_loadingPrefab, type == Data.ChatType.clan ? _chatGridClan : _chatGridGlobal);
            */
        }

        private void Sync()
        {
            timer = 0;
            updating = true;
            Packet packet = new Packet();
            packet.Write((int)Player.RequestsID.GETCHATS);
            packet.Write((int)type);
            long lastMessage = 0;
            if(type == Data.ChatType.clan)
            {
                if(clanChats.Count > 0)
                {
                    lastMessage = clanChats[clanChats.Count - 1].id;
                }
            }
            else
            {
                if (globalChats.Count > 0)
                {
                    lastMessage = globalChats[globalChats.Count - 1].id;
                }
            }
            packet.Write(lastMessage);
            Sender.TCP_Send(packet);
        }

        public void Open()
        {
            sending = false;
            _chatGridGlobal.gameObject.SetActive(false);
            _chatGridClan.gameObject.SetActive(false);
            _buttonClan.interactable = false;
            _buttonGlobal.interactable = false;
            _elements.SetActive(true);
            if (Player.instanse.data.clanID > 0)
            {
                type = Data.ChatType.clan;
            }
            else
            {
                type = Data.ChatType.global;
            }
            StartCoroutine(_Open());
        }

        private IEnumerator _Open()
        {
            transitionTimer = 0;
            while (_panel.anchoredPosition != openPosition)
            {
                transitionTimer += Time.deltaTime; if (transitionTimer > transitionDuration) { transitionTimer = transitionDuration; }
                _panel.anchoredPosition = Vector2.Lerp(closePosition, openPosition, transitionTimer / transitionDuration);
                yield return null;
            }
            if (type == Data.ChatType.clan)
            {
                _buttonGlobal.interactable = true;
                _chatGridClan.gameObject.SetActive(true);
            }
            else
            {
                _buttonClan.interactable = (Player.instanse.data.clanID > 0);
                _chatGridGlobal.gameObject.SetActive(true);
            }
            _inputMessage.interactable = true;
            _buttonSend.interactable = true;
            _active = true;
            Sync();
        }

        public void Close()
        {
            _active = false;
            _chatGridGlobal.gameObject.SetActive(false);
            _chatGridClan.gameObject.SetActive(false);
            _inputMessage.interactable = false;
            _buttonSend.interactable = false;
            StartCoroutine(_Close());
        }

        private IEnumerator _Close()
        {
            transitionTimer -= 1;
            while (_panel.anchoredPosition != closePosition)
            {
                transitionTimer += Time.deltaTime; if (transitionTimer > transitionDuration) { transitionTimer = transitionDuration; }
                _panel.anchoredPosition = Vector2.Lerp(openPosition, closePosition, transitionTimer / transitionDuration);
                yield return null;
            }
            _elements.SetActive(false);
        }

    }
}