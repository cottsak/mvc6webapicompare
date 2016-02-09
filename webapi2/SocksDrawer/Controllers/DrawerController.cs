using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using NHibernate;
using NHibernate.Linq;
using SocksDrawer.Models;

namespace SocksDrawer.Controllers
{
    public class DrawerController : ApiController
    {
        private readonly ISession _session;

        public DrawerController(ISession session)
        {
            _session = session;
        }

        [Route("api/drawer/socks")]
        public IEnumerable<SocksPair> Get()
        {
            return _session.Query<SocksPair>().ToList();
        }

        public IHttpActionResult Get(int id)
        {
            var pair = _session.Query<SocksPair>().SingleOrDefault(p => p.Id == id);
            if (pair == null)
                return NotFound();

            return Ok(pair);
        }

        public class NewPairDto
        {
            public string Colour { get; set; }
        }
        public IHttpActionResult Post(NewPairDto newPairDto)
        {
            var newSocksPair = new SocksPair((SocksColour)Enum.Parse(typeof(SocksColour), newPairDto.Colour, ignoreCase: true));

            // yes, this is domain logic and shouldn't live in the controller
            if (_session.Query<SocksPair>().Count(p => p.Colour == SocksColour.White) == 6
                && newSocksPair.Colour == SocksColour.White)
                return Content(HttpStatusCode.Forbidden, "Maximum of 6 white pairs in the drawer at one time.");

            _session.Save(newSocksPair);
            _session.Flush();

            return Created($"api/drawer/{newSocksPair.Id}", newSocksPair);
        }

        public IHttpActionResult Delete(int id)
        {
            var pairToDeltete = _session.Query<SocksPair>().SingleOrDefault(p => p.Id == id);
            if (pairToDeltete == null)
                return NotFound();

            _session.Delete(pairToDeltete);
            return Ok();
        }
    }
}