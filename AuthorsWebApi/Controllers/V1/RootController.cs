using AuthorsWebApi.DTOs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthorsWebApi.Controllers.V1
{
    [ApiController]
    [Route("api/v1")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class RootController : ControllerBase
    {
        private readonly IAuthorizationService authorizationService;

        public RootController(IAuthorizationService authorizationService)
        {
            this.authorizationService = authorizationService;
        }

        [HttpGet(Name = "getRootsV1")]
        public async Task<ActionResult<IEnumerable<DataHATEOAS>>> GetRoots()
        {
            List<DataHATEOAS> data = new List<DataHATEOAS>();

            var isAdmin = await authorizationService.AuthorizeAsync(User, "IsAdmin");

            data.Add(new DataHATEOAS(
                link: Url.Link("getRootsV1", new { }), description: "self", method: "GET"
            ));

            data.Add(new DataHATEOAS(
                link: Url.Link("getAuthorsV1", new { }), description: "authors-list", method: "GET"
            ));

            data.Add(new DataHATEOAS(
                link: Url.Link("createAuthorV1", new { }), description: "author-create", method: "POST"
            ));

            data.Add(new DataHATEOAS(
                link: Url.Link("getBooksV1", new { }), description: "books-list", method: "GET"
            ));

            data.Add(new DataHATEOAS(
                link: Url.Link("createBookV1", new { }), description: "book-create", method: "POST"
            ));

            data.Add(new DataHATEOAS(
                link: Url.Link("createLoginV1", new { }), description: "login-create", method: "POST"
            ));

            if (isAdmin.Succeeded)
            {

                data.Add(new DataHATEOAS(
                    link: Url.Link("createAccountV1", new { }), description: "account-create", method: "POST"
                ));

                data.Add(new DataHATEOAS(
                    link: Url.Link("getRefreshTokenV1", new { }), description: "token-refresh", method: "GET"
                ));

                data.Add(new DataHATEOAS(
                    link: Url.Link("createAdminV1", new { }), description: "admin-create", method: "POST"
                ));

                data.Add(new DataHATEOAS(
                    link: Url.Link("deleteAdminV1", new { }), description: "admin-delete", method: "POST"
                ));
            }


            return data;
        }

    }
}
