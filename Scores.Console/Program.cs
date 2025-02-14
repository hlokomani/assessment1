using Scores.Core.Services;

if (args.Length == 0)
{
    Console.WriteLine("Please provide a path to the CSV file.");
    Console.WriteLine("Usage: dotnet run -- path/to/file.csv");
    return;
}

var parser = new CSVParser();
var processor = new CsvProcessor(parser);
await processor.ProcessFile(args[0]);