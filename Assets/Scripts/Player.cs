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
    private float Health { get; set; } = 100;

    [field: SerializeField]
    private Collider2D MeleeAttackCollider2D { get; set; }

    [field: SerializeField]
    private Slider HealthSlider { get; set; }

    [field: SerializeField]
    private TMP_Text NameTextMeshPro { get; set; }

    public Vector2 Direction { get; set; }

    public string Name
    {
        get => this.NameTextMeshPro.text;
        set => this.NameTextMeshPro.text = value;
    }

    private void Update()
    {
        this.Rigidbody2D.velocity = this.Direction.normalized * this.Speed;

        var x = Input.GetAxis("Horizontal");
        var y = Input.GetAxis("Vertical");

        this.Direction = new Vector2(x, y);

        this.transform.up = -this.Direction;
    }

    public void MeleeAttack()
    {
        this.Animator.SetTrigger("MeleeAttack");
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