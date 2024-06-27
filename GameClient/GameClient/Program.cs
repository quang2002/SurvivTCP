using SDK.Client;

var client = new Bot("localhost", 12345);

if (!client.Connect())
{
    throw new("Connect error");
}

client.Initial("Trieu Dinh Quang");

new Thread(() =>
{
    while (true)
    {
        try
        {
            client.KeepAlive();
            client.FetchGameInfo();

            if (client.GameInfo is not null)
            {
                Console.WriteLine($"Bullet Count: {client.GameInfo.Bullets.Count()}");
            }
        }
        catch
        {
            // ignored
        }

        Thread.Sleep(100);
    }
}).Start();

new Thread(() =>
{
    while (true)
    {
        try
        {
            var position = client.GameInfo?.Me.Position;

            if (position is not null)
            {
                var weaponPosition = client.GameInfo?.Weapons.MinBy(info =>
                    (info.Position.X - position.Value.X) * (info.Position.X - position.Value.X)
                    + (info.Position.Y - position.Value.Y) * (info.Position.Y - position.Value.Y)
                ).Position;

                client.Attack();
                client.Drop();

                if (weaponPosition is not null)
                    client.Move(new()
                    {
                        X = weaponPosition.Value.X - position.Value.X,
                        Y = weaponPosition.Value.Y - position.Value.Y,
                    });
            }
        }
        catch
        {
            // ignored
        }

        Thread.Sleep(100);
    }
}).Start();