namespace Scores.Core.Models;

public class Score
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string SecondName { get; set; } = string.Empty;
    public int ScoreValue { get; set; }
}