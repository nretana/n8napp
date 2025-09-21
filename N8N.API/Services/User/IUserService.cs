using N8N.API.Context.Entities;

namespace N8N.API.Services
{
    public interface IUserService
    {
        public Task<IEnumerable<User>> GetUsersAsync();

        public Task<User?> GetUserAsync(Guid userId);

        public Task AddUserAsync(User user);

        public void RemoveUser(User user);

        public Task<bool> IsUserExistsAsync(Guid userId);

    }
}
