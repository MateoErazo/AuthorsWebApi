using AuthorsWebApi.DTOs;
using AuthorsWebApi.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuthorsWebApi.Controllers
{
    [ApiController]
    [Route("api/books/{bookId:int}/comments")]
    public class CommentsController:ControllerBase
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IMapper mapper;

        public CommentsController(ApplicationDbContext dbContext, IMapper mapper)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<CommentDTO>>> GetAllCommentsByBookId(int bookId)
        {
            bool bookExist = await dbContext.Books.AnyAsync(x => x.Id == bookId);

            if (!bookExist)
            {
                return NotFound($"Does not exist a book with id {bookId}");
            }

            List<Comment> comments = await dbContext.Comments
                .Include(x => x.Book)
                .Where(x => x.BookId == bookId)
                .ToListAsync();

            return mapper.Map<List<CommentDTO>>(comments);
        }

        [HttpPost]
        public async Task<ActionResult> Create(int bookId, CommentCreationDTO commentCreationDTO)
        {
            bool bookExist = await dbContext.Books.AnyAsync(x => x.Id == bookId);

            if (!bookExist) {
                return NotFound($"Does not exist a book with id {bookId}");
            }

            Comment comment = mapper.Map<Comment>(commentCreationDTO);
            comment.BookId = bookId;
            dbContext.Add(comment);
            await dbContext.SaveChangesAsync();
            return Ok();
        }
    }
}
