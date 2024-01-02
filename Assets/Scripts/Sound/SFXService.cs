using UnityEngine;

public class SFXService : MonoBehaviour
{
    [SerializeField] private SFXSO SFXSO;
    [SerializeField] private BoardService boardService;
    [SerializeField] private GameStateService gameStateService;

    private float volume = 1;


    private void OnEnable()
    {
        boardService.Matched += BoardService_OnMatched;
        boardService.NotMatched += BoardService_OnNotMatched;
        gameStateService.OnGameOver += GameStateService_OnGameOver;
        gameStateService.OnLevelComplete += GameStateService_OnLevelComplete;
    }
    private void OnDisable()
    {
        boardService.Matched -= BoardService_OnMatched;
        boardService.NotMatched -= BoardService_OnNotMatched;
        gameStateService.OnGameOver -= GameStateService_OnGameOver;
        gameStateService.OnLevelComplete -= GameStateService_OnLevelComplete;
    }

    private void GameStateService_OnLevelComplete()
    {
        PlaySFX(SFXSO.WinSFX, transform.position, volume);
    }

    private void GameStateService_OnGameOver()
    {
        PlaySFX(SFXSO.LossSFX, transform.position, volume);
    }

    private void BoardService_OnNotMatched()
    {
        PlaySFX(SFXSO.WrongTurnSFX, transform.position, volume);
    }

    private void BoardService_OnMatched()
    {
        PlaySFX(SFXSO.RightTurnSFX, transform.position, volume);
    }

    public void PlaySFX(AudioClip audioClip, Vector2 position, float volumeMultiplayer = 1)
    {
        AudioSource.PlayClipAtPoint(audioClip, position, volumeMultiplayer * volume);
    }

    public void PlaySFX(AudioClip[] audioClips, Vector2 position, float volume = 1)
    {
        PlaySFX(audioClips[Random.Range(0, audioClips.Length)], position, volume);
    }
}
