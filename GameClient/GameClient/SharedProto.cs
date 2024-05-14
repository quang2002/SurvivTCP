using System.Runtime.InteropServices;
using System.Text;
namespace GameClient
{
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

        public int ProtoSize => sizeof(float) + sizeof(float);
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct AttackProto : IProto
    {

        public int ProtoSize => 0;
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct ReadonlyStringProto : IProto
    {
        private int count;
        private byte* rawData;

        public override string ToString()
        {
            return Encoding.UTF8.GetString(this.rawData, this.count);
        }

        public int ProtoSize => sizeof(int) + this.count;
    }
}