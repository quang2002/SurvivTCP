using System.Net.Sockets;
using System.Runtime.InteropServices;
using GameClient;
var client = new TcpClient();
client.Connect("127.0.0.1", 12345);

if (!client.Connected)
{
    throw new ("Unable to connect to server");
}

unsafe void SendCommand<T>(Proto<T> proto)
    where T : unmanaged, IProto
{
    var ptr = (byte*)&proto;
    var size = proto.ProtoSize;
    var data = new byte[512];

    for (var i = 0; i < size; i++)
    {
        data[i] = ptr[i];
    }

    client.Client.Send(data[..(size + sizeof(CommandProto))]);
}

void KeepAlive()
{
    client.Client.Send([0, 0, 0, 0]);
}

unsafe void SendInitial(string name)
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

    client.Client.Send(data[..(sizeof(CommandProto) + sizeof(int) + name.Length)]);
}

SendInitial("Trieu Dinh Quang");

new Thread(() =>
{
    while (true)
    {
        KeepAlive();
        Thread.Sleep(100);
    }
}).Start();

new Thread(() =>
{
    while (true)
    {
        var key = Console.ReadKey(true);
        var x = 0f;
        var y = 0f;

        if (key.Key == ConsoleKey.Spacebar)
        {
            SendCommand(new Proto<AttackProto>
            {
                CommandProto = CommandProto.AttackProto,
                Data = new (),
            });
        }

        if (key.Key == ConsoleKey.A) x -= 1f;
        if (key.Key == ConsoleKey.D) x += 1f;
        if (key.Key == ConsoleKey.W) y += 1f;
        if (key.Key == ConsoleKey.S) y -= 1f;

        SendCommand(new Proto<MoveProto>
        {
            CommandProto = CommandProto.MoveProto,
            Data = new ()
            {
                X = x,
                Y = y,
            },
        });
    }
}).Start();

[StructLayout(LayoutKind.Sequential)]
internal struct Proto<T> : IProto
    where T : unmanaged, IProto
{
    public CommandProto CommandProto { get; set; }
    public T Data { get; set; }
    public int ProtoSize => sizeof(CommandProto) + this.Data.ProtoSize;
}