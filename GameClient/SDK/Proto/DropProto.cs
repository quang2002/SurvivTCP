namespace SDK.Proto;

using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential)]
public struct DropProto : IProto
{
    public int ProtoSize
    {
        get => 0;
    }
}