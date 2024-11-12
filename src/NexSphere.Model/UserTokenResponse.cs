namespace NexSphere.Model
{
    public class UserTokenResponse
    {
        public uint UserID { get; private set; }

        public string AccessToken { get; private set; }

        public UserTokenResponse(uint UserID, string AccessToken)
        {
            this.UserID = UserID;
            this.AccessToken = AccessToken;
        }
    }
}
