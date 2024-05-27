using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
[StructLayout(LayoutKind.Sequential)]
public struct Vec2
{
    public float X { get; set; }
    public float Y { get; set; }
}

[StructLayout(LayoutKind.Sequential)]
public struct PlayerInfo
{
    public Vec2 Position { get; set; }
    public Vec2 Direction { get; set; }
    public float Speed { get; set; }
    public float Health { get; set; }
    public bool IsAlly { get; set; }
}

[StructLayout(LayoutKind.Sequential)]
public struct BulletInfo
{
    public Vec2 Position { get; set; }
    public Vec2 Direction { get; set; }
    public float Speed { get; set; }
}

public class GameInfoBuilder
{
    public PlayerInfo Me { get; private set; }
    public IEnumerable<PlayerInfo> Others { get; private set; }
    public IEnumerable<BulletInfo> Bullets { get; private set; }

    public GameInfoBuilder SetCurrentPlayerInfo(PlayerInfo playerInfo)
    {
        this.Me = playerInfo;
        return this;
    }

    public GameInfoBuilder SetOtherPlayers(IEnumerable<PlayerInfo> players)
    {
        this.Others = players;
        return this;
    }

    public GameInfoBuilder SetBullets(IEnumerable<BulletInfo> bullets)
    {
        this.Bullets = bullets;
        return this;
    }

    public unsafe byte[] Build()
    {
        // 1. Calc size of proto
        var totalSize = 0;
        var othersCount = this.Others.Count();
        var bulletsCount = this.Bullets.Count();

        totalSize += sizeof(PlayerInfo);// Me
        totalSize += sizeof(PlayerInfo) * othersCount + sizeof(int);// Others
        totalSize += sizeof(BulletInfo) * bulletsCount + sizeof(int);// Bullets

        // 2. Build proto
        var proto = new byte[totalSize];

        fixed (byte* ptr = proto.AsSpan())
        {
            var cur = ptr;
            // Me
            *(PlayerInfo*)cur = this.Me;
            cur += sizeof(PlayerInfo);

            // Others
            *(int*)cur = othersCount;
            cur += sizeof(int);

            foreach (var other in this.Others)
            {
                *(PlayerInfo*)cur = other;
                cur += sizeof(PlayerInfo);
            }

            // Bullets
            *(int*)cur = bulletsCount;
            cur += sizeof(int);

            foreach (var bullet in this.Bullets)
            {
                *(BulletInfo*)cur = bullet;
                cur += sizeof(BulletInfo);
            }
        }

        return proto;
    }

    public static unsafe GameInfoBuilder FromProto(byte[] proto)
    {
        var builder = new GameInfoBuilder();

        fixed (byte* ptr = proto.AsSpan())
        {
            var cur = ptr;

            // Me
            {
                var me = *(PlayerInfo*)cur;
                cur += sizeof(PlayerInfo);

                builder.Me = me;
            }

            // Others
            {
                var others = new PlayerInfo[*(int*)cur];
                cur += sizeof(int);

                for (var i = 0; i < others.Length; i++)
                {
                    others[i] = *(PlayerInfo*)cur;
                    cur += sizeof(PlayerInfo);
                }

                builder.Others = others;
            }

            // Bullets
            {
                var bullets = new BulletInfo[*(int*)cur];
                cur += sizeof(int);

                for (var i = 0; i < bullets.Length; i++)
                {
                    bullets[i] = *(BulletInfo*)cur;
                    cur += sizeof(BulletInfo);
                }

                builder.Bullets = bullets;
            }
        }

        return builder;
    }
}