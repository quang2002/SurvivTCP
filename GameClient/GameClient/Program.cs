using SDK.Client;
var client = new Bot("127.0.0.1", 12345);

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
        var key = Console.ReadKey(true);
        var x = 0f;
        var y = 0f;

        switch (key.Key)
        {
            case ConsoleKey.Spacebar:
                client.Attack(default);
                break;
            case ConsoleKey.A:
                x -= 1f;
                break;
            case ConsoleKey.D:
                x += 1f;
                break;
            case ConsoleKey.W:
                y += 1f;
                break;
            case ConsoleKey.S:
                y -= 1f;
                break;
        }

        client.Move(new ()
        {
            X = x,
            Y = y,
        });
    }
}).Start();