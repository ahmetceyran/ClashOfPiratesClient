namespace AhmetsHub.ClashOfPirates
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    using DevelopersHub.RealtimeNetworking.Client;

    public class UI_Build : MonoBehaviour
    {

        [SerializeField] public GameObject _elements = null;
        public RectTransform buttonConfirm = null;
        public RectTransform buttonCancel = null;
        [HideInInspector] public Button clickConfirmButton = null;

        private static UI_Build _instance = null; public static UI_Build instanse { get { return _instance; } }

        private void Awake()
        {
            _instance = this;
            _elements.SetActive(false);
            clickConfirmButton = buttonConfirm.gameObject.GetComponent<Button>();
        }

        private void Start()
        {
            buttonConfirm.gameObject.GetComponent<Button>().onClick.AddListener(Confirm);
            buttonCancel.gameObject.GetComponent<Button>().onClick.AddListener(Cancel);
            buttonConfirm.anchorMin = Vector3.zero;
            buttonConfirm.anchorMax = Vector3.zero;
            buttonCancel.anchorMin = Vector3.zero;
            buttonCancel.anchorMax = Vector3.zero;
        }

        private void Update()
        {
            if(Building.buildInstanse != null && CameraController.instanse.isPlacingBuilding)
            {
                Vector3 end = UI_Main.instanse._grid.GetEndPosition(Building.buildInstanse);

                Vector3 planDownLeft = CameraController.instanse.planDownLeft;
                Vector3 planTopRight = CameraController.instanse.planTopRight;

                float w = planTopRight.x - planDownLeft.x;
                float h = planTopRight.z - planDownLeft.z;

                float endW = end.x - planDownLeft.x;
                float endH = end.z - planDownLeft.z;

                Vector2 screenPoint = new Vector2(endW / w * Screen.width, endH / h * Screen.height);

                Vector2 confirmPoint = screenPoint;
                confirmPoint.x += (buttonConfirm.rect.width + 45f);
                buttonConfirm.anchoredPosition = confirmPoint;

                Vector2 cancelPoint = screenPoint;
                cancelPoint.x -= (buttonCancel.rect.width + 45f);
                buttonCancel.anchoredPosition = cancelPoint;
            }
        }

        public void SetStatus(bool status)
        {
            _elements.SetActive(status);
        }

        private void Confirm()
        {
            if(Building.buildInstanse != null/* && UI_Main.instanse._grid.CanPlaceBuilding(Building.instanse, Building.instanse.currentX, Building.instanse.currentY)*/)
            {
                Packet packet = new Packet();
                packet.Write((int)Player.RequestsID.BUILD);
                packet.Write(SystemInfo.deviceUniqueIdentifier);
                packet.Write(Building.buildInstanse.id.ToString());
                packet.Write(Building.buildInstanse.currentX);
                packet.Write(Building.buildInstanse.currentY);
                Sender.TCP_Send(packet);
                Cancel();
            }
        }

        public void Cancel()
        {
            if(Building.buildInstanse != null)
            {
                CameraController.instanse.isPlacingBuilding = false;
                Building.buildInstanse.RemovedFromGrid();
            }
        }

    }
}