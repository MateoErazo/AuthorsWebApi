using AuthorsWebApi.DTOs;
using AuthorsWebApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AuthorsWebApi.Filters
{
    public class HATEOASAuthorFilterAttribute: HATEOASFilterAttribute
    {
        private readonly LinksGenerator linksGenerator;

        public HATEOASAuthorFilterAttribute(LinksGenerator linksGenerator)
        {
            this.linksGenerator = linksGenerator;
        }
        public override async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            bool mustInclude = MustIncludeHATEOAS(context);

            if (!mustInclude) { 
                await next();
                return;
            }

            var result = context.Result as ObjectResult;
            
            var authorDTO = result.Value as AuthorDTO;

            if (authorDTO == null) {

                var authorsDTO = result.Value as List<AuthorDTO> ??
                    throw new ArgumentException("An instance of AuthorDTO or List<AuthorDTO> was expected.");

                authorsDTO.ForEach(author => linksGenerator.GenerateLinks(author));
                result.Value = authorsDTO;
            }
            else
            {
                linksGenerator.GenerateLinks(authorDTO);
            }

            await next();
        }
    }
}
