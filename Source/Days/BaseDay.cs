using System.Configuration;
using System.IO;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

[AttributeUsage(AttributeTargets.Method)]
public class TestAttribute : Attribute
{
};


public class TestCase
{
    public bool Ignore { get; set; }
    public int Part { get; set; }
    public string Expected { get; set; }

    [YamlMember(ScalarStyle = YamlDotNet.Core.ScalarStyle.Literal)]
    public string InputText { get; set; }
    public string InputFile { get; set; }

    public string Input(string path)
    {
        if (string.IsNullOrWhiteSpace(InputFile))
        {
            return InputText;
        }
        return File.ReadAllText(Path.Combine(path, InputFile));
    }

}


public class BaseDay
{
    public virtual string Run(int part, string rawData) => "Not Implemented";

    protected Stopwatch Stopwatch;
    public string Run(int part)
    {
        string partName = $"Source/Days/{Name}/input{part}.txt";
        if (!File.Exists(partName))
        {
            partName = $"Source/Days/{Name}/input1.txt";

            if (!File.Exists(partName))
            {
                return $"input {partName} not found";
            }
        }
        double benchmark = double.Parse(ConfigurationManager.AppSettings["Benchmark"]);
        string result = "";
        string rawData = File.ReadAllText(partName);
        Stopwatch = Stopwatch.StartNew();
        result = Run(part, rawData, false);
        double elapsed = Stopwatch.Elapsed.TotalMilliseconds;
        Trace.WriteLine($"\n  Solution: {elapsed}ms");
        if (benchmark > elapsed)
        {
            int warmup = (int)((benchmark*.1) / elapsed);
            int iterations = (int)((benchmark * .9) / elapsed);
            Trace.WriteLine($"  Benchmarking for {benchmark  }ms with warmup: {warmup} iterations: {iterations}");

            if (warmup > 0)
            {
                for (int i = 0; i < warmup; i++)
                {
                    Run(part, rawData, false);
                }
            }
            Stopwatch = Stopwatch.StartNew();
            for (int i = 0; i < iterations; i++)
            {
                Run(part, rawData, false);
            }
            Trace.WriteLine($"  Benchmark:       {Stopwatch.Elapsed.TotalMilliseconds / iterations:0.000}ms");
        }
        return result;
    }

    protected void DoneParsing()
    {
        if (Stopwatch != null)
        {
            Trace.Write($"\n  Parsing:  {Stopwatch.Elapsed.TotalMilliseconds}ms");
            Stopwatch.Restart();
        }
    }

    public string TestInput(int part, int testNum)
    {
        string testName = $"Source/Days/{Name}/test{part}-{testNum}.txt";
        if (!File.Exists(testName))
        {
            if (part != 1)
            {
                return TestInput(1, testNum);
            }
            if (testNum != 1)
            {
                return TestInput(part, 1);
            }
            return null;
        }

        return File.ReadAllText(testName);
    }

    public string TestResult(int part, int testNum)
    {
        string testName = $"Source/Days/{Name}/result{part}-{testNum}.txt";
        if (!File.Exists(testName))
        {
            return null;
        }

        return File.ReadAllText(testName);
    }

    public virtual string Run(int part, string rawData, bool isTest) { InTest = isTest; return Run(part, rawData); }

    public bool RunTests(int part)
    {
        bool passed = true;
        foreach (var test in GetType().GetMethods().Where(x => x.GetCustomAttributes(typeof(TestAttribute), false).Any()))
        {
            try
            {
                test.Invoke(this, null);
            }
            catch (Exception e)
            {
                passed = false;
                Trace.WriteLine($"\n{e}");
            }
        }
        if (!passed) { return false; }

        string testFile = $"Source/Days/{Name}/tests.yaml";

        if (!File.Exists(testFile))
        {
            return true;
        }


        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(UnderscoredNamingConvention.Instance)
            .Build();
        string yaml = File.ReadAllText(testFile);
        var allTests = deserializer.Deserialize<TestCase[]>(yaml);

        int testNum = 0;
        foreach (var test in allTests)
        {
            testNum++;
            if (test.Ignore || (test.Part != 0 && part != test.Part))
            {
                continue;
            }
            try
            {
                string result = Run(part, test.Input(Path.GetDirectoryName(testFile)), true);
                if (result != test.Expected)
                {
                    throw new Exception($"Test Failed: expected '{test.Expected}', received '{result}'");
                }
                Trace.Write($" ");
            }
            catch (Exception e)
            {
                Trace.WriteLine($"\nTest {part}-{testNum} failed: {e.Message}\n  ");
                passed = false;
            }
        }
        return passed;
    }

    public string Name => this.GetType().Namespace.TrimStart('_');

    public int Number => int.Parse(Name[3..]);

    public int Part { get => 0; }


    public bool InTest { get; set; }
}