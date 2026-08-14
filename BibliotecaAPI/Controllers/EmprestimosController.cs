using BibliotecaAPI.DTOs;
using BibliotecaAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace BibliotecaAPI.Controllers;

[ApiController]
[Route("api/emprestimos")]
public class EmprestimosController : ControllerBase
{
    private readonly IEmprestimoService _service;

    public EmprestimosController(IEmprestimoService service)
    {
        _service = service;
    }

    /// <summary>Lista todos os empréstimos.</summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _service.GetAllAsync();
        return Ok(result);
    }

    /// <summary>Busca empréstimo por ID. Retorna 404 se não encontrado.</summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id);
        return Ok(result);
    }

    /// <summary>Cria um novo empréstimo. Valida estoque e duplicidade.</summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CriarEmprestimoDto dto)
    {
        var result = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    /// <summary>Registra a devolução de um empréstimo. Retorna 409 se já devolvido.</summary>
    [HttpPut("{id:int}/devolucao")]
    public async Task<IActionResult> Devolver(int id)
    {
        var result = await _service.DevolverAsync(id);
        return Ok(result);
    }
}
