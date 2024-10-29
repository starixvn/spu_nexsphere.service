namespace NexSphere.Data.Features.UserAgent
{
    using System.Collections.Generic;
    using System.Linq;
    using NexSphere.Data.Features.UserAgent.Entities;
    
    public interface IUserAgentServices
    {
        List<UserAgentEntity> GetUserAgent(int size);
    }

    public class UserAgentServices : IUserAgentServices
    {
        private readonly IUserAgentDataAccess dataAccess;
        
        public UserAgentServices(IUserAgentDataAccess dataAccess)
        {
            this.dataAccess = dataAccess;
        }

        public List<UserAgentEntity> GetUserAgent(int size)
        {
            return dataAccess.GetUserAgent(size).ToList();
        }
    }
}
