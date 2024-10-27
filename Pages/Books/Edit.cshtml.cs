using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Moldovan_Maria_Karina_Lab2.Data;
using Moldovan_Maria_Karina_Lab2.Models;

namespace Moldovan_Maria_Karina_Lab2.Pages.Books
{
    public class EditModel : BookCategoriesPageModel
    {
        private readonly Moldovan_Maria_Karina_Lab2.Data.Moldovan_Maria_Karina_Lab2Context _context;

        public EditModel(Moldovan_Maria_Karina_Lab2.Data.Moldovan_Maria_Karina_Lab2Context context)
        {
            _context = context;
        }

        [BindProperty]
        
        public Book Book { get; set; }
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Book = await _context.Book
              .Include(b => b.Publisher)
              .Include(b => b.Author)
              .Include(b => b.BookCategories).ThenInclude(b => b.Category)
              .AsNoTracking()
              .FirstOrDefaultAsync(m => m.ID == id);
            
            var book =  await _context.Book.FirstOrDefaultAsync(m => m.ID == id);
            if (book == null)
            {
                return NotFound();
            }

            PopulateAssignedCategoryData(_context, Book);
           


        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        var authorList = _context.Author.Select(x => new
        {
            x.ID,
            FullName = x.LastName + " " + x.FirstName
        });
        ViewData["AuthorID"] = new SelectList(authorList, "ID", "FullName");
        ViewData["PublisherID"] = new SelectList(_context.Publisher, "ID",
       "PublisherName");
         return Page();
    }

    public async Task<IActionResult> OnPostAsync(int? id, string[]
       selectedCategories)
        {
            if (id == null)
            {
                return NotFound();
            }
            
            var bookToUpdate = await _context.Book
            .Include(i => i.Publisher)
            .Include(b => b.Author)
            .Include(i => i.BookCategories)
            .ThenInclude(i => i.Category)
            .FirstOrDefaultAsync(s => s.ID == id);
            if (bookToUpdate == null)
            {
                return NotFound();
            }
            
            if (await TryUpdateModelAsync<Book>(
            bookToUpdate,
            "Book",
            i => i.Title, i => i.Author,
            i => i.Price, i => i.PublishingDate, i => i.PublisherID))
            {
                UpdateBookCategories(_context, selectedCategories, bookToUpdate);
                await _context.SaveChangesAsync();
                return RedirectToPage("./Index");
            }
           
            UpdateBookCategories(_context, selectedCategories, bookToUpdate);
            PopulateAssignedCategoryData(_context, bookToUpdate);
            return Page();
        }
    }
}