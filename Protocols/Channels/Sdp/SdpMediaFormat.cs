// ---------------------------------------------------------------------------------------------------------------------------------------------------
// Copyright ElcomPlus LLC. All rights reserved.
// Author: Work
// ---------------------------------------------------------------------------------------------------------------------------------------------------

namespace Protocols.Channels.Sdp
{
    public class SdpMediaFormat
    {
        #region Properties

        public byte PayloadType { get; set; }

        public string EncodingName { get; set; }

        public string ClockRate { get; set; }

        public string[] EncodingParams { get; set; }

        #endregion Properties
    }
}
