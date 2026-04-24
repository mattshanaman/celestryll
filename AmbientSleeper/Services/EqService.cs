using AmbientSleeper.Models;

namespace AmbientSleeper.Services;

public class EqService : IEqService
{
    public EqSettingsBase? Current { get; private set; }
    public event EventHandler<EqSettingsBase?>? Changed;

    public void Apply(EqSettingsBase settings)
    {
        Current = settings;
        Changed?.Invoke(this, Current);
        // TODO: hook into your audio pipeline here to apply filters
    }

    public void Clear()
    {
        Current = null;
        Changed?.Invoke(this, Current);
        // TODO: clear filters in your audio engine
    }
}