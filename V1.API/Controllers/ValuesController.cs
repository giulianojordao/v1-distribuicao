using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using V1.API.Util;
using V1.Configuration;
using V1.Configuration.Dominio;

namespace V1.API.Controllers
{
    [Route("api/Values"), Produces("application/json"), EnableCors("AllowAll")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly RedisConfiguration _redis;
        //private readonly IDistributedCache _cache;
        private readonly IRedisConnectionFactory _fact;

        public ValuesController(IOptions<RedisConfiguration> redis, IRedisConnectionFactory factory)
        {
            _redis = redis.Value;
            // _cache = cache;
            _fact = factory;
        }

        // GET api/values
        [HttpGet]
        public Object Get()
        {

            return null;
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {


            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post()
        {
            try
            {
                var db = _fact.Connection().GetDatabase();


                //db.StringSet("StackExchange.Redis.Key", "Stack Exchange Redis is Awesome");
                //var aa = db.StringGet("StackExchange.Redis.Key");
                var redis = new RedisMotoristaService<Motorista>(this._fact);
                var model = redis.GetAll("motorista");


                for (int i = 0; i < 1000; i++)
                {
                    var motorista = new Motorista();
                    motorista.Latitude = 23234;
                    motorista.Longitude = 234234;
                    motorista.Nome = "Lionan Dantas";
                    redis.Save(i.ToString(), motorista);

                    // redis.Save("RedisMotorista" + i.ToString(), motorista);

                    //var model = redis.Get("RedisMotorista");
                }
                // List<Motorista> teste_ = redis.GetAllByKey<Motorista>("teste");
            }
            catch (Exception ex)
            {

            }
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
