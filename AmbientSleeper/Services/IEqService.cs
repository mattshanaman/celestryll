using AmbientSleeper.Models;

namespace AmbientSleeper.Services;

public interface IEqService
{
    EqSettingsBase? Current { get; }
    event EventHandler<EqSettingsBase?>? Changed;

    void Apply(EqSettingsBase settings);
    void Clear();
}