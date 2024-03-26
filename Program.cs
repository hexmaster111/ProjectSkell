// See https://aka.ms/new-console-template for more information



if (args.Length < 1)
{
    Console.WriteLine("No skell file given!");
    Environment.Exit(1);
}

if (!File.Exists(args[0]))
{
    Console.WriteLine("File dose not exist!");
    Environment.Exit(1);
}

var fileText = File.ReadAllText(args[0]);


List<Var> vars = new();
List<SkellFile> skellFiles = new();
// read for [vars] section
{
    var lines = fileText.Split(Environment.NewLine);

    bool inVars = false;
    bool inFile = false;
    string? inFileName = null;
    List<string>? inFileLines = null;
    foreach (var rawLine in lines)
    {
        var l = rawLine.Trim();
        if (l.StartsWith('[') && l.EndsWith(']'))
        {
            var inBrackets = l[1..^1];

            if (inBrackets.Equals("vars", StringComparison.CurrentCultureIgnoreCase))
            {
                if (inVars) throw new Exception("Already in vars!");
                inVars = true;
                continue;
            }

            if (inBrackets.Equals("endvars", StringComparison.CurrentCultureIgnoreCase))
            {
                if (!inVars) throw new Exception("Unexpected end vars!");
                inVars = false;
                continue;
            }

            if (inBrackets.StartsWith("file", StringComparison.CurrentCultureIgnoreCase))
            {
                if (inFile) throw new Exception("Already in file!");
                inFile = true;
                var split = inBrackets.Split(' ');
                inFileName = split[1];
                inFileLines = new List<string>();
                continue;
            }

            if (inBrackets.StartsWith("eof", StringComparison.CurrentCultureIgnoreCase))
            {
                if (!inFile) throw new Exception("Not in a file!");
                inFile = false;
                var c = "";
                foreach (var ifl in inFileLines!) c += ifl + Environment.NewLine;
                var f = new SkellFile(inFileName!, c);
                skellFiles.Add(f);
                continue;
            }
        }

        if (inFile)
        {
            inFileLines!.Add(rawLine);
        }

        if (inVars)
        {
            try
            {
                vars.Add(Var.FromVarLine(l));
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"Error read var from line! {l} {Environment.NewLine} {ex.Message} {Environment.NewLine} {ex}");
                Environment.Exit(1);
            }
        }
    }
}

List<SkellFile> toWrite = new();

foreach (var file in skellFiles)
{
    var name = file.Name;
    var fileContents = file.Contents;
    foreach (var var in vars)
    {
        fileContents = fileContents.Replace("{" + var.Name + "}", var.Value);
        name = name.Replace("{" + var.Name + "}", var.Value);
    }

    toWrite.Add(new SkellFile(name, fileContents));
}

var outFolderName = "out";

foreach (var file in toWrite)
{
    var fPath = Path.Combine(outFolderName, file.Name);
    if (File.Exists(fPath))
    {
        Console.WriteLine($"Oh no! the file \"{fPath}\" already exists!");
        Console.WriteLine($"[(O)ver write , (N)othing] n");
        var ch = Console.ReadKey();
        if (ch.Key == ConsoleKey.N || ch.Key == ConsoleKey.Enter)
        {
            Console.WriteLine($"Skipping \"{fPath}\"...");
            continue;
        }

        if (ch.Key == ConsoleKey.O)
        {
            File.Delete(fPath);
        }
    }

    if (!Directory.Exists(outFolderName))
    {
        Directory.CreateDirectory(outFolderName);
    }

    var fsw = File.CreateText(fPath);

    try
    {
        fsw.Write(file.Contents);
    }
    finally
    {
        fsw.Close();
    }
}


// read for [file] sections
// ask for any ask vars
// change strings
// write files

record SkellFile(string Name, string Contents)
{
};

record Var(string Name, string Value)
{
    public static Var FromVarLine(string raw)
    {
        // varName="the value"

        var split = raw.Split('=');
        if (split.Length != 2) throw new Exception("Expected: varName=The String Value");

        return new(split[0], split[1]);
    }
}