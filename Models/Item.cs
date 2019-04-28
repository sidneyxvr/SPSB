using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SPSB.Models
{
    public class Item
    {
        public string Id { get; set; }

        public string ProdutoId { get; set; }
        public Produto Produto { get; set; }
        public decimal Preco { get; set; }
        public int Quantidade { get; set; }
        
        public string VendaId { get; set; }
        public virtual Venda Venda { get; set; }
    }
}
