using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CoronaAppAPI.Data;
using CoronaAppAPI.Models;

namespace CoronaAppAPI.Controllers.api
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoronaTracksAPIController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CoronaTracksAPIController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/CoronaTracksAPI
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CoronaTrack>>> GetCoronaTrack()
        {
            return await _context.CoronaTrack.ToListAsync();
        }

        // GET: api/CoronaTracksAPI
        //[HttpGet]
        //[Route("")]
        //public async Task<ActionResult<IEnumerable<CoronaTrack>>> GetCoronaTrack()
        //{
        //    return await _context.CoronaTrack.ToListAsync();
        //}

        // GET: api/CoronaTracksAPI/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CoronaTrack>> GetCoronaTrack(Guid id)
        {
            var coronaTrack = await _context.CoronaTrack.FindAsync(id);

            if (coronaTrack == null)
            {
                return NotFound();
            }

            return coronaTrack;
        }

        // PUT: api/CoronaTracksAPI/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCoronaTrack(Guid id, CoronaTrack coronaTrack)
        {
            if (id != coronaTrack.Id)
            {
                return BadRequest();
            }

            _context.Entry(coronaTrack).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CoronaTrackExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/CoronaTracksAPI
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<CoronaTrack>> PostCoronaTrack(CoronaTrack coronaTrack)
        {
            _context.CoronaTrack.Add(coronaTrack);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCoronaTrack", new { id = coronaTrack.Id }, coronaTrack);
        }

        // DELETE: api/CoronaTracksAPI/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<CoronaTrack>> DeleteCoronaTrack(Guid id)
        {
            var coronaTrack = await _context.CoronaTrack.FindAsync(id);
            if (coronaTrack == null)
            {
                return NotFound();
            }

            _context.CoronaTrack.Remove(coronaTrack);
            await _context.SaveChangesAsync();

            return coronaTrack;
        }

        private bool CoronaTrackExists(Guid id)
        {
            return _context.CoronaTrack.Any(e => e.Id == id);
        }
    }
}
