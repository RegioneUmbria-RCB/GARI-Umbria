namespace InData.RequestHeaders
{
    public class ClientTimeZoneInfo
    {
        public ClientTimeZoneInfo() {
            ClientCountryCode = string.Empty;
            ClientTimeZoneId = string.Empty;
            UTCOffset = 0;
        }

        public string ClientCountryCode { get; set; }
        public string ClientTimeZoneId { get; set; }
        public int UTCOffset { get; set; }

    }
}
