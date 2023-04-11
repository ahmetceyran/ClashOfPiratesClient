namespace AhmetsHub.ClashOfPirates
{
    using DevelopersHub.RealtimeNetworking.Client;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    public class UI_Search : MonoBehaviour
    {

        [SerializeField] private GameObject _elements = null;
        [SerializeField] private Button _closeButton = null;
        [SerializeField] private Button _findButton = null;

        private static UI_Search _instance = null; public static UI_Search instanse { get { return _instance; } }
        private bool _active = true; public bool isActive { get { return _active; } }
        private long lastTarget = 0;

        private void Awake()
        {
            _instance = this;
            _elements.SetActive(false);
        }

        private void Start()
        {
            _closeButton.onClick.AddListener(Close);
            _findButton.onClick.AddListener(Find);
        }

        public void SetStatus(bool status)
        {
            if (status)
            {
                lastTarget = 0;
            }
            _active = status;
            _elements.SetActive(status);
        }

        private void Close()
        {
            SetStatus(false);
            UI_Main.instanse.SetStatus(true);
        }

        public void Find()
        {
            Packet packet = new Packet();
            packet.Write((int)Player.RequestsID.BATTLEFIND);
            Sender.TCP_Send(packet);
        }

        public void FndResponded(long target, Data.OpponentData opponent)
        {
            if(target > 0 && opponent != null && target != lastTarget)
            {
                lastTarget = target;
                UI_Battle.instanse.Display(opponent.buildings, target);
                SetStatus(false);
            }
            else
            {
                UI_Battle.instanse.NoTarget();
                Debug.Log("No target found.");
            }
        }

    }
}