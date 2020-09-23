namespace Protocols.Channels.Sip
{
    using System;

    public class SipUri
    {
        #region Constants

        private const string START = "sip:";
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

        // TODO "asterisk" <sip:asterisk@192.168.56.105> -> Aliase <User@Host:Port>
        public static SipUri Parse(string uid) 
        {
            int offset = uid.IndexOf(START);
            if (offset == -1)
                throw new ArgumentException();

            var uri = new SipUri();

            string[] items = uid.Substring(offset).Split(TAG, 2, StringSplitOptions.RemoveEmptyEntries);
            if (items.Length == 2)
            {
                uri.Tag = items[1];
            }

            //items = items[0].Split(SPACE, 2, StringSplitOptions.RemoveEmptyEntries);
            items = items[0].Split(SEPARATOR, 2, StringSplitOptions.RemoveEmptyEntries);
            if(items.Length != 2)
                throw new ArgumentException();

            uri.Address = items[0].Replace(START, string.Empty);
            uri.Domain = items[1][0..^1];

            return uri;
        }

        public override string ToString()
        {
            // TODO Aliace name.
            return $"{START}{Address}{SEPARATOR}{Domain}{(!string.IsNullOrWhiteSpace(Tag) ? $"{TAG}{Tag}" : string.Empty)}";
        }

        #endregion Methods
    }
}
