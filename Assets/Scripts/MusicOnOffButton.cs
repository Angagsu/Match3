using UnityEngine;
using UnityEngine.UI;

[DefaultExecutionOrder(500)]
public class MusicOnOffButton : MonoBehaviour
{
    [SerializeField] private Button musicOnOffButton;
    [SerializeField] private Sprite muteImage;
    [SerializeField] private Sprite unmuteImage;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = MusicService.Instance.AudioSource;  

        musicOnOffButton.onClick.AddListener(() =>
        {
            if (audioSource == null)
            {
                audioSource = FindFirstObjectByType<MusicService>().GetComponent<AudioSource>();
            }
            MuteUnmuteMusic();
        });
    }

    private void MuteUnmuteMusic()
    {
        audioSource.mute = !audioSource.mute;
        if (audioSource.mute)
        {
            musicOnOffButton.image.sprite = muteImage;
        }
        else
        {
            musicOnOffButton.image.sprite = unmuteImage;
        }
    }

    private void OnDestroy()
    {
        musicOnOffButton.onClick.RemoveAllListeners();
    }
}
