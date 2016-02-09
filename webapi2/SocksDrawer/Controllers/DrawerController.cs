﻿using System;
using System.Collections.Generic;
using System.Linq;
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