using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SPSB.Data;
using SPSB.Models;

namespace SPSB.Controllers
{
    public class CarrinhoController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CarrinhoController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var cart = SessionHelper.GetObjectFromJson<List<Item>>(HttpContext.Session, "carrinho");
            ViewBag.Cart = cart;
            ViewBag.Total = cart != null ? cart.Sum(item => item.Produto.Preco * item.Quantidade) : 0;
            return View();
        }

        public IActionResult Buy(string id)
        {
            if (SessionHelper.GetObjectFromJson<List<Item>>(HttpContext.Session, "carrinho") == null)
            {
                List<Item> cart = new List<Item>();
                var product = _context.Produto.Find(id);
                cart.Add(new Item { ProdutoId = product.Id, Produto = product, Quantidade = 1, Preco = product.Preco });
                SessionHelper.SetObjectAsJson(HttpContext.Session, "carrinho", cart);
            }
            else
            {
                List<Item> cart = SessionHelper.GetObjectFromJson<List<Item>>(HttpContext.Session, "carrinho");
                int index = isExist(id);
                if (index != -1)
                {
                    cart[index].Quantidade++;
                }
                else
                {
                    var product = _context.Produto.Find(id);
                    cart.Add(new Item { Produto = product, Quantidade = 1, Preco = product.Preco });
                }
                SessionHelper.SetObjectAsJson(HttpContext.Session, "carrinho", cart);
            }
            return RedirectToAction("Index");
        }

        public IActionResult Remove(string id)
        {
            List<Item> cart = SessionHelper.GetObjectFromJson<List<Item>>(HttpContext.Session, "carrinho");
            int index = isExist(id);
            cart.RemoveAt(index);
            SessionHelper.SetObjectAsJson(HttpContext.Session, "carrinho", cart);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult RefreshItem(string id, int quantidadeItem)
        {
            var product = _context.Produto.Find(id);
            if(product != null)
            {
                if(product.Quantidade < quantidadeItem)
                {
                    return BadRequest("Quantidade acima do estoque");
                }
                List<Item> cart = SessionHelper.GetObjectFromJson<List<Item>>(HttpContext.Session, "carrinho");
                int index = isExist(id);
                cart[index].Quantidade = quantidadeItem;
                SessionHelper.SetObjectAsJson(HttpContext.Session, "carrinho", cart);
            }
            return RedirectToAction(nameof(Index));
        }

        private int isExist(string id)
        {
            List<Item> cart = SessionHelper.GetObjectFromJson<List<Item>>(HttpContext.Session, "carrinho");
            for (int i = 0; i < cart.Count; i++)
            {
                if (cart[i].Produto.Id.Equals(id))
                {
                    return i;
                }
            }
            return -1;
        }
    }
}