﻿using AuthorsWebApi.DTOs;
using AuthorsWebApi.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuthorsWebApi.Controllers
{
    [ApiController]
    [Route("api/books/{bookId:int}/comments")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class CommentsController : ControllerBase
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

        [HttpGet("{id:int}", Name ="GetCommentById")]
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

            CommentDTO commentDTO = mapper.Map<CommentDTO>(comment);
            return CreatedAtRoute("GetCommentById", new {bookId = comment.BookId, id = comment.Id}, commentDTO);
        }

        [HttpPut("{id:int}")]
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
