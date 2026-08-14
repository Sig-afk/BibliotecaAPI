using BibliotecaAPI.DTOs;
using BibliotecaAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace BibliotecaAPI.Controllers;

[ApiController]
[Route("api/livros")]
public class LivrosController : ControllerBase
{
    private readonly ILivroService _service;

    public LivrosController(ILivroService service)
    {
        _service = service;
    }

    /// <summary>Lista todos os livros. Suporta filtros: ?titulo=xxx e ?autor=yyy</summary>
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? titulo, [FromQuery] string? autor)
    {
        var result = await _service.GetAllAsync(titulo, autor);
        return Ok(result);
    }

    /// <summary>Busca livro por ID. Retorna 404 se não encontrado.</summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id);
        return Ok(result);
    }

    /// <summary>Cadastra um novo livro.</summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CriarLivroDto dto)
    {
        var result = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }
}
