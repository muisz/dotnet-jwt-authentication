using JwtAuthTutorial.Data;
using JwtAuthTutorial.Models;

namespace JwtAuthTutorial.Services.AuthService
{
    public interface IAuthService
    {
        public User Register(AuthRequest payload);
        public User Login(AuthRequest payload);
        public JwtToken GenerateAuthToken(User user);
        public string CreateToken(User user, int activeHours);
    }
}
