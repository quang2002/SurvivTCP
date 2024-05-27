using Game;
using UnityEngine;
namespace Entities
{

    public class Bullet : MonoBehaviour
    {
        public GameObject vfxWhenDestroy;
        private float damage;
        private Player src;

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

                otherPlayer.TakeDamage(this.damage, otherPlayer);
                return;
            }

            if (this.vfxWhenDestroy) Instantiate(this.vfxWhenDestroy);
            if (this.gameObject) Destroy(this.gameObject);
        }

        public void Release(float bulletDamage, float bulletSpeed, Player source)
        {
            this.src = source;
            this.damage = bulletDamage;
            Vector2 direction = this.transform.up;
            this.GetComponent<Rigidbody2D>().velocity = direction * bulletSpeed;
        }
    }
}