using Scores.Core.Models;

namespace Scores.Core.Services;

public class CSVProcessor
{
    private readonly CSVParser _parser;

    public CSVProcessor(CSVParser parser)
    {
        _parser = parser;
    }

    public async Task<IEnumerable<Score>> ProcessFile(string filePath)
    {
        try
        {
            string content = await File.ReadAllTextAsync(filePath);
            var scores = _parser.ParseCsv(content);
            
            var maxScore = scores.Max(s => s.ScoreValue);
            var topScores = scores
                .Where(s => s.ScoreValue == maxScore)
                .OrderBy(s => s.FirstName)
                .ThenBy(s => s.SecondName)
                .ToList();

            Console.WriteLine($"\nTop score: {maxScore}");
            Console.WriteLine("\nTop scorers:");
            foreach (var score in topScores)
            {
                Console.WriteLine($"{score.FirstName} {score.SecondName}");
            }

            return scores;
        }
        catch (FileNotFoundException)
        {
            Console.WriteLine($"Error: File not found at {filePath}");
            return Enumerable.Empty<Score>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error processing file: {ex.Message}");
            return Enumerable.Empty<Score>();
        }
    }
}