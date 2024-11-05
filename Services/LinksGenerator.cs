using AuthorsWebApi.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using System;

namespace AuthorsWebApi.Services
{
    public class LinksGenerator
    {
        private readonly IActionContextAccessor actionContextAccessor;
        private readonly IHttpContextAccessor httpContextAccessor;

        public LinksGenerator(IActionContextAccessor actionContextAccessor, IHttpContextAccessor httpContextAccessor)
        {
            this.actionContextAccessor = actionContextAccessor;
            this.httpContextAccessor = httpContextAccessor;
        }

        private IUrlHelper BuildUrlHelper()
        {
            var factory = httpContextAccessor.HttpContext.RequestServices.GetRequiredService<IUrlHelperFactory>();
            return factory.GetUrlHelper(actionContextAccessor.ActionContext);
        }
        public void GenerateLinks(AuthorDTO authorDTO)
        {
            var Url = BuildUrlHelper();

            authorDTO.Links.Add(new DataHATEOAS(
                link: Url.Link("getAuthorById", new { id = authorDTO.Id }),
                description: "self",
                method: "GET"));
            authorDTO.Links.Add(new DataHATEOAS(
                link: Url.Link("getAuthorsByName", new { name = authorDTO.Name }),
                description: "author-get",
                method: "GET"));
            authorDTO.Links.Add(new DataHATEOAS(
                link: Url.Link("updateAuthorById", new { id = authorDTO.Id }),
                description: "author-update",
                method: "PUT"));
            authorDTO.Links.Add(new DataHATEOAS(
                link: Url.Link("deleteAuthorById", new { id = authorDTO.Id }),
                description: "author-delete",
                method: "DELETE"));
        }
    }
}
