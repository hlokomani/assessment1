using System.Text;
using Scores.Core.Models;

namespace Scores.Core.Services;

public class CSVParseException : Exception
{
    public int LineNumber { get; }
    public string Line { get; }

    public CSVParseException(string message, int lineNumber, string line, Exception? inner = null) 
        : base($"Line {lineNumber}: {message}", inner)
    {
        LineNumber = lineNumber;
        Line = line;
    }
}

public class CSVParser
{
    private readonly string[] _expectedHeaders = { "First Name", "Second Name", "Score" }; //given format from test data csv

    public IEnumerable<Score> ParseCsv(string content)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            throw new ArgumentException("CSV content cannot be empty", nameof(content));
        }

        var lines = content.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
        
        if (lines.Length < 2)
        {
            throw new CSVParseException("CSV must contain a header row and at least one data row", 0, content);
        }

        //is header what is expected
        var headerFields = ParseCsvLine(lines[0]);
        ValidateHeader(headerFields, lines[0]);

        var scores = new List<Score>();
        
        //parse data
        for (int i = 1; i < lines.Length; i++)
        {
            var line = lines[i];
            
            if (string.IsNullOrWhiteSpace(line)) 
            {
                Console.WriteLine($"Warning: Empty line at position {i + 1}");
                continue;
            }

            try 
            {
                var score = ParseLine(line, i + 1);
                scores.Add(score);
            }
            catch (CSVParseException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new CSVParseException($"Unexpected error: {ex.Message}", i + 1, line, ex);
            }
        }

        if (!scores.Any())
        {
            throw new CSVParseException("No valid data rows found in CSV", 0, content);
        }

        return scores;
    }

    private Score ParseLine(string line, int lineNumber)
    {
        var fields = ParseCsvLine(line);

        if (fields.Length != 3)
        {
            throw new CSVParseException(
                $"Expected 3 fields but found {fields.Length}. Fields must be: First Name, Second Name, Score", 
                lineNumber, 
                line
            );
        }

        // Validate names are not empty
        if (string.IsNullOrWhiteSpace(fields[0]))
        {
            throw new CSVParseException("First Name cannot be empty", lineNumber, line);
        }

        if (string.IsNullOrWhiteSpace(fields[1]))
        {
            throw new CSVParseException("Second Name cannot be empty", lineNumber, line);
        }

        // Validate and parse score
        if (!int.TryParse(fields[2], out int scoreValue))
        {
            throw new CSVParseException(
                $"Invalid score value '{fields[2]}'. Score must be a valid integer", 
                lineNumber, 
                line
            );
        }

        if (scoreValue < 0 || scoreValue > 100)
        {
            throw new CSVParseException(
                $"Score value {scoreValue} is out of valid range (0-100)", 
                lineNumber, 
                line
            );
        }

        return new Score
        {
            FirstName = fields[0].Trim(),
            SecondName = fields[1].Trim(),
            ScoreValue = scoreValue
        };
    }

    private string[] ParseCsvLine(string line)
    {
        var fields = new List<string>();
        var currentField = new StringBuilder();
        bool inQuotes = false;
        bool hadQuotes = false;

        for (int i = 0; i < line.Length; i++)
        {
            char c = line[i];
            
            if (c == '"')
            {
                if (inQuotes && i + 1 < line.Length && line[i + 1] == '"')
                {
                    currentField.Append('"');
                    i++;
                }
                else
                {
                    inQuotes = !inQuotes;
                    hadQuotes = true;
                }
            }
            else if (c == ',' && !inQuotes)
            {
                fields.Add(hadQuotes ? currentField.ToString() : currentField.ToString().Trim());
                currentField.Clear();
                hadQuotes = false;
            }
            else
            {
                currentField.Append(c);
            }
        }

        if (inQuotes)
        {
            throw new FormatException("Unclosed quotation mark in CSV line");
        }

        fields.Add(hadQuotes ? currentField.ToString() : currentField.ToString().Trim());
        return fields.ToArray();
    }

    private void ValidateHeader(string[] headerFields, string headerLine)
    {
        if (headerFields.Length != _expectedHeaders.Length)
        {
            throw new CSVParseException(
                $"Invalid header. Expected {string.Join(", ", _expectedHeaders)}", 
                1, 
                headerLine
            );
        }

        for (int i = 0; i < _expectedHeaders.Length; i++)
        {
            if (!string.Equals(headerFields[i].Trim(), _expectedHeaders[i], StringComparison.OrdinalIgnoreCase))
            {
                throw new CSVParseException(
                    $"Invalid header field at position {i + 1}. Expected '{_expectedHeaders[i]}' but found '{headerFields[i]}'",
                    1,
                    headerLine
                );
            }
        }
    }
}