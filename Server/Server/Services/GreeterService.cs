using Grpc.Core;
using ConnectionComponents.Enums;
using Microsoft.AspNetCore.SignalR.Protocol;

namespace GrpcService.Services
{
    public class GreeterService : Greeter.GreeterBase
    {
        #region Private Fields

        private const int MAX_COUNT_RECTANGLES = 1000;
        private static List<int> _usersList = new();
        private readonly ILogger<GreeterService> _logger;
        private List<RectangleReply> _rectanglesArray = new(MAX_COUNT_RECTANGLES);

        #endregion Private Fields

        #region Public Constructors

        public GreeterService(ILogger<GreeterService> logger)
        {
            _logger = logger;
        }

        #endregion Public Constructors

        #region Public Methods

        /// <summary>
        ///
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task<ConnectionReply> Connect(ConnectionRequest request, ServerCallContext context)
        {
            ConnectionReply reply = new();
            if (_usersList.Contains(request.Id) == false)
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

        public override async Task SendRectangle(
            EmptyMessage empty,
            IServerStreamWriter<RectangleReply> responseStream,
            ServerCallContext context)
        {
            while (context.CancellationToken.IsCancellationRequested == false)
            {
                RectangleReply rectangle = new RectangleReply
                {
                    Id = _rectanglesArray.Count(),
                    Height = 25,
                    Width = 75,
                    X = Random.Shared.Next(0, MAX_COUNT_RECTANGLES),
                    Y = Random.Shared.Next(0, MAX_COUNT_RECTANGLES),
                };

                _rectanglesArray.Add(rectangle);

                await responseStream.WriteAsync(rectangle);
                await Task.Delay(TimeSpan.FromSeconds(10), context.CancellationToken);
            }
        }

        #endregion Public Methods
    }
}