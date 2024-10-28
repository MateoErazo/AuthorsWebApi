﻿using AuthorsWebApi.DTOs;
using AuthorsWebApi.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuthorsWebApi.Controllers
{
    [ApiController]
    [Route("api/books")]
    public class BooksController: ControllerBase
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IMapper mapper;

        public BooksController( ApplicationDbContext dbContext, IMapper mapper ) {
            this.dbContext = dbContext;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<List<BookDTO>> GetAll()
        {
            List<Book> books = await dbContext.Books.ToListAsync();
            return mapper.Map<List<BookDTO>>(books);
        }


        [HttpGet("{id:int}")]
        public async Task<ActionResult<BookWithAuthorsDTO>> GetBookById(int id)
        {
            Book book = await dbContext.Books
                .Include(x => x.BookAuthor)
                .ThenInclude(x => x.Author)
                .FirstOrDefaultAsync(book => book.Id == id);

            if (book == null) { 
                return NotFound($"There is not a book with id {id}");
            }

            book.BookAuthor = book.BookAuthor.OrderBy(x => x.Order).ToList();

            return mapper.Map<BookWithAuthorsDTO>(book);
        }

        [HttpPost]
        public async Task<ActionResult> CreateNew(BookCreationDTO bookCreationDTO)
        {
            if(bookCreationDTO.AuthorIds == null || bookCreationDTO.AuthorIds.Count == 0)
            {
                return BadRequest("Don't is possible create a book without authors.");
            }

            List<int> authorIds = await dbContext.Authors
                .Where(x => bookCreationDTO.AuthorIds.Contains(x.Id))
                .Select(x => x.Id)
                .ToListAsync();

            if (authorIds.Count != bookCreationDTO.AuthorIds.Count)
            {
                return NotFound("Don't exist one of the authors sended. Please check.");
            }

            Book book = mapper.Map<Book>(bookCreationDTO);

            if (book.BookAuthor == null || book.BookAuthor.Count == 0)
            {
                return BadRequest("An unexpected error occurred. Please contact the help desk.");
            }

            for (int i = 0; i < book.BookAuthor.Count; i++)
            {
                book.BookAuthor[i].Order = i;
            }

            dbContext.Add(book);
            await dbContext.SaveChangesAsync();
            return Ok(book);
        }


    }
}
