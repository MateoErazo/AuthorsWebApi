using System.ComponentModel.DataAnnotations;

namespace AuthorsWebApi.Entities
{
    public class Comment
    {
        public int Id { get; set; }
        [Required]
        [StringLength(maximumLength:2000)]
        public string Content { get; set; }
        public int BookId { get; set; }
        public Book Book { get; set; }
    }
}
