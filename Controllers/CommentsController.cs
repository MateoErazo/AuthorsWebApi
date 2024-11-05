using AuthorsWebApi.DTOs;
using AuthorsWebApi.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AuthorsWebApi.Controllers
{
    [ApiController]
    [Route("api/books/{bookId:int}/comments")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class CommentsController : ControllerBase
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IMapper mapper;
        private readonly UserManager<IdentityUser> userManager;

        public CommentsController(
            ApplicationDbContext dbContext, 
            IMapper mapper,
            UserManager<IdentityUser> userManager)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
            this.userManager = userManager;
        }

        [HttpGet(Name ="getCommentsByBookId")]
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

        [HttpGet("{id:int}", Name ="getCommentById")]
        public async Task<ActionResult<CommentWithBookDTO>> GetCommentById(int id)
        {
            Comment comment = await dbContext.Comments
                .Include(x => x.Book)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (comment == null) {
                return NotFound($"Don't exist a comment with id {id}.");
            }

            return mapper.Map<CommentWithBookDTO>(comment);
        }

        [HttpPost(Name = "createCommentByBookId")]
        public async Task<ActionResult> Create(int bookId, CommentCreationDTO commentCreationDTO)
        {
            bool bookExist = await dbContext.Books.AnyAsync(x => x.Id == bookId);

            if (!bookExist) {
                return NotFound($"Does not exist a book with id {bookId}");
            }

            Claim emailClaim = HttpContext.User.Claims.Where(x => x.Type == "email").FirstOrDefault();
            string email = emailClaim.Value;

            IdentityUser user = await userManager.FindByEmailAsync(email);
            string userId = user.Id;

            Comment comment = mapper.Map<Comment>(commentCreationDTO);
            comment.BookId = bookId;
            comment.UserId = userId;
            dbContext.Add(comment);
            await dbContext.SaveChangesAsync();

            CommentDTO commentDTO = mapper.Map<CommentDTO>(comment);
            return CreatedAtRoute("getCommentById", new {bookId = comment.BookId, id = comment.Id}, commentDTO);
        }

        [HttpPut("{id:int}", Name ="updateCommentByBookIdCommentId")]
        public async Task<ActionResult> UpdateComment(int bookId, int id, CommentCreationDTO commentCreationDTO)
        {
            bool bookExist = await dbContext.Books.AnyAsync(x => x.Id == bookId);

            if (!bookExist)
            {
                return NotFound($"Does not exist a book with id {bookId}");
            }

            bool commentExist = await dbContext.Comments.AnyAsync(x => x.Id == id);

            if (!commentExist) {
                return NotFound($"Don't exist a comment with id {id}.");
            }

            Comment comment = mapper.Map<Comment>(commentCreationDTO);
            comment.BookId = bookId;
            comment.Id = id;

            dbContext.Update(comment);
            await dbContext.SaveChangesAsync();
            return NoContent();

        }
    }
}
