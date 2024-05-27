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

public class GameInfo
{
    public PlayerInfo Me { get; private set; }
    public IEnumerable<PlayerInfo> Others { get; private set; } = [];
    public IEnumerable<BulletInfo> Bullets { get; private set; } = [];

    public static unsafe GameInfo FromProto(byte[] proto)
    {
        var builder = new GameInfo();

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