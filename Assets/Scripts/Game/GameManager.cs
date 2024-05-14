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
        private GameObject PlayerPrefab { get; set; }

        [field: SerializeField]
        public TcpServer TcpServer { get; private set; }

        [field: SerializeField]
        public CameraController CameraController { get; private set; }

        [field: SerializeField]
        public LobbyScreen LobbyScreen { get; private set; }

        [field: SerializeField]
        public GameScreen GameScreen { get; private set; }

        public static GameManager Instance { get; private set; }

        private Queue<Action> MainThreadActionQueue { get; } = new ();

        public IGameState CurrentState { get; private set; }

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

        public Player SpawnPlayer()
        {
            var player = Instantiate(
                this.PlayerPrefab,
                (Vector3)Random.insideUnitCircle * 100f,
                Quaternion.identity
            ).GetComponent<Player>();

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