using System.Runtime.InteropServices;
using UnityEngine;
public enum CommandProto
{
    KeepAlive,
    MoveProto,
    InitialProto,
    AttackProto,
}

public interface IProto
{
    // manual implements this for performance
    // or use reflection for generic proto size
    public int ProtoSize { get; }
}

[StructLayout(LayoutKind.Sequential)]
public struct MoveProto : IProto
{
    public float X { get; set; }
    public float Y { get; set; }

    public Vector2 Direction => new (this.X, this.Y);
    public int ProtoSize => sizeof(float) + sizeof(float);
}

[StructLayout(LayoutKind.Sequential)]
public struct AttackProto : IProto
{

    public int ProtoSize => 0;
}