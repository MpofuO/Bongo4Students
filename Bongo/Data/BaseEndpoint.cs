using System.Net.Http.Headers;

namespace Bongo.Data
{
    public class BaseEndpoint : IBaseEndpoint
    {
        public string URI { get; set; }
        protected static HttpClient Client;
        public BaseEndpoint(IHttpContextAccessor httpContextAccessor)
        {
            URI = "https://localhost:7098/";
            Client = new HttpClient();

            var httpContext = httpContextAccessor.HttpContext;
            if (httpContext != null)
            {
                Client.DefaultRequestHeaders.Authorization = 
                    new AuthenticationHeaderValue("Bearer", httpContext.Request.Cookies["userToken"]);
            }
        }
    }
}
