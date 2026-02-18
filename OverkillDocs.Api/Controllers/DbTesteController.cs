using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OverkillDocs.Infrastructure.Data;
using System.Data;

namespace OverkillDocs.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class DbTesteController : ControllerBase
{
    private readonly Infrastructure.Data.AppDbContext _context;

    public DbTesteController(Infrastructure.Data.AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("status")]
    public async Task<IActionResult> GetStatus()
    {
        try
        {
            // 1. Testa se o EF consegue "pingar" o banco
            var canConnect = await _context.Database.CanConnectAsync();

            if (!canConnect)
            {
                return StatusCode(500, new
                {
                    status = "Erro",
                    mensagem = "O banco de dados não está respondendo. Verifique o container 'db' e a ConnectionString."
                });
            }

            // 2. Executa uma query SQL bruta pra ter certeza absoluta
            using var command = _context.Database.GetDbConnection().CreateCommand();
            command.CommandText = "SELECT @@VERSION";
            _context.Database.OpenConnection();

            var versaoSql = command.ExecuteScalar()?.ToString();

            return Ok(new
            {
                status = "Sucesso",
                mensagem = "Conexão com o SQL Server estabelecida!",
                versaoBanco = versaoSql,
                timestamp = DateTime.Now
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                status = "Falha Crítica",
                erro = ex.Message,
                detalhe = ex.InnerException?.Message
            });
        }
    }
}