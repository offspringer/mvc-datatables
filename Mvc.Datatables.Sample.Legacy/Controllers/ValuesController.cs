using AutoFixture;
using Mvc.Datatables.Processing;
using Mvc.Datatables.Sample.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web.Http;

namespace Mvc.Datatables.Sample.Controllers
{
    public class ValuesController : ApiController
    {
        private static readonly ICollection<UserProfile> UserProfiles = new Collection<UserProfile>();

        static ValuesController()
        {
            foreach (var userProfile in new Fixture().CreateMany<UserProfile>(50))
            {
                UserProfiles.Add(userProfile);
            }
        }

        // GET api/values
        public ICollection<UserProfile> Get([FromBody] FilterRequest filter)
        {
            return UserProfiles;
        }

        // GET api/values/5
        public UserProfile Get(int id)
        {
            return UserProfiles.FirstOrDefault(x => x.Id == id);
        }

        // POST api/values
        public IHttpActionResult Post([FromBody] FilterRequest filter)
        {
            IDataTableFilterProcessor filterProcessor = new DataTableFilterProcessor();
            IDataTableProcessor processor = new DataTableProcessor(filterProcessor);
            IPageResponse<UserProfile> response = processor.Process(UserProfiles.AsQueryable(), filter,
                (x) => x.Where(y => y.BirthDate > new DateTime(2014, 01, 01)));

            return Ok(response);
        }
    }
}
