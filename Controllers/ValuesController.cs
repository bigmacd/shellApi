using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace shellApi.Models
{
    [Produces("application/json")]
    [Route("api/values")]
    public class ValuesController : Controller
    {
        private readonly ShellContext _context;
        private readonly IAuditPublisher _auditPublisher;

        public ValuesController(ShellContext context, IAuditPublisher auditPublisher)
        {
            _context = context;
            _auditPublisher = auditPublisher;
        }

        // GET: api/ValuesModels
        [HttpGet]
        public IEnumerable<Values> GetValues()
        {
            _auditPublisher.SendMessageAsync("Get Values", string.Format("retrieving all values by user {0}", "bigmacd"));
            return _context.Values;
        }

        // GET: api/ValuesModels/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetValues([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var valuesModel = await _context.Values.SingleOrDefaultAsync(m => m.id == id);

            if (valuesModel == null)
            {
                return NotFound();
            }

            return Ok(valuesModel);
        }

        // PUT: api/ValuesModels/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutValues([FromRoute] int id, [FromBody] Values valuesModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != valuesModel.id)
            {
                return BadRequest();
            }

            _context.Entry(valuesModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ValuesExists(id))
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

        // POST: api/ValuesModels
        [HttpPost]
        public async Task<IActionResult> PostValues([FromBody] Values values)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Values.Add(values);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetValues", new { id = values.id }, values);
        }

        // DELETE: api/ValuesModels/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteValues([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var values = await _context.Values.SingleOrDefaultAsync(m => m.id == id);
            if (values == null)
            {
                return NotFound();
            }

            _context.Values.Remove(values);
            await _context.SaveChangesAsync();

            return Ok(values);
        }

        private bool ValuesExists(int id)
        {
            return _context.Values.Any(e => e.id == id);
        }
    }
}