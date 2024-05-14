using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace Entities
{
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
        private float Health { get; set; } = 100;

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
        
        private Weapon Weapon { get; set; }

        public Vector2 Direction { get; set; }

        public string Name
        {
            get => this.NameTextMeshPro.text;
            set => this.NameTextMeshPro.text = value;
        }

        private void Update()
        {
            this.Rigidbody2D.velocity = this.Direction.normalized * this.Speed;

            this.Visual.up = -this.Direction;
            this.HealthSlider.value = this.Health / 100.0f;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.transform.GetComponent<Weapon>() is { IsMounted: false } weapon)
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
                target.TakeDamage(10);
            }
        }

        public void TakeDamage(float damage)
        {
            this.Health -= damage;
        }
    }
}