using Grpc.Core;
using ConnectionComponents.Enums;
using Microsoft.AspNetCore.SignalR.Protocol;
using Server.Models;

namespace GrpcService.Services
{
    /// <summary>
    /// gRPC ������ ��� �������� ����������� ���������.
    /// </summary>
    public class GreeterService : Greeter.GreeterBase
    {
        #region Private Fields

        /// <summary>
        /// ������������ ���������� ���������������.
        /// </summary>
        private const int MAX_COUNT_RECTANGLES = 1000;

        /// <summary>
        /// ������ �� ����� ��������������-���������.
        /// </summary>
        private static List<int> _usersList = new();

        /// <summary>
        /// ������ ��� ������ ����������.
        /// </summary>
        private readonly ILogger<GreeterService> _logger;

        /// <summary>
        /// ������ ���� ���������������.
        /// </summary>
        private List<RectangleReply> _rectanglesArray = new(MAX_COUNT_RECTANGLES);

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// �������������� ��������� ������ <see cref="GreeterService"/>.
        /// </summary>
        /// <param name="logger">������ ��� �������.</param>
        public GreeterService(ILogger<GreeterService> logger)
        {
            _logger = logger;
        }

        #endregion Public Constructors

        #region Public Methods

        /// <summary>
        /// ���������� ����������� � �������.
        /// </summary>
        /// <param name="request">������ �� �����������.</param>
        /// <param name="context"><inheritdoc/></param>
        /// <returns>��������� ������ <see cref="ConnectionReply"/>, ������� �������� ������� �������.</returns>
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

        /// <summary>
        /// ���������� ��������� ������ <see cref="RectangleReply"/> �������.
        /// </summary>
        /// <param name="empty">������ ������.</param>
        /// <param name="responseStream">����� ������, � ������� �������� ����������� <see cref="RectangleReply"/>.</param>
        /// <param name="context"><inheritdoc/></param>
        public override async Task SendRectangle(
            EmptyMessage empty,
            IServerStreamWriter<RectangleReply> responseStream,
            ServerCallContext context)
        {
            while (context.CancellationToken.IsCancellationRequested == false)
            {
                RectangleReply rectangle = new()
                {
                    Id = _rectanglesArray.Count(),
                    Height = ServerConsts.HEIGHT_OF_RECTANGLE,
                    Width = ServerConsts.WIDTH_OF_RECTANGLE,
                    X = Random.Shared.Next(0, MAX_COUNT_RECTANGLES),
                    Y = Random.Shared.Next(0, MAX_COUNT_RECTANGLES),
                };

                _rectanglesArray.Add(rectangle);

                await responseStream.WriteAsync(rectangle);

                await Task.Delay(TimeSpan.FromSeconds(1), context.CancellationToken);
            }
        }

        #endregion Public Methods
    }
}