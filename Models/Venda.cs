using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SPSB.Models
{
    public class Venda
    {
        public string Id { get; set; }
        public string UsuarioId { get; set; }
        public virtual Usuario Usuario { get; set; }

        [Display(Name = "Data da Compra")]
        public DateTime DataCompra { get; set; }
        public string Cartao { get; set; }

        [Display(Name = "Valor Total")]
        public decimal ValorTotal { get; set; }
        public virtual List<Item> Itens { get; set; }
    }
}
