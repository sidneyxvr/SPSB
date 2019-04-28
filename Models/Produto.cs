using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SPSB.Models
{
    public class Produto
    {
        public string Id { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public decimal Preco { get; set; }
        public int Quantidade { get; set; }
    }
}
