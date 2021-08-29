﻿using AutoMapper;
using Mediatorium.Model.Enum;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project.Model.Core;
using Project.Model.Model.JsonResponse;
using Project.Model.Model.Token;
using Project.Model.Model.User;
using Project.Service.AppService;
using Project.Service.ThirdParty;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Project.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {

        private readonly IUserService _userService;
        private readonly IJwtAuthManager _jwtAuthManager;
        private readonly IMapper _mapper;

        public UserController(IUserService userService, IJwtAuthManager jwtAuthManager, IMapper mapper)
        {
            _userService = userService;
            _jwtAuthManager = jwtAuthManager;
            _mapper = mapper;
        }

        [SwaggerOperation(Summary = "Logs user in application and gets JWT token back")]
        [ProducesResponseType(typeof(TokenResponse), 200)]
        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginInput loginInput)
        {
            // Check if model is invalid
            if (!ModelState.IsValid)
            {
                return BadRequest(new ErrorResponse()
                {
                    // Get first message from model
                    ErrorMessage = ModelState.First().Value.Errors.First().ErrorMessage,
                    Time = DateTime.Now.ToString()
                });
            }

            // Check if username and password exist in database
            if (!await _userService.CheckLogin(loginInput.Username, loginInput.Password))
            {
                return Unauthorized(new ErrorResponse()
                {
                    // Get first message from model
                    ErrorMessage = "Username or password is incorrect!",
                    Time = DateTime.Now.ToString()
                });
            }

            // Get user by his unique username
            User user = await _userService.GetUser(loginInput.Username);

            // Crate claims for current user
            Claim[] claims = new[]
            {
                 new Claim(ClaimTypes.Name, user.Username),
                 new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                 new Claim(ClaimTypes.Role, user.RoleId.ToString()),
            };

            // Generate token for this user
            TokenResponse tokenResponse = _jwtAuthManager.GenerateTokens(claims);

            // Return token data if login is successfull
            return Ok(tokenResponse);
        }

        [SwaggerOperation(Summary = "Creates new user")]
        [ProducesResponseType(typeof(SuccessResponse), 200)]
        [HttpPost("CreateUser")]
        public async Task<IActionResult> CreateUser([FromBody] UserInput userInput)
        {
            // Check if model is invalid
            if (!ModelState.IsValid)
            {
                return BadRequest(new ErrorResponse()
                {
                    // Get first message from model
                    ErrorMessage = ModelState.First().Value.Errors.First().ErrorMessage,
                    Time = DateTime.Now.ToString()
                });
            }

            // Check if typed username already exists in database
            if(await _userService.CheckUsername(userInput.Username))
            {
                return BadRequest(new ErrorResponse()
                {
                    // Get first message from model
                    ErrorMessage = "Typed username already exists!",
                    Time = DateTime.Now.ToString()
                });
            }

            // Get logged user and check his role
            User logeedUser = await _userService.GetUser(User.Identity.Name);

            if(logeedUser.RoleId != (int)RoleType.Admin)
            {
                return Unauthorized(new ErrorResponse() 
                { 
                    ErrorMessage = "User is not authorized for executing this method",
                    Time = DateTime.Now.ToString()
                });
            }

            // Map UserInput data to User object
            User user = _mapper.Map<User>(userInput);
            // Create new user
            await _userService.CreateUser(user);

            // Return success message
            return Ok(new SuccessResponse()
            {
                SuccessMessage = "New user is successfully created!",
                Time = DateTime.Now.ToString()
            });
        }

        [SwaggerOperation(Summary = "Returns list of users")]
        [ProducesResponseType(typeof(List<UserList>), 200)]
        [HttpGet("GetUsers")]
        public async Task<IActionResult> GetUsers()
        {
            // Get logged user and check his role
            User logeedUser = await _userService.GetUser(User.Identity.Name);

            if (logeedUser.RoleId != (int)RoleType.Admin)
            {
                return Unauthorized(new ErrorResponse()
                {
                    ErrorMessage = "User is not authorized for executing this method",
                    Time = DateTime.Now.ToString()
                });
            }

            List<User> users = await _userService.GetUsers();

            // Create list type to return
            List<UserList> userList = new List<UserList>() { };

            foreach(User user in users)
            {
                userList.Add(new UserList()
                {
                    Id = user.Id,
                    Username = user.Username,
                    FistName = user.FirstName,
                    LastName = user.LastName,
                    Role = user.Role.Name
                });
            }

            return Ok(userList);
        }
    }
}