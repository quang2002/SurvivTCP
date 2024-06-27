namespace Items
{
    using Entities;
    using UnityEngine;

    public class HealingItem : MonoBehaviour
    {
        [SerializeField] private float healingCount;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.GetComponentInParent<Player>() is { } player)
            {
                player.Heal(this.healingCount);
                this.gameObject.SetActive(false);
            }
        }
    }
}