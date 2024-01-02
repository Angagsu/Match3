using UnityEngine;
using UnityEngine.UI;

[DefaultExecutionOrder(501)]
public class Slider : MonoBehaviour
{
    [SerializeField] private UnityEngine.UI.Slider slider;

    private const string PLAYER_PREFS_MUSIC_VOLUME = "MusicVolume";

    private static float volume;
    private AudioSource audioSource;

    private void Awake()
    {
        volume = MusicService.Instance.Volume;
    }

    private void OnEnable()
    {
        audioSource = MusicService.Instance.AudioSource;

        if (audioSource == null)
        {
            audioSource = FindFirstObjectByType<MusicService>().GetComponent<AudioSource>();
        }

        
        slider.value = volume;

        slider.onValueChanged.AddListener(delegate
        {
            ChangeVolume();
        });
    }

    private void ChangeVolume()
    {
        audioSource.volume = slider.value;
        volume = slider.value;
        MusicService.Instance.Volume = volume;

        PlayerPrefs.SetFloat(PLAYER_PREFS_MUSIC_VOLUME, volume);
        PlayerPrefs.Save();
    }

    public static float GetVolume()
    {
        return volume;
    }

    private void OnDestroy()
    {
        slider.onValueChanged.RemoveAllListeners();
    }
}
