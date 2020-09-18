namespace Protocols.Channels.Sip
{
    // =====================================================================
    //INVITEm           =  %x49.4E.56.49.54.45 ; INVITE in caps
    //ACKm              =  %x41.43.4B ; ACK in caps
    //OPTIONSm          =  %x4F.50.54.49.4F.4E.53 ; OPTIONS in caps
    //BYEm              =  %x42.59.45 ; BYE in caps
    //CANCELm           =  %x43.41.4E.43.45.4C ; CANCEL in caps
    //REGISTERm         =  %x52.45.47.49.53.54.45.52 ; REGISTER in caps
    //Method            =  INVITEm / ACKm / OPTIONSm / BYEm
    //                     / CANCELm / REGISTERm
    //                     / extension-method
    // =====================================================================

    public enum SipRequestMethods
    {
        REGISTER,
        INVITE,
        OPTIONS,
        CANCEL,
        BYE,
        ACK,
    }
}