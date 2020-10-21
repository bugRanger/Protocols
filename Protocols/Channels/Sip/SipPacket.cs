namespace Protocols.Channels.Sip
{
    using System;
    using System.Text;
    using System.Collections.Generic;

    using Framework.Common;

    using Protocols.Extensions;

    // ==============================================================
    //          https://tools.ietf.org/html/rfc3261
    // ==============================================================
    //
    //        Alice's  . . . . . . . . . . . . . . . . . . . .  Bob's
    //      softphone                                        SIP Phone
    //         |                |                |                |
    //         |    INVITE F1   |                |                |
    //         |--------------->|    INVITE F2   |                |
    //         |  100 Trying F3 |--------------->|    INVITE F4   |
    //         |<---------------|  100 Trying F5 |--------------->|
    //         |                |<-------------- | 180 Ringing F6 |
    //         |                | 180 Ringing F7 |<---------------|
    //         | 180 Ringing F8 |<---------------|     200 OK F9  |
    //         |<---------------|    200 OK F10  |<---------------|
    //         |    200 OK F11  |<---------------|                |
    //         |<---------------|                |                |
    //         |                       ACK F12                    |
    //         |------------------------------------------------->|
    //         |                   Media Session                  |
    //         |<================================================>|
    //         |                       BYE F13                    |
    //         |<-------------------------------------------------|
    //         |                     200 OK F14                   |
    //         |------------------------------------------------->|
    //         |                                                  |
    //
    // ==============================================================



    // OPTIONS sip:carol @chicago.com SIP/2.0
    // Via: SIP/2.0/UDP pc33.atlanta.com; branch=z9hG4bKhjhs8ass877
    // Max-Forwards: 70
    // To: <sip:carol @chicago.com>
    // From: Alice<sip:alice@atlanta.com>; tag=1928301774
    // Call-ID: a84b4c76e66710
    // CSeq: 63104 OPTIONS
    // Contact: <sip:alice @pc33.atlanta.com>
    // Accept: application/sdp
    // Content-Length: 0


    // INVITE sip:bob @biloxi.com SIP/2.0
    // Via: SIP/2.0/UDP pc33.atlanta.com; branch=z9hG4bK776asdhds
    // Max-Forwards: 70
    // To: Bob<sip:bob@biloxi.com>
    // From: Alice<sip:alice@atlanta.com>;tag=1928301774
    // Call-ID: a84b4c76e66710 @pc33.atlanta.com
    // CSeq: 314159 INVITE
    // Contact: <sip:alice @pc33.atlanta.com>
    // Content-Type: application/sdp
    // Content-Length: 142


    // SIP/2.0 100 Trying
    // Via: SIP/2.0/UDP pc33.atlanta.com; branch=z9hG4bKnashds8;received=192.0.2.1
    // To: Bob<sip:bob@biloxi.com>
    // From: Alice<sip:alice@atlanta.com>;tag=1928301774
    // Call-ID: a84b4c76e66710
    // CSeq: 314159 INVITE
    // Content-Length: 0

    // SIP/2.0 200 OK
    // Via: SIP/2.0/UDP server10.biloxi.com; branch=z9hG4bKnashds8;received=192.0.2.3
    // Via: SIP/2.0/UDP bigbox3.site3.atlanta.com; branch=z9hG4bK77ef4c2312983.1;received=192.0.2.2
    // Via: SIP/2.0/UDP pc33.atlanta.com; branch=z9hG4bK776asdhds ;received=192.0.2.1
    // To: Bob<sip:bob@biloxi.com>;tag=a6c85cf
    // From: Alice<sip:alice@atlanta.com>;tag=1928301774
    // Call-ID: a84b4c76e66710 @pc33.atlanta.com
    // CSeq: 314159 INVITE
    // Contact: <sip:bob@192.0.2.4>
    // Content-Type: application/sdp
    // Content-Length: 131

    // HINT> Use Content-Length: N for concat udp message.
    // =========================================================
    // <MESSAGE_TYPE>
    // =========================================================
    //  Informational  = 
    //    /   "100"  ;  Trying
    //    /   "180"  ;  Ringing
    //    /   "181"  ;  Call Is Being Forwarded
    //    /   "182"  ;  Queued
    //    /   "183"  ;  Session Progress
    //  Success  =  
    //    /   "200"  ;  OK
    //
    //  Redirection = 
    //    /   "300"  ; Multiple Choices
    //    /   "301"  ;  Moved Permanently
    //    /   "302"  ;  Moved Temporarily
    //    /   "305"  ;  Use Proxy
    //    /   "380"  ;  Alternative Service
    //
    //  Client-Error  =  
    //    /   "400"  ;  Bad Request
    //    /   "401"  ;  Unauthorized
    //    /   "402"  ;  Payment Required
    //    /   "403"  ;  Forbidden
    //    /   "404"  ;  Not Found
    //    /   "405"  ;  Method Not Allowed
    //    /   "406"  ;  Not Acceptable
    //    /   "407"  ;  Proxy Authentication Required
    //    /   "408"  ;  Request Timeout
    //    /   "410"  ;  Gone
    //    /   "413"  ;  Request Entity Too Large
    //    /   "414"  ;  Request-URI Too Large
    //    /   "415"  ;  Unsupported Media Type
    //    /   "416"  ;  Unsupported URI Scheme
    //    /   "420"  ;  Bad Extension
    //    /   "421"  ;  Extension Required
    //    /   "423"  ;  Interval Too Brief
    //    /   "480"  ;  Temporarily not available
    //    /   "481"  ;  Call Leg/Transaction Does Not Exist
    //    /   "482"  ;  Loop Detected
    //    /   "483"  ;  Too Many Hops
    //    /   "484"  ;  Address Incomplete
    //    /   "485"  ;  Ambiguous
    //    /   "486"  ;  Busy Here
    //    /   "487"  ;  Request Terminated
    //    /   "488"  ;  Not Acceptable Here
    //    /   "491"  ;  Request Pending
    //    /   "493"  ;  Undecipherable
    //
    //  Server-Error  =  
    //    /   "500"  ;  Internal Server Error
    //    /   "501"  ;  Not Implemented
    //    /   "502"  ;  Bad Gateway
    //    /   "503"  ;  Service Unavailable
    //    /   "504"  ;  Server Time-out
    //    /   "505"  ;  SIP Version not supported
    //    /   "513"  ;  Message Too Large
    //
    //  Global-Failure  =  
    //    /   "600"  ;  Busy Everywhere
    //    /   "603"  ;  Decline
    //    /   "604"  ;  Does not exist anywhere
    //    /   "606"  ;  Not Acceptable
    //
    // =========================================================
    // <MESSAGE_HEADER>
    // =========================================================
    //
    //    /  Accept
    //    /  Accept-Encoding
    //    /  Accept-Language
    //    /  Alert-Info
    //    /  Allow
    //    /  Authentication-Info
    //    /  Authorization
    //    /  Call-ID
    //    /  Call-Info
    //    /  Contact
    //    /  Content-Disposition
    //    /  Content-Encoding
    //    /  Content-Language
    //    /  Content-Length
    //    /  Content-Type
    //    /  CSeq
    //    /  Date
    //    /  Error-Info
    //    /  Expires
    //    /  From
    //    /  In-Reply-To
    //    /  Max-Forwards
    //    /  MIME-Version
    //    /  Min-Expires
    //    /  Organization
    //    /  Priority
    //    /  Proxy-Authenticate
    //    /  Proxy-Authorization
    //    /  Proxy-Require
    //    /  Record-Route
    //    /  Reply-To
    //    /  Require
    //    /  Retry-After
    //    /  Route
    //    /  Server
    //    /  Subject
    //    /  Supported
    //    /  Timestamp
    //    /  To
    //    /  Unsupported
    //    /  User-Agent
    //    /  Via
    //    /  Warning
    //    /  WWW-Authenticate
    //    /  extension-header
    //
    // =========================================================
    // <MESSAGE_BODY>
    // =========================================================

    public class SipPacket : IPacket
    {
        #region Classes

        private class Property
        {
            #region Properties

            public Func<SipPacket, string> Get { get; }

            public Action<SipPacket, string> Set { get; }

            public Func<SipPacket, bool> Available { get; }

            #endregion Properties

            #region Constructors

            public Property(Func<SipPacket, string> getter, Action<SipPacket, string> settter) : this(getter, settter, (packet) => true) { }

            public Property(Func<SipPacket, string> getter, Action<SipPacket, string> settter, Func<SipPacket, bool> available) 
            {
                Get = getter;
                Set = settter;
                Available = available;
            }

            #endregion Constructors
        }

        #endregion Classes

        #region Constants

        private const string SPACE = " ";
        private const string CRLF = "\r\n";
        private const string SEPARATOR = ":";
        private const string VERSION = "SIP/2.0";

        private static Encoding Encoding = Encoding.ASCII;
        private static readonly Dictionary<string, Property> _properties;

        #endregion Constants

        #region Fields

        private List<string> _cached;

        #endregion Fields

        #region Property

        public SipMethod Method { get; set; }

        public SipStatus? Status { get; set; }
        
        public SipUri Request { get; set; }

        public SipUri From { get; set; }

        public SipUri To { get; set; }

        public SipUri Concact { get; set; }

        public string CallId { get; set; }

        public int CSeq { get; set; }

        public string ContentType { get; private set; }

        public int ContentLength { get; private set; }

        public ArraySegment<byte> Content { get; private set; }

        #endregion Property

        #region Constructors

        static SipPacket()
        {
            // TODO: Impl others attributes.
            _properties = new Dictionary<string, Property>(StringComparer.InvariantCultureIgnoreCase)
            {
                { "From", new Property((packet) => packet.From.Pack(true), (packet, value) => packet.From = SipUri.Parse(value)) },
                { "To", new Property((packet) => packet.To.Pack(true), (packet, value) => packet.To = SipUri.Parse(value)) },
                { "CSeq", new Property(CSeqGdetter, CSeqSetter) },
                { "Call-ID", new Property((packet) => packet.CallId, (packet, value) => packet.CallId = value) },
                { "Contact", new Property((packet) => packet.Concact.Pack(true), (packet, value) => packet.Concact = SipUri.Parse(value), (packet) => packet.Method == SipMethod.INVITE) },
                { "Content-Type", new Property((packet) => packet.ContentType, (packet, value) => packet.ContentType = value, (packet) => packet.ContentLength > 0) },
                { "Content-Length", new Property((packet) => packet.ContentLength.ToString(), (packet, value) => packet.ContentLength = int.Parse(value)) },
            };
        }

        public SipPacket() 
        {
            _cached = new List<string>();
        }

        #endregion Constructors

        #region Methods

        public SipPacket SetContent(string type, ArraySegment<byte> content)
        {
            ContentType = type;
            ContentLength = content.Count;
            Content = content;

            return this;
        }

        public bool TryUnpack(byte[] buffer, ref int offset)
        {
            /* Rfc 2822 2.2 Header Fields
				Header fields are lines composed of a field name, followed by a colon
				(":"), followed by a field body, and terminated by CRLF.  A field
				name MUST be composed of printable US-ASCII characters (i.e.,
				characters that have values between 33 and 126, inclusive), except
				colon.  A field body may be composed of any US-ASCII characters,
				except for CR and LF.  However, a field body may contain CRLF when
				used in header "folding" and  "unfolding" as described in section
				2.2.3.  All field bodies MUST conform to the syntax described in
				sections 3 and 4 of this standard. 
				
			   Rfc 2822 2.2.3 Long Header Fields
				The process of moving from this folded multiple-line representation
				of a header field to its single line representation is called
				"unfolding". Unfolding is accomplished by simply removing any CRLF
				that is immediately followed by WSP.  Each header field should be
				treated in its unfolded form for further syntactic and semantic
				evaluation.
				
				Example:
					Subject: aaaaa<CRLF>
					<TAB or SP>aaaaa<CRLF>
			*/

            var tmpOffset = offset;
            var count = buffer.Length - tmpOffset;
            if (count <= 0)
                return false;

            string[] message = Encoding.GetString(buffer, tmpOffset, count).Split(CRLF, StringSplitOptions.RemoveEmptyEntries);

            if (message.Length <= 1)
                return false;

            if (!TryStartLineParse(message[0], ref tmpOffset))
                return false;

            if (!TryHeaderParse(message, ref tmpOffset))
                return false;

            if (ContentLength > 0)
            {
                if (ContentLength > count - tmpOffset)
                    return false;

                Content = new ArraySegment<byte>(buffer, tmpOffset, ContentLength);
                tmpOffset += ContentLength;
            }

            offset = tmpOffset;
            return true;
        }

        public void Pack(ref byte[] buffer, ref int offset)
        {
            byte[] message = Encoding.GetBytes(StartLineBuild() + HeaderBuild());

            BufferBits.Prepare(ref buffer, offset, (message.Length + ContentLength));

            BufferBits.SetBytes(message, buffer, ref offset);
            if (ContentLength > 0)
                BufferBits.SetBytes(Content.ToArray(), buffer, ref offset);
        }

        public ArraySegment<byte> Pack()
        {
            var buffer = new byte[0];
            var offset = 0;

            Pack(ref buffer, ref offset);
            return new ArraySegment<byte>(buffer, 0, offset);
        }

        private string StartLineBuild()
        {
            var result = string.Empty;

            if (Status != null)
            {
                result = $"{VERSION} {(int)Status.Value} {((SipStatus)Status).GetDescription()}";
            }
            else
            {
                result = $"{Method} {Request.Pack(false)} {VERSION}";
            }

            return result + SPACE + CRLF;
        }

        private string HeaderBuild()
        {
            string header = string.Empty;

            foreach (var item in _cached)
                header += item + CRLF;

            foreach (KeyValuePair<string, Property> item in _properties)
                if (item.Value.Available(this))
                    header += $"{item.Key}: {item.Value.Get(this)}" + SPACE + CRLF;

            return header + SPACE + CRLF;
        }

        private bool TryStartLineParse(string message, ref int offset)
        {
            string[] items = message.TrimEnd().Split(SPACE, 3, StringSplitOptions.RemoveEmptyEntries);
            if (items.Length != 3)
                return false;

            if (items[0] == VERSION && int.TryParse(items[^2], out var status) && Enum.IsDefined(typeof(SipStatus), status))
            {
                Status = (SipStatus)status;
            }
            else if (items[^1] == VERSION && Enum.TryParse(items[0], true, out SipMethod method))
            {
                Request = SipUri.Parse(items[1]);
                Method = method;
            }
            else
            {
                return false;
            }

            offset += Encoding.GetByteCount(message + CRLF);

            return true;
        }

        private bool TryHeaderParse(string[] message, ref int offset)
        {
            _cached.Clear();

            for (int i = 1; i < message.Length; i++)
            {
                offset += Encoding.GetByteCount(message[i] + CRLF);

                if (message[i] == SPACE)
                    return true;

                var items = message[i].TrimEnd().Split(SEPARATOR, 2, StringSplitOptions.RemoveEmptyEntries);
                if (items.Length != 2)
                    return false;

                if (!_properties.TryGetValue(items[0], out var property))
                {
                    _cached.Add(message[i]);
                    continue;
                }

                try
                {
                    property.Set(this, items[1].TrimStart());
                }
                catch
                {
                    // TODO Handle error.
                    //throw new ArgumentException(items[0], ex);
                }
            }

            return false;
        }

        private static void CSeqSetter(SipPacket packet, string value)
        {
            string[] items = value.Split(SPACE, 2, StringSplitOptions.RemoveEmptyEntries);
            if (items.Length != 2)
                throw new ArgumentException();

            if (!int.TryParse(items[0], out var cseq))
                throw new ArgumentException();

            Enum.TryParse<SipMethod>(items[1], out var method);

            packet.Method = method;
            packet.CSeq = cseq;
        }

        private static string CSeqGdetter(SipPacket packet)
        {
            return packet.CSeq + SPACE + packet.Method;
        }

        #endregion Methods
    }
}
