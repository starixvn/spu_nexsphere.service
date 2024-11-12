namespace NexSphere.Core.Models
{
    public class TokenSetting
    {
        public string SecretKey { get; set; }

        public string Issuer { get; set; }

        public TokenSetting(string SecretKey, string Issuer)
        {
            this.SecretKey = SecretKey;
            this.Issuer = Issuer;
        }

    }
}