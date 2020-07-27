namespace Protocols.Network
{
    using System;
    using System.Collections.Generic;
    using System.Net;

    public class ReceiveData
    {
        public IPEndPoint Local { get; }

        public IPEndPoint Remote { get; }

        public IReadOnlyCollection<byte> Data { get; }

        internal ReceiveData(IPEndPoint local, IPEndPoint remote, byte[] data)
        {
            Data = data;
            Local = local;
            Remote = Remote;
        }
    }
}
