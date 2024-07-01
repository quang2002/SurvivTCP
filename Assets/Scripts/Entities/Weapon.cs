using Entities.Configs;
using UnityEngine;
namespace Entities
{

    public class Weapon : MonoBehaviour
    {
        [field: SerializeField] private SpriteRenderer SpriteRenderer { get; set; }

        [field: SerializeField] private Sprite OnGroundSprite { get; set; }

        [field: SerializeField] private Sprite OnHandSprite { get; set; }

        [field: SerializeField] private Transform ShootTf { get; set; }

        [field: SerializeField] private Collider2D Collider { get; set; }

        [field: SerializeField] private WeaponConfig WeaponConfig { get; set; }

        private float timer;
        private float lockedTime;

        public bool IsMounted { get; private set; }
        public Player MountedPlayer { get; private set; }

        private void Update()
        {
            this.timer += Time.deltaTime;

            if (this.lockedTime > 0)
            {
                this.lockedTime -= Time.deltaTime;
            }

            if (this.IsMounted)
            {
                this.transform.localPosition = Vector3.zero;
                this.transform.localEulerAngles = Vector3.back * 90f;
            }
        }

        public void Mount(Transform mountedTf)
        {
            if (this.lockedTime > 0) return;
            if (this.IsMounted) return;

            this.MountedPlayer = mountedTf.GetComponentInParent<Player>();
            if (!this.MountedPlayer) return;

            this.IsMounted = true;
            this.SpriteRenderer.sprite = this.OnHandSprite;
            this.Collider.enabled = false;

            this.transform.SetParent(mountedTf);
        }

        public void Unmount()
        {
            if (!this.IsMounted) return;
            this.IsMounted = false;
            this.SpriteRenderer.sprite = this.OnGroundSprite;
            this.Collider.enabled = true;

            this.transform.SetParent(null);
            this.lockedTime = 1;
        }

        public void Attack()
        {
            if (this.WeaponConfig == null) return;
            if (this.timer < this.WeaponConfig.sleepTime) return;
            var startAngle = -this.WeaponConfig.angleBetweenBullet * (this.WeaponConfig.numberBullet - 1) / 2;

            for (var i = 0; i < this.WeaponConfig.numberBullet; i++)
            {
                var angles = this.transform.eulerAngles;
                var angle = new Vector3(angles.x, angles.y, angles.z + startAngle - 90f);
                var bullet = Instantiate(this.WeaponConfig.bullet, this.ShootTf.position, this.transform.rotation).GetComponent<Bullet>();
                bullet.transform.eulerAngles = angle;
                bullet.Release(this.WeaponConfig.damage, this.WeaponConfig.bulletSpeed, this.MountedPlayer);
                startAngle += this.WeaponConfig.angleBetweenBullet;
            }

            this.timer = 0;
        }
    }
}