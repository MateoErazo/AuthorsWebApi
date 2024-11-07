using System.ComponentModel.DataAnnotations;

namespace AuthorsWebApi.DTOs
{
    public class UserAdminEditDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
