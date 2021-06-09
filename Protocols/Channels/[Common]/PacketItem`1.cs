using System;

namespace Protocols.Channels
{
    public delegate bool Available<T>(T packet) where T : Packet;
    public delegate string Getter<T>(T packet) where T : Packet;
    public delegate void Setter<T>(T packet, string value) where T : Packet;

    public class PacketItem<T> where T : Packet
    {
        #region Properties

        public string Name { get; set; }

        public string CompactName { get; set; }

        public bool HasOrdered { get; set; }

        public bool HasConstant { get; set; }

        public bool HasCompact => !string.IsNullOrWhiteSpace(CompactName);

        public Getter<T> Get { get; set; }

        public Setter<T> Set { get; set; }

        public Available<T> GetAvailable { get; set; }

        #endregion Properties

        #region Constructors

        public PacketItem() { }

        public PacketItem(Getter<T> getter, Setter<T> settter = null, Available<T> available = null)
        {
            Get = getter;
            Set = (packet, value) => settter?.Invoke(packet, value);
            GetAvailable = (packet) => available?.Invoke(packet) ?? true;
        }

        public PacketItem(string name, string compact, Getter<T> getter, Setter<T> settter, Available<T> available = null)
            : this(name, getter, settter, available)
        {
            CompactName = compact;
        }

        public PacketItem(string name, Getter<T> getter, Setter<T> settter, Available<T> available = null)
            : this(getter, settter, available)
        {
            Name = name;
        }

        public string GetName(T packet)
        {
            return packet.UseCompact && HasCompact ? CompactName : Name;
        }

        #endregion Constructors
    }
}
