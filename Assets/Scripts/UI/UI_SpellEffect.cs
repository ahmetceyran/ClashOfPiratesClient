namespace AhmetsHub.ClashOfPirates
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class UI_SpellEffect : MonoBehaviour
    {

        private Data.SpellID _id = Data.SpellID.lightning;
        private long _databaseID = 0; public long DatabaseID { get { return _databaseID; } }

        public void Initialize(Data.SpellID id, long databaseID, float radius)
        {
            _id = id;
            _databaseID = databaseID;
            Vector3 scale = transform.localScale;
            scale.x = radius;
            scale.z = radius;
            transform.localScale = scale;
        }

        public void Pulse()
        {
           
        }

        public void End()
        {
            Destroy(gameObject);
        }

    }
}