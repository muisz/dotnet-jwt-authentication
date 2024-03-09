namespace JwtAuthTutorial.Data
{
    public class AuthResponse
    {
        public string Username { get; set; } = string.Empty;
        public JwtToken? Token { get; set; }
    }
}
