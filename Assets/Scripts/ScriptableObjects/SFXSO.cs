using UnityEngine;

[CreateAssetMenu(fileName = "SFXSO", menuName = "SFX")]
public class SFXSO : ScriptableObject
{
    public AudioClip[] WrongTurnSFX;
    public AudioClip[] RightTurnSFX;
    public AudioClip[] LossSFX;
    public AudioClip[] WinSFX;
}
