namespace Helpers
{
    using UnityEngine;

    public class AutoDestroy : MonoBehaviour
    {
        public  float timeDelay;
        private float time;

        private void Start()
        {
            this.time = 0;
        }

        private void Update()
        {
            if (!(this.time >= this.timeDelay)) return;
            if (this.gameObject)
            {
                Destroy(this.gameObject);
            }
        }
    }
}