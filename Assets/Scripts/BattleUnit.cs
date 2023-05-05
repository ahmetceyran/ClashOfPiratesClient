namespace AhmetsHub.ClashOfPirates
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class BattleUnit : MonoBehaviour
    {

        public Data.UnitID id = Data.UnitID.barbarian;
        private Vector3 lastPosition = Vector3.zero;
        private int i = -1; public int index { get { return i; } }
        private long _id = 0; public long databaseID { get { return _id; } }
        [HideInInspector] public UI_Bar healthBar = null;

        public void Initialize(int index, long id)
        {
            _id = id;
            i = index;
            lastPosition = transform.position;
        }

        private void Update()
        {
            if(transform.position != lastPosition)
            {
                Vector3 direction = transform.position - lastPosition;
                lastPosition = transform.position;
                transform.rotation = Quaternion.LookRotation(direction);
            }
        }

    }
}