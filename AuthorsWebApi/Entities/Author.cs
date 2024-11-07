using AuthorsWebApi.Validations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthorsWebApi.Entities
{
    public class Author
    {
        public int Id { get; set; }

        [StringLength(maximumLength:50,ErrorMessage ="Sorry, the maximum lenght for the field {0} is {1}")]
        [FirstCapitalLetter]

        public string Name { get; set; }
        public List<BookAuthor> BookAuthor { get; set; }

    }
}
