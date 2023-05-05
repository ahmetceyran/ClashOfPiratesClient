namespace AhmetsHub.ClashOfPirates
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    public class UI_Button : MonoBehaviour
    {

        public Button button = null;
        public RectTransform rect = null;

        private void Awake()
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.zero;
        }

        private void OnDestroy()
        {
            button.onClick.RemoveAllListeners();
        }

    }
}