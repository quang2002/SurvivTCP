using UnityEngine;

namespace Entities
{
    using System;
    using Entities.Configs;

    public class Weapon : MonoBehaviour
    {
        [field: SerializeField] private SpriteRenderer SpriteRenderer { get; set; }

        [field: SerializeField] private Sprite OnGroundSprite { get; set; }

        [field: SerializeField] private Sprite OnHandSprite { get; set; }

        [field: SerializeField] private Transform ShootTf { get; set; }

        [field: SerializeField] private Collider2D Collider { get; set; }

        [field: SerializeField] private WeaponConfig WeaponConfig { get; set; }

        public bool IsMounted { get; private set; }

        private float timer;

        private void Update() { this.timer += Time.deltaTime; }

        public void Mount(Transform mountedTf)
        {
            if (this.IsMounted) return;
            this.IsMounted             = true;
            this.SpriteRenderer.sprite = this.OnHandSprite;
            this.Collider.enabled      = false;

            this.transform.SetParent(mountedTf);
            this.transform.rotation      = Quaternion.Euler(0, 0, 90);
            this.transform.localPosition = Vector3.down;
        }

        public void Unmount()
        {
            if (!this.IsMounted) return;
            this.IsMounted             = false;
            this.SpriteRenderer.sprite = this.OnGroundSprite;
            this.Collider.enabled      = true;

            this.transform.SetParent(null);
        }

        public void Attack()
        {
            if (this.WeaponConfig == null) return;
            if (this.timer < this.WeaponConfig.sleepTime) return;
            var startAngle = -this.WeaponConfig.angleBetweenBullet * this.WeaponConfig.numberBullet * 1.0f / 2;
            for (var i = 0; i < this.WeaponConfig.numberBullet; i++)
            {
                var localEulerAngles = this.transform.localEulerAngles;
                var angle            = new Vector3(localEulerAngles.x, localEulerAngles.y, localEulerAngles.z + startAngle);
                var bullet           = Instantiate(this.WeaponConfig.bullet, this.ShootTf.position, this.transform.rotation).GetComponent<Bullet>();
                bullet.transform.localEulerAngles = angle;
                bullet.Release(this.WeaponConfig.damage, this.WeaponConfig.bulletSpeed);
                startAngle += this.WeaponConfig.angleBetweenBullet;
            }

            this.timer = 0;
        }
    }
}