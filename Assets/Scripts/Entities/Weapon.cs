using UnityEngine;
namespace Entities
{
    public class Weapon : MonoBehaviour
    {
        [field: SerializeField]
        private SpriteRenderer SpriteRenderer { get; set; }

        [field: SerializeField]
        private Sprite OnGroundSprite { get; set; }

        [field: SerializeField]
        private Sprite OnHandSprite { get; set; }

        [field: SerializeField]
        private Transform ShootTf { get; set; }

        [field: SerializeField]
        private Collider2D Collider { get; set; }

        public bool IsMounted { get; private set; }

        public void Mount(Transform mountedTf)
        {
            if (this.IsMounted) return;
            this.IsMounted = true;
            this.SpriteRenderer.sprite = this.OnHandSprite;
            this.Collider.enabled = false;

            this.transform.SetParent(mountedTf);
            this.transform.rotation = Quaternion.Euler(0, 0, 90);
            this.transform.localPosition = Vector3.down;
        }

        public void Unmount()
        {
            if (!this.IsMounted) return;
            this.IsMounted = false;
            this.SpriteRenderer.sprite = this.OnGroundSprite;
            this.Collider.enabled = true;

            this.transform.SetParent(null);
        }

        public void Attack()
        {
            
        }
    }
}