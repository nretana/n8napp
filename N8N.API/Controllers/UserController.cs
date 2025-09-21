using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using N8N.API.Context.Entities;
using N8N.API.Models;
using N8N.API.Services;
using N8N.API.Validators;

namespace N8N.API.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UserController: ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        public UserController(IUserService userService, IMapper mapper) {
            _userService = userService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var userList = await _userService.GetUsersAsync();
            return Ok(_mapper.Map<IEnumerable<UserDto>>(userList));
        }

        [HttpGet("{userId}", Name = "GetUser")]
        public async Task<IActionResult> GetUser(Guid userId)
        {
            if (userId == Guid.Empty) return BadRequest("userId not provided");

            var userFound = await _userService.GetUserAsync(userId);
            if(userFound == null) return NotFound("user not found");
            return Ok(_mapper.Map<UserDto>(userFound));
        }

        [HttpPost]
        public async Task<IActionResult> AddUser(CreateUserDto user)
        {
            if (user == null) return BadRequest("userId not provided");

            var newUser = _mapper.Map<User>(user);
            await _userService.AddUserAsync(newUser);
            var userResponse = _mapper.Map<UserDto>(newUser);
            return CreatedAtRoute("GetUser", new { userId = newUser.UserId }, userResponse);
        }

        [HttpDelete("{userId}")]
        public async Task<IActionResult> RemoveUser(Guid userId) 
        {
            if (userId == Guid.Empty) return BadRequest("userId not provided");

            var userFound = await _userService.GetUserAsync(userId);
            if (userFound == null) return NotFound("user not found");
            _userService.RemoveUser(userFound);
            return NoContent();
        }
    }
}
