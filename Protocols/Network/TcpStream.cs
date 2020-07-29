namespace Protocols.Network
{
    using System;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading;
    using System.Threading.Tasks;

    public class TcpStream : ITcpStream
    {
        #region Fields

        private readonly IPEndPoint _local;
        private readonly IPEndPoint _remote;
        private TcpClient _client;
        private bool _disposed;

        #endregion Fields

        #region Constructors

        public TcpStream(IPEndPoint local, IPEndPoint remote = null) 
        {
            _local = local;
            _remote = remote;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~TcpStream()
        {
            Dispose(false);
        }

        #endregion Constructors

        #region Methods

        public async Task<NetworkStream> Open(CancellationToken token)
        {
            _client = new TcpClient(_local);
            token.Register(() => Close());

            if (_remote != null)
            {
                await _client.ConnectAsync(_remote.Address, _remote.Port);
            }
            else
            {
                _client.Client.Listen(1);
                _client.Client = await _client.Client.AcceptAsync();
            }

            return _client.GetStream();
        }

        public void Close()
        {
            _client.Close();
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

        protected void FreeClient()
        {
            TcpClient client = _client;
            if (client == null)
                return;

            _client = null;

            client.Close();
            client.Dispose();
        }

        #endregion Methods
    }
}
