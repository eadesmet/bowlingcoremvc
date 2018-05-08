using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

using BowlingCoreMVC.Data;
using BowlingCoreMVC.Models;

namespace BowlingCoreMVC.Controllers
{
    [Authorize]
    public class LocationsController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public LocationsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _db = context;
            _userManager = userManager;
        }

        private Task<ApplicationUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);

        // GET: Locations
        public async Task<IActionResult> Index()
        {
            var LocationList = await _db.Locations.ToListAsync();
            foreach (var l in LocationList)
            {
                l.CreatedByUserName = (await _userManager.FindByIdAsync(l.CreatedByID)).UserName;
            }
            return View(LocationList);
        }

        // GET: Locations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var location = await _db.Locations
                .SingleOrDefaultAsync(m => m.ID == id);
            if (location == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(location.CreatedByID);
            location.CreatedByUserName = user.UserName;

            return View(location);
        }

        // GET: Locations/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Locations/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name")] Location location)
        {
            if (ModelState.IsValid)
            {
                location.CreatedDate = DateTime.Now;
                location.ModifiedDate = DateTime.Now;
                var user = await GetCurrentUserAsync();
                if (user != null) //should never be null..
                {
                    location.CreatedByID = user.Id;
                }
                
                _db.Add(location);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(location);
        }

        // GET: Locations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var location = await _db.Locations.SingleOrDefaultAsync(m => m.ID == id);
            if (location == null)
            {
                return NotFound();
            }
            return View(location);
        }

        // POST: Locations/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Name")] Location location)
        {
            if (id != location.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    location.ModifiedDate = DateTime.Now;
                    _db.Update(location);
                    await _db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LocationExists(location.ID))
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
            return View(location);
        }

        // GET: Locations/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var location = await _db.Locations
                .SingleOrDefaultAsync(m => m.ID == id);
            if (location == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(location.CreatedByID);
            location.CreatedByUserName = user.UserName;

            return View(location);
        }

        // POST: Locations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var location = await _db.Locations.SingleOrDefaultAsync(m => m.ID == id);
            _db.Locations.Remove(location);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LocationExists(int id)
        {
            return _db.Locations.Any(e => e.ID == id);
        }
    }
}
