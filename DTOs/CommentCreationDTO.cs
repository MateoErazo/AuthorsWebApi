using System.ComponentModel.DataAnnotations;

namespace AuthorsWebApi.DTOs
{
    public class CommentCreationDTO
    {
        [Required]
        [StringLength(maximumLength: 2000)]
        public string Content { get; set; }
    }
}
