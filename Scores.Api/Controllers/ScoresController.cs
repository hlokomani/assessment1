using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Scores.Api.Data;
using Scores.Core.Models;
using Scores.Core.Services;

namespace Scores.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ScoresController : ControllerBase
{
    private readonly ScoreDbContext _context;
    private readonly CSVParser _csvParser;

    public ScoresController(ScoreDbContext context, CSVParser csvParser)
    {
        _context = context;
        _csvParser = csvParser;
    }
    [HttpGet("all")]
    public async Task<ActionResult<IEnumerable<Score>>> GetAllScores()
    {
        var scores = await _context.Scores
            .OrderByDescending(s => s.ScoreValue)
            .ToListAsync();
        return Ok(scores);
    }

    [HttpGet("top")]
    public async Task<ActionResult<IEnumerable<Score>>> GetTopScores()
    {
        var maxScore = await _context.Scores.MaxAsync(s => s.ScoreValue);
        var topScores = await _context.Scores
            .Where(s => s.ScoreValue == maxScore)
            .OrderBy(s => s.FirstName)
            .ThenBy(s => s.SecondName)
            .ToListAsync();

        return Ok(topScores);
    }

    [HttpGet("{firstName}/{secondName}")]
    public async Task<ActionResult<Score>> GetScore(string firstName, string secondName)
    {
        var score = await _context.Scores
            .FirstOrDefaultAsync(s => 
                s.FirstName == firstName && 
                s.SecondName == secondName);

        if (score == null)
            return NotFound();

        return Ok(score);
    }

    [HttpPost]
    public async Task<ActionResult<Score>> PostScore(Score score)
    {
        _context.Scores.Add(score);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetScore), 
            new { firstName = score.FirstName, secondName = score.SecondName }, 
            score);
    }

    [HttpPost("upload")]
    public async Task<ActionResult<IEnumerable<Score>>> UploadCsv([FromBody] string content)
    {
        if (string.IsNullOrEmpty(content))
        {
            return BadRequest("CSV content cannot be empty");
        }

        try
        {
            var scores = _csvParser.ParseCsv(content);
            await _context.Scores.AddRangeAsync(scores);
            await _context.SaveChangesAsync();
            return Ok(scores);
        }
        catch (Exception ex)
        {
            return BadRequest($"Error processing CSV: {ex.Message}");
        }
    }
}