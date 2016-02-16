using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Mvc;
using NHibernate;
using NHibernate.Linq;
using SocksDrawer.Mvc6.Models;

namespace SocksDrawer.Mvc6.Controllers
{
    [Route("api/[controller]")]
    public class DrawerController : Controller
    {
        private readonly ISession _session;

        public DrawerController(ISession session)
        {
            _session = session;
        }

        [HttpGet("socks")]
        public IEnumerable<SocksPair> Get()
        {
            return _session.Query<SocksPair>().ToList();
        }

        public class NewPairDto
        {
            public string Colour { get; set; }
        }
        public IActionResult Post([FromBody]NewPairDto newPairDto)
        {
            var newSocksPair = new SocksPair((SocksColour)Enum.Parse(typeof(SocksColour), newPairDto.Colour, ignoreCase: true));

            //// yes, this is domain logic and shouldn't live in the controller
            //if (_session.Query<SocksPair>().Count(p => p.Colour == SocksColour.White) == 6
            //    && newSocksPair.Colour == SocksColour.White)
            //    return Content(HttpStatusCode.Forbidden, "Maximum of 6 white pairs in the drawer at one time.");

            _session.Save(newSocksPair);

            return Created($"api/drawer/{newSocksPair.Id}", newSocksPair);
        }
    }
}
