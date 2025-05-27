using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LibraryManagementApp.Models;
using Microsoft.EntityFrameworkCore;
using LibraryManagementApp.Areas.Identity.Data;
using Microsoft.AspNetCore.Http;

[Authorize]
public class BooksController : Controller
{
    private readonly ApplicationDbContext _context;

    public BooksController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Index(string genreFilter, int page = 1)
    {
        // If genreFilter is explicitly empty (e.g., from "All Genres" or "Clear Filter"), clear the session
        if (string.IsNullOrEmpty(genreFilter))
        {
            genreFilter = "";
            HttpContext.Session.SetString("GenreFilter", "");
        }
        else
        {
            HttpContext.Session.SetString("GenreFilter", genreFilter);
        }

        // Determine the last applied filter
        string lastAppliedFilter = HttpContext.Session.GetString("LastAppliedFilter") ?? "None";
        string previousFilter = HttpContext.Session.GetString("PreviousFilter") ?? "";

        if (string.IsNullOrEmpty(genreFilter))
        {
            lastAppliedFilter = string.IsNullOrEmpty(previousFilter) ? "None" : previousFilter;
        }
        else
        {
            lastAppliedFilter = string.IsNullOrEmpty(previousFilter) ? "None" : previousFilter;
        }

        HttpContext.Session.SetString("PreviousFilter", HttpContext.Session.GetString("GenreFilter") ?? "");
        HttpContext.Session.SetString("LastAppliedFilter", lastAppliedFilter);

        // Query all books initially
        var booksQuery = _context.Books.AsQueryable();
        if (!string.IsNullOrEmpty(genreFilter))
        {
            booksQuery = booksQuery.Where(b => b.Genre == genreFilter);
        }

        int pageSize = 3; // Maximum 3 books per page
        int totalBooks = booksQuery.Count();
        int totalPages = (int)Math.Ceiling((double)totalBooks / pageSize);
        page = Math.Max(1, Math.Min(page, totalPages)); // Ensure page is within valid range

        var books = booksQuery
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        // Define hardcoded genres
        var genres = _context.Books.Select(b => b.Genre).Distinct().ToList();

        // Create the view model
        var viewModel = new BookFilterViewModel
        {
            GenreFilter = genreFilter,
            LastAppliedFilter = lastAppliedFilter,
            Books = books,
            Genres = genres,
            CurrentPage = page,
            TotalPages = totalPages,
            PageSize = pageSize
        };

        return View(viewModel);
    }

    // Other actions (Add, Edit, Delete, Lend) remain unchanged
    public IActionResult Add()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Add(Book book)
    {
        if (!ModelState.IsValid)
        {
            return View(book);
        }
        _context.Books.Add(book);
        _context.SaveChanges();
        return RedirectToAction("Index");
    }

    public IActionResult Edit(int id)
    {
        var book = _context.Books.Find(id);
        if (book == null) return NotFound();
        return View(book);
    }

    [HttpPost]
    public IActionResult Edit(Book book)
    {
        if (!ModelState.IsValid)
        {
            return View(book);
        }
        _context.Books.Update(book);
        _context.SaveChanges();
        return RedirectToAction("Index");
    }

    public IActionResult Delete(int id)
    {
        var book = _context.Books.Find(id);
        if (book == null) return NotFound();
        return View(book);
    }

    [HttpPost]
    public IActionResult DeleteConfirmed(int id)
    {
        var book = _context.Books.Find(id);
        if (book != null)
        {
            _context.Books.Remove(book);
            _context.SaveChanges();
        }
        return RedirectToAction("Index");
    }

    public IActionResult Lend(int id)
    {
        var book = _context.Books.Find(id);
        if (book == null) return NotFound();
        ViewBag.BookId = id;
        return View();
    }

    [HttpPost]
    public IActionResult Lend(int bookId, string borrower)
    {
        if (string.IsNullOrWhiteSpace(borrower))
        {
            ModelState.AddModelError("borrower", "Borrower name is required.");
            ViewBag.BookId = bookId;
            return View();
        }
        var lentBook = new LentBook { BookId = bookId, Borrower = borrower, LendDate = DateTime.Now };
        _context.LentBooks.Add(lentBook);
        _context.SaveChanges();
        return RedirectToAction("Index");
    }
}