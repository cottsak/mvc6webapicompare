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
        private readonly IValuesGetter _valuesGetter;

        public ValuesController(IValuesGetter valuesGetter)
        {
            _valuesGetter = valuesGetter;
        }

        public object Get()
        {
            return _valuesGetter.GetValues();
        }
    }

    public interface IValuesGetter
    {
        string[] GetValues();
    }

    class ValuesGetter : IValuesGetter
    {
        public string[] GetValues()
        {
            return new[] { "val1", "val2" };
        }
    }
}
