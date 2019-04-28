using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SPSB.Data;
using SPSB.Models;

namespace SPSB.Controllers
{
    public class VendaController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Usuario> _userManager;

        public VendaController(ApplicationDbContext context, UserManager<Usuario> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Venda
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index(string initialDate, string finalDate)
        {
            if(initialDate != null && finalDate != null)
            {
                DateTime date1 = DateTime.Parse(initialDate);
                DateTime date2 = DateTime.Parse(finalDate).AddHours(23).AddMinutes(59).AddSeconds(59);
                ViewBag.Vendas = _context.Venda.Where(v => v.DataCompra >= date1 && v.DataCompra <= date2).Count();
                ViewBag.Periodo = date1.ToString("dd/MM/yyyy") + " à " + date2.ToString("dd/MM/yyyy");
                var listDate = _context.Venda.Include(v => v.Usuario).Include(v => v.Itens).Where(v => v.DataCompra >= date1 && v.DataCompra <= date2).OrderByDescending(v => v.DataCompra);
                return View(await listDate.ToListAsync());
            }
            ViewBag.Vendas = _context.Venda.Count();
            ViewBag.Periodo = "completo";
            var list = _context.Venda.Include(v => v.Usuario).Include(v => v.Itens).OrderByDescending(v => v.DataCompra);
            return View(await list.ToListAsync());
        }

        [Authorize(Roles = "Usuario")]
        public async Task<IActionResult> Purchase()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var list = _context.Venda.Include(v => v.Usuario).Include(v => v.Itens).OrderByDescending(v => v.DataCompra).Where(v => v.UsuarioId == user.Id);
            return View(await list.ToListAsync());
        }

        // GET: Venda/Details/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var venda = await _context.Venda
                .Include(v => v.Usuario)
                .Include(v => v.Itens)
                    .ThenInclude(i => i.Produto)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (venda == null)
            {
                return NotFound();
            }

            return View(venda);
        }

        [Authorize(Roles = "Usuario")]
        public async Task<IActionResult> PurchaseDetails(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var venda = await _context.Venda
                .Include(v => v.Usuario)
                .Include(v => v.Itens)
                    .ThenInclude(i => i.Produto)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (venda == null)
            {
                return NotFound();
            }

            return View(venda);
        }

        // POST: Venda/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Usuario")]
        public async Task<IActionResult> Create([Bind("Id,UsuarioId,Cartao,ValorTotal")] Venda venda)
        {
            if (ModelState.IsValid)
            {
                venda.DataCompra = DateTime.Now;
                var purchase = _context.Venda.Add(venda);
                if (!AddVendaItem(purchase.Entity.Id))
                {
                    return RedirectToAction(nameof(Index));
                }
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Dashboard");
            }
            ViewData["UsuarioId"] = new SelectList(_context.Set<Usuario>(), "Id", "Id", venda.UsuarioId);
            return View(venda);
        }

        private bool VendaExists(string id)
        {
            return _context.Venda.Any(e => e.Id == id);
        }

        private bool AddVendaItem(string vendaId)
        {
            try
            {
                var cart = SessionHelper.GetObjectFromJson<List<Item>>(HttpContext.Session, "carrinho");
                foreach (var i in cart)
                {
                    _context.Item.Add(new Item() { VendaId = vendaId, ProdutoId = i.Produto.Id, Preco = i.Preco, Quantidade = i.Quantidade });
                    var product = _context.Produto.Find(i.Produto.Id);
                    product.Quantidade -= i.Quantidade;
                    _context.Produto.Update(product);
                }
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
