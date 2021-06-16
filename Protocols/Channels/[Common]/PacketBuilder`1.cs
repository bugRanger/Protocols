namespace Protocols.Channels
{
    using System;
    using System.Text;
    using System.Collections;
    using System.Collections.Generic;

    using Framework.Common;

    public class PacketBuilder<T> : IEnumerable<PacketItem<T>> where T : Packet
    {
        #region Fields

        private readonly List<PacketItem<T>> _headers;
        private readonly Dictionary<string, PacketItem<T>> _fields;

        #endregion Fields

        #region Properties

        public Encoding Encoding { get; internal set; }

        public string Equal { get; set; }

        public string Separator { get; set; }

        public bool TrailingSeparator { get; set; }

        #endregion Properties

        #region Constructors

        public PacketBuilder(int capacity)
        {
            _headers = new List<PacketItem<T>>(capacity);
            _fields = new Dictionary<string, PacketItem<T>>(capacity);
        }

        #endregion Constructors

        #region Methods

        public void Add(PacketGroup<T> item)
        {
            item.SetParam(p => p.Builder.Encoding = Encoding);

            AddImpl(item);
        }

        public void Add(PacketItem<T> item)
        {
            AddImpl(item);
        }

        public void Pack(T packet, ref byte[] buffer, ref int offset, int extraBytes = 0)
        {
            byte[] message = Encoding.GetBytes(Pack(packet));

            BufferBits.Prepare(ref buffer, offset, message.Length + extraBytes);
            BufferBits.SetBytes(message, buffer, ref offset);
        }

        public string Pack(T packet)
        {
            var line = 1;
            var result = string.Empty;
            var separator = string.Empty;

            using var enumeration = GetItems(packet).GetEnumerator();

            foreach (KeyValuePair<int, string> unspecific in packet.UnspecificRows)
            {
                while (line < unspecific.Key)
                {
                    if (!enumeration.MoveNext())
                        break;

                    result += separator + enumeration.Current;
                    separator = Separator;
                    line++;
                }

                result += separator + unspecific.Value;
                separator = Separator;
                line++;
            }

            while (enumeration.MoveNext())
            {
                result += separator + enumeration.Current;
                separator = Separator;
            }

            result = TrailingSeparator
                ? result + separator + separator
                : result;

            return result;
        }

        public void Unpack(T packet, byte[] buffer, ref int offset, int count)
        {
            string message = Encoding.GetString(buffer, offset, count);

            Unpack(packet, message, ref offset, count);
        }

        public void Unpack(T packet, string message, ref int offset, int count)
        {
            int tmpOffset = offset;
            int index = 0;
            int row = 0;

            string line = null;

            packet.Unspecific.Clear();

            for (; tmpOffset < message.Length; row++)
            {
                index = message.IndexOf(Separator, tmpOffset);
                if (index == -1)
                {
                    if (TrailingSeparator)
                    {
                        throw new FormatException();
                    }

                    line = message[tmpOffset..message.Length];
                    tmpOffset = message.Length;
                }
                else
                {
                    line = message[tmpOffset..index];
                    tmpOffset = index + Separator.Length;
                }

                if (line == string.Empty)
                {
                    if (!TrailingSeparator)
                        throw new FormatException();

                    break;
                }

                if (row <= _headers.Count - 1)
                {
                    if (!_headers[row].HasConstant)
                    {
                        _headers[row].Set(packet, line);
                    }
                    else
                    {
                        if (_headers[row].Get(packet) != line)
                            throw new FormatException();
                    }

                    continue;
                }

                if (string.IsNullOrWhiteSpace(Equal))
                {
                    continue;
                }

                var tokens = line.Split(Equal, 2, StringSplitOptions.RemoveEmptyEntries);

                if (tokens.Length != 2)
                {
                    throw new FormatException();
                }

                var propertyName = tokens[0];
                var propertyValue = tokens[1];

                if (!_fields.TryGetValue(propertyName, out var attribute))
                {
                    packet.Unspecific[propertyName] = propertyValue;
                    packet.UnspecificRows[row] = line;
                    continue;
                }

                if (attribute.HasConstant && attribute.Get(packet) != propertyValue)
                {
                    throw new FormatException();
                }

                attribute.Set(packet, propertyValue);
            }

            if (TrailingSeparator)
            {
                if (line != string.Empty)
                {
                    throw new FormatException();
                }
            }
            else
            {
                if (tmpOffset != message.Length)
                {
                    throw new FormatException();
                }
            }

            offset = tmpOffset;
        }

        public void Unpack(T packet, string message)
        {
            int offset = 0;

            Unpack(packet, message, ref offset, message.Length);
        }

        public IEnumerable<string> GetItems(T packet)
        {
            if (_headers.Count > 0)
            {
                foreach (var header in _headers)
                {
                    if (header.GetAvailable(packet))
                    {
                        yield return header.Get(packet);
                    }
                }
            }

            var added = new HashSet<string>();
            foreach (PacketItem<T> field in _fields.Values)
            {
                if (added.Add(field.Name) && field.GetAvailable(packet))
                {
                    yield return field.GetName(packet) + Equal + field.Get(packet);
                }
            }
        }

        public IEnumerator<PacketItem<T>> GetEnumerator()
        {
            foreach (var item in _headers)
            {
                yield return item;
            }

            foreach (var item in _fields.Values)
            {
                yield return item;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private void AddImpl(PacketItem<T> item)
        {
            if (item.HasOrdered)
            {
                _headers.Add(item);
                return;
            }

            _fields[item.Name] = item;

            if (item.HasCompact)
            {
                _fields[item.CompactName] = item;
            }
        }

        #endregion Methods
    }
}