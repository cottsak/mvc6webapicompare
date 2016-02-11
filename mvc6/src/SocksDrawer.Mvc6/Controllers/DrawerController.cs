using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using NHibernate;
using NHibernate.Linq;
using SocksDrawer.Mvc6.Models;

namespace SocksDrawer.Mvc6.Controllers
{
    public class DrawerController:Controller
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
    }
}
