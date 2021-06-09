namespace Protocols.Channels
{
    using System;
    using System.Linq;

    public class PacketGroup<T> : PacketItem<T> where T : Packet
    {
        #region Fields

        public readonly PacketBuilder<T> _builder;

        #endregion Fields

        #region Constructors

        public PacketGroup(params PacketItem<T>[] properties)
        {
            _builder = new PacketBuilder<T>(properties.Length);
            foreach (var item in properties)
            {
                _builder.Add(item);
            }

            HasOrdered = true;

            Set = (p, v) => _builder.Unpack(p, v);
            Get = (p) => _builder.Pack(p);
            GetAvailable = (p) => _builder.Any(a => a.GetAvailable(p));
        }

        #endregion Constructors

        #region Methods

        public PacketGroup<T> SetBuilder(Action<PacketBuilder<T>> prepare)
        {
            prepare(_builder);
            return this;
        }

        #endregion Methods
    }
}
