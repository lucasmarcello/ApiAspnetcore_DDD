using System;
using System.Net;
using System.Threading.Tasks;
using Api.Domain.DTO;
using Api.Domain.Interfaces.Services.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Application.Controllers
{
    [Route("api/v1/[controller]")]
    public class LoginController:ControllerBase
    {
        
        private ILoginService _service;

        public LoginController(ILoginService service)
        {
            _service = service;
        }

        private bool VerificaInvalido()
        {
            return !ModelState.IsValid;
        }

        //Outro modo para injetar dependencia pelo metodo e nao pelo construtor
        // public async Task<object> Login([FromBody] UserEntity userEntity, [FromServices] ILoginService service)
        [AllowAnonymous]
        [HttpPost]
        public async Task<object> Login([FromBody] LoginDTO loginDTO)
        {
            if (VerificaInvalido() || loginDTO == null)
            {
                return BadRequest(ModelState); //Http 400
            }

            try
            {
                var result = await _service.FindByLogin(loginDTO);
                if (result != null)
                {
                    return Ok(result);
                }
                else
                {
                    return NotFound();
                }
            }
            catch (ArgumentException e)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, e.Message); //Http 500
            }

        }
    }
}
