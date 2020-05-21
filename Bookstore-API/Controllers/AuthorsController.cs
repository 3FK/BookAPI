using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Bookstore_API.Contracts;
using Bookstore_API.Data;
using Bookstore_API.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Bookstore_API.Controllers
{
    /// <summary>
    /// Endpoint for Authors
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)] 
    public class AuthorsController : ControllerBase
    {
        private readonly IAuthorRepository _authorRepository;
        private readonly ILoggerService _logger;
        private readonly IMapper _mapper;

        public AuthorsController(IAuthorRepository authorRepository, ILoggerService logger, IMapper mapper)
        {
            _authorRepository = authorRepository;
            _logger = logger ;
            _mapper = mapper;  
        }
        /// <summary>
        /// Get all Authors
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAuthors()
        {
            try
            {
                _logger.LogInfo("Attemted to get all Authors");
                var authors = await _authorRepository.FindAll();
                var response = _mapper.Map<IList<AuthorDTO>>(authors);
                _logger.LogInfo("Successfully got all Authors");
                return Ok(response);
            } catch (Exception e)
            {
                return  InternalError($"{e.Message} - {e.InnerException}"); 
            }
        }
        /// <summary>
        /// Get Author by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>An Authors record</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAuthor(int id)
        {
            try
            {
                _logger.LogInfo($"Attemted to get one Author by id: {id}");
                var isExists = await _authorRepository.IsExists(id);
                if(!isExists)
                {
                    _logger.LogWarn($"Author id: {id} was not found");
                    return NotFound();
                }
                var author = await _authorRepository.FindById(id);
                var response = _mapper.Map<AuthorDTO>(author);
                _logger.LogInfo($"Successfully got Author by id: {id}");
                return Ok(response);
            } catch (Exception e)
            {
                return InternalError($"{e.Message} - {e.InnerException}");
                
            }
        }
        /// <summary>
        /// Create an Author
        /// </summary>
        /// <param name="authorDTO"></param>
        /// <returns>Created Author</returns>
        [HttpPost]
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateAuthor([FromBody] AuthorCreateDTO authorDTO)
        {
            try
            {
                _logger.LogInfo($"Attemted to create an Author");
                if (authorDTO == null)
                {
                    _logger.LogWarn("Empty request was submitted");
                    return BadRequest(ModelState );
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogWarn("Required data was not provided");
                    return BadRequest(ModelState);
                }
                _logger.LogInfo("Attemted to create Author");
                var author = _mapper.Map<Author>(authorDTO);
                var isSuccess = await _authorRepository.Create(author);
                if (!isSuccess)
                {
                    return InternalError("Author creation failed");
                }
                _logger.LogInfo($"Successfully create an Author");
                return Created("author created", new { author }); 
            }
            catch (Exception e)
            {
                return InternalError($"{e.Message} - {e.InnerException}");
            }
        }
        /// <summary>
        /// Update an Author
        /// </summary>
        /// <param name="id"></param>
        /// <param name="authorDTO"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateAuthor (int id, [FromBody] AuthorUpdateDTO authorDTO)
        {
            try
            {
                _logger.LogInfo($"Attemted to Update an Author");
                if (id < 1 || authorDTO == null || id != authorDTO.Id)
                {
                    _logger.LogWarn("Empty request was submitted");
                    return BadRequest();
                }
                var isExists = await _authorRepository.IsExists(id);
                if (!isExists)
                {
                    _logger.LogWarn($"Author id: {id} was not found");
                    return NotFound();
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogWarn("Required data was not provided");
                    return BadRequest(ModelState);
                }
                var author = _mapper.Map<Author>(authorDTO);
                var isSuccess = await _authorRepository.Update(author);
                if (!isSuccess)
                {
                    return InternalError($"Author update failed");
                }
                _logger.LogInfo($"Successfully Update an Author");
                return NoContent();
            }
            catch (Exception e)
            {
                return InternalError($"{e.Message} - {e.InnerException}");
            }
        }
        /// <summary>
        /// Delete an Author
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteAuthor (int id)
        {
            try
            {
                _logger.LogInfo("Attemted to Delete an Author");
                if(id < 1)
                {
                    _logger.LogWarn("Empty request was submitted");
                    return BadRequest();
                }
                var isExists = await _authorRepository.IsExists(id);
                if (!isExists)
                {
                    _logger.LogWarn($"Author id: {id} was not found");
                    return NotFound();
                }
                var author = await _authorRepository.FindById(id);
                var isSuccess = await _authorRepository.Delete(author);
                if (!isSuccess)
                {
                    return InternalError($"Author Delete failed");
                }
                _logger.LogInfo($"Successfully Delete an Author");
                return NoContent();
            }
            catch (Exception e)
            {
                return InternalError($"{e.Message} - {e.InnerException}");
            }
        }

        private ObjectResult InternalError(string message)
        {
            _logger.LogError(message);
            return StatusCode(500, "Somehing went wrong. Please contact the Administrator ");
        }
    }
}
