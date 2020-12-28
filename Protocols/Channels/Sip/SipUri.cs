namespace Protocols.Channels.Sip
{
    using System;

    public class SipUri
    {
        #region Constants

        private const string START = "sip:";
        private const string CARRAGE_L = "<";
        private const string CARRAGE_R = ">";
        private const string TAG = ";tag=";
        private const string SEPARATOR = "@";

        #endregion Constants

        #region Properties

        public string Address { get; private set; }

        public string Domain { get; private set;  }

        public string Tag { get; private set; }

        #endregion Properties

        #region Constructors

        internal SipUri() { }

        public SipUri(string address, string domain, string tag = null)
        {
            Address = address;
            Domain = domain;
            Tag = tag ?? string.Empty;
        }

        #endregion Constructors

        #region Methods

        public string Pack(bool isCarriage)
        {
            string uri = $"{START}{Address}{SEPARATOR}{Domain}";

            if (isCarriage)
                uri = $"<{uri}>";

            if (!string.IsNullOrWhiteSpace(Tag))
                uri += $"{TAG}{Tag}";

            return uri;
        }

        // TODO "asterisk" <sip:asterisk@192.168.56.105> -> Aliase <User@Host:Port>
        public static SipUri Parse(string uid)
        {
            int offset = uid.IndexOf(START);
            if (offset == -1)
                throw new ArgumentException();

            var uri = new SipUri();

            string[] items = uid
                .Substring(offset)
                .Replace(START, string.Empty)
                .Replace(CARRAGE_L, string.Empty)
                .Replace(CARRAGE_R, string.Empty)
                .Split(TAG, 2, StringSplitOptions.RemoveEmptyEntries);

            if (items.Length == 2)
            {
                uri.Tag = items[1];
            }

            //items = items[0].Split(SPACE, 2, StringSplitOptions.RemoveEmptyEntries);
            items = items[0].Split(SEPARATOR, 2, StringSplitOptions.RemoveEmptyEntries);
            if (items.Length != 2)
                throw new ArgumentException();

            uri.Address = items[0];
            uri.Domain = items[1];

            return uri;
        }

        #endregion Methods
    }
}
