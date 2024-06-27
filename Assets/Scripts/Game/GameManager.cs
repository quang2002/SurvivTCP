using System;
using System.Collections.Generic;
using Entities;
using Screen;
using Server;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game
{
    public class GameManager : MonoBehaviour
    {
        [field: SerializeField]
        private Player PlayerPrefab { get; set; }

        [field: SerializeField]
        private GameObject RockPrefab { get; set; }
        
        [field: SerializeField]
        private GameObject[] ItemsPrefab { get; set; }

        [field: SerializeField]
        private Weapon[] WeaponPrefabs { get; set; }

        [field: SerializeField]
        public TcpServer TcpServer { get; private set; }

        [field: SerializeField]
        public CameraController CameraController { get; private set; }

        [field: SerializeField]
        public LobbyScreen LobbyScreen { get; private set; }

        [field: SerializeField]
        public GameScreen GameScreen { get; private set; }

        public List<Weapon> Weapons { get; } = new ();

        public List<GameObject> Rocks { get; } = new ();
        public List<GameObject> Items { get; } = new ();

        public static GameManager Instance { get; private set; }

        private Queue<Action> MainThreadActionQueue { get; } = new ();

        public IGameState CurrentState { get; private set; }

        public HashSet<Bullet> Bullets { get; } = new ();

        private void Awake()
        {
            Instance = this;
            this.TransitionTo<GameLobbyState>();
        }

        private void Update()
        {
            while (this.MainThreadActionQueue.TryDequeue(out var queue))
            {
                queue();
            }

            this.CurrentState?.Tick();
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void RuntimeInitializeOnLoad()
        {
            Instance = null;
        }

        public void RandomSpawnWeapons(int count)
        {
            for (var i = 0; i < count; i++)
            {
                var randomIdx = (int)(Random.value * this.WeaponPrefabs.Length);
                var weapon = Instantiate(
                    this.WeaponPrefabs[randomIdx],
                    (Vector3)Random.insideUnitCircle * 100f,
                    Quaternion.identity
                );

                this.Weapons.Add(weapon);
            }
        }

        public void RandomSpawnRocks(int count)
        {
            for (var i = 0; i < count; i++)
            {
                var rock = Instantiate(
                    this.RockPrefab,
                    (Vector3)Random.insideUnitCircle * 100f,
                    Quaternion.identity
                );

                rock.transform.localScale = (Random.value * 5 + 1) * Vector3.one;

                this.Rocks.Add(rock);
            }
        }
        
        public void RandomSpawnItems(int count)
        {
            for (var i = 0; i < count; i++)
            {
                var item = Instantiate(
                    this.ItemsPrefab[(int)(Random.value * this.ItemsPrefab.Length)],
                    (Vector3)Random.insideUnitCircle * 100f,
                    Quaternion.identity
                );

                this.Items.Add(item);
            }
        }

        public void ClearWeapons()
        {
            foreach (var weapon in this.Weapons)
            {
                Destroy(weapon.gameObject);
            }
        }
        
        public void ClearItems()
        {
            foreach (var item in this.Items)
            {
                Destroy(item.gameObject);
            }
        }

        public Player SpawnPlayer()
        {
            var player = Instantiate(
                this.PlayerPrefab,
                (Vector3)Random.insideUnitCircle * 100f,
                Quaternion.identity
            );

            return player;
        }

        public void DespawnPlayer(Player player)
        {
            if (!player) return;
            Destroy(player.gameObject);
        }

        public void RunOnMainThread(Action action)
        {
            this.MainThreadActionQueue.Enqueue(action);
        }

        public void TransitionTo<T>()
            where T : IGameState, new()
        {
            var state = new T();
            this.CurrentState?.Exit();
            this.CurrentState = state;
            this.CurrentState?.Enter();
        }
    }
}