namespace Protocols.Network
{
    using System;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading;
    using System.Threading.Tasks;

    using NLog;
    using Protocols.Utils;

    public delegate ITcpClient TcpClientFactory(IPEndPoint local, IPEndPoint remote);

    public class TcpProvider : ITcpProvider, IDisposable
    {
        #region Constants

        private const int DISABLED = 0;

        private const int ENABLED = 1;

        private const int HEADER_SIZE = 2;

        private const int PACKET_SIZE = short.MaxValue;

        #endregion Constants

        #region Fields

        private readonly ILogger _logger;
        private readonly TcpClientFactory _clientFactory;
        private readonly byte[] _buffer;

        private ITcpClient _client;
        private NetworkStream _stream;
        private CancellationTokenSource _cancellation;
        private bool _disposed;
        private int _state;

        #endregion Fields

        #region Properties

        public IPEndPoint Remote { get; private set; }

        public IPEndPoint Local { get; private set; }

        public bool IsEnabled => Interlocked.CompareExchange(ref _state, ENABLED, ENABLED) == ENABLED;

        #endregion Properties

        #region Events

        public event EventHandler Opened;
        public event EventHandler Closed;
        public event EventHandler<ReceiveData> ReceiveData;

        #endregion Events

        #region Constructors/Destructors

        public TcpProvider(TcpClientFactory clientFactory)
        {
            _logger = LogManager.GetCurrentClassLogger();
            _clientFactory = clientFactory;
            _buffer = new byte[PACKET_SIZE];
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~TcpProvider()
        {
            Dispose(false);
        }

        #endregion Constructors/Destructors

        #region Methods

        public async Task StartAsync(IPEndPoint local, IPEndPoint remote = null)
        {
            if (Interlocked.CompareExchange(ref _state, ENABLED, DISABLED) == ENABLED)
            {
                return;
            }

            _cancellation = new CancellationTokenSource();
            _client = _clientFactory?.Invoke(Local = local, Remote = remote);
            _stream = await _client.Open(_cancellation.Token);

            var listener = Task
                .Run(ReadStream, _cancellation.Token)
                .ContinueWith((task, o) => 
                {
                    if (!task.IsCanceled)
                        Stop();
                }, 
                null);

            // TODO Перенести на события ITcpClient.
            Opened?.Invoke(this, new EventArgs());
            await listener;
        }

        public void Stop()
        {
            if (Interlocked.CompareExchange(ref _state, DISABLED, ENABLED) == DISABLED)
            {
                return;
            }

            FreeCancellationToken();
            FreeClient();

            // TODO Перенести на события ITcpClient.
            Closed?.Invoke(this, new EventArgs());
        }

        public async Task Send(byte[] bytes)
        {
            try
            {
                await _stream.WriteAsync(bytes, 0, 0, _cancellation.Token);
            }
            finally
            {
                _logger.Trace(() => $"send packet: {bytes}");
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                FreeClient();
            }

            _disposed = true;
        }

        private async Task ReadStream()
        {
            int offset = 0;

            while (_cancellation.IsCancellationRequested)
            {
                try
                {
                    int size = PACKET_SIZE - offset;

                    int bytes = await _stream.ReadAsync(_buffer, offset, size, _cancellation.Token);
                    if (bytes == 0)
                    {
                        _logger.Error($"socket stream closed");
                        break;
                    }

                    offset += bytes;

                    while (bytes >= HEADER_SIZE)
                    {
                        int dataSize = BufferBits.GetUInt16(_buffer, 0);
                        if (bytes < HEADER_SIZE + dataSize)
                        {
                            break;
                        }

                        int blockSize = HEADER_SIZE + dataSize;
                        byte[] data = new byte[dataSize];
                        Buffer.BlockCopy(_buffer, HEADER_SIZE, data, 0, dataSize);

                        offset = 0;

                        ReceiveData?.Invoke(this, new ReceiveData(Local, Remote, data));
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error($"read error: {ex}");
                    break;
                }
            }
        }

        private void FreeClient()
        {
            ITcpClient client = _client;
            if (client == null)
                return;

            _client = null;

            client.Close();
            client.Dispose();

            _logger.Trace("free connect tcp");
        }

        private void FreeCancellationToken()
        {
            CancellationTokenSource cancellation = _cancellation;
            if (cancellation == null)
                return;

            _cancellation = null;

            cancellation.Cancel();
            cancellation.Dispose();
        }

        #endregion Methods
    }
}
