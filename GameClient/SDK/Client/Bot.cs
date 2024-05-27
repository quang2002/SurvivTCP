using System.Net.Sockets;
using SDK.Proto;
namespace SDK.Client
{
    public unsafe class Bot(string host, int port)
    {
        private readonly TcpClient client = new ();
        private readonly byte[] rBuffer = new byte[4096];

        public GameInfo? GameInfo { get; private set; }

        public bool Connect()
        {
            try
            {
                this.client.Connect(host, port);
                this.client.Client.Blocking = false;

                if (!this.client.Connected)
                {
                    throw new ("Unable to connect to server");
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool Attack(AttackProto proto)
        {
            try
            {
                var data = new byte[sizeof(CommandProto) + proto.ProtoSize];

                fixed (byte* ptr = data.AsSpan())
                {
                    *(CommandProto*)ptr = CommandProto.AttackProto;
                    *(AttackProto*)(ptr + sizeof(CommandProto)) = proto;
                }

                this.client.Client.Send(data);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool Move(MoveProto proto)
        {
            try
            {
                var data = new byte[sizeof(CommandProto) + proto.ProtoSize];

                fixed (byte* ptr = data.AsSpan())
                {
                    *(CommandProto*)ptr = CommandProto.MoveProto;
                    *(MoveProto*)(ptr + sizeof(CommandProto)) = proto;
                }

                this.client.Client.Send(data);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool KeepAlive()
        {
            try
            {
                this.client.Client.Send([0, 0, 0, 0]);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void Initial(string name)
        {
            var data = new byte[512];

            fixed (byte* ptr = data.AsSpan())
            {
                var cursor = ptr;

                *(CommandProto*)cursor = CommandProto.InitialProto;
                cursor += sizeof(CommandProto);

                *(int*)cursor = name.Length;
                cursor += sizeof(int);

                foreach (var c in name)
                {
                    *(char*)cursor++ = c;
                }
            }

            this.client.Client.Send(data[..(sizeof(CommandProto) + sizeof(int) + name.Length)]);
        }

        public bool FetchGameInfo()
        {
            try
            {
                var len = this.client.Client.Receive(this.rBuffer);
                this.GameInfo = GameInfo.FromProto(this.rBuffer[..len]);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}