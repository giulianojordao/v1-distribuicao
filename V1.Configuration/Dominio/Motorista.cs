using System;
using System.Collections.Generic;
using System.Text;

namespace V1.Configuration.Dominio
{
    public class Motorista
    {
        //public int Codigo { get; set; }
        public Double Latitude { get; set; }
        public Double Longitude { get; set; }
        public string Nome { get; set; }
       /* public string TipoServico { get; set; }
        public string Telefone { get; set; }
        public string Status { get; set; }*/

    }
    public class MotoristaSolicitacao
    {
        public int motoristaId { get; set; }
        public int solicitacaoId { get; set; }


    }
}
