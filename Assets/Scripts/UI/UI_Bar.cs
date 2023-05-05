namespace AhmetsHub.ClashOfPirates
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    using TMPro;

    public class UI_Bar : MonoBehaviour
    {

        public Image bar = null;
        public RectTransform rect = null;
        public TextMeshProUGUI[] texts = null;

        private void Awake()
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.zero;
        }

    }
}