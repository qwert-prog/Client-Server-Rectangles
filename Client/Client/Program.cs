using ConnectionComponents.Enums;
using Grpc.Net.Client;
using GrpcService;


// создаем канал для обмена сообщениями с сервером
// параметр - адрес сервера gRPC
using var channel = GrpcChannel.ForAddress("https://localhost:7288");
// создаем клиента
var client = new Greeter.GreeterClient(channel);

// обмениваемся сообщениями с сервером
Reply reply = await client.ConnectAsync(new Request { Id = 1 });

if((ConnectionCodeEnums)reply.Code == ConnectionCodeEnums.Success)
{
    Console.WriteLine("Connect");
}
else
{
    Console.WriteLine("ERROR");
}

Console.ReadKey();
        
