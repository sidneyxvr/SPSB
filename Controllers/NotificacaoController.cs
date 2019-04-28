using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SPSB.Data;
using SPSB.Models;

namespace SPSB.Controllers
{
    public class NotificacaoController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Usuario> _userManager;

        public NotificacaoController(ApplicationDbContext context, UserManager<Usuario> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        [Authorize(Roles = "Usuario")]
        public async Task<IActionResult> Create(string id)
        {
            var product = _context.Produto.AsNoTracking().SingleOrDefault(p => p.Id == id);
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var notification = new Notificacao() { ProdutoId = id, Produto = product, UsuarioId = user.Id, Usuario = user };
            return View(notification);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Usuario")]
        public IActionResult Create([Bind("UsuarioId, ProdutoId")]Notificacao notificacao)
        {
            notificacao.DataCadastrado = DateTime.Now;
            notificacao.Enviado = false;
            var not = _context.Notificacao.Add(notificacao);
            _context.SaveChanges();
            return RedirectToAction("Index", "Dashboard");
        }
    }
}