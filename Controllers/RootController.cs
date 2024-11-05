using AuthorsWebApi.DTOs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthorsWebApi.Controllers
{
    [ApiController]
    [Route("api")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class RootController:ControllerBase
    {
        private readonly IAuthorizationService authorizationService;

        public RootController(IAuthorizationService authorizationService)
        {
            this.authorizationService = authorizationService;
        }

        [HttpGet(Name = "getRoots")]
        public async Task<ActionResult<IEnumerable<DataHATEOAS>>> GetRoots()
        {
            List<DataHATEOAS> data = new List<DataHATEOAS>();

            var isAdmin = await authorizationService.AuthorizeAsync(User, "IsAdmin");
            
            data.Add(new DataHATEOAS (
                link: Url.Link("getRoots", new {}), description: "self", method:"GET"
            ));

            data.Add(new DataHATEOAS(
                link: Url.Link("getAuthors", new { }), description: "authors-list", method: "GET"
            ));

            data.Add(new DataHATEOAS(
                link: Url.Link("createAuthor", new { }), description: "author-create", method: "POST"
            ));

            data.Add(new DataHATEOAS(
                link: Url.Link("getBooks", new { }), description: "books-list", method: "GET"
            ));

            data.Add(new DataHATEOAS(
                link: Url.Link("createBook", new { }), description: "book-create", method: "POST"
            ));

            data.Add(new DataHATEOAS(
                link: Url.Link("createLogin", new { }), description: "login-create", method: "POST"
            ));

            if (isAdmin.Succeeded) {

                data.Add(new DataHATEOAS(
                    link: Url.Link("createAccount", new { }), description: "account-create", method: "POST"
                ));

                data.Add(new DataHATEOAS(
                    link: Url.Link("getRefreshToken", new { }), description: "token-refresh", method: "GET"
                ));

                data.Add(new DataHATEOAS(
                    link: Url.Link("createAdmin", new { }), description: "admin-create", method: "POST"
                ));

                data.Add(new DataHATEOAS(
                    link: Url.Link("deleteAdmin", new { }), description: "admin-delete", method: "POST"
                ));
            }


            return data;
        }

    }
}
