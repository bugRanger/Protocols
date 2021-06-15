namespace Protocols.Channels
{
    using System;
    using System.Linq;
    using System.Collections;
    using System.Collections.Generic;

    public class PacketGroup<T> : PacketItem<T>, IEnumerable<PacketItem<T>> where T : Packet
    {
        #region Properties

        public PacketBuilder<T> Builder { get; }

        #endregion Properties

        #region Constructors

        public PacketGroup(string name, string compact, int capacity) : this(name, capacity)
        {
            CompactName = compact;
        }

        public PacketGroup(string name, int capacity) : this(capacity)
        {
            Name = name;
        }

        public PacketGroup(int capacity)
        {
            Builder = new PacketBuilder<T>(capacity);

            Set = (p, v) => Builder.Unpack(p, v);
            Get = (p) => Builder.Pack(p);
            GetAvailable = (p) => Builder.Any(a => a.GetAvailable(p));
        }

        #endregion Constructors

        #region Methods

        public PacketGroup<T> SetParam(Action<PacketGroup<T>> prepare)
        {
            prepare(this);
            return this;
        }

        public void Add(PacketItem<T> item)
        {
            Builder.Add(item);
        }

        public IEnumerator<PacketItem<T>> GetEnumerator()
        {
            return Builder.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion Methods
    }
}
