using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
public class GameManager : MonoBehaviour
{
    [field: SerializeField]
    private GameObject PlayerPrefab { get; set; }

    [field: SerializeField]
    public Server Server { get; private set; }

    private Queue<Action<Player>> CreatePlayerQueue { get; } = new ();

    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        while (this.CreatePlayerQueue.TryDequeue(out var onComplete))
        {
            var player = Instantiate(
                this.PlayerPrefab,
                (Vector3)Random.insideUnitCircle * 100f,
                Quaternion.identity
            ).GetComponent<Player>();

            onComplete(player);
        }
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void RuntimeInitializeOnLoad()
    {
        Instance = null;
    }

    public void SpawnPlayer(Action<Player> onComplete)
    {
        this.CreatePlayerQueue.Enqueue(onComplete);
    }

    public void DespawnPlayer(Player player)
    {
        if (!player) return;
        Destroy(player.gameObject);
    }
}