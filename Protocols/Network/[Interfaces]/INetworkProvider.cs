namespace Protocols.Network
{
    using System;
    using System.Net;
    using System.Threading.Tasks;

    public interface INetworkProvider
    {
        #region Properties

        IPEndPoint Remote { get; }

        IPEndPoint Local { get; }

        #endregion Properties

        #region Events

        event EventHandler Opened;

        event EventHandler Closed;

        event EventHandler<ReceiveData> ReceiveData;

        #endregion Events

        #region Methods

        Task StartAsync(IPEndPoint local, IPEndPoint remote = null);

        void Stop();

        Task Send(byte[] bytes);

        #endregion Methods
    }
}
