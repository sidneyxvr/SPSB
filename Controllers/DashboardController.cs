using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SPSB.Data;
using SPSB.Models;

namespace SPSB.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Usuario> _userManager;

        public DashboardController(ApplicationDbContext context, UserManager<Usuario> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Dashboard
        public ActionResult Index()
        {
            return View(_context.Produto.OrderByDescending(p => p.Quantidade));
        }
               
        [Authorize(Roles = "Usuario")]
        public async Task<ActionResult> Buy()
        {
            var valorTotal = SessionHelper.GetObjectFromJson<List<Item>>(HttpContext.Session, "carrinho").Sum(item => item.Quantidade * item.Produto.Preco);
            var user = await _userManager.GetUserAsync(HttpContext.User);
            return View(new Venda() { UsuarioId = user.Id, Usuario = user, ValorTotal = valorTotal });
        }
    }
}