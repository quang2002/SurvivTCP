using System.Runtime.InteropServices;
using System.Text;
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
public struct InitialProto : IProto
{
    private StringProto name;

    public string Name => this.name.ToString();
    public int ProtoSize => this.name.ProtoSize;
}

[StructLayout(LayoutKind.Sequential)]
public struct AttackProto : IProto
{

    public int ProtoSize => 0;
}

[StructLayout(LayoutKind.Sequential)]
public unsafe struct StringProto : IProto
{
    private int count;
    private byte* rawData;

    public override string ToString()
    {
        return Encoding.UTF8.GetString(this.rawData, this.count);
    }

    public int ProtoSize => sizeof(int) + this.count;
}