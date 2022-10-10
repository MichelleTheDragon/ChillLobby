using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using System.Security.Principal;
using System.Text;

namespace ChillLobbyLoginAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        public static List<User> users = new List<User>();
        public static List<Server> servers = new List<Server>();
        private readonly IConfiguration _configuration;

        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("RegisterUser")]
        public async Task<ActionResult<User>> RegisterUser(UserRegister request)
        {
            CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);

            User newUser = new User();
            newUser.Username = request.Username;
            newUser.PasswordHash = passwordHash;
            newUser.PasswordSalt = passwordSalt;
            users.Add(newUser);

            return Ok(newUser);
        }

        [HttpPost("RegisterServer")]
        public async Task<ActionResult<Server>> RegisterServer(ServerRegister request)
        {
            CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);

            Server newServer = new Server();
            newServer.Username = request.Username;
            newServer.PasswordHash = passwordHash;
            newServer.PasswordSalt = passwordSalt;
            servers.Add(newServer);

            return Ok(newServer);
        }

        [HttpPost("LoginUser")]
        public async Task<ActionResult<string>> LoginUser(UserRegister request)
        {
            foreach (User user in users)
            {
                if (user.Username == request.Username)
                {
                    if (VerifyPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt) == true)
                    {
                        string token = CreateToken(user);
                        return Ok(token);
                    }
                    else
                    {
                        return BadRequest("Wrong password");
                    }
                }
            }
            return BadRequest("User not found.");
        }
        [HttpPost("LoginServer")]
        public async Task<ActionResult<string>> LoginServer(ServerRegister request)
        {
            foreach (Server server in servers)
            {
                if (server.Username == request.Username)
                {
                    if (VerifyPasswordHash(request.Password, server.PasswordHash, server.PasswordSalt) == true)
                    {
                        string token = CreateToken(server);
                        return Ok(token);
                    }
                    else
                    {
                        return BadRequest("Wrong password");
                    }
                }
            }
            return BadRequest("User not found.");
        }


        [HttpPost("CheckUser"), Authorize(Roles = "Admin")]
        public async Task<ActionResult<string>> CheckToken(string token, string name)
        {
            //var mySecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value));
            var tokenHandler = new JwtSecurityTokenHandler();
            
            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value)),
                    ValidateIssuer = false,
                    ValidateAudience = false
                }, out SecurityToken validatedToken);
            }
            catch
            {
                return BadRequest("Token not valid");
            }
            foreach (User user in users)
            {
                if (user.Username == name)
                {
                    if (user.UserToken == token)
                    {
                        return Ok("Token valid");
                    } else
                    {
                        return BadRequest("Token is not owned by this user");
                    }
                }
            }
            return BadRequest("Username does not exist");
        }

        private string CreateToken(User user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, "User")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds
                );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            user.UserToken = jwt;

            return jwt;
        }
        private string CreateToken(Server server)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, server.Username),
                new Claim(ClaimTypes.Role, "Admin")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(2),
                signingCredentials: creds
                );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }
    }
}
