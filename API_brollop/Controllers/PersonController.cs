using DataBase;
using DataBase.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;

namespace API_brollop.Controllers
{
    [RoutePrefix("person"), EnableCors(origins: "http://localhost:9000", headers: "*", methods: "*")]
    public class PersonController : ApiController
    {
        [Route("registration"),HttpPost]
        public IHttpActionResult Post(CompanyPostDto persons)
        {
            using (var helper = new DataBaseHelper())
            {
                helper.RegisterCompany(persons);
                return Ok();
            }
        }

    }
}