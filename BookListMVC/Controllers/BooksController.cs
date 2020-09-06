using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookListMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookListMVC.Controllers
{
    public class BooksController : Controller
    {
        private readonly ApplicationDbContext _db;

        [BindProperty]
        public Book Book { get; set; }

        public BooksController(ApplicationDbContext db)
        {
            this._db = db;
        }
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// This view is able to create and edit books.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult Upsert(int? id)
        {
            Book = new Book();
            if (id == null)
            { //No id provided, which means that the user is trying to add a new book.
                return View(Book);
            }
            // The user is trying to update an existing book.
            Book = _db.Books.FirstOrDefault(u => u.Id == id); //Look for the book in the DB instance.
            if (Book == null)
            { //The book doesn't exist anymore.
                return NotFound();
            }

            return View(Book);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert()
        {
            if (ModelState.IsValid)
            {
                if (Book.Id == 0)
                { // Id is 0, so this is a new book
                    _db.Books.Add(Book);
                }
                else
                { // Otherwise, this is an existing book being updated.
                    _db.Books.Update(Book);
                }
                _db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(Book);
        }

        #region API CALLS
        [HttpGet] // .../books/getall/
        public async Task<IActionResult> GetAll()
        {
            return Json(new { data = await _db.Books.ToListAsync() });
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var bookFromDb = await _db.Books.FirstOrDefaultAsync(u => u.Id == id);
            if (bookFromDb == null)
            {
                return Json(new { success = false, message = "Error while deleting!" });
            }
            _db.Books.Remove(bookFromDb);
            await _db.SaveChangesAsync();
            return Json(new { sucess = true, message = "Delete successful!" });
        }
        #endregion

    }
}
