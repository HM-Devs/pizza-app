using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.IIS;
using Microsoft.EntityFrameworkCore;
using Xunit.Sdk;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PizzaVotesController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;

        public PizzaVotesController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // GET api/pizzavotes
        [HttpGet]
        public async Task<ActionResult<List<PizzaVotes>>> Get()
        {
            return await _dbContext.PizzaVotes.ToListAsync();
        }

        // GET api/pizzavotes/{email}
        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<PizzaVotes>> Get(string id)
        {
            return await _dbContext.PizzaVotes.FindAsync(id);
        }

        // POST api/pizzavotes
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Post(PizzaVotes pizzaVotes)
        {
            try
            {
                if (pizzaVotes != null)
                {
                    await _dbContext.AddAsync(pizzaVotes);
                    await _dbContext.SaveChangesAsync();
                    return CreatedAtAction("Get", new { id = pizzaVotes.Id }, pizzaVotes);
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                // probably a good idea to log the exception for debugging purposes :) 
                var errorResponse = new
                {
                    Error = "An error occurred while processing the request.",
                    Details = ex.Message 
                };

                return new ObjectResult(errorResponse)
                {
                    StatusCode = 500
                };
            }
        }

        // PUT api/pizzavotes/{email}
        [Authorize]
        [HttpPut("{id}")]
        public async Task<ActionResult> Put(string id, PizzaVotes pizzaVotes)
        {
            if(string.IsNullOrEmpty(id) || pizzaVotes == null)
            {
                return BadRequest("Invalid input data.");
            }

            var existingRecord = await _dbContext.PizzaVotes.AnyAsync(f => f.Id == id);
            if (!existingRecord)
            {
                return NotFound($"PizzaVotes with ID {id} not found.");
            }

            _dbContext.PizzaVotes.Update(pizzaVotes);

            await _dbContext.SaveChangesAsync();

            return Ok();
        }
    }
}