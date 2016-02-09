using System;
using System.Web.Http;
using NHibernate;
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

        public class NewPairDto
        {
            public string Colour { get; set; }
        }
        public IHttpActionResult Post(NewPairDto newPairDto)
        {
            var newSocksPair = new SocksPair((SocksColour)Enum.Parse(typeof(SocksColour), newPairDto.Colour, ignoreCase: true));

            _session.Save(newSocksPair);
            _session.Flush();

            return Created($"api/drawer/{newSocksPair.Id}", newSocksPair);
        }
    }
}