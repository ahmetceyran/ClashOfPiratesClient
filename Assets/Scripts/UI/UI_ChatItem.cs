namespace AhmetsHub.ClashOfPirates
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    using TMPro;
    using System;

    public class UI_ChatItem : MonoBehaviour
    {

        [SerializeField] private TextMeshProUGUI _nameText = null;
        [SerializeField] private TextMeshProUGUI _messageText = null;
        [SerializeField] private TextMeshProUGUI _timeText = null;

        private Data.CharMessage _data = null; public long id { get { return _data.id; } }

        public void Inirialize(Data.CharMessage data)
        {
            _data = data;
            _nameText.text = _data.name;
            _messageText.text = _data.message;
            DateTime time = Player.instanse.data.nowTime;
            DateTime.TryParse(_data.time, out time);
            _timeText.text = time.ToString();
        }

    }
}