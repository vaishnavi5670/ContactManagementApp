using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ContactApp.Data;
using ContactApp.Models;

namespace ContactApp.Controllers
{
    [Route("Contacts")] 
    public class ContactsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ContactsController(ApplicationDbContext context)
        {
            _context = context;
        }

        
        // GET: contacts/details/5
        [HttpGet("details/{id}")] 
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var contact = await _context.Contacts.FirstOrDefaultAsync(m => m.Id == id);
            if (contact == null) return NotFound();

            return View(contact);
        }

        // GET: contacts/create
        [HttpGet("create")]
        public IActionResult Create()
        {
            return View(new Contact());
        }

        // POST: contacts/create
        [HttpPost("create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Contact contact)
        {
            contact.Email = string.Empty;
            contact.ConfirmPassword = string.Empty;
            if (ModelState.IsValid)
            {
                _context.Add(contact);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
           
            return View(contact);
        }
        // GET: contacts/edit
        [HttpGet("edit/{id}")] 
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var contact = await _context.Contacts.FindAsync(id);
            if (contact == null) return NotFound();

            return View(contact);
        }

        [HttpPost("edit/{id}")] 
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id)
        {
            var contactToUpdate = await _context.Contacts.FindAsync(id);

            if (contactToUpdate == null)
            {
                return NotFound();
            }

            if (await TryUpdateModelAsync<Contact>(
                contactToUpdate,
                "",
                c => c.Name, c => c.Email, c => c.PhoneNumber, c => c.Password, c => c.ConfirmPassword, c => c.IsFavorite))
            {
                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ContactExists(contactToUpdate.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return View(contactToUpdate);
        }

        
        // GET: contacts/delete/5
        [HttpGet("delete/{id}")] 
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var contact = await _context.Contacts.FirstOrDefaultAsync(m => m.Id == id);
            if (contact == null) return NotFound();

            return View(contact);
        }

        // POST: contacts/delete/5
        [HttpPost("delete/{id}")] // Maps to POST requests for confirming deletion
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var contact = await _context.Contacts.FindAsync(id);
            if (contact != null)
            {
                _context.Contacts.Remove(contact);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ContactExists(int id)
        {
            return _context.Contacts.Any(e => e.Id == id);
        }
        // GET: contacts
        [HttpGet] // Maps to GET requests
        public async Task<IActionResult> Index(string searchString)
        {
            var contacts = from c in _context.Contacts
                           select c;

            if (!String.IsNullOrEmpty(searchString))
            {
                contacts = contacts.Where(s => s.Name.Contains(searchString) || s.Email.Contains(searchString));
            }

            return View(await contacts.ToListAsync());
        }
        [HttpPost]
        public async Task<IActionResult> ToggleFavorite(int id)
        {
            var contact = await _context.Contacts.FindAsync(id);
            if (contact == null)
            {
                return NotFound();
            }

            contact.IsFavorite = !contact.IsFavorite;
            _context.Update(contact);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}

