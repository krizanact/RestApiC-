using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Project.Model.Model.Token;
using Microsoft.IdentityModel.Tokens;

namespace Project.Service.ThirdParty
{
    public interface IJwtAuthManager
    {
      
        /// <summary>
        /// Generates new access token for user with specific claims
        /// </summary>
        /// <param name="claims"></param>
        /// <returns></returns>
        TokenResponse GenerateTokens(Claim[] claims);

    }

    public class JwtAuthManager : IJwtAuthManager
    {
        private readonly JwtConfiguration _jwtConfiguration;
        private readonly byte[] _secret;

        public JwtAuthManager(JwtConfiguration jwtConfiguration)
        {
            _jwtConfiguration = jwtConfiguration;
            _secret = Encoding.ASCII.GetBytes(jwtConfiguration.Secret);
        }

        public TokenResponse GenerateTokens(Claim[] claims)
        {
            // Check if claim is empty
            bool shouldAddAudienceClaim = String.IsNullOrWhiteSpace(claims?.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Aud)?.Value);

            // Generate new jwt security
            JwtSecurityToken jwtToken = new JwtSecurityToken(
                _jwtConfiguration.Issuer,
                shouldAddAudienceClaim ? _jwtConfiguration.Audience : String.Empty,
                claims,
                expires: DateTime.Now.AddHours(_jwtConfiguration.AccessTokenExpiration),
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(_secret), SecurityAlgorithms.HmacSha256Signature));

            // Generate new access token
            string accessToken = new JwtSecurityTokenHandler().WriteToken(jwtToken);

            // Return refresh and access token
            return new TokenResponse
            {
                AccessToken = accessToken,
                Expires = DateTime.Now.AddHours(_jwtConfiguration.AccessTokenExpiration).ToString(),
            };
        }


    }

}
