using Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class Tile : MonoBehaviour
{
    [SerializeField] Image faceImage, backImage;
    [SerializeField] Button button;

    [field: SerializeField] public float FlipDuration { get; private set; } = .3f;
    [field: SerializeField] public TileState CurrentState { get; private set; }
    [field: SerializeField] public string SpriteId { get; private set; }

    MatchProcessor matchProcessor;

    public void Initialize(string spriteId, Sprite faceSprite, Sprite backSprite, MatchProcessor processor)
    {
        SpriteId = spriteId;
        faceImage.sprite = faceSprite;
        backImage.sprite = backSprite;
        matchProcessor = processor;

        faceImage.gameObject.SetActive ( false );
        backImage.gameObject.SetActive ( true );

        button.onClick.AddListener ( OnTileTapped );
        button.interactable = true;
    }

    void OnTileTapped()
    {
        if ( CurrentState != TileState.Hidden || !matchProcessor.CanProcessTap ( ) )
            return;

        AudioManager.Instance.PlayFlip ( );

        CurrentState = TileState.Revealed;
        button.interactable = false;
        matchProcessor.AddTileToQueue ( this );
        StartCoroutine ( FlipUpAnimation ( ) );
    }

    public void HandleMismatch()
    {
        CurrentState = TileState.Processing;
        button.interactable = false;
        StartCoroutine ( FlipDownAnimation ( ) );
    }

    public void HandleMatch()
    {
        CurrentState = TileState.Matched;
        button.interactable = false;
        StartCoroutine ( MatchEffectAnimation ( ) );
    }

    IEnumerator FlipUpAnimation()
    {
        Vector3 originalScale = transform.localScale;
        Vector3 targetScale = new Vector3 ( 0, originalScale.y, originalScale.z );

        for(float t = 0; t < FlipDuration /2; t += Time.deltaTime )
        {
            transform.localScale = Vector3.Lerp ( originalScale, targetScale, t / ( FlipDuration / 2 ) );
            yield return null;
        }
        transform.localScale = targetScale;

        backImage.gameObject.SetActive ( false );
        faceImage.gameObject.SetActive ( true );

        targetScale = originalScale;

        for ( float t = 0; t < FlipDuration / 2; t += Time.deltaTime )
        {
            transform.localScale = Vector3.Lerp ( transform.localScale, targetScale, t / ( FlipDuration / 2 ) );
            yield return null;
        }
        transform.localScale = targetScale;
    }

    IEnumerator FlipDownAnimation()
    {
        yield return new WaitForSeconds ( 1.0f );

        Vector3 originalScale = transform.localScale;
        Vector3 targetScale = new Vector3 ( 0, originalScale.y, originalScale.z );

        for ( float t = 0; t < FlipDuration / 2; t += Time.deltaTime )
        {
            transform.localScale = Vector3.Lerp ( originalScale, targetScale, t / ( FlipDuration / 2 ) );
            yield return null;
        }
        transform.localScale = targetScale;

        backImage.gameObject.SetActive ( true );
        faceImage.gameObject.SetActive ( false );

        targetScale = originalScale;

        for ( float t = 0; t < FlipDuration / 2; t += Time.deltaTime )
        {
            transform.localScale = Vector3.Lerp ( transform.localScale, targetScale, t / ( FlipDuration / 2 ) );
            yield return null;
        }
        transform.localScale = targetScale;

        AudioManager.Instance.PlayMismatch ( );
        CurrentState = TileState.Hidden;
        button.interactable = true;
    }

    private IEnumerator MatchEffectAnimation ( )
    {
        yield return new WaitForSeconds ( 0.5f );

        Vector3 originalScale = transform.localScale;
        Vector3 targetScale = Vector3.zero;

        for ( float t = 0; t < FlipDuration; t += Time.deltaTime )
        {
            transform.localScale = Vector3.Lerp ( originalScale, targetScale, t / FlipDuration );
            yield return null;
        }
        AudioManager.Instance.PlayMatch ( );
        gameObject.SetActive ( false );
    }

    public TileSaveData GetSaveData ( )
    {
        TileState stateToSave = CurrentState;
        if ( stateToSave == TileState.Revealed || stateToSave == TileState.Processing )
            stateToSave = TileState.Hidden;

        return new TileSaveData
        {
            spriteId = SpriteId,
            state = stateToSave
        };
    }
    public void RestoreState ( TileState state )
    {
        CurrentState = state;

        if ( state == TileState.Matched )
        {
            // Tile was already matched, just hide it
            gameObject.SetActive ( false );
            button.interactable = false;
        }
        else // state == TileState.Hidden
        {
            // Ensure it's in the default hidden state
            faceImage.gameObject.SetActive ( false );
            backImage.gameObject.SetActive ( true );
            transform.localScale = Vector3.one; // Reset scale
            button.interactable = true;
        }
    }
}
