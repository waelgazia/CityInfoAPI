using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace CityInfo.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthenticationController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public AuthenticationController(IConfiguration configuration)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    [HttpPost]
    public ActionResult<string> Authenticate([FromBody] AuthenticationBodyRequest authenticationBodyRequest)
    {
        // 1. validate username/password
        CityInfoUser user =
            ValidateUserCredential(authenticationBodyRequest.UserName, authenticationBodyRequest.Password);
        if (user is null) return Unauthorized();

        // 2. create a token
        var securityKey = new SymmetricSecurityKey( /* key to sign the token, and the signing algorithm */
            Convert.FromBase64String(_configuration["Authentication:SecretForKey"]));
        var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claimsForToken = new List<Claim>(); /* represent the token payload*/
        claimsForToken.Add(new Claim("sub", user.UserId.ToString()));
        claimsForToken.Add(new Claim("given_name", user.FirstName));
        claimsForToken.Add(new Claim("family_name", user.LastName));
        claimsForToken.Add(new Claim("city", user.City));

        var jwtSecurityToken = new JwtSecurityToken(
            _configuration["Authentication:Issuer"],
            _configuration["Authentication:Audience"],
            claimsForToken,
            DateTime.Now,
            DateTime.Now.AddHours(1),
            signingCredentials);

        // 3. return token
        var tokenToReturn = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
        return Ok(tokenToReturn);
    }

    private CityInfoUser ValidateUserCredential(string? userName, string? password)
    {
        // assume credential are always correct
        return new CityInfoUser(
            1,
            userName ?? "",
            "Kevin",
            "Dockx",
            "Antwerp");
    }

    public class AuthenticationBodyRequest
    {
        [Required] public string? UserName { get; set; }
        [Required] public string? Password { get; set; }
    }

    private class CityInfoUser
    {
        public int UserId { get; set; }
        public string? UserName { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? City { get; set; }

        public CityInfoUser(
            int userId,
            string userName,
            string firstName,
            string lastName,
            string city)
        {
            UserId = userId;
            UserName = userName;
            FirstName = firstName;
            LastName = lastName;
            City = city;
        }
    }
}