using AuthorsWebApi.Entities;
using System.ComponentModel.DataAnnotations;

namespace AuthorsWebApi.DTOs
{
    public class CommentDTO
    {
        public int Id { get; set; }
        public string Content { get; set; }
    }
}
