using Grpc.Core;
using ConnectionComponents.Enums;

namespace GrpcService.Services
{
    public class GreeterService : Greeter.GreeterBase
    {
        private readonly ILogger<GreeterService> _logger;
        public GreeterService(ILogger<GreeterService> logger)
        {
            _logger = logger;
        }

        static List<int> _usersList = new();

        public override Task<Reply> Connect(Request request, ServerCallContext context)
        {
            Reply reply = new Reply();
            if(_usersList.Contains(request.Id) == false)
            {
                _usersList.Add(request.Id);
                reply.Code = (int)ConnectionCodeEnums.Success;
            }
            else
            {
                reply.Code = (int)ConnectionCodeEnums.Error;
                
            }
            return Task.FromResult(reply);
        }
    }
}