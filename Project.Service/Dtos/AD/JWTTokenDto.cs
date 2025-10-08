namespace Project.Service.Dtos.AD
{
    public class JWTTokenDto
    {
        public string? AccessToken { get; set; }
        public DateTime? ExpireDate { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? ExpireDateRefreshToken { get; set; }
        public AccountDto? AccountInfo { get; set; }
    }
}
