using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class Profiler
{
    public Profiler()
    {
        _stopwatch = Stopwatch.StartNew();
        EndRegion("start");
    }
    [Conditional("PROFILE")]
    public void EndRegion(string region)
    {
        _timepoint.Add(Tuple.Create(region, _stopwatch.ElapsedTicks));
        if (region.Length > _lonestRegion)
        {
            _lonestRegion = region.Length;
        }
    }

    [Conditional("PROFILE")]
    public void Report()
    {
        long total_time = _timepoint.Last().Item2;
        string region = "region".PadRight(_lonestRegion + 4) + "ms        percent";
        Trace.WriteLine("\n"+region);
        for (int i = 1; i < _timepoint.Count; i++)
        {
            long ticks = _timepoint[i].Item2-_timepoint[i-1].Item2;
            double percent = (ticks*100.0/total_time);
            Trace.WriteLine($"{_timepoint[i].Item1.PadRight(_lonestRegion+4)}{$"{TimeSpan.FromTicks(ticks).TotalMilliseconds:0.000}".PadRight(10)}{percent:00.00}");
        }
    }

    private Stopwatch _stopwatch;

    int _lonestRegion = 0;

    List<Tuple<string, long>> _timepoint = new();
}

