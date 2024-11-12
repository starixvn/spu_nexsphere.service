using NexSphere.Model;

namespace NexSphere.AuthenticateAPI.Services
{
    public interface IUserService
    {
        UserModel GetUserInfo(uint userID);

        IEnumerable<UserModel> GetUserList();

        bool RegisterUser(string username, string passwordHash);

        uint? LoginUser(string username, string password);

        string GetUserRole(int userID);

        IEnumerable<string> GetUserRoles(uint userID);
    }

    public class UserService : IUserService
    {
        public UserModel GetUserInfo(uint userID)
        {
            if (userID == 1)
            {
                return new(UserID: 1, Username: "Admin");
            }

            return new(UserID: 2, Username: "User");
        }

        public IEnumerable<UserModel> GetUserList()
        {
            return new List<UserModel> {
                new(UserID:1, Username:"Admin"),
                new(UserID:2, Username:"User")
            };
        }
        public string GetUserRole(int userID)
        {
            if (userID == 1) return "Admin";

            return "User";
        }

        public uint? LoginUser(string username, string password)
        {
            if (username == "admin")
            {
                return 1;
            }
            else if(username == "user")
            {
                return 2;
            }

            return null;
        }

        public bool RegisterUser(string username, string passwordHash)
        {
            return false;
        }

        public IEnumerable<string> GetUserRoles(uint userID)
        {
            if (userID == 1)
            {
                return ["Admin"];
            }

            return ["User"];
        }
    }
}
