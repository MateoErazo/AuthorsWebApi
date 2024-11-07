using AuthorsWebApi.DTOs;
using AuthorsWebApi.Entities;
using AuthorsWebApi.Filters;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuthorsWebApi.Controllers.V1
{
    [ApiController]
    [Route("api/v1/authors")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class AuthorsController : ControllerBase
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IMapper mapper;

        public AuthorsController(ApplicationDbContext dbContext, IMapper mapper)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
        }

        [HttpGet(Name = "getAuthorsV1")]
        [ServiceFilter(typeof(HATEOASAuthorFilterAttribute))]
        public async Task<ActionResult<List<AuthorDTO>>> GetAll()
        {
            List<Author> authors = await dbContext.Authors
                .ToListAsync();

            return mapper.Map<List<AuthorDTO>>(authors);
        }

        [HttpGet("{id:int}", Name = "getAuthorByIdV1")]
        [ServiceFilter(typeof(HATEOASAuthorFilterAttribute))]
        public async Task<ActionResult<AuthorWithBooksDTO>> GetById(int id)
        {
            Author author = await dbContext.Authors
                .Include(x => x.BookAuthor)
                .ThenInclude(x => x.Book)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (author == null)
            {
                return NotFound($"Don't exist an author with id {id}.");
            }

            author.BookAuthor = author.BookAuthor.OrderBy(x => x.Order).ToList();

            AuthorWithBooksDTO authors = mapper.Map<AuthorWithBooksDTO>(author);

            return authors;
        }

        [HttpGet("{name}", Name = "getAuthorsByNameV1")]
        public async Task<List<AuthorDTO>> GetAuthorsByName(string name)
        {
            List<Author> authors = await dbContext.Authors
                .Where(x => x.Name.Contains(name))
                .ToListAsync();

            return mapper.Map<List<AuthorDTO>>(authors);
        }


        [HttpPost(Name = "createAuthorV1")]
        public async Task<ActionResult> CreateNew(AuthorCreationDTO authorCreationDTO)
        {
            Author author = mapper.Map<Author>(authorCreationDTO);
            dbContext.Add(author);
            await dbContext.SaveChangesAsync();

            AuthorDTO authorDTO = mapper.Map<AuthorDTO>(author);
            return CreatedAtRoute("getAuthorByIdV1", new { id = author.Id }, authorDTO);
        }

        [HttpPut("{id:int}", Name = "updateAuthorByIdV1")]
        public async Task<ActionResult> Update(AuthorCreationDTO authorCreationDTO, int id)
        {

            bool authorExist = await dbContext.Authors.AnyAsync(author => author.Id == id);

            if (!authorExist)
            {
                return NotFound($"The author with id {id} don't exist.");
            }

            Author author = mapper.Map<Author>(authorCreationDTO);
            author.Id = id;

            dbContext.Authors.Update(author);
            await dbContext.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>
        /// Delete an author by their Id.
        /// </summary>
        /// <param name="id">Author Id to delete.</param>
        /// <returns></returns>
        [HttpDelete("{id:int}", Name = "deleteAuthorByIdV1")]
        public async Task<ActionResult> Delete(int id)
        {
            bool authorExist = await dbContext.Authors.AnyAsync(author => author.Id == id);

            if (!authorExist)
            {
                return NotFound($"The author with id {id} don't exist.");
            }

            dbContext.Remove(new Author() { Id = id });
            await dbContext.SaveChangesAsync();
            return Ok();
        }

    }
}
