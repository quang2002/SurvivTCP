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
    public Vec2  Position  { get; set; }
    public Vec2  Direction { get; set; }
    public float Speed     { get; set; }
    public float Health    { get; set; }
    public bool  IsAlly    { get; set; }
}

[StructLayout(LayoutKind.Sequential)]
public struct BulletInfo
{
    public Vec2  Position  { get; set; }
    public Vec2  Direction { get; set; }
    public float Speed     { get; set; }
}

public enum ItemTypes
{
    Heal,
    Speed,
}

[StructLayout(LayoutKind.Sequential)]
public struct ItemInfo
{
    public Vec2      Position { get; set; }
    public ItemTypes Type     { get; set; }
}

public enum WeaponTypes
{
    Pistol,
    Riffle,
    Shotgun,
}


[StructLayout(LayoutKind.Sequential)]
public struct WeaponInfo
{
    public Vec2        Position { get; set; }
    public WeaponTypes Type     { get; set; }
}


public class GameInfoBuilder
{
    public PlayerInfo              Me      { get; private set; }
    public IEnumerable<PlayerInfo> Others  { get; private set; }
    public IEnumerable<BulletInfo> Bullets { get; private set; }
    public IEnumerable<ItemInfo>   Items   { get; private set; }
    public IEnumerable<WeaponInfo> Weapons { get; private set; }

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

    public GameInfoBuilder SetWeapons(IEnumerable<WeaponInfo> weapons)
    {
        this.Weapons = weapons;
        return this;
    }

    public GameInfoBuilder SetItems(IEnumerable<ItemInfo> items)
    {
        this.Items = items;
        return this;
    }

    public unsafe byte[] Build()
    {
        // 1. Calc size of proto
        var totalSize = 0;
        var othersCount = this.Others.Count();
        var bulletsCount = this.Bullets.Count();
        var weaponsCount = this.Weapons.Count();
        var itemsCount = this.Items.Count();

        totalSize += sizeof(PlayerInfo); // Me
        totalSize += sizeof(PlayerInfo) * othersCount + sizeof(int); // Others
        totalSize += sizeof(BulletInfo) * bulletsCount + sizeof(int); // Bullets
        totalSize += sizeof(ItemInfo) * itemsCount + sizeof(int); // Items
        totalSize += sizeof(WeaponInfo) * weaponsCount + sizeof(int); // Weapons

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

            // Items
            *(int*)cur = itemsCount;
            cur += sizeof(int);

            foreach (var item in this.Items)
            {
                *(ItemInfo*)cur = item;
                cur += sizeof(ItemInfo);
            }

            // Weapons
            *(int*)cur = weaponsCount;
            cur += sizeof(int);

            foreach (var weapon in this.Weapons)
            {
                *(WeaponInfo*)cur = weapon;
                cur += sizeof(WeaponInfo);
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

            // Items
            {
                var items = new ItemInfo[*(int*)cur];
                cur += sizeof(int);

                for (var i = 0; i < items.Length; i++)
                {
                    items[i] = *(ItemInfo*)cur;
                    cur += sizeof(ItemInfo);
                }

                builder.Items = items;
            }

            // Weapons
            {
                var weapons = new WeaponInfo[*(int*)cur];
                cur += sizeof(int);

                for (var i = 0; i < weapons.Length; i++)
                {
                    weapons[i] = *(WeaponInfo*)cur;
                    cur += sizeof(WeaponInfo);
                }

                builder.Weapons = weapons;
            }
        }

        return builder;
    }
}