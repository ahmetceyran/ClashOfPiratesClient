namespace AhmetsHub.ClashOfPirates
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    using DevelopersHub.RealtimeNetworking.Client;
    using TMPro;

    public class UI_Unit : MonoBehaviour
    {

        [SerializeField] private Data.UnitID _id = Data.UnitID.pirate;
        [SerializeField] private Button _button = null;
        [SerializeField] private TextMeshProUGUI _haveUnitsText = null; 

        private void Start()
        {

            _button.onClick.AddListener(Clicked);
        }

        private void Clicked()
        {
            
        }
    }
}