namespace Protocols.Channels
{
    using System;

    public delegate string Getter<T>(T packet) where T : IBuildPacket;
    public delegate void Setter<T>(T packet, string value) where T : IBuildPacket;
    public delegate bool Available<T>(T packet) where T : IBuildPacket;

    public class PacketProperty<T> where T: IBuildPacket
    {
        #region Properties

        public string Name { get; }

        public string CompactName { get; }

        public Getter<T> Get { get; }

        public Setter<T> Set { get; }

        public Available<T> Available { get; }

        public bool HasCompact { get; }

        #endregion Properties

        #region Constructors

        public PacketProperty(string name, string compact, Getter<T> getter, Setter<T> settter, Available<T> available = null)
            : this(name, getter, settter, available)
        {
            CompactName = compact;
            HasCompact = !string.IsNullOrWhiteSpace(CompactName);
        }

        public PacketProperty(string name, Getter<T> getter, Setter<T> settter, Available<T> available = null)
        {
            Name = name;
            Get = getter;
            Set = settter;
            Available = (packet) => available?.Invoke(packet) ?? true;
        }

        #endregion Constructors
    }
}
