using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EvaluacionNuxiva.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginsController : ControllerBase
    {
        private readonly LoginContext _context;

        public LoginsController(LoginContext context)
        {
            _context = context;
        }

        // GET api/logins
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Login>>> GetLogins()
        {
            return await _context.Logins.ToListAsync();
        }

        // POST api/logins
        [HttpPost]
        public async Task<ActionResult<Login>> PostLogin(Login login)
        {
            _context.Logins.Add(login);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetLogin), new { id = login.Id }, login);
        }

        // GET api/logins/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Login>> GetLogin(int id)
        {
            var login = await _context.Logins.FindAsync(id);
            if (login == null)
            {
                return NotFound();
            }
            return login;
        }

        // PUT api/logins/5
        [HttpPut("{id}")]
        public async Task<ActionResult> PutLogin(int id, Login login)
        {
            if (id != login.Id)
            {
                return BadRequest();
            }
            _context.Entry(login).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE api/logins/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Login>> DeleteLogin(int id)
        {
            var login = await _context.Logins.FindAsync(id);
            if (login == null)
            {
                return NotFound();
            }
            _context.Logins.Remove(login);
            await _context.SaveChangesAsync();
            return login;
        }

        [HttpGet]
        [Route("csv")]
        public async Task<IActionResult> GetCSV()
        {
            var logins = await _context.Logins
                .Include(static l => l.User)
                .Include(l => l.User.Area)
                .ToListAsync();

            var csv = new StringBuilder();
            csv.AppendLine("Nombre de usuario,Nombre completo,Área,Total de horas trabajadas");

            foreach (var login in logins)
            {
                if (login.User != null)
                {
                    var user = login.User;
                    var area = user.Area;
                    var totalHoras = login.TipoMov == 1 ? (login.Fecha - login.Fecha).TotalHours : (login.Fecha - login.Fecha).TotalHours;

                    csv.AppendLine($"{user.Login},{user.Nombres} {user.ApellidoPaterno} {user.ApellidoMaterno},{area.Nombre},{totalHoras}");
                }
            }

            return File(Encoding.UTF8.GetBytes(csv.ToString()), "text/csv", "logins.csv");
        }
    }
}
