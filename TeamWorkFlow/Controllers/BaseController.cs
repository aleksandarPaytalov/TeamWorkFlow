using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TeamWorkFlow.Controllers
{
    [Authorize]
    public class BaseController : Controller
    {
        
    }
}
