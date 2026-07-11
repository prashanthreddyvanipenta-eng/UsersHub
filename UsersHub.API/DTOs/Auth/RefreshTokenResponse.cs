namespace UsersHub.API.DTOs.Auth
{
    public class RefreshTokenResponse
    {
        public bool Success { get; set; }

        public string Message { get; set; } = string.Empty;

        public string AccessToken { get; set; } = string.Empty;
    }
}
