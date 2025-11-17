using UnityEngine;

[RequireComponent ( typeof ( AudioSource ), typeof ( AudioSource ) )]
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] private AudioSource 
        musicSource,
        sfxSource;

    public AudioClip 
        backgroundMusic,
        cardFlipClip,
        matchClip,
        mismatchClip,
        gameOverClip;

    void Awake ( )
    {
        if ( Instance == null )
        {
            Instance = this;
            DontDestroyOnLoad ( gameObject ); // Make this object persist
        }
        else
            Destroy ( gameObject );
    }

    void Start ( )
    {
        if ( musicSource != null && backgroundMusic != null )
        {
            musicSource.clip = backgroundMusic;
            musicSource.loop = true;
            musicSource.Play ( );
        }
    }

    public void PlayFlip ( )
    {
        if ( cardFlipClip != null )
            sfxSource.PlayOneShot ( cardFlipClip );
    }

    public void PlayMatch ( )
    {
        if ( matchClip != null )
            sfxSource.PlayOneShot ( matchClip );
    }

    public void PlayMismatch ( )
    {
        if ( mismatchClip != null )
            sfxSource.PlayOneShot ( mismatchClip );
    }

    public void PlayGameOver ( )
    {
        if ( gameOverClip != null )
            sfxSource.PlayOneShot ( gameOverClip );
    }
}