using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CLDVT2.Models;
using Microsoft.AspNetCore.Http;

namespace CLDVT2.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ABCSTOREContext _context;

        public ProductsController(ABCSTOREContext context)
        {
            _context = context;
        }

        // GET: Products
        public async Task<IActionResult> Index()
        {
            return View(await _context.Product.OrderBy(p => p.ProductName).ToListAsync());
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductId,ProductName,ProductDescription,ProductPrice,ProductImgUrl")] Product product, IFormFile Image)
        {
            if (Image != null)
            {
                if (!Image.ContentType.Contains("image"))
                {
                    ModelState.AddModelError("Image", "Please submit an image file");
                }
            }

            if (ModelState.IsValid)
            {
                _context.Add(product);
                //if there is an image to assign to a product, assign it
                if (Image != null)
                {
                    Console.WriteLine(Image.FileName);
                    product.ProductImgUrl = BlobManager.uploadImage(Image);
                }
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Product.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductId,ProductName,ProductDescription,ProductPrice,ProductImgUrl")] Product product, IFormFile Image)
        {
            if (id != product.ProductId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if (Image != null)
                {
                    if (!Image.ContentType.Contains("image"))
                    {
                        ModelState.AddModelError("Image", "Please submit an image file");
                    }
                }

                try
                {

                    //if there is a new image to assign to a product, upload it and get the URi and add it to the product
                    if (Image != null)
                    {
                        product.ProductImgUrl = BlobManager.updateImage(Image, product.ProductImgUrl);
                    }
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.ProductId))
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
            return View(product);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Product
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Product.FindAsync(id);
            BlobManager.removeBlob(product.ProductImgUrl);
            _context.Product.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Product.Any(e => e.ProductId == id);
        }
    }
}
