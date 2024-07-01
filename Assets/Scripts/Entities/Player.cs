namespace Entities
{
    using System.Collections.Generic;
    using System.Linq;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class Player : MonoBehaviour
    {
        [field: SerializeField]
        private SpriteRenderer BodySpriteRenderer { get; set; }

        [field: SerializeField]
        private Rigidbody2D Rigidbody2D { get; set; }

        [field: SerializeField]
        private Animator Animator { get; set; }

        [field: SerializeField]
        private float Speed { get; set; }

        [field: SerializeField]
        private float BonusSpeed { get; set; }

        [field: SerializeField]
        public float Health { get; private set; } = 100;

        [field: SerializeField]
        private Collider2D MeleeAttackCollider2D { get; set; }

        [field: SerializeField]
        private Slider HealthSlider { get; set; }

        [field: SerializeField]
        private TMP_Text NameTextMeshPro { get; set; }

        [field: SerializeField]
        private Transform Visual { get; set; }

        [field: SerializeField]
        private Transform LHand { get; set; }

        [field: SerializeField]
        private bool Debug { get; set; }

        [field: SerializeField]
        public float DeadTime { get; set; }

        [field: SerializeField]
        public SpriteRenderer[] Renderers { get; set; }

        private float BonusSpeedDuration { get; set; }

        public int Point { get; set; }

        private Weapon Weapon { get; set; }

        public Vector2 Direction { get; set; }

        public bool IsDead => this.DeadTime > 0;

        public float Velocity
        {
            get => this.Rigidbody2D.velocity.magnitude;
        }

        private void SetAlpha(float alpha)
        {
            foreach (var renderer in this.Renderers)
            {
                renderer.color = new Color(
                    renderer.color.r,
                    renderer.color.g,
                    renderer.color.b,
                    alpha
                );
            }
        }

        public string Name
        {
            get => this.NameTextMeshPro.text;
            set => this.NameTextMeshPro.text = value;
        }

        private void Update()
        {
            //this.gameObject.SetActive(!this.IsDead);

            this.SetAlpha(this.IsDead ? 0.5f : 1.0f);
            
            if (this.IsDead)
            {
                this.DeadTime -= Time.deltaTime;
                if (!this.IsDead)
                {
                    this.OnRevive();
                }

                return;
            }

            this.Rigidbody2D.velocity = this.Direction.normalized * (this.Speed + this.BonusSpeed);

            this.Visual.up = -this.Direction;
            this.HealthSlider.value = this.Health / 100.0f;

            this.BonusSpeedDuration -= Time.deltaTime;

            if (this.BonusSpeedDuration <= 0)
            {
                this.BonusSpeedDuration = 0;
                this.BonusSpeed = 0;
            }

            if (this.Debug)
            {
                var hor = Input.GetAxis("Horizontal");
                var ver = Input.GetAxis("Vertical");

                this.Direction = new(hor, ver);

                if (Input.GetKeyDown(KeyCode.Space))
                {
                    this.Attack();
                }
            }
        }

        private void OnRevive()
        {
            this.Health = 100;
            
            this.Rigidbody2D.simulated = true;
            foreach (var child in this.GetComponentsInChildren<Collider2D>())
            {
                child.enabled = true;
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.transform.GetComponentInParent<Weapon>() is { IsMounted: false } weapon)
            {
                if (this.Weapon) return;
                this.Weapon = weapon;
                weapon.Mount(this.LHand);
            }
        }

        public void Attack()
        {
            if (this.Weapon)
            {
                this.Weapon.Attack();
            }
            else
            {
                this.Animator.SetTrigger("MeleeAttack");
            }
        }

        public void Drop()
        {
            if (this.Weapon)
            {
                this.Weapon.Unmount();
                this.Weapon = null;
            }
        }

        public void OnMeleeAttack()
        {
            var filter = new ContactFilter2D
            {
                layerMask = LayerMask.NameToLayer("Player"),
            };

            var contacts = new List<Collider2D>();

            this.MeleeAttackCollider2D.OverlapCollider(filter, contacts);

            var targets = contacts.Select(e => e.transform.GetComponentInParent<Player>()).ToHashSet();

            foreach (var target in targets.Where(target => target != this))
            {
                target?.TakeDamage(10, this);
            }
        }

        public void TakeDamage(float damage, Player source)
        {
            if (this.Health <= 0)
                return;

            this.Health -= damage;

            if (source)
            {
                source.Point++;
            }

            if (this.Health <= 0)
                this.OnDead();
        }

        private void OnDead()
        {
            this.Drop();
            this.DeadTime = 5;
            this.Direction = Vector2.zero;

            this.Rigidbody2D.simulated = false;
            foreach (var child in this.GetComponentsInChildren<Collider2D>())
            {
                child.enabled = false;
            }
        }

        public void Heal(float healingCount)
        {
            this.Health += healingCount;
        }

        public void SpeedupFor(float speedupCount, float duration)
        {
            this.BonusSpeed = Mathf.Max(speedupCount, this.BonusSpeed);
            this.BonusSpeedDuration = Mathf.Max(duration, this.BonusSpeedDuration);
        }
    }
}