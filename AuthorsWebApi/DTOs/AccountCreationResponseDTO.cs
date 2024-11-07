namespace AuthorsWebApi.DTOs
{
    public class AccountCreationResponseDTO
    {
        public string Token { get; set; }
        public DateTime Expiration { get; set; }
    }
}
