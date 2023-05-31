namespace AhmetsHub.ClashOfPirates
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class UI_Projectile : MonoBehaviour
    {

        private Transform _target = null;
        private Vector3 _lastKnowPosition = Vector3.zero;
        private Vector3 _start = Vector3.zero;
        private bool active = false;
        private float time = 0;
        private float timer = 0;

        public void Initialize(Vector3 start, Transform target, float speed)
        {
            if (target != null)
            {
                float distance = Vector3.Distance(start, target.position);
                time = distance / speed;
                timer = 0;
                _start = start;
                _target = target;
                _lastKnowPosition = _target.position;
                active = true;
                transform.position = _target.position;
            }
        }

        private void Update()
        {
            if (active)
            {
                timer += Time.deltaTime;
                if(timer > time) { timer = time; }
                if(_target && _target != null)
                {
                    _lastKnowPosition = _target.position;
                }
                transform.position = Vector3.Lerp(_start, _lastKnowPosition, timer / time);
                if(transform.position == _lastKnowPosition)
                {
                    Destroy(gameObject);
                }
            }
        }

    }
}