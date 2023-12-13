using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.CognitiveServices.ContentModerator;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using webapi.event_.Domains;
using webapi.event_.Repositories;

namespace webapi.event_.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class ComentariosEventoController : ControllerBase
    {
        // Repositório responsável por lidar com operações de comentários
        ComentariosEventoRepository _comentariosRepository = new ComentariosEventoRepository();

        // Cliente utilizado para interagir com o serviço de moderação de conteúdo da Azure
        private readonly ContentModeratorClient _contentModeratorClient;

        /// <summary>
        /// Construtor da classe responsável por inicializar o controlador de comentários do evento.
        /// Recebe um cliente para acesso ao Content Moderator da Azure.
        /// </summary>
        /// <param name="contentModeratorClient">Cliente do Content Moderator para moderação de texto</param>
        public ComentariosEventoController(ContentModeratorClient contentModeratorClient)
        {
            _contentModeratorClient = contentModeratorClient; // Inicializa o cliente do Content Moderator
        }

        // Endpoint HTTP POST utilizado para submeter um novo comentário para moderação de IA
        [HttpPost("CometarioIA")]
        public async Task<IActionResult> PostIA(ComentariosEvento novoComentario)
        {
            try
            {
                // Verifica se a descrição do comentário está vazia
                if (novoComentario.Descricao.IsNullOrEmpty())
                {
                    return BadRequest("A Descrição do Comentário não pode estar vazia");
                }

                // Converte a descrição do comentário em um fluxo de bytes para moderação
                using var stream = new MemoryStream(Encoding.UTF8.GetBytes(novoComentario.Descricao!));

                // Realiza a moderação de texto utilizando o Content Moderator da Azure
                var moderationResult = await _contentModeratorClient.TextModeration.
                    ScreenTextAsync("text/plain", stream, "por", false, false, null, true);

                // Se o resultado da moderação contém termos inadequados
                if (moderationResult.Terms != null)
                {
                    novoComentario.Exibe = false; // Define que o comentário não deve ser exibido publicamente

                    _comentariosRepository.Cadastrar(novoComentario); // Armazena o comentário no repositório
                }
                else
                {
                    novoComentario.Exibe = true; // Define que o comentário pode ser exibido publicamente
                    _comentariosRepository.Cadastrar(novoComentario); // Armazena o comentário no repositório
                }

                return StatusCode(201, novoComentario); // Retorna o status 201 (Created) com o novo comentário
            }
            catch (Exception e)
            {
                return BadRequest(e.Message); // Retorna uma resposta de erro com a mensagem da exceção ocorrida
            }
        }



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

        [HttpGet("ListarSomenteExibe")]
        public IActionResult GetShow()
        {
            try
            {
                return Ok(_comentariosRepository.ListarSomenteExibe());
            }

            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }


        [HttpGet("BuscarPorIdUsuario")]

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
        public IActionResult Delete(Guid id)
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
