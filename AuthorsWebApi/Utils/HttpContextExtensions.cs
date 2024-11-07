using Microsoft.EntityFrameworkCore;

namespace AuthorsWebApi.Utils
{
    public static class HttpContextExtensions
    {
        public async static Task InsertTotalRecordsInHeader<T>(this HttpContext httpContext,
            IQueryable<T> queryable)
        {
            if (httpContext == null) {  throw new ArgumentNullException(nameof(httpContext));}

            int amount = await queryable.CountAsync();
            httpContext.Response.Headers.Add("totalRecords", amount.ToString());
        }
    }
}
