using DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;

namespace API_brollop.Controllers
{
    [RoutePrefix("admin"), EnableCors(origins: "http://localhost:9000", headers: "*", methods: "*")]
    public class AdminController : ApiController
    {
        [Route("{password}")]
        public IHttpActionResult Get(string password)
        {
            if (password == "JA2018-pass")
                return Ok();
            return BadRequest();
        }

        [Route("loading"),HttpGet]
        public IHttpActionResult GetLoadings()
        {
            using (var helper = new DataBaseHelper())
            {
                var loadings = helper.GetNumberOfLoadings();
                return Ok(loadings);
            }
        }
    }
}