using AuthorsWebApi.Validations;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace AuthorsWebApi.DTOs
{
    public class BookCreationDTO
    {
        [StringLength(50, ErrorMessage = "Sorry, the maximum lenght for the field {0} is {1}")]
        [FirstCapitalLetter]
        public string Title { get; set; }
        public List<int> AuthorIds { get; set; }
        public DateTime PublicationDate { get; set; }
    }
}
