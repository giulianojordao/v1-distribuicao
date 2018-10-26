using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using StackExchange.Redis;
using V1.API.Util;
using V1.API.ViewModels;
using V1.Configuration;
using V1.Configuration.Dominio;

namespace V1.API.Controllers
{
    [Route("api/Motorista"), Produces("application/json"), EnableCors("AllowAll")]
    [ApiController]
    public class MotoristaController : ControllerBase
    {
        private readonly RedisConfiguration _redis;
        private readonly IRedisConnectionFactory _fact;

        public MotoristaController(IOptions<RedisConfiguration> redis, IRedisConnectionFactory factory)
        {
            _redis = redis.Value;
            _fact = factory;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var db = _fact.Connection().GetDatabase();
                var queryResult = await db.HashValuesAsync(Constants.KEY_MOTORISTA_LOGADO);

                if (!queryResult.Any())
                    return NotFound();

                var collectionJson = string.Join(",", queryResult.ToStringArray());

                return Ok(JsonConvert.DeserializeObject<MotoristaViewModel[]>("[" + collectionJson + "]"));
            }
            catch (Exception ex)
            {
                return StatusCode((int)System.Net.HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var db = _fact.Connection().GetDatabase();
                var queryResult = await db.HashGetAsync(Constants.KEY_MOTORISTA_LOGADO, id);

                if (queryResult.IsNullOrEmpty)
                    return NotFound();

                return Ok(JsonConvert.DeserializeObject<MotoristaViewModel>(queryResult.ToString()));
            }
            catch (Exception ex)
            {
                return StatusCode((int)System.Net.HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Post(MotoristaViewModel model)
        {
            try
            {
                var db = _fact.Connection().GetDatabase();
                await db.HashSetAsync(Constants.KEY_MOTORISTA_LOGADO, model.Codigo.ToString(), JsonConvert.SerializeObject(model));

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode((int)System.Net.HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var db = _fact.Connection().GetDatabase();
                await db.HashDeleteAsync(Constants.KEY_MOTORISTA_LOGADO, id.ToString());

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode((int)System.Net.HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpGet("carga")]
        public IActionResult carga()
        {
            Helper.InserirMotoristas(_fact.Connection().GetDatabase());

            return Ok();
        }
    }
}
