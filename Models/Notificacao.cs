using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SPSB.Models
{
    public class Notificacao
    {
        public string Id { get; set; }

        public string UsuarioId { get; set; }
        public Usuario Usuario { get; set; }

        public string ProdutoId { get; set; }
        public Produto Produto { get; set; }

        public DateTime DataCadastrado { get; set; }

        public bool Enviado { get; set; }
    }
}
