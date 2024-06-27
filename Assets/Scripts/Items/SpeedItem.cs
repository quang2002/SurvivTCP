namespace Items
{
    using Entities;
    using UnityEngine;

    public class SpeedItem : MonoBehaviour
    {
        [SerializeField] private float speedupCount;
        [SerializeField] private float duration;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.GetComponentInParent<Player>() is { } player)
            {
                player.SpeedupFor(this.speedupCount, this.duration);
                this.gameObject.SetActive(false);
            }
        }
    }
}