using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LibraryManagementApp.Models;
using Microsoft.EntityFrameworkCore;
using LibraryManagementApp.Areas.Identity.Data;

[Authorize]
public class LentBooksController : Controller
{
    private readonly ApplicationDbContext _context;

    public LentBooksController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        var lentBooks = _context.LentBooks.Include(l => l.Book).ToList();
        return View(lentBooks);
    }

    public IActionResult Return(int id)
    {
        var lentBook = _context.LentBooks.Include(l => l.Book).FirstOrDefault(l => l.Id == id);
        if (lentBook == null) return NotFound();
        return View(lentBook);
    }

    [HttpPost]
    public IActionResult ReturnConfirmed(int id)
    {
        var lentBook = _context.LentBooks.Find(id);
        if (lentBook != null)
        {
            _context.LentBooks.Remove(lentBook);
            _context.SaveChanges();
        }
        return RedirectToAction("Index");
    }
}