namespace AhmetsHub.ClashOfPirates
{ 
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    using TMPro;
    using DevelopersHub.RealtimeNetworking.Client;

    public class UI_Settings : MonoBehaviour
    {

        [SerializeField] private GameObject _elements = null;
        [SerializeField] private Button _closeButton = null;
        [SerializeField] private Button _editButton = null;
        [SerializeField] private Button _cancelButton = null;
        [SerializeField] private Button _saveButton = null;
        [SerializeField] private Button _logoutButton = null;
        [SerializeField] private TMP_InputField _emailInput = null;

        private static UI_Settings _instance = null; public static UI_Settings instanse { get { return _instance; } }
        private bool _active = false; public bool isActive { get { return _active; } }
        private string email = "";

        private void Awake()
        {
            _instance = this;
            _elements.SetActive(false);
        }

        private void Start()
        {
            _closeButton.onClick.AddListener(Close);
            _editButton.onClick.AddListener(EditEmail);
            _cancelButton.onClick.AddListener(CancelEmail);
            _saveButton.onClick.AddListener(SaveEmail);
            _logoutButton.onClick.AddListener(LogOut);
        }

        public void Open()
        {
            _saveButton.interactable = true;
            _cancelButton.interactable = true;
            _saveButton.gameObject.SetActive(false);
            _editButton.gameObject.SetActive(true);
            _cancelButton.gameObject.SetActive(false);
            _emailInput.text = Player.instanse.data.email;
            _emailInput.interactable = false;
            _active = true;
            _elements.SetActive(true);
        }

        public void Close()
        {
            _active = false;
            _elements.SetActive(false);
        }

        private void EditEmail()
        {
            _emailInput.interactable = true;
            _saveButton.gameObject.SetActive(true);
            _editButton.gameObject.SetActive(false);
            _cancelButton.gameObject.SetActive(true);
        }

        private void CancelEmail()
        {
            _saveButton.gameObject.SetActive(false);
            _editButton.gameObject.SetActive(true);
            _cancelButton.gameObject.SetActive(false);
            _emailInput.text = Player.instanse.data.email;
            _emailInput.interactable = false;
        }

        private void SaveEmail()
        {
            email = _emailInput.text.Trim();
            if (!string.IsNullOrEmpty(email) && email != Player.instanse.data.email)
            {
                Loading.Open();
                _saveButton.interactable = false;
                _cancelButton.interactable = false;
                Packet packet = new Packet();
                packet.Write((int)Player.RequestsID.EMAILCODE);
                string device = SystemInfo.deviceUniqueIdentifier;
                packet.Write(device);
                packet.Write(email);
                Sender.TCP_Send(packet);
            }
        }

        public void EmailSendResponse(int response, int expiration)
        {
            Loading.Close();
            if (response == 1)
            {
                MessageBox.Open(2, 0.8f, true, MessageResponded, null, null, null, new string[] { "" });
            }
            else if(response == 3)
            {
                _cancelButton.interactable = true;
                _saveButton.interactable = true;
                MessageBox.Open(1, 0.8f, true, MessageResponded, new string[] { "Email is in sync with another account." }, new string[] { "OK" });
            }
            else
            {
                _cancelButton.interactable = true;
                _saveButton.interactable = true;
                MessageBox.Open(1, 0.8f, true, MessageResponded, new string[] { "Email is not valid." }, new string[] { "OK" });
            }
        }

        public void EmailConfirmResponse(int response, string password)
        {
            if (response == 1)
            {
                Player.instanse.data.email = email;
                Open();
            }
            else if (response == 3)
            {
                _cancelButton.interactable = true;
                _saveButton.interactable = true;
                MessageBox.Open(1, 0.8f, true, MessageResponded, new string[] { "Email is in sync with another account." }, new string[] { "OK" });
            }
            else
            {
                _cancelButton.interactable = true;
                _saveButton.interactable = true;
                MessageBox.Open(1, 0.8f, true, MessageResponded, new string[] { "Code is not valid." }, new string[] { "OK" });
            }
        }

        private void MessageResponded(int layoutIndex, int buttonIndex)
        {
            if (layoutIndex == 1)
            {
                MessageBox.Close();
            }
            else if(layoutIndex == 2)
            {
                if (buttonIndex == 0)
                {
                    MessageBox.Close();
                    string code = MessageBox.GetInputValue(2, 0).Trim();
                    if (!string.IsNullOrEmpty(code))
                    {
                        Packet packet = new Packet();
                        packet.Write((int)Player.RequestsID.EMAILCONFIRM);
                        string device = SystemInfo.deviceUniqueIdentifier;
                        packet.Write(device);
                        packet.Write(email);
                        packet.Write(code);
                        Sender.TCP_Send(packet);
                    }
                }
                else
                {
                    _cancelButton.interactable = true;
                    _saveButton.interactable = true;
                    MessageBox.Close();
                }
            }
            else if (layoutIndex == 3)
            {
                if (buttonIndex == 0)
                {
                    Packet packet = new Packet();
                    packet.Write((int)Player.RequestsID.LOGOUT);
                    string device = SystemInfo.deviceUniqueIdentifier;
                    packet.Write(device);
                    Sender.TCP_Send(packet);
                    PlayerPrefs.DeleteAll();
                    Player.RestartGame();
                }
                MessageBox.Close();
            }
        }

        private void LogOut()
        {
            MessageBox.Open(3, 0.8f, true, MessageResponded, new string[] { "Are you sure that you want to log out of your account? If you haven't bind your account to an email address then you will loose all your progress.." }, new string[] { "Log Out", "Cancel" });
        }

    } 
}