using System;
using System.Web.Http;
using SocksDrawer.Models;

namespace SocksDrawer.Controllers
{
    public class DrawerController : ApiController
    {
        private readonly ISocksDrawerRepository _socksDrawerRepository;

        public DrawerController(ISocksDrawerRepository socksDrawerRepository)
        {
            _socksDrawerRepository = socksDrawerRepository;
        }

        public class NewPairDto
        {
            public string Colour { get; set; }
        }
        public IHttpActionResult Post(NewPairDto newPairDto)
        {
            var socksPair = new SocksPair((SocksColour)Enum.Parse(typeof(SocksColour), newPairDto.Colour, ignoreCase: true));
            var pair = _socksDrawerRepository.AddPair(
                socksPair);

            return Created($"api/drawer/{pair.Id}", pair);
        }
    }
}