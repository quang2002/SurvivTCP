using SDK.Client;
var client = new Bot("localhost", 12345);

if (!client.Connect())
{
    throw new ("Connect error");
}

client.Initial("Trieu Dinh Quang");

new Thread(() =>
{
    while (true)
    {
        client.KeepAlive();
        client.FetchGameInfo();

        if (client.GameInfo is not null)
        {
            Console.WriteLine($"Bullet Count: {client.GameInfo.Bullets.Count()}");
        }

        Thread.Sleep(100);
    }
}).Start();

new Thread(() =>
{
    while (true)
    {
        var position = client.GameInfo?.Me.Position;

        if (position is not null)
        {
            client.Attack(new ());

            client.Move(new ()
            {
                X = 5 - position.Value.X,
                Y = 6 - position.Value.Y,
            });
        }

        Thread.Sleep(100);
    }
}).Start();