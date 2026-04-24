using AmbientSleeper.Models;
using AmbientSleeper.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace AmbientSleeper.ViewModels;

public partial class EqViewModel : ObservableObject
{
    private readonly FeatureGate _features;
    private readonly IEqService _eq;

    [ObservableProperty] private bool isAdvanced; // Pro+ = true
    public GraphicEqSettings Graphic { get; }
    public ParametricEqSettings Parametric { get; }

    public ObservableCollection<string> PresetNames { get; } = new(ParametricEqSettings.Presets.Keys);

    // Selected preset for the Picker
    [ObservableProperty]
    private string? selectedPresetName;

    public EqViewModel(FeatureGate features, IEqService eq)
    {
        _features = features;
        _eq = eq;

        IsAdvanced = _features.AdvancedEditorEnabled;
        Graphic = GraphicEqSettings.CreateDefault5Band();
        Parametric = ParametricEqSettings.CreateDefault10Band();

        // Seed from current service if present
        if (_eq.Current is GraphicEqSettings g)
        {
            Graphic.Bands.Clear();
            foreach (var b in g.Bands) Graphic.Bands.Add(new EqBand { CenterHz = b.CenterHz, GainDb = b.GainDb });
        }
        else if (_eq.Current is ParametricEqSettings p)
        {
            Parametric.Bands.Clear();
            foreach (var b in p.Bands) Parametric.Bands.Add(new ParametricEqBand { FrequencyHz = b.FrequencyHz, Q = b.Q, GainDb = b.GainDb });
        }
    }

    [RelayCommand]
    public void Apply()
    {
        if (IsAdvanced)
            _eq.Apply(Clone(Parametric));
        else
            _eq.Apply(Clone(Graphic));
    }

    [RelayCommand]
    public void Clear() => _eq.Clear();

    [RelayCommand]
    public void LoadPreset(string? presetName)
    {
        if (!IsAdvanced) return;
        if (string.IsNullOrWhiteSpace(presetName)) return;
        if (ParametricEqSettings.Presets.TryGetValue(presetName, out var preset))
        {
            Parametric.Bands.Clear();
            foreach (var b in preset.Bands)
                Parametric.Bands.Add(new ParametricEqBand { FrequencyHz = b.FrequencyHz, Q = b.Q, GainDb = b.GainDb });
        }
    }

    private static GraphicEqSettings Clone(GraphicEqSettings s)
    {
        var c = new GraphicEqSettings();
        foreach (var b in s.Bands)
            c.Bands.Add(new EqBand { CenterHz = b.CenterHz, GainDb = b.GainDb });
        return c;
    }

    private static ParametricEqSettings Clone(ParametricEqSettings s)
    {
        var c = new ParametricEqSettings();
        foreach (var b in s.Bands)
            c.Bands.Add(new ParametricEqBand { FrequencyHz = b.FrequencyHz, Q = b.Q, GainDb = b.GainDb });
        return c;
    }
}