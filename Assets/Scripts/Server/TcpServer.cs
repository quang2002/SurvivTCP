namespace Server
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Sockets;
    using Game;
    using Handlers;
    using UnityEngine;

    public class TcpServer : MonoBehaviour
    {
        private const float MaxPlayer               = 15;
        private const float MaxTimeout              = 5;
        private const float GameInfoInterval        = 1;
        private       float currentGameInfoInterval = GameInfoInterval;

        public bool IsOpenForConnection { get; set; }

        public IReadOnlyList<ClientInfo> PublicClientInfos
        {
            get => this.ClientInfos;
        }

        private List<ClientInfo> ClientInfos { get; }      = new();
        private TcpListener      TcpListener { get; }      = new(IPAddress.Parse("127.0.0.1"), 12345);
        private bool             AcceptFlag  { get; set; } = true;

        private void Update()
        {
            this.AcceptPlayers();
            this.ResolvePlayerPackets();
            this.SendGameInfo();
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
                                    case CommandProto.DropProto:
                                    {
                                        var data = *(DropProto*)cursor;
                                        cursor += data.ProtoSize;

                                        new DropHandler().Handler(info, data);
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

                GameManager.Instance.RunOnMainThread(() => { info.Player = GameManager.Instance.SpawnPlayer(); });

                this.OnConnected?.Invoke(info);

                Debug.Log("Client Connected");
            }, null);
        }

        public void Kick(ClientInfo info)
        {
            GameManager.Instance.RunOnMainThread(() => { GameManager.Instance.DespawnPlayer(info.Player); });

            info.Client.Close();
            this.ClientInfos.Remove(info);
            this.OnDisconnected?.Invoke(info);
            Debug.Log("Closed Connections");
        }

        private void SendGameInfo()
        {
            if (GameManager.Instance.CurrentState is not (GamePlayingState or GameWaitingState))
            {
                return;
            }

            this.currentGameInfoInterval -= Time.deltaTime;

            if (this.currentGameInfoInterval <= 0)
            {
                this.currentGameInfoInterval = GameInfoInterval;
            }
            else
            {
                return;
            }

            var builder = new GameInfoBuilder();

            builder.SetBullets(
                GameManager.Instance.Bullets
                           .Select(bullet => new BulletInfo
                           {
                               Position = new()
                               {
                                   X = bullet.transform.position.x,
                                   Y = bullet.transform.position.y,
                               },
                               Direction = new()
                               {
                                   X = bullet.transform.up.x,
                                   Y = bullet.transform.up.y,
                               },
                               Speed = bullet.Velocity,
                           })
            );

            builder.SetItems(
                GameManager.Instance.Items
                           .Select(bullet => new ItemInfo
                           {
                               Position = new()
                               {
                                   X = bullet.transform.position.x,
                                   Y = bullet.transform.position.y,
                               },
                               Type = bullet.name.StartsWith("Heal") ? ItemTypes.Heal : ItemTypes.Speed,
                           })
            );


            builder.SetWeapons(
                GameManager.Instance.Weapons
                           .Where(weapon => !weapon.IsMounted)
                           .Select(weapon => new WeaponInfo
                           {
                               Position = new()
                               {
                                   X = weapon.transform.position.x,
                                   Y = weapon.transform.position.y,
                               },
                               Type = weapon.name.StartsWith("Shotgun") ? WeaponTypes.Shotgun :
                                   weapon.name.StartsWith("Riffle") ? WeaponTypes.Riffle :
                                   WeaponTypes.Pistol,
                           })
            );

            foreach (var clientInfo in this.ClientInfos)
            {
                builder.SetCurrentPlayerInfo(new()
                {
                    Position = new()
                    {
                        X = clientInfo.Player.transform.position.x,
                        Y = clientInfo.Player.transform.position.y,
                    },
                    Direction = new()
                    {
                        X = clientInfo.Player.Direction.x,
                        Y = clientInfo.Player.Direction.y,
                    },
                    Health = clientInfo.Player.Health,
                    Speed = clientInfo.Player.Velocity,
                    IsAlly = true,
                });

                builder.SetOtherPlayers(
                    this.ClientInfos
                        .Where(info => info != clientInfo)
                        .Select(info => new PlayerInfo
                        {
                            Position = new()
                            {
                                X = info.Player.transform.position.x,
                                Y = info.Player.transform.position.y,
                            },
                            Direction = new()
                            {
                                X = info.Player.Direction.x,
                                Y = info.Player.Direction.y,
                            },
                            Health = info.Player.Health,
                            Speed = info.Player.Velocity,
                            IsAlly = true,
                        })
                );

                var proto = builder.Build();
                clientInfo.Client.Client.Send(proto);
            }
        }
    }
}