using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using V1.API.Util;
using V1.API.ViewModels;
using V1.Configuration;
using V1.Configuration.Dominio;
using Newtonsoft.Json;

namespace V1.API.Controllers
{
    [Route("api/Localizacao"), Produces("application/json"), EnableCors("AllowAll")]
    [ApiController]
    public class LocalizacaoController : ControllerBase
    {
        private readonly RedisConfiguration _redis;
        private readonly IRedisConnectionFactory _fact;


        public IConfiguration Configuration { get; }
        public LocalizacaoController(IOptions<RedisConfiguration> redis, IRedisConnectionFactory factory, IConfiguration configuration)
        {
            _redis = redis.Value;
            _fact = factory;
            Configuration = configuration;

            var db = _fact.Connection().GetDatabase();
            //if (!db.KeyExists(Constants.KEY_MOTORISTA_ONLINE))
                Helper.InserirCoordenadas(db);
            //db.GeoAdd(Constants.KEY_MOTORISTA, Helper.GetData().ToArray());

        }

        // GET: api/Motorista
        [HttpGet("{lat}/{lng}/{km}")]
        public async Task<IActionResult> Get(double lat, double lng, int km)
        {
            var db = _fact.Connection().GetDatabase();
            int limit = int.Parse(Configuration.GetSection("limit").Value.ToString());
            var result = await db.GeoRadiusAsync(Constants.KEY_MOTORISTA_ONLINE, lng, lat, km, GeoUnit.Kilometers);
            var lista = new List<Parent>();
            var taskArray = new List<Task>();
            result.OrderBy(v => v.Distance).Take(limit).ToList().ForEach(x =>
             {
                 var task = Task.Run(() =>
                 {
                     lista.Add(Helper.MapsAPICall(x, lat, lng, "driving"));
                 });
                 taskArray.Add(task);
             });

            Task.WaitAll(taskArray.ToArray());

            var listaMotorista = lista.ToList().Where(s => s.rows.Any())
                    .Select(x => new
                    {
                        distancia = x.rows.ToList().Where(g => g.elements.Any()).SelectMany(i => i.elements.ToList().Select(y => y.distance.value)).ToList().FirstOrDefault(),
                        x.status,
                        x.rows,
                        x.destination_addresses,
                        x.origin_addresses,
                        x.CodigoMotorista,
                        x.Latitude,
                        x.Longitude
                    });
            return Ok(new { Count = listaMotorista.Count(), Data = listaMotorista.OrderBy(x => x.distancia).ToList() });
        }

        // GET: api/Motorista/5
        [HttpGet("{id}", Name = "Get")]
        public async Task<IActionResult> Get(int id)
        {
            var db = _fact.Connection().GetDatabase();
            var item = await db.GeoPositionAsync(Constants.KEY_MOTORISTA_ONLINE, id.ToString());

            return Ok(item);
        }

        // POST AND PUT: api/Motorista
        [HttpPost]
        public async void Post([FromBody] CoordenadasMotoristaViewModel value)
        {
            var db = _fact.Connection().GetDatabase();
            if (String.IsNullOrEmpty(value.Codigo.ToString()))
                throw new Exception("Referência é obrigatório.");
            await db.GeoAddAsync(Constants.KEY_MOTORISTA_ONLINE, new GeoEntry(value.Longitude, value.Latitude, value.Codigo.ToString()));

        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public async void Delete(int id)
        {
            var db = _fact.Connection().GetDatabase();
            await db.GeoRemoveAsync(Constants.KEY_MOTORISTA_ONLINE, id.ToString());
        }

        [HttpGet("all")]
        public async Task<IActionResult> All()
        {
            var db = _fact.Connection().GetDatabase();
            var item = await db.GeoRadiusAsync(Constants.KEY_MOTORISTA_ONLINE, -20.19, -40.21, 20000, GeoUnit.Kilometers);

            return Ok(item);
        }

    }


}
