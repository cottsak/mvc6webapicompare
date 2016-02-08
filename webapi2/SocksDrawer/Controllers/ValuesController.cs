using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SocksDrawer.Controllers
{
    public class ValuesController : ApiController
    {
        public object Get()
        {
            return Json(new { header = "yo!", values = new[] { "val1", "val2" } });
        }
    }
}
