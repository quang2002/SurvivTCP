using System.Net.Sockets;
using Entities;
public class ClientInfo
{
    public TcpClient Client { get; set; }
    public float Timeout { get; set; }
    public byte[] Buffer { get; } = new byte[2048];
    public Player Player { get; set; }
}