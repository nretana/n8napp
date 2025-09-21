using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using N8N.API.Context;
using N8N.API.Context.Entities;

namespace N8N.API.Services
{
    public class UserService : IUserService
    {
        private readonly CalendarDbContext _context;
        public UserService(CalendarDbContext context) {
            _context = context;
        }

        public async Task<IEnumerable<User>> GetUsersAsync()
        {
            var userList = await _context.Users.ToListAsync();
            return userList;
        }

        public async Task<User?> GetUserAsync(Guid userId)
        {
            if(userId == Guid.Empty) throw new ArgumentNullException(nameof(userId));

            var userFound = await _context.Users.FirstOrDefaultAsync(x => x.UserId == userId);
            return userFound;
        }

        public async Task AddUserAsync(User user)
        {
            if(user is null) throw new ArgumentNullException(nameof(user));

            user.UserId = Guid.NewGuid();
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public void RemoveUser(User user)
        {
            if (user is null) throw new ArgumentNullException(nameof(user));

            _context.Users.Remove(user);
            _context.SaveChanges();
        }

        public async Task<bool> IsUserExistsAsync(Guid userId)
        {
            if (userId == Guid.Empty) throw new ArgumentNullException(nameof(userId));

            var userFound = await _context.Users.FirstOrDefaultAsync(user => user.UserId == userId);
            return userFound != null;
        }
    }
}
