using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Jinkong.RC.Config.Demo.Controllers
{
    [ApiController]
    public class DemoController : ControllerBase
    {
        [HttpGet("/demo")]
        public object Test([FromServices] IConfiguration configuration)
        {
            return configuration.GetValue<string>("Demo");
        }
    }
}
