namespace Protocols.Channels.Sdp
{
    using System;

    public class SdpTiming
    {
        #region Constants

        public const string SEPARATOR = " ";

        #endregion Constants

        #region Properties

        public ulong Start { get; set; }

        public ulong Stop { get; set; }

        #endregion Properties

        #region Methods

        public string Pack() 
        {
            return $"{Start}{SEPARATOR}{Stop}";
        }

        public static SdpTiming Parse(string line) 
        {
            var tokens = line.Split(SEPARATOR, StringSplitOptions.RemoveEmptyEntries);
            if (tokens.Length != 2)
            {
                throw new ArgumentException();
            }

            return new SdpTiming
            {
                Start = ulong.Parse(tokens[0]),
                Stop = ulong.Parse(tokens[1]),
            };
        }

        #endregion Methods
    }
}
