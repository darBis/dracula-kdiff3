using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

Console.WriteLine("Processing started.");
var args = System.Environment.GetCommandLineArgs().Skip(2).ToArray();
KDiff3ConfigPatcher.Main(args);

public class KDiff3ConfigPatcher
{
    public static void Main(string[] args)
    {
        // Check for input file argument
        if (args.Length < 2)
        {
            Console.WriteLine($"Usage: {nameof(KDiff3ConfigPatcher)} <inputFilePath> <patchFile> [outputFile]" +
                              $"        If output file is not specified, input file is overwritten!");
            return;
        }

        string inputFilePath = args[0];
        string pathcFilePath = args[1];
        string outputFilePath = args[0];

        if (args.Length > 2)
        {
            outputFilePath = args[2]; // use output file of specified, otherwise overwrite input file
        }

        Console.WriteLine($"Input file: {inputFilePath}");
        Console.WriteLine($"Patch file: {pathcFilePath}");
        Console.WriteLine($"Output file: {outputFilePath}");
        Console.WriteLine();

        var config = ReadConfigFile(inputFilePath);
        var patch = ParsePatch(ReadConfigFile(pathcFilePath));
        Console.WriteLine("Patching config file values:");

        var updateCount = 0;

        // Write the config lines back to the file
        using (var writer = new StreamWriter(outputFilePath))
        {
            foreach (var line in config)
            {
                var prop = ParseConfigLine(line);

                // write new config value if present in patch file
                if (prop.Key != null && patch.ContainsKey(prop.Key))
                {
                    var patchValue = patch[prop.Key];
                    Console.WriteLine($"  {prop.Key}: '{prop.Value}' => '{patchValue}'");
                    writer.WriteLine($"{prop.Key}={patchValue}");

                    updateCount++;
                    continue;
                }

                // write original line otherwise
                writer.WriteLine(line);
            }
        }

        Console.WriteLine();
        Console.WriteLine($"Updated {updateCount} key(s).");
        Console.WriteLine("Processing complete.");
    }

    private static IList<string> ReadConfigFile(string inputFilePath)
    {
        var lines = new List<string>();

        using (var reader = new StreamReader(inputFilePath))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                // Skip empty lines
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                lines.Add(line);
            }
        }

        return lines;
    }

    private static KeyValuePair<string, string> ParseConfigLine(string line)
    {
        // Skip comments
        if (!line.StartsWith("#"))
        {
            // Split the line into key and value by first '=' character
            var kvp = line.Split(new[] { '=' }, 2);

            if (kvp.Length == 2)
            {
                return new KeyValuePair<string, string>(kvp[0].Trim(), kvp[1]);
            }
        }

        return new KeyValuePair<String, String>(null, line);
    }

    private static IDictionary<string, string> ParsePatch(IList<string> lines)
    {
        var patch = new Dictionary<string, string>();

        // Read the file and display it line by line.
        foreach (var line in lines)
        {
            var cfgLine = ParseConfigLine(line);

            if (cfgLine.Key == null)
            {
                continue;
            }

            // Add the key and value to the dictionary
            patch.Add(cfgLine.Key.Trim(), cfgLine.Value);
        }

        return patch;
    }
}