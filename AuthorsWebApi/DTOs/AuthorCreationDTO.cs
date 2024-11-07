using AuthorsWebApi.Validations;
using System.ComponentModel.DataAnnotations;

namespace AuthorsWebApi.DTOs
{
    public class AuthorCreationDTO
    {
        [StringLength(maximumLength: 50, ErrorMessage = "Sorry, the maximum lenght for the field {0} is {1}")]
        [FirstCapitalLetter]
        public string Name { get; set; }
    }
}
