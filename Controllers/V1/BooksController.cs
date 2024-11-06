using AuthorsWebApi.DTOs;
using AuthorsWebApi.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace AuthorsWebApi.Controllers.V1
{
    [ApiController]
    [Route("api/v1/books")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class BooksController : ControllerBase
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IMapper mapper;

        public BooksController(ApplicationDbContext dbContext, IMapper mapper)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
        }

        [HttpGet(Name = "getBooksV1")]
        public async Task<List<BookDTO>> GetAll()
        {
            List<Book> books = await dbContext.Books.ToListAsync();
            return mapper.Map<List<BookDTO>>(books);
        }


        [HttpGet("{id:int}", Name = "getBookByIdV1")]
        public async Task<ActionResult<BookWithAuthorsDTO>> GetBookById(int id)
        {
            Book book = await dbContext.Books
                .Include(x => x.BookAuthor)
                .ThenInclude(x => x.Author)
                .FirstOrDefaultAsync(book => book.Id == id);

            if (book == null)
            {
                return NotFound($"There is not a book with id {id}");
            }

            book.BookAuthor = book.BookAuthor.OrderBy(x => x.Order).ToList();

            return mapper.Map<BookWithAuthorsDTO>(book);
        }

        [HttpPost(Name = "createBookV1")]
        public async Task<ActionResult> Create(BookCreationDTO bookCreationDTO)
        {
            if (bookCreationDTO.AuthorIds == null || bookCreationDTO.AuthorIds.Count == 0)
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

            SetAuthorsOrder(book);

            dbContext.Add(book);
            await dbContext.SaveChangesAsync();
            BookDTO bookDTO = mapper.Map<BookDTO>(book);
            return CreatedAtRoute("getBookByIdV1", new { id = book.Id }, bookDTO);
        }

        [HttpPut("{id:int}", Name = "updateBookByIdV1")]
        public async Task<ActionResult> Update(int id, BookCreationDTO bookCreationDTO)
        {
            Book book = await dbContext.Books
                .Include(x => x.BookAuthor)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (book == null)
            {
                return NotFound($"Don't exist a book with id {id}.");
            }

            book = mapper.Map(bookCreationDTO, book);
            SetAuthorsOrder(book);

            await dbContext.SaveChangesAsync();
            return NoContent();
        }

        [HttpPatch("{id:int}", Name = "partialUpdateBookByIdV1")]
        public async Task<ActionResult> PartialUpdate(int id, JsonPatchDocument<BookPatchDTO> patchDocument)
        {
            if (patchDocument == null)
            {
                return BadRequest("Something went wrong. Please check your request.");
            }

            Book bookDB = await dbContext.Books.FirstOrDefaultAsync(x => x.Id == id);

            if (bookDB == null)
            {
                return NotFound($"Don't exist a book with id {id}.");
            }

            BookPatchDTO bookPatchDTO = mapper.Map<BookPatchDTO>(bookDB);
            patchDocument.ApplyTo(bookPatchDTO, ModelState);

            bool isValid = TryValidateModel(bookPatchDTO);

            if (!isValid)
            {
                return BadRequest(ModelState);
            }

            mapper.Map(bookPatchDTO, bookDB);
            await dbContext.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}", Name = "deleteBookByIdV1")]
        public async Task<ActionResult> Delete(int id)
        {
            bool bookExist = await dbContext.Books.AnyAsync(x => x.Id == id);

            if (!bookExist)
            {
                return NotFound($"Don't exist a book with id {id}.");
            }

            dbContext.Remove(new Book() { Id = id });
            await dbContext.SaveChangesAsync();
            return NoContent();
        }

        private void SetAuthorsOrder(Book book)
        {
            if (book.BookAuthor != null && book.BookAuthor.Count != 0)
            {
                for (int i = 0; i < book.BookAuthor.Count; i++)
                {
                    book.BookAuthor[i].Order = i;
                }
            }
        }
    }
}
