using Grpc.Core;
using ConnectionComponents.Enums;
using Microsoft.AspNetCore.SignalR.Protocol;
using Server.Models;

namespace GrpcService.Services
{
    /// <summary>
    /// gRPC сервис для отправки необходимых координат.
    /// </summary>
    public class GreeterService : Greeter.GreeterBase
    {
        #region Private Fields

        /// <summary>
        /// Максимальное количество прямоугольников.
        /// </summary>
        private const int MAX_COUNT_RECTANGLES = 1000;

        /// <summary>
        /// Список со всеми пользователями-клиентами.
        /// </summary>
        private static List<int> _usersList = new();

        /// <summary>
        /// Логгер для записи исключений.
        /// </summary>
        private readonly ILogger<GreeterService> _logger;

        /// <summary>
        /// Список всех прямоугольников.
        /// </summary>
        private List<RectangleReply> _rectanglesArray = new(MAX_COUNT_RECTANGLES);

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Инициализирует экземпляр класса <see cref="GreeterService"/>.
        /// </summary>
        /// <param name="logger">Логгер для сервиса.</param>
        public GreeterService(ILogger<GreeterService> logger)
        {
            _logger = logger;
        }

        #endregion Public Constructors

        #region Public Methods

        /// <summary>
        /// Производит подключение к серверу.
        /// </summary>
        /// <param name="request">Запрос на подключение.</param>
        /// <param name="context"><inheritdoc/></param>
        /// <returns>Экземпляр класса <see cref="ConnectionReply"/>, который является ответом сервера.</returns>
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
        /// Отправляет экземпляр класса <see cref="RectangleReply"/> клиенту.
        /// </summary>
        /// <param name="empty">Пустой запрос.</param>
        /// <param name="responseStream">Поток данных, в котором хранятся отпраляемые <see cref="RectangleReply"/>.</param>
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