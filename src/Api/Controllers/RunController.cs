
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Api.Controllers
{
	[Route("[controller]")]
    public class RunController : ControllerBase
    {
		[HttpGet]
		public IActionResult Get()
		{
            return new JsonResult("Dummy");
		}
    }
}
