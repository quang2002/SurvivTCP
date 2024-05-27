using System.Runtime.InteropServices;
namespace SDK.Proto
{
    [StructLayout(LayoutKind.Sequential)]
    public struct MoveProto : IProto
    {
        public float X { get; set; }
        public float Y { get; set; }

        public int ProtoSize => sizeof(float) + sizeof(float);
    }
}