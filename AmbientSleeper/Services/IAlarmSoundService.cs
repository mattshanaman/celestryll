using System.Threading.Tasks;

namespace AmbientSleeper.Services;

public interface IAlarmSoundService
{
    // Return a URI or identifier for the selected alarm sound
    Task<string> PickSystemSoundAsync();

    // Play the alarm now (one-shot or loop=false)
    void PlayAlarm(string uriOrName, float volume = 1.0f, bool loop = false);

    // Stop any playing alarm sound (if you later support looped alarms)
    void StopAlarm();
}