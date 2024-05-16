namespace Entities
{
    using UnityEngine;

    public class Bullet : MonoBehaviour
    {
        private float      damage;
        public  GameObject vfxWhenDestroy;

        public void Release(float bulletDamage, float bulletSpeed)
        {
            this.damage = bulletDamage;
            Vector2 direction = this.transform.up;
            this.GetComponent<Rigidbody2D>().velocity = direction * bulletSpeed;
        }

        private void OnCollisionEnter2D(Collision2D col)
        {
            var otherPlayer = col.gameObject.GetComponent<Player>();
            if (otherPlayer != null)
            {
                otherPlayer.TakeDamage(this.damage, otherPlayer);

                return;
            }
            if (this.vfxWhenDestroy) Instantiate(this.vfxWhenDestroy);
            if (this.gameObject) Destroy(this.gameObject);
        }
    }
}