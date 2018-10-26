using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace V1.API.ViewModels
{
    public class MotoristaViewModel
    {
        public int Codigo { get; set; }
        public string Nome { get; set; }
        public string Telefone { get; set; }
        public string TipoServico { get; set; }
        public string UrlFoto { get; set; }
        public string Status { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
