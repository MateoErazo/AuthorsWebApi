using AuthorsWebApi.Validations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthorsWebApi.Entities
{
    public class Book
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "Sorry, the maximum lenght for the field {0} is {1}")]
        [FirstCapitalLetter]
        public string Title { get; set; }
        [Column("Publication_Date")]
        public DateTime? PublicationDate { get; set; }
        public List<Comment> Comments { get; set; }

        public List<BookAuthor> BookAuthor { get; set; }


    }
}
