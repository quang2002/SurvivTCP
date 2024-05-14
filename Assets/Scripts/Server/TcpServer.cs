using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using Game;
using Server.Handlers;
using UnityEngine;
namespace Server
{
    public class TcpServer : MonoBehaviour
    {
        private const float MaxPlayer = 15;
        private const float MaxTimeout = 5;

        public bool IsOpenForConnection { get; set; }

        public IReadOnlyList<ClientInfo> PublicClientInfos => this.ClientInfos;
        private List<ClientInfo> ClientInfos { get; } = new ();
        private TcpListener TcpListener { get; } = new (IPAddress.Parse("127.0.0.1"), 12345);
        private bool AcceptFlag { get; set; } = true;

        private void Update()
        {
            this.AcceptPlayers();
            this.ResolvePlayerPackets();
        }

        private void OnEnable()
        {
            this.TcpListener.Start();
            this.AcceptFlag = true;
            Debug.Log("Tcp Listener Started");
        }

        private void OnDisable()
        {
            this.TcpListener.Stop();
            this.AcceptFlag = false;
            Debug.Log("Tcp Listener Stopped");
        }

        public event Action<ClientInfo> OnConnected;
        public event Action<ClientInfo> OnDisconnected;

        private void ResolvePlayerPackets()
        {
            var deltaTime = Time.deltaTime;

            foreach (var info in this.ClientInfos.ToArray())
            {
                info.Timeout -= deltaTime;

                if (info.Timeout < 0)
                {
                    this.Kick(info);
                    continue;
                }

                try
                {
                    var buffer = info.Buffer;
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

                                        new MoveHandler().Handler(info, data);
                                        break;
                                    }
                                    case CommandProto.InitialProto:
                                    {
                                        cursor += new InitialHandler().Handler(info, cursor);
                                        break;
                                    }
                                    case CommandProto.AttackProto:
                                    {
                                        var data = *(AttackProto*)cursor;
                                        cursor += data.ProtoSize;

                                        new AttackHandler().Handler(info, data);
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
        }

        private void AcceptPlayers()
        {
            if (!this.AcceptFlag || !this.IsOpenForConnection) return;

            this.AcceptFlag = false;
            this.TcpListener.BeginAcceptTcpClient(ar =>
            {
                this.AcceptFlag = true;

                var client = this.TcpListener.EndAcceptTcpClient(ar);

                if (!this.IsOpenForConnection || this.ClientInfos.Count >= MaxPlayer)
                {
                    client.Close();
                    return;
                }

                client.Client.Blocking = false;
                var info = new ClientInfo
                {
                    Client = client,
                    Timeout = MaxTimeout,
                };

                this.ClientInfos.Add(info);

                GameManager.Instance.RunOnMainThread(() =>
                {
                    info.Player = GameManager.Instance.SpawnPlayer();
                });

                this.OnConnected?.Invoke(info);

                Debug.Log("Client Connected");
            }, null);
        }

        public void Kick(ClientInfo info)
        {
            GameManager.Instance.RunOnMainThread(() =>
            {
                GameManager.Instance.DespawnPlayer(info.Player);
            });

            info.Client.Close();
            this.ClientInfos.Remove(info);
            this.OnDisconnected?.Invoke(info);
            Debug.Log("Closed Connections");
        }
    }
}