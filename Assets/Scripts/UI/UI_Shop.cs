namespace AhmetsHub.ClashOfPirates
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    public class UI_Shop : MonoBehaviour
    {

        [SerializeField] public GameObject _elements = null;
        [SerializeField] private Button _closeButton = null;

        private static UI_Shop _instance = null; public static UI_Shop instanse { get { return _instance; } }
       
        private void Awake()
        {
            _instance = this;
            _elements.SetActive(false);
        }

        private void Start()
        {
            _closeButton.onClick.AddListener(CloseShop);
        }

        public void SetStatus(bool status)
        {
            _elements.SetActive(status);
        }

        private void CloseShop()
        {
            SetStatus(false);
            UI_Main.instanse.SetStatus(true);
        }

    }
}