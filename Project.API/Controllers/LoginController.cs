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

namespace Project.Api.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {

        private readonly IUserService _userService;
        private readonly IJwtAuthManager _jwtAuthManager;

        public LoginController(IUserService userService, IJwtAuthManager jwtAuthManager)
        {
            _userService = userService;
            _jwtAuthManager = jwtAuthManager;
        }

        [SwaggerOperation(Summary = "Logs user in application and gets JWT token back")]
        [ProducesResponseType(typeof(TokenResponse), 200)]
        [HttpPost]
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
    }
}
