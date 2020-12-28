namespace Protocols.Tests
{
    using System;
    using System.Text;
    using System.Collections.Generic;

    using NUnit.Framework;

    using Protocols.Channels.Sip;

    public class SipMessageTests
    {
        #region Constants

        private const string TxInvite =
            "INVITE sip:115@192.168.56.105:5061 SIP/2.0 \r\n" +
            "Via: SIP/2.0/UDP 192.168.56.1:5061;branch=z9hG4bKPj3e715762683b4c95b5609f613dcee8bf \r\n" +
            "Max-Forwards: 70 \r\n" +
            "User-Agent: Claro BS6282 89F8 \r\n" +
            "Allow: INVITE,OPTIONS,CANCEL,ACK,BYE,PRACK,INFO,REFER,NOTIFY \r\n" +
            "Supported: replaces,100rel \r\n" +
            "From: <sip:115@192.168.56.105:5061>;tag=94b1fa2000614ec090b2c45af2d4cee1 \r\n" +
            "To: <sip:31337@192.168.56.105:5061> \r\n" +
            "Call-ID: c7e6ad40e49e44d4b665d621c4627149 \r\n" +
            "CSeq: 16564 INVITE \r\n" +
            "Contact: <sip:31337@192.168.56.105:5061> \r\n" +
            "Content-Type: application/sdp \r\n" +
            "Content-Length: 322 \r\n" +
            " \r\n" +
            "v=0 \r\n" +
            "o=root 1350070557 1350070557 IN IP4 192.168.56.105 \r\n" +
            "s=Asterisk PBX 11.7.0~dfsg-1ubuntu1 \r\n" +
            "c=IN IP4 192.168.56.105 \r\n" +
            "t=0 0 \r\n" +
            "m=audio 30004 RTP/AVP 8 18 2 0 120 \r\n" +
            "a=rtpmap:8 PCMA/8000 \r\n" +
            "a=rtpmap:18 G729/8000 \r\n" +
            "a=rtpmap:2 G726-32/8000 \r\n" +
            "a=rtpmap:0 PCMU/8000 \r\n" +
            "a=rtpmap:120 BSRtcp/8000 \r\n" +
            "a=fmtp:120 ver2 \r\n" +
            "a=sendrecv \r\n";

        private const string RxTrying =
            "SIP/2.0 100 Trying \r\n" +
            "Via: SIP/2.0/UDP 192.168.56.1:5061;branch=z9hG4bKPj3e715762683b4c95b5609f613dcee8bf;received=192.168.56.1 \r\n" +
            "From: <sip:115@192.168.56.105>;tag=94b1fa2000614ec090b2c45af2d4cee1 \r\n" +
            "To: <sip:31337@192.168.56.105> \r\n" +
            "Call-ID: c7e6ad40e49e44d4b665d621c4627149 \r\n" +
            "CSeq: 16564 INVITE \r\n" +
            "Server: Asterisk PBX 11.7.0~dfsg-1ubuntu1 \r\n" +
            "Allow: INVITE, ACK, CANCEL, OPTIONS, BYE, REFER, SUBSCRIBE, NOTIFY, INFO, PUBLISH \r\n" +
            "Supported: replaces, timer \r\n" +
            "Session-Expires: 1800;refresher=uas \r\n" +
            "Contact: <sip:31337@192.168.56.105:5060> \r\n" +
            "Content-Length: 0 \r\n" +
            " \r\n";

        private const string RxOk =
            "SIP/2.0 200 OK \r\n" +
            "Via: SIP/2.0/UDP 192.168.56.1:5061;branch=z9hG4bKPj3e715762683b4c95b5609f613dcee8bf;received=192.168.56.1 \r\n" +
            "From: <sip:115@192.168.56.105>;tag=94b1fa2000614ec090b2c45af2d4cee1 \r\n" +
            "To: <sip:31337@192.168.56.105>;tag=as56ee551e \r\n" +
            "Call-ID: c7e6ad40e49e44d4b665d621c4627149 \r\n" +
            "CSeq: 16564 INVITE \r\n" +
            "Server: Asterisk PBX 11.7.0~dfsg-1ubuntu1 \r\n" +
            "Allow: INVITE, ACK, CANCEL, OPTIONS, BYE, REFER, SUBSCRIBE, NOTIFY, INFO, PUBLISH \r\n" +
            "Supported: replaces, timer \r\n" +
            "Session-Expires: 1800;refresher=uas \r\n" +
            "Contact: <sip:31337@192.168.56.105:5060> \r\n" +
            "Require: timer \r\n" +
            "Content-Type: application/sdp \r\n" +
            "Content-Length: 289 \r\n" +
            " \r\n" +
            "v=0 \r\n" +
            "o=root 1350070557 1350070557 IN IP4 192.168.56.105 \r\n" +
            "s=Asterisk PBX 11.7.0~dfsg-1ubuntu1 \r\n" +
            "c=IN IP4 192.168.56.105 \r\n" +
            "t=0 0 \r\n" +
            "m=audio 19516 RTP/AVP 0 8 101 \r\n" +
            "a=rtpmap:0 PCMU/8000 \r\n" +
            "a=rtpmap:8 PCMA/8000 \r\n" +
            "a=rtpmap:101 telephone-event/8000 \r\n" +
            "a=fmtp:101 0-16 \r\n" +
            "a=ptime:20 \r\n" +
            "a=sendrecv \r\n";

        private const string TxAck =
            "ACK sip:31337@192.168.56.105:5060 SIP/2.0 \r\n" +
            "Via: SIP/2.0/UDP 192.168.56.1:5061;branch=z9hG4bKPj3e715762683b4c95b5609f613dcee8bf \r\n" +
            "Max-Forwards: 70 \r\n" +
            "From: <sip:115@192.168.56.105>;tag=94b1fa2000614ec090b2c45af2d4cee1 \r\n" +
            "To: <sip:31337@192.168.56.105>;tag=as56ee551e \r\n" +
            "Call-ID: c7e6ad40e49e44d4b665d621c4627149 \r\n" +
            "CSeq: 16564 ACK \r\n" +
            "Content-Length: 0 \r\n" +
            " \r\n";

        private const string TxBadEvent =
            "SIP/2.0 489 Bad Event \r\n" +
            "Via: SIP/2.0/UDP 192.168.56.1:5061;branch=z9hG4bKPj3e715762683b4c95b5609f613dcee8bf;received=192.168.56.1 \r\n" +
            "From: <sip:115@192.168.56.105>;tag=94b1fa2000614ec090b2c45af2d4cee1 \r\n" +
            "To: <sip:31337@192.168.56.105>;tag=as56ee551e \r\n" +
            "Call-ID: c7e6ad40e49e44d4b665d621c4627149 \r\n" +
            "CSeq: 16564 REGISTER \r\n" +
            "Server: Asterisk PBX 11.7.0~dfsg-1ubuntu1 \r\n" +
            "Allow: INVITE, ACK, CANCEL, OPTIONS, BYE, REFER, SUBSCRIBE, NOTIFY, INFO, PUBLISH \r\n" +
            "Supported: replaces, timer \r\n" +
            "Content-Length: 0 \r\n" +
            " \r\n";

        #endregion Constants

        #region Fields

        private Dictionary<string, string> _compactMapper;
        private SipVia _via;
        private SipPacket _packet;
        private SipUri _from;
        private SipUri _to;
        private string _callId;
        private int _cseq;

        #endregion Fields

        #region Methods

        [SetUp]
        public void Setup()
        {
            _compactMapper = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
            {
                { "\r\nVia:", "\r\nv:" },
                { "\r\nFrom:", "\r\nf:" },
                { "\r\nTo:", "\r\nt:" },
                { "\r\nCall-ID:", "\r\ni:" },
                { "\r\nContact:", "\r\nm:" },
                { "\r\nContent-Type:", "\r\nc:" },
                { "\r\nContent-Length:", "\r\nl:" },
            };

            _via = new SipVia()
            { 
                Protocol = "UDP",
                Aliase = "192.168.56.1:5061",
                Branch = "z9hG4bKPj3e715762683b4c95b5609f613dcee8bf",
            };
            _from = new SipUri("115", "192.168.56.105", "94b1fa2000614ec090b2c45af2d4cee1");
            _to = new SipUri("31337", "192.168.56.105", "as56ee551e");

            _callId = "c7e6ad40e49e44d4b665d621c4627149";
            _cseq = 16564;
            _packet = new SipPacket();
        }

        // TODO Impl test for ever message.

        //[TestCase(TxInvite, SipMethod.INVITE, null)]
        //[TestCase(RxTrying, SipMethod.INVITE, SipStatus.Trying)]
        //[TestCase(RxOk, SipMethod.INVITE, SipStatus.Ok)]
        //[TestCase(TxAck, SipMethod.ACK, null)]

        [TestCase(TxInvite)]
        [TestCase(RxOk)]
        [TestCase(RxTrying)]
        [TestCase(TxAck)]
        [TestCase(TxBadEvent)]
        public void UnpackTest(string message)
        {
            // Arrage            
            var offset = 0;
            byte[] bytes = Encoding.ASCII.GetBytes(message);

            // Act
            bool result = _packet.TryUnpack(bytes, ref offset);

            // TODO Add more asserts.
            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(offset, bytes.Length);
            Assert.AreEqual(_callId, _packet.CallId);
            Assert.AreEqual(_cseq, _packet.CSeq);
            Assert.AreEqual(_from.Address, _packet.From.Address);
            Assert.AreEqual(_to.Address, _packet.To.Address);
            Assert.AreEqual(1, _packet.Via.Count);
            Assert.AreEqual(_via, _packet.Via[0]);
        }

        [TestCase(TxInvite)]
        [TestCase(RxTrying)]
        [TestCase(RxOk)]
        [TestCase(TxAck)]
        [TestCase(TxBadEvent)]
        public void UnpackWithOffsetTest(string message)
        {
            // Arrage            
            var offset = 0;
            var result = false;
            var concat = string.Empty;
            var lines = message.Split("\r\n");

            // Act
            foreach (var item in lines)
            {
                concat += item + "\r\n";
                result = _packet.TryUnpack(Encoding.ASCII.GetBytes(concat), ref offset);
                if (result)
                    break;
            }

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(offset, Encoding.ASCII.GetByteCount(message));
        }

        [TestCase(TxInvite, false)]
        [TestCase(TxInvite, true)]
        [TestCase(RxTrying, false)]
        [TestCase(RxTrying, true)]
        [TestCase(RxOk, false)]
        [TestCase(RxOk, true)]
        [TestCase(TxAck, false)]
        [TestCase(TxAck, true)]
        [TestCase(TxBadEvent, false)]
        [TestCase(TxBadEvent, true)]
        public void PackTest(string message, bool compact)
        {
            // Arrage
            _packet = new SipPacket(compact);

            var offset = 0;
            var bytes = new byte[0];
            if (compact)
            {
                foreach (var item in _compactMapper)
                {
                    message = message.Replace(item.Key, item.Value);
                }
            }
            var lines = message.Split("\r\n");

            // Act
            UnpackTest(message);
            _packet.Pack(ref bytes, ref offset);
            var newLine = Encoding.ASCII.GetString(bytes).Split("\r\n");

            // Assert
            Assert.AreNotEqual(offset, 0);
            Assert.AreEqual(offset, bytes.Length);
            CollectionAssert.AreEquivalent(lines, newLine);
        }

        #endregion Methods
    }
}