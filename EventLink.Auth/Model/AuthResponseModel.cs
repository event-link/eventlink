namespace EventLink.Auth.Model
{
    public class AuthResponseModel
    {
        public string Token { get; }
        public string RefreshToken { get; }
        public string Message { get; }
        public AuthResponseModel(string token, string refreshToken, string message)
        {
            Token = token;
            RefreshToken = refreshToken;
            Message = message;
        }
    }
}