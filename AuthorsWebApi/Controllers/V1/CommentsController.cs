using AuthorsWebApi.DTOs;
using AuthorsWebApi.Entities;
using AuthorsWebApi.Utils;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AuthorsWebApi.Controllers.V1
{
    [ApiController]
    [Route("api/v1/books/{bookId:int}/comments")]
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

        [HttpGet(Name = "getCommentsByBookIdV1")]
        public async Task<ActionResult<List<CommentDTO>>> GetAllCommentsByBookId(int bookId, [FromQuery] PaginationDTO paginationDTO)
        {
            bool bookExist = await dbContext.Books.AnyAsync(x => x.Id == bookId);

            if (!bookExist)
            {
                return NotFound($"Does not exist a book with id {bookId}");
            }

            var queryable = dbContext.Comments
                .Where(comment => comment.BookId == bookId)
                .AsQueryable();

            await HttpContext.InsertTotalRecordsInHeader(queryable);

            List<Comment> comments = await queryable
                .OrderBy(comment => comment.Id)
                .Paginate(paginationDTO)
                .ToListAsync();

            return mapper.Map<List<CommentDTO>>(comments);
        }

        [HttpGet("{id:int}", Name = "getCommentByIdV1")]
        public async Task<ActionResult<CommentWithBookDTO>> GetCommentById(int id)
        {
            Comment comment = await dbContext.Comments
                .Include(x => x.Book)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (comment == null)
            {
                return NotFound($"Don't exist a comment with id {id}.");
            }

            return mapper.Map<CommentWithBookDTO>(comment);
        }

        [HttpPost(Name = "createCommentByBookIdV1")]
        public async Task<ActionResult> Create(int bookId, CommentCreationDTO commentCreationDTO)
        {
            bool bookExist = await dbContext.Books.AnyAsync(x => x.Id == bookId);

            if (!bookExist)
            {
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
            return CreatedAtRoute("getCommentByIdV1", new { bookId = comment.BookId, id = comment.Id }, commentDTO);
        }

        [HttpPut("{id:int}", Name = "updateCommentByBookIdCommentIdV1")]
        public async Task<ActionResult> UpdateComment(int bookId, int id, CommentCreationDTO commentCreationDTO)
        {
            bool bookExist = await dbContext.Books.AnyAsync(x => x.Id == bookId);

            if (!bookExist)
            {
                return NotFound($"Does not exist a book with id {bookId}");
            }

            bool commentExist = await dbContext.Comments.AnyAsync(x => x.Id == id);

            if (!commentExist)
            {
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
