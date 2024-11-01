using AuthorsWebApi.DTOs;
using AuthorsWebApi.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuthorsWebApi.Controllers
{
    [ApiController]
    [Route("api/authors")]
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

        [HttpGet]
        public async Task<ActionResult<List<AuthorDTO>>> GetAll()
        {
            List<Author> authors = await dbContext.Authors
                .ToListAsync();
            return mapper.Map<List<AuthorDTO>>(authors);
        }

        [HttpGet("{id:int}",Name ="GetAuthorById")]
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

            return mapper.Map<AuthorWithBooksDTO>(author);
        }

        [HttpGet("{name}")]
        public async Task<List<AuthorDTO>> GetAuthorsByName(string name)
        {
            List<Author> authors = await dbContext.Authors
                .Where(x => x.Name.Contains(name))
                .ToListAsync();

            return mapper.Map<List<AuthorDTO>>(authors);
        }


        [HttpPost]
        public async Task<ActionResult> CreateNew(AuthorCreationDTO authorCreationDTO)
        {
            Author author = mapper.Map<Author>(authorCreationDTO);
            dbContext.Add(author);
            await dbContext.SaveChangesAsync();

            AuthorDTO authorDTO = mapper.Map<AuthorDTO>(author);
            return CreatedAtRoute("GetAuthorById",new {id = author.Id}, authorDTO);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Update(AuthorCreationDTO authorCreationDTO, int id)
        {

            bool authorExist = await dbContext.Authors.AnyAsync(author => author.Id == id);

            if (!authorExist) {
                return NotFound($"The author with id {id} don't exist.");
            }

            Author author = mapper.Map<Author>(authorCreationDTO);
            author.Id = id;

            dbContext.Authors.Update(author);
            await dbContext.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            bool authorExist = await dbContext.Authors.AnyAsync(author => author.Id == id);

            if (!authorExist)
            {
                return NotFound($"The author with id {id} don't exist.");
            }

            dbContext.Remove(new Author() { Id = id});
            await dbContext.SaveChangesAsync();
            return Ok();
        }
        
    }
}
