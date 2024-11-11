using Microsoft.AspNetCore.Identity;

namespace NexSphere.AuthenticateAPI.Services
{
    public interface IIdentityService
    {
        void RegisterUser(string username, string passwordHash);

        int LoginUser(string username);

        string GetUserRole(int userId);
    }

    public class IdentityService : IIdentityService
    {
        public string GetUserRole(int userId)
        {
            return "UserPermissions";
        }

        public int LoginUser(string username)
        {
            return 0;
        }

        public void RegisterUser(string username, string passwordHash)
        {
            return ;
        }
    }
}
