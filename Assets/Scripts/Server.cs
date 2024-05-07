using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
public class Server : MonoBehaviour
{
    private const float MaxTimeout = 5;

    public IReadOnlyList<ClientInfo> Players => this.ClientInfos;

    private List<ClientInfo> ClientInfos { get; } = new ();
    private TcpListener TcpListener { get; } = new (IPAddress.Parse("127.0.0.1"), 12345);
    public bool IsOpenForConnection { get; set; } = true;

    private void Update()
    {
        if (this.IsOpenForConnection)
        {
            this.IsOpenForConnection = false;
            this.TcpListener.BeginAcceptTcpClient(ar =>
            {
                var client = this.TcpListener.EndAcceptTcpClient(ar);
                client.Client.Blocking = false;
                var info = new ClientInfo
                {
                    Client = client,
                    Timeout = MaxTimeout,
                };

                this.ClientInfos.Add(info);

                GameManager.Instance.SpawnPlayer(player => info.Player = player);

                Debug.Log("Client Connected");
                this.IsOpenForConnection = true;
            }, null);
        }

        var deltaTime = Time.deltaTime;

        foreach (var info in this.ClientInfos)
        {
            info.Timeout -= deltaTime;

            if (info.Timeout < 0)
            {
                continue;
            }

            try
            {
                var buffer = info.Buffer;
                var player = info.Player;
                var count = info.Client.Client.Receive(buffer);

                unsafe
                {
                    fixed (byte* proto = buffer.AsSpan())
                    {
                        var cursor = proto;

                        while (count - (cursor - proto) >= sizeof(CommandProto))
                        {
                            var command = *(CommandProto*)cursor;
                            cursor += sizeof(CommandProto);

                            switch (command)
                            {
                                case CommandProto.MoveProto:
                                {
                                    var data = *(MoveProto*)cursor;
                                    cursor += data.ProtoSize;

                                    player.Direction = data.Direction;
                                    Debug.Log($"Move: (${data.X}, ${data.Y})");
                                    break;
                                }
                                case CommandProto.InitialProto:
                                {
                                    var data = *(InitialProto*)cursor;
                                    cursor += data.ProtoSize;

                                    player.Name = data.Name;
                                    Debug.Log($"Initial: (${data.Name})");
                                    break;
                                }
                                case CommandProto.AttackProto:
                                {
                                    var data = *(AttackProto*)cursor;
                                    cursor += data.ProtoSize;

                                    player.MeleeAttack();
                                    Debug.Log("Attack: ()");
                                    break;
                                }
                                case CommandProto.KeepAlive:
                                {
                                    info.Timeout = MaxTimeout;

                                    Debug.Log("Keep Alive: ()");
                                    break;
                                }
                                default:
                                    Debug.LogError($"Unknown command: ${command}");
                                    break;
                            }
                        }
                    }
                }
            }
            catch
            {
                // ignore
            }
        }

        foreach (var info in this.ClientInfos.ToArray())
        {
            if (info.Timeout >= 0) continue;
            GameManager.Instance.DespawnPlayer(info.Player);
            info.Client.Close();
            this.ClientInfos.Remove(info);
            Debug.Log("Closed Connections");
        }
    }

    private void OnEnable()
    {
        this.TcpListener.Start();
        this.IsOpenForConnection = true;
        Debug.Log("Tcp Listener Started");
    }

    private void OnDisable()
    {
        this.TcpListener.Stop();
        this.IsOpenForConnection = false;
        Debug.Log("Tcp Listener Stopped");
    }
}