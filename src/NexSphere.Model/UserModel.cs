using System;

namespace NexSphere.Model
{
    public class UserModel
    {
        public uint UserID { get; private set; }

        public string Username { get; private set; }

        public UserModel(uint UserID, string Username)
        {
            if (string.IsNullOrWhiteSpace(Username))
            {
                throw new ArgumentException("Username cannot be empty", nameof(Username));
            }

            this.UserID = UserID;
            this.Username = Username;
        }
    }
}
