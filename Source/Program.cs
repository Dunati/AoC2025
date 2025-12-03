using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Configuration;
using System.Threading.Tasks;


public static class Runner
{
    private static readonly string CookiePath = ConfigurationManager.AppSettings["CookieFile"];
    private static readonly int AoC_Year = int.Parse(ConfigurationManager.AppSettings["Year"]);

    private static void RunAllDays()
    {
        foreach (Type t in Assembly.GetExecutingAssembly().GetTypes().Where(x => x.Name.StartsWith("Day")).OrderBy(x => x.Name))
        {
            Run(t, 0);
        }
        foreach (Type t in Assembly.GetExecutingAssembly().GetTypes().Where(x => x.Name.StartsWith("_Day")).OrderBy(x => x.Name))
        {
            Run(t, 0);
        }
    }

    static bool IsDayOpen(int day, int year)
    {
        var now = DateTime.UtcNow.AddHours(-5);
        if (now.Year > year || (now.Month == 12 && now.Day >= day))
        {
            return true;
        }

        var open = new DateTime(year, 12, day);

        var remaining = (open - now);
        Trace.Write($"The next puzzle opens in {remaining.Days*24+remaining.Hours}:{remaining.Minutes}:{remaining.Seconds:00} ");
        return false;
    }

    static async Task<bool> GetInput(int day, int year, string filename)
    {
        if (!IsDayOpen(day, year))
        {
            return false;
        }


        if (!File.Exists(filename))
        {
            string cookie = File.ReadAllText(CookiePath);
            var uri = new Uri("https://adventofcode.com");
            var cookies = new CookieContainer();
            cookies.Add(uri, new System.Net.Cookie("session", cookie));
            using var file = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.None);
            using var handler = new HttpClientHandler() { CookieContainer = cookies };
            using var client = new HttpClient(handler) { BaseAddress = uri };
            using var response = await client.GetAsync($"/{year}/day/{day}/input");
            using var stream = await response.Content.ReadAsStreamAsync();
            await stream.CopyToAsync(file);
        }
        return true;
    }

    public static bool CheckCookie()
    {
        if (!File.Exists(CookiePath))
        {
            Trace.WriteLine(@$"
To download inputs you need to copy your AoC cookie to {CookiePath}:

1. Log in to Advent of Code 
2. Right click the page and click ""inspect""
3. Navigate to the ""Network"" tab
4. Click repoad
5. Click on any request, and go to the ""Headers"" tab
6. Search through the ""Request Headers"" for a header named cookie.
7. You should find one value named Cookie that starts with ""session="", followed by a long string of hexadecimal characters. Copy the hex part of the value (without ""session="") and save it to {CookiePath}

");
            return false;
        }
        return true;
    }

    public static void Main(string[] args)
    {
        if (!Debugger.IsAttached)
        {
            Trace.Listeners.Add(new ConsoleTraceListener());
            Trace.AutoFlush = true;
        }
        Type type;
        int day = 0;
        int part = 0;
        bool newDay = false;

        if (!CheckCookie())
        {
            return;
        }

        if (args.Length > 0)
        {
            if (args[0] == "all")
            {
                RunAllDays();
                return;
            }
            else if (args[0] == "NewDay")
            {
                newDay = true;
            }
            else
            {
                if (!int.TryParse(args[0], out day))
                {
                    Trace.WriteLine($"Cannot parse {args[0]} as a day number");
                    return;
                }
            }
            if (args.Length > 1 && !int.TryParse(args[1], out part))
            {
                Trace.WriteLine($"Cannot parse {args[1]} as a part number");
                return;
            }

            if (part < 0 || part > 2)
            {
                Trace.WriteLine($"part {part} is not a valid part number");
                return;
            }
        }
        if (day == 0)
        {
            type = Assembly.GetExecutingAssembly().GetTypes().Where(x => x.Name.StartsWith("Day")).OrderBy(x => x.Namespace).Last();
            if (type.Namespace == null){
                Trace.WriteLine("No days created, automatically generating a new day for you");
                newDay = true;
            }
        }
        else
        {
            type = Assembly.GetExecutingAssembly().GetType($"Day{day:00}.Day");
            if (type == null)
            {
                Trace.WriteLine($"Cannot find class Day{day:00} specified on the command line");
                return;
            }
        }

        if (newDay)
        {
            if (type.Namespace == null)
            {
                day = 1;
            }
            else
            {
                day = int.Parse(type.Namespace[^2..]) + 1;
            }
            Directory.CreateDirectory($"{DayPath(day)}");
            string template = File.ReadAllText($"{DayPath(0)}/Day.cs");
            using var stream = new StreamWriter($"{DayPath(day)}/Day.cs");
            stream.WriteLine($"namespace Day{day:00};");
            stream.WriteLine();
            stream.WriteLine(template);

            CopyFiles(day);

            GetInput(day, AoC_Year, $"{DayPath(day)}/input1.txt").Wait();
        }
        else
        {
            Run(type, part);
        }
    }

    static void CopyFiles(int day)
    {
        Directory.GetFiles(DayPath(0), "*.txt").AsParallel().ForAll(x => File.Copy(x, Path.Combine(DayPath(day), Path.GetFileName(x))));
    }

    static string DayPath(int day)
    {
        return $"Source/Days/Day{day:00}";
    }

    static void Run(Type type, int part)
    {
        BaseDay day = (BaseDay)Activator.CreateInstance(type);
        

        int number = day.Number;

        if (!GetInput(number, AoC_Year, $"{DayPath(number)}/input1.txt").Result)
        {
            Trace.WriteLine("hold yer horses");
            return;
        }

        if (part != 0)
        {
            RunPart(day, part);
        }
        else
        {
            if (day.Part == 0)
            {
                var _ = RunPart(day, 1) && RunPart(day, 2);
            }
            else
            {
                RunPart(day, day.Part);
            }
        }
    }


    static bool RunPart(BaseDay day, int part)
    {
        Trace.Write($"Running {day.Name}-{part} ");
        if (day.RunTests(part))
        {
            day.InTest = false;
            Trace.WriteLine($"  result: {day.Run(part)}\n");
            return true;
        }
        return false;
    }


}