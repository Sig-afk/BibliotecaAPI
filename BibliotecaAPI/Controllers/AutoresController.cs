using BibliotecaAPI.DTOs;
using BibliotecaAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace BibliotecaAPI.Controllers;

[ApiController]
[Route("api/autores")]
public sealed class AutoresController : ControllerBase
{
    private readonly IAutorService _service;

    public AutoresController(IAutorService service)
    {
        _service = service;
    }

    /// <summary>Lista todos os autores.</summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _service.GetAllAsync();
        return Ok(result);
    }

    /// <summary>Busca autor por ID. Retorna 404 se não encontrado.</summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id);
        return Ok(result);
    }

    /// <summary>Cadastra um novo autor.</summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CriarAutorDto dto)
    {
        var result = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }
}
