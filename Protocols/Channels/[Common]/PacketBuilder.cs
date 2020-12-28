namespace Protocols.Channels
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public class PacketBuilder<T> : IEnumerable<PacketProperty<T>> where T : IBuildPacket
    {
        public const string CRLF = "\r\n";

        #region Fields

        private readonly Dictionary<string, PacketProperty<T>> _container;
        private readonly string _delimiter;

        #endregion Fields

        #region Constructors

        public PacketBuilder(string delimiter = CRLF) 
        {
            _delimiter = delimiter;
            _container = new Dictionary<string, PacketProperty<T>>(StringComparer.InvariantCultureIgnoreCase);
        }

        #endregion Constructors

        #region Methods

        public bool TryGetValue(string name, out PacketProperty<T> property)
        {
            return _container.TryGetValue(name, out property);
        }

        public void Build(T packet, Action<string, string> prepare) 
        {
            var added = new HashSet<string>();
            foreach (PacketProperty<T> item in _container.Values)
            {
                if (added.Add(item.Name) && item.Available(packet))
                {
                    var property = packet.UseCompact && item.HasCompact ? item.CompactName : item.Name;
                    var values = item.Get(packet).Split(_delimiter);

                    foreach (var value in values)
                    {
                        prepare(property, value);
                    }
                }
            }
        }

        public IEnumerator<PacketProperty<T>> GetEnumerator()
        {
            return _container.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(PacketProperty<T> property)
        {
            _container[property.Name] = property;

            if (property.HasCompact)
                _container[property.CompactName] = property;
        }

        #endregion Methods
    }
}
