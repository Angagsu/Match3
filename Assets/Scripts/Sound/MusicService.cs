using UnityEngine;
using UnityEngine.UI;

[DefaultExecutionOrder(10)]
public class MusicService : MonoBehaviour
{
    public static MusicService Instance { get; private set; }


    public AudioSource AudioSource { get; private set; }
    public float Volume { get; set; }

    [SerializeField] private MusicSFXSO musicSFXSOs;

    private AsyncSceneLoader asyncSceneLoader;

    private readonly string PLAYER_PREFS_MUSIC_VOLUME = "MusicVolume";
    private readonly string musicTag = "Music";

    private void Awake()
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag(musicTag);
        
        if (objs.Length > 1)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);

        Instance = this;
        Debug.Log("Heloo");
        asyncSceneLoader = AsyncSceneLoader.Instance;

        Volume = PlayerPrefs.GetFloat(PLAYER_PREFS_MUSIC_VOLUME, 0.3f);

        AudioSource = GetComponent<AudioSource>();

        PlayMusic(musicSFXSOs.Musics[Random.Range(0, musicSFXSOs.Musics.Length)], Volume);
    }

    private void OnEnable()
    {
        asyncSceneLoader.OnLoadingIsDone += SceneLoader_OnLoadingIsDone;
        asyncSceneLoader.OnLoadingIsStarted += AsyncSceneLoader_OnLoadingIsStarted;
    }

    private void AsyncSceneLoader_OnLoadingIsStarted()
    {
        Volume = Slider.GetVolume();
        PlayerPrefs.SetFloat(PLAYER_PREFS_MUSIC_VOLUME, Volume);
    }

    private void SceneLoader_OnLoadingIsDone()
    {  
        PlayMusic(musicSFXSOs.Musics[Random.Range(0, musicSFXSOs.Musics.Length)], Volume);
    }

    public void PlayMusic(AudioClip audioClip, float volumeMultiplayer = 1)
    {   
        AudioSource.clip = audioClip;
        AudioSource.volume = volumeMultiplayer * Volume;
        AudioSource.Play();
        AudioSource.loop = true;
    }

    public void PlayMusic(AudioClip[] audioClips, float volume)
    {
        PlayMusic(audioClips[Random.Range(0, audioClips.Length)], volume);
    }
}
