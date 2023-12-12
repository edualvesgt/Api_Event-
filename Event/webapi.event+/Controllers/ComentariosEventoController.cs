using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using webapi.event_.Domains;
using webapi.event_.Repositories;

namespace webapi.event_.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class ComentariosEventoController : ControllerBase
    {
        ComentariosEventoRepository _comentariosRepository = new ComentariosEventoRepository();

        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                return Ok(_comentariosRepository.Listar());
            }

            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }


        [HttpGet("BuscarPorIdUsuario/{id}")]

        public IActionResult GetByIdUser(Guid idAluno, Guid idEvento)
        {
            try
            {
                return Ok(_comentariosRepository.SearchByIdUser(idAluno, idEvento));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost]

        public IActionResult Post(ComentariosEvento comentario)
        {
            try
            {
                _comentariosRepository.Cadastrar(comentario);
                return StatusCode(201, comentario);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpDelete]
        public IActionResult Delete (Guid id)
        {
            try
            {
                _comentariosRepository.Deletar(id);
                return NoContent();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }


}
