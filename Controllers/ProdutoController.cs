using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SPSB.Data;
using SPSB.Models;

namespace SPSB.Controllers
{
    
    public class ProdutoController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProdutoController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Produto
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Produto.ToListAsync());
        }
        
        // GET: Produto/Details/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var produto = await _context.Produto
                .FirstOrDefaultAsync(m => m.Id == id);
            if (produto == null)
            {
                return NotFound();
            }

            return View(produto);
        }

        // GET: Produto/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Produto/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Id,Nome,Descricao,Preco,Quantidade")] Produto produto)
        {
            if (ModelState.IsValid)
            {
                _context.Add(produto);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(produto);
        }

        // GET: Produto/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var produto = await _context.Produto.FindAsync(id);
            if (produto == null)
            {
                return NotFound();
            }
            return View(produto);
        }

        // POST: Produto/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Nome,Descricao,Preco,Quantidade")] Produto produto)
        {
            if (id != produto.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(produto);
                    await _context.SaveChangesAsync();
                    if(produto.Quantidade > 0)
                    {
                        PushNotification(produto.Id);
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProdutoExists(produto.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(produto);
        }

        // GET: Produto/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var produto = await _context.Produto
                .FirstOrDefaultAsync(m => m.Id == id);
            if (produto == null)
            {
                return NotFound();
            }

            return View(produto);
        }

        // POST: Produto/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var produto = await _context.Produto.FindAsync(id);
            _context.Produto.Remove(produto);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProdutoExists(string id)
        {
            return _context.Produto.Any(e => e.Id == id);
        }

        [HttpGet]
        public IActionResult BestSellers()
        {
            var list = _context.Item.Include(i => i.Produto).GroupBy(i => i.Produto.Nome).OrderByDescending(i => i.Sum(s => s.Quantidade)).OrderByDescending(i => i.Sum(s => s.Quantidade * s.Preco));//.Select(i => new { Nome = i.Produto.Nome, Qtde = i.Sum(s => s.Quantidade), Preco = i.Produto.Preco });
            var pretty = list.Select(i => new { Nome = i.Key, Qtde = i.Sum(s => s.Quantidade), ValorTotal = (i.Sum(s => s.Quantidade) * i.Select(s => s.Preco).First()) });
            List<Tuple<string, string, string>> tupleList = new List<Tuple<string, string, string>>();
            foreach (var i in pretty)
            {
                tupleList.Add(new Tuple<string, string, string>(i.Nome, i.Qtde.ToString(), i.ValorTotal.ToString()));
            }
            return View(tupleList);
        }

        public void PushNotification(string id)
        {
            var notifications = _context.Notificacao.Include(n => n.Usuario).Include(n => n.Produto).Where(n => n.ProdutoId == id);
            foreach (var notification in notifications)
            {
                var email = notification.Usuario.Email;
                var produto = notification.Produto.Nome;
                //disparar notificações para os emails dos usuarios com o nome do produto
            }
        }
    }
}
