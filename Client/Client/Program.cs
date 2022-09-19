using ConnectionComponents.Enums;
using Grpc.Core;
using Grpc.Net.Client;
using GrpcService;

// создаем канал для обмена сообщениями с сервером
// параметр - адрес сервера gRPC
using var channel = GrpcChannel.ForAddress("https://localhost:7288");
// создаем клиента
var client = new Greeter.GreeterClient(channel);

// обмениваемся сообщениями с сервером
ConnectionReply reply = await client.ConnectAsync(new ConnectionRequest { Id = 0 });

if ((ConnectionCodeEnums)reply.Code == ConnectionCodeEnums.Success)
{
    Console.WriteLine("Connect");

    AsyncServerStreamingCall<RectangleReply> response = client.SendRectangle(new EmptyMessage());

    await foreach (var update in response.ResponseStream.ReadAllAsync())
    {
        Console.WriteLine(update.Id);
        Console.WriteLine(update.X);
        Console.WriteLine(update.Y);
        Console.WriteLine();
    }
}
else
{
    Console.WriteLine("ERROR");
}

Console.ReadKey();