namespace Entities.Configs
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "WeaponConfig", menuName = "ScriptableObjects/WeaponConfig", order = 1)]
    public class WeaponConfig : ScriptableObject
    {
        public float  damage;
        public float  sleepTime;
        public float  bulletSpeed;
        public int    numberBullet;
        public float  angleBetweenBullet;
        public Bullet bullet;
    }
}