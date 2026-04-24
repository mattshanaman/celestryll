using System.Collections.ObjectModel;

namespace AmbientSleeper.Models;

public abstract class EqSettingsBase { }

public class EqBand
{
    public double CenterHz { get; set; }   // e.g., 60, 250, 1k, 4k, 8k
    public double GainDb { get; set; }     // -12..+12
}

public class GraphicEqSettings : EqSettingsBase
{
    public ObservableCollection<EqBand> Bands { get; } = new();

    public static GraphicEqSettings CreateDefault5Band()
    {
        return new GraphicEqSettings
        {
            Bands =
            {
                new EqBand { CenterHz = 60, GainDb = 0 },
                new EqBand { CenterHz = 250, GainDb = 0 },
                new EqBand { CenterHz = 1000, GainDb = 0 },
                new EqBand { CenterHz = 4000, GainDb = 0 },
                new EqBand { CenterHz = 8000, GainDb = 0 },
            }
        };
    }
}

public class ParametricEqBand
{
    public double FrequencyHz { get; set; } // 20..20k
    public double Q { get; set; } = 1.0;    // 0.3..10
    public double GainDb { get; set; }      // -12..+12
}

public class ParametricEqSettings : EqSettingsBase
{
    public ObservableCollection<ParametricEqBand> Bands { get; } = new();

    public static ParametricEqSettings CreateDefault10Band()
    {
        var s = new ParametricEqSettings();
        var freqs = new[] { 31.5, 63, 125, 250, 500, 1000, 2000, 4000, 8000, 16000 };
        foreach (var f in freqs)
            s.Bands.Add(new ParametricEqBand { FrequencyHz = f, Q = 1.0, GainDb = 0 });
        return s;
    }

    public static readonly Dictionary<string, ParametricEqSettings> Presets = new()
    {
        ["Flat"] = CreateDefault10Band(),
        ["Bass Boost"] = new ParametricEqSettings
        {
            Bands =
            {
                new ParametricEqBand { FrequencyHz = 31.5, Q = 0.7, GainDb = 6 },
                new ParametricEqBand { FrequencyHz = 63,   Q = 0.7, GainDb = 4 },
                new ParametricEqBand { FrequencyHz = 125,  Q = 0.7, GainDb = 2 },
                new ParametricEqBand { FrequencyHz = 250,  Q = 0.7, GainDb = 0 },
                new ParametricEqBand { FrequencyHz = 500,  Q = 0.7, GainDb = 0 },
                new ParametricEqBand { FrequencyHz = 1000, Q = 0.7, GainDb = 0 },
                new ParametricEqBand { FrequencyHz = 2000, Q = 0.7, GainDb = 0 },
                new ParametricEqBand { FrequencyHz = 4000, Q = 0.7, GainDb = -1 },
                new ParametricEqBand { FrequencyHz = 8000, Q = 0.7, GainDb = -2 },
                new ParametricEqBand { FrequencyHz = 16000,Q = 0.7, GainDb = -3 },
            }
        },
        ["Treble Boost"] = new ParametricEqSettings
        {
            Bands =
            {
                new ParametricEqBand { FrequencyHz = 31.5, Q = 0.7, GainDb = -2 },
                new ParametricEqBand { FrequencyHz = 63,   Q = 0.7, GainDb = -1 },
                new ParametricEqBand { FrequencyHz = 125,  Q = 0.7, GainDb = 0 },
                new ParametricEqBand { FrequencyHz = 250,  Q = 0.7, GainDb = 0 },
                new ParametricEqBand { FrequencyHz = 500,  Q = 0.7, GainDb = 0 },
                new ParametricEqBand { FrequencyHz = 1000, Q = 0.7, GainDb = 0 },
                new ParametricEqBand { FrequencyHz = 2000, Q = 0.7, GainDb = 2 },
                new ParametricEqBand { FrequencyHz = 4000, Q = 0.7, GainDb = 3 },
                new ParametricEqBand { FrequencyHz = 8000, Q = 0.7, GainDb = 4 },
                new ParametricEqBand { FrequencyHz = 16000,Q = 0.7, GainDb = 5 },
            }
        }
    };
}