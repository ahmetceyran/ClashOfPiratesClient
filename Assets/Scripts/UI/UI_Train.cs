namespace AhmetsHub.ClashOfPirates
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    public class UI_Train : MonoBehaviour
    {
        
        [SerializeField] public GameObject _elements = null;
        [SerializeField] private Button _closeButton = null;

        [SerializeField] private UI_UnitsTraining _trainPrefab = null;
        [SerializeField] private RectTransform _trainGrid = null;

        private static UI_Train _instance = null; public static UI_Train instanse { get { return _instance; } }
       
        private void Awake()
        {
            _instance = this;
            _elements.SetActive(false);
        }

        private void Start()
        {
            _closeButton.onClick.AddListener(Close);
        }

        public void SetStatus(bool status)
        {
            _elements.SetActive(status);
        }

        private void Close()
        {
            SetStatus(false);
            UI_Main.instanse.SetStatus(true);
        }
    }
}
