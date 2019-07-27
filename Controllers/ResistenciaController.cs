using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Electro.Models;

namespace Electro.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResistenciaController : ControllerBase
    {
        private readonly ElectroContext _context;
        public ResistenciaController (ElectroContext context){
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Resistencia>>> GetResistencias(){
            return await _context.Resistencias.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Resistencia>>  GetResistencia(int id){
            var resitencia = await _context.Resistencias.FindAsync(id);
            if (resitencia == null ){
                return NotFound();
            }
            return resitencia;
        }

        [HttpPost("Serie")]
        public async Task<ActionResult<Resistencia>> PostResistenciaSerie(Resistencia item){
            int num =  _context.Resistencias.Count();
            item.Ok=true;
            item.Id= num+1;
            _context.Resistencias.Add(item);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetResistencia), new {id = item.Id}, item);
        }

        [HttpPost("ParaleloAdd")]
        public async Task<ActionResult<Paralelo>> PostResistenciaParaleloAdd(Paralelo item){
            int num =  _context.Resistencias.Count();
            item.Id= num+5;
            item.Resistenica = 1/item.Resistenica;
            _context.Paralelos.Add(item);
            Resistencia resistencia = new Resistencia();
            for (int i=1; i ==_context.Resistencias.Count(); i++){
                var auxResis = await _context.Resistencias.FindAsync(i);
                if (auxResis.Ok==false){
                    resistencia.Id = auxResis.Id;
                }
                else {
                    resistencia.Id = _context.Resistencias.Count()+1;
                }
            }
            resistencia.Ok = false;
            resistencia.Resisten = item.Resistenica;
            await this.PostResistenciaParalelloSum(resistencia);
            await _context.SaveChangesAsync();
            return Ok();
        }
        
        [HttpPost("ParaleloSum")]
        public async Task<ActionResult<Resistencia>> PostResistenciaParalelloSum(Resistencia item){
            var resaux = await _context.Resistencias.FindAsync(item.Id);
            if (resaux==null){
                _context.Resistencias.Add(item);
                await _context.SaveChangesAsync();
                return Ok();
            } else {
                resaux.Resisten=resaux.Resisten+item.Resisten;
                await _context.SaveChangesAsync();
                return Ok();
            }
        }

        [HttpPost("ParaleloResult")]
        public async Task<ActionResult<Resistencia>> PostResistenciaParaleloResult(){
            int id=0;
            for (int i=1; i==_context.Resistencias.Count(); i++){
                var aux = await _context.Resistencias.FindAsync(i);
                if (aux.Ok==false){
                    id = i;
                }
                else {
                    return NotFound();
                }
            }
            var item = await _context.Resistencias.FindAsync(id);
            if (item!=null){
                item.Resisten = 1/item.Resisten;
                item.Ok=true;
                int valor = _context.Paralelos.Count();
                for (int i=1; i==valor;i++){
                    var paralelo = await _context.Paralelos.FindAsync(i);
                    _context.Paralelos.Remove(paralelo);
                }
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetResistencia), new {id=item.Id}, item);
            }
            else {
                return NotFound();
            }
            
        }
    }
}