using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BuffetAPI.Data;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using BuffetAPI.Migrations;
using BuffetAPI.Models.Plats;
using AutoMapper;

namespace BuffetAPI.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class PlatsController(BuffetAPIContext context, ILogger<PlatsController> logger, IMapper mapper) : ControllerBase
    {
        private readonly BuffetAPIContext _context = context;
        private readonly ILogger<PlatsController> _logger = logger;
        private readonly IMapper _mapper = mapper;

        // GET: api/Plats/citation
        [HttpGet("citation")]
        [AllowAnonymous]
        public ActionResult<string> GetCitation()
        {
            _logger.LogInformation("On m'a demandé la citation");
            return Ok("Bienvenue au buffet!");
        }

        // GET: api/Plats/favori
        [HttpGet("favori")]
        [Authorize]
        public async Task<ActionResult<Plat>> GetFavori()
        {
            var plat = await _context.Plat.FindAsync(2);

            if (plat == null)
            {
                return NotFound();
            }

            return plat;
        }


        // GET: api/Plats
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Plat>>> GetPlat()
        {
            return await _context.Plat.ToListAsync();
        }

        // GET: api/Plats/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<Plat>> GetPlat(int id)
        {
            var plat = await _context.Plat.Include(p => p.TypePlat).FirstOrDefaultAsync(p => p.Id == id);

            if (plat == null)
            {
                return NotFound();
            }

            return plat;
        }

        // PUT: api/Plats/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize(Roles = "Cuisinier")]
        public async Task<IActionResult> PutPlat(int id, PutPlatDto platDto)
        {
            if (id != platDto.Id)
            {
                return BadRequest();
            }

            var platModifie = await _context.Plat.FindAsync(id);
            if (platModifie == null)
                return BadRequest();

            if (platModifie.CuisinierId == null || platModifie.CuisinierId != GetUserName())
                return Unauthorized();


            var typePlat = await _context.TypePlat.FindAsync(platDto.TypePlatId);
            if (typePlat == null)
            {
                _logger.LogWarning("Le client a demandé le type plat {Typeplat}, qui n'existe pas.", platDto.TypePlatId);
                return NotFound($"Le type de plat #{platDto.TypePlatId} n'existe pas.");

            }

            _mapper.Map(platDto, platModifie);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError("Exception {e}", ex.Message);
                if (!PlatExists(id))
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

        // POST: api/Plats
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(Roles = "Cuisinier")]
        public async Task<ActionResult<PostPlatDto>> PostPlat(PostPlatDto platDto) //dto
        {
            Plat plat = _mapper.Map<Plat>(platDto);
            plat.CuisinierId = GetUserName();
            if (plat.CuisinierId == null)
                return Unauthorized();

            var typePlat = await _context.TypePlat.FindAsync(plat.TypePlatId);
            if (typePlat == null)
            {
                return NotFound($"Le type de plat #{plat.TypePlatId} n'existe pas.");

            }
            plat.TypePlat = typePlat;
            _context.Plat.Add(plat);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPlat", new { id = plat.Id }, plat);
        }

        // DELETE: api/Plats/5
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePlat(int id)
        {
            var plat = await _context.Plat.FindAsync(id);
            if (plat == null)
            {
                return NotFound();
            }

            _context.Plat.Remove(plat);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PlatExists(int id)
        {
            return _context.Plat.Any(e => e.Id == id);
        }

        private string? GetUserName()
        {
            return HttpContext.User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                       ?? HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        }
    }
}
