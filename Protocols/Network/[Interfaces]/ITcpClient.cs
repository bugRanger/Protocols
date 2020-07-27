namespace Protocols.Network
{
    using System;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading;
    using System.Threading.Tasks;

    public interface ITcpClient : IDisposable
    {
        Task<NetworkStream> Open(CancellationToken token);

        void Close();
    }
}
