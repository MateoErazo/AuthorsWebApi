using AuthorsWebApi.Validations;
using System.ComponentModel.DataAnnotations;

namespace AuthorsWebApi.DTOs
{
    public class BookDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
    }
}
