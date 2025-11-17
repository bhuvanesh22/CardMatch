using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build.Content;
using UnityEngine;

public class MatchProcessor : MonoBehaviour
{
    [SerializeField] GameManager gameManager;

    [SerializeField] int maxQueuedTaps = 4;

    Queue<Tile> tapQueue = new ( );
    bool isProcessing = false;

    public void AddTileToQueue(Tile tappedTile) => tapQueue.Enqueue( tappedTile );
    public bool CanProcessTap() => tapQueue.Count < maxQueuedTaps;

    private void Update ( )
    {
        if ( isProcessing || tapQueue.Count < 2 )
            return;

        isProcessing = true;
        StartCoroutine ( ProcessMatchCheck ( ) );
    }

    IEnumerator ProcessMatchCheck()
    {
        Tile tileA = tapQueue.Dequeue ( );
        Tile tileB = tapQueue.Dequeue ( );

        yield return new WaitForSeconds ( tileA.FlipDuration );
        Debug.Log ( tileA.SpriteId );
        Debug.Log ( tileB.SpriteId );
        if (tileA.SpriteId == tileB.SpriteId )
        {
            tileA.HandleMatch ( );
            tileB.HandleMatch ( );
            gameManager?.OnMatchFound ( );
        }
        else
        {
            tileA.HandleMismatch ( );
            tileB.HandleMismatch ( );
            gameManager?.OnMismatch ( );
        }

        yield return new WaitForSeconds ( 1.2f );
        isProcessing = false;
    }

    public void ResetQueue ( )
    {
        tapQueue.Clear ( );
        isProcessing = false;
    }
}
