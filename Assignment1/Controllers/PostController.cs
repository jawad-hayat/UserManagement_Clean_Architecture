using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Assignment1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PostController : ControllerBase
    {
        [HttpGet("posts")]
        public async Task<IActionResult> GetPosts()
        {

            var posts = new List<dynamic>();
            var url = "https://jsonplaceholder.typicode.com/posts/";
            posts = await Common.Utilities.Utility.GetData(url);
            return Ok(posts);

        }
    }
}
