using System;
using System.Net;
using System.Threading.Tasks;
using Api.Domain.Entities;
using Api.Domain.Interfaces.Services.User;
using Microsoft.AspNetCore.Mvc;

namespace Api.Application.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class UsersController : ControllerBase
    {
        private IUserService _service;

        public UsersController(IUserService service)
        {
            _service = service;
        }

        private bool VerificaInvalido()
        {
            return !ModelState.IsValid;
        }

        [HttpGet]
        public async Task<ActionResult> GetAll()
        {
            if (VerificaInvalido())
            {
                return BadRequest(ModelState); //Http 400
            }

            try
            {
                return Ok(await _service.GetAll());
            }
            catch (ArgumentException e)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, e.Message); //Http 500
            }
        }

        [HttpGet]
        [Route("{id}", Name = "GetWithId")]
        public async Task<ActionResult> Get(Guid id)
        {
            if (VerificaInvalido())
            {
                return BadRequest(ModelState); //Http 400
            }
            
            try
            {
                return Ok(await _service.Get(id));
            }
            catch (ArgumentException e)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, e.Message);
            }

        }
        
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] UserEntity user)
        {
            if (VerificaInvalido())
            {
                return BadRequest(ModelState); //Http 400
            }

            try
            {
                var result = await _service.Post(user);
                
                if (result != null)
                {
                    return Created(new Uri(Url.Link("GetWithId", new { id = result.Id })), result); //Http 201
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (ArgumentException e)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, e.Message);
            }
        }

        [HttpPut]
        public async Task<ActionResult> Put([FromBody] UserEntity user)
        {
            if (VerificaInvalido())
            {
                return BadRequest(ModelState); //Http 400
            }

            try
            {
                var result = await _service.Put(user);
                
                if (result != null)
                {
                    return Ok (result);
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (ArgumentException e)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, e.Message);
            }
        }

        [HttpDelete]
        [Route("{id}", Name = "DeleteWithId")]
        public async Task<ActionResult> Delete(Guid id)
        {
            if (VerificaInvalido())
            {
                return BadRequest(ModelState); //Http 400
            }

            try
            {
                return Ok (await _service.Delete(id));
            }
            catch (ArgumentException e)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, e.Message);
            }
        }
    }
}
