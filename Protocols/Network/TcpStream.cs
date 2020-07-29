namespace Protocols.Network
{
    using System;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading;
    using System.Threading.Tasks;

    using Framework.Common;

    public class TcpStream : ITcpStream
    {
        #region Fields

        private readonly Locker _locker;
        private readonly IPEndPoint _local;
        private readonly IPEndPoint _remote;
        private TcpClient _client;
        private NetworkStream _stream;
        private bool _disposed;

        #endregion Fields

        #region Constructors

        public TcpStream(IPEndPoint local, IPEndPoint remote = null) 
        {
            _locker = new Locker();
            
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
            if (!_locker.SetEnabled())
            {
                return _stream;
            }
            
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

            return _stream = _client.GetStream();
        }

        public void Close()
        {
            if (!_locker.SetDisabled())
            {
                return;
            }
                
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
