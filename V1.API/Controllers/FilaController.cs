using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using V1.API.ViewModels;
using V1.Configuration;
using V1.API.Util;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Cors;

namespace V1.API.Controllers
{
    [Route("api/Fila"), Produces("application/json"), EnableCors("AllowAll")]
    [ApiController]
    public class FilaController : Controller
    {
        private readonly RedisConfiguration _redis;
        private readonly IRedisConnectionFactory _fact;


        public IConfiguration Configuration { get; }
        public FilaController(IOptions<RedisConfiguration> redis, IRedisConnectionFactory factory, IConfiguration configuration)
        {
            _redis = redis.Value;
            _fact = factory;
            Configuration = configuration;
        }


        [HttpPost("Aceite/{motoristaId}/{solicitacaoId}")]
        public void Post(int motoristaId, int solicitacaoId)
        {
            Boolean emTeste = true;
            var db = _fact.Connection().GetDatabase();

            if (emTeste)
            {
                Helper.InserirMotoristaSolicitacao(_fact, motoristaId, solicitacaoId);
            }
            else
            {
                MotoristaSolicitacaoViewModel motorista = new MotoristaSolicitacaoViewModel { motoristaId = motoristaId, solicitacaoId = solicitacaoId };
                db.ListRightPush(Constants.KEY_MOTORISTA_SOLICITACAO, JsonConvert.SerializeObject(motorista).ToString());
            }
            
        }

        [HttpGet("Aceite")]
        public object Get()
        {
            var db = _fact.Connection().GetDatabase();
            return db.ListLeftPop(Constants.KEY_MOTORISTA_SOLICITACAO);

        }
    }
}