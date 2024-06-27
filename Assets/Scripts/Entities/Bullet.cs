using Game;
using UnityEngine;
namespace Entities
{
    using System;

    public class Bullet : MonoBehaviour
    {
        public  GameObject vfxWhenDestroy;
        private float      damage;
        private Player     src;
        
        [SerializeField]
        private float      timeout = 10;

        public float Velocity => this.GetComponent<Rigidbody2D>().velocity.magnitude;

        private void OnEnable()
        {
            GameManager.Instance.Bullets.Add(this);
        }

        private void OnDisable()
        {
            GameManager.Instance.Bullets.Remove(this);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.GetComponentInParent<Player>() is { } otherPlayer)
            {
                if (otherPlayer == this.src)
                    return;

                otherPlayer.TakeDamage(this.damage, this.src);
                if (this.gameObject) Destroy(this.gameObject);
                if (this.vfxWhenDestroy) Instantiate(this.vfxWhenDestroy);
            }
        }

        public void Release(float bulletDamage, float bulletSpeed, Player source)
        {
            this.src = source;
            this.damage = bulletDamage;
            Vector2 direction = this.transform.up;
            this.GetComponent<Rigidbody2D>().velocity = direction * bulletSpeed;
        }

        private void Update()
        {
            this.timeout -= Time.deltaTime;
            if (this.timeout < 0)
            {
                Destroy(this.gameObject);
            }
        }
    }
}