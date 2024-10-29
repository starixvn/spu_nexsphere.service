namespace NexSphere.Data.Features.UserAgent
{
    using System.Collections.Generic;
    using Fanex.Data.Repository;
    using NexSphere.Data.Features.UserAgent.Criteria;
    using NexSphere.Data.Features.UserAgent.Entities;
    
    public interface IUserAgentDataAccess
    {
        IEnumerable<UserAgentEntity> GetUserAgent(int size);

    }

    public class UserAgentDataAccess : IUserAgentDataAccess
    {
        private readonly IDynamicRepository repository;

        public UserAgentDataAccess(IDynamicRepository repository)
        {
            this.repository = repository;
        }

        public IEnumerable<UserAgentEntity> GetUserAgent(int size)
        {
            var criteria = new GetUserAgentCriteria
            {
                Size = size
            };

            return repository.Fetch<UserAgentEntity>(criteria);

        }

    }
}
