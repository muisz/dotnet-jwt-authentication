using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using JwtAuthTutorial.Configurations;
using JwtAuthTutorial.Data;
using JwtAuthTutorial.Models;
using JwtAuthTutorial.Services.PasswordHasher;
using Microsoft.IdentityModel.Tokens;

namespace JwtAuthTutorial.Services.AuthService
{
    public class AuthService : IAuthService
    {
        private readonly DatabaseContext _context;
        private readonly IPasswordHasher _hasher;
        private readonly IConfiguration _configuration;

        public AuthService(DatabaseContext context, IPasswordHasher hasher, IConfiguration configuration)
        {
            _context = context;
            _hasher = hasher;
            _configuration = configuration;
        }

        public User Register(AuthRequest payload)
        {
            User? usernameExist = GetUserByUsername(payload.Username);
            if (usernameExist != null)
            {
                throw new Exception("Username already exist");
            }

            string hash = _hasher.Hash(payload.Password);
            User user = new User
            {
                Username = payload.Username,
                Password = hash,
            };
            _context.Users.Add(user);
            _context.SaveChanges();
            return user;
        }

        public User Login(AuthRequest payload)
        {
            User? user = GetUserByUsername(payload.Username) ?? throw new Exception("Username not found");
            if (!_hasher.Check(payload.Password, user.Password))
            {
                throw new Exception("Wrong password");
            }
            return user;
        }

        private User? GetUserByUsername(string value)
        {
            User? user = _context.Users.SingleOrDefault(user => user.Username == value);
            return user;
        }

        public JwtToken GenerateAuthToken(User user)
        {
            string access = CreateToken(user, 3);
            string refresh = CreateToken(user, 6);
            return new JwtToken
            {
                Access = access,
                Refresh = refresh,
            };
        }

        public string CreateToken(User user, int activeHours)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
            };
            JwtConfig jwtConfig = new JwtConfig();
            _configuration.GetSection(JwtConfig.Jwt).Bind(jwtConfig);
            SymmetricSecurityKey key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(jwtConfig.Key));
            SigningCredentials credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);
            JwtSecurityToken token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddHours(activeHours),
                signingCredentials: credentials
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}