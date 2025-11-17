using Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class GridManager : MonoBehaviour
{
    //[field : SerializeField] public LevelData LevelData { get; private set; }

    [SerializeField] GameObject 
        tilePrefab,
        blockerPrefab,
        emptyPrefab;

    [SerializeField]
    float
        tileWidth = 128f,
        tileHeight = 128f,
        spacing = 40f;

    [ SerializeField] RectTransform gridContainer;
    [SerializeField] MatchProcessor matchProcessor;
    [SerializeField] GameManager gameManager;

    List<Tile> allTiles = new ( );
    LevelData currentLevelData;

    public void ClearGrid ()
    {
        foreach ( Transform child in gridContainer )
            Destroy ( child.gameObject );

        allTiles.Clear();
    }

    public void GenerateGrid ( LevelData levelData, InLevelState savedState = null)
    {
        currentLevelData = levelData;

        if ( currentLevelData == null
            || tilePrefab == null
            || matchProcessor == null
            || gameManager == null
            || gridContainer == null )
            return;

        int totalTappableTiles = 0;
        int numRows = currentLevelData.gridLayout.Count;
        int maxCols = 0;

        if ( numRows == 0 )
            return;

        foreach ( LevelRow row in currentLevelData.gridLayout )
        {
            if ( row.tiles.Count > maxCols )
                maxCols = row.tiles.Count;

            totalTappableTiles += row.tiles.Count ( t => t == TileType.Tappable );
        }

        if ( totalTappableTiles % 2 != 0 )
            return;

        int numPairs = totalTappableTiles / 2;
        if ( currentLevelData.cardSprites.Count < numPairs )
            return;

        List<Sprite> shuffledSprites;

        if(savedState == null)
        {
            List<Sprite> spritesForGrid = new ( );
            for ( int i = 0; i < numPairs; i++ )
            {
                spritesForGrid.Add ( currentLevelData.cardSprites [ i ] );
                spritesForGrid.Add ( currentLevelData.cardSprites [ i ] );
            }
            System.Random rng = new ( );
            shuffledSprites = spritesForGrid.OrderBy ( a => rng.Next ( ) ).ToList ( );
        }
        else
        {
            shuffledSprites = new List<Sprite> ( );
            foreach ( var tileSave in savedState.tileData )
                shuffledSprites.Add ( GetSpriteByName ( tileSave.spriteId ) );

        }
        int spriteIndex = 0;
        float totalGridWidth = ( maxCols * tileWidth ) + ( ( maxCols - 1 ) * spacing );
        float totalGridHeight = ( numRows * tileHeight ) + ( ( numRows - 1 ) * spacing );
        float startX = -totalGridWidth / 2f + tileWidth / 2f;
        float startY = totalGridHeight / 2f - tileHeight / 2f;
        allTiles.Clear ( );

        for ( int y = 0; y < numRows; y++ )
        {
            LevelRow row = currentLevelData.gridLayout[y];
            for ( int x = 0; x < row.tiles.Count; x++ )
            {
                TileType tileType = row.tiles[x];

                Vector2 position = new Vector2 (
                    startX + ( x * ( tileWidth + spacing ) ),
                    startY - ( y * ( tileHeight + spacing ) )
                );

                GameObject prefabToSpawn = tileType switch
                {
                    TileType.Tappable => tilePrefab,
                    TileType.Blocker => blockerPrefab,
                    TileType.Empty => emptyPrefab,
                    _ => null
                };

                if ( prefabToSpawn == null )
                    continue;

                GameObject newTileObj = Instantiate ( prefabToSpawn, gridContainer );
                RectTransform newTileRect = newTileObj.GetComponent<RectTransform> ( );

                newTileRect.anchoredPosition = position;
                newTileRect.sizeDelta = new Vector2 ( tileWidth, tileHeight );

                newTileObj.name = $"{tileType}_{x}_{y}";

                if(tileType == TileType.Tappable )
                {
                    Tile tileComponent = newTileObj.GetComponent<Tile> ( );
                    if(tileComponent != null)
                    {
                        if ( spriteIndex >= shuffledSprites.Count )
                        {
                            //Debug.LogError ( $"CRITICAL SAVE/LEVEL MISMATCH: Ran out of sprites at index {spriteIndex} (shuffledSprites.Count is {shuffledSprites.Count}). This means the LevelData has more Tappable tiles than the save file. Stopping grid generation to prevent crash." );
                            ClearGrid ( ); // Clear the half-built grid
                            GenerateGrid ( currentLevelData, null ); // Recall this function, but force a new level
                            return; // Stop the entire GenerateGrid function
                        }
                        Sprite faceSprite = shuffledSprites [spriteIndex]; 
                        string spriteId = faceSprite.name;

                        tileComponent.Initialize ( spriteId, faceSprite, currentLevelData.backSprite, matchProcessor );
                        allTiles.Add ( tileComponent );

                        spriteIndex++;
                    }
                }
            }
        }

        if ( savedState != null )
        {
            for ( int i = 0; i < allTiles.Count; i++ )
                allTiles [ i ].RestoreState ( savedState.tileData [ i ].state );
            gameManager.InitializeGame ( numPairs, savedState.matchesFound, savedState.turnsTaken );
        }
        else
            gameManager.InitializeGame ( numPairs, 0, 0 );

        matchProcessor.ResetQueue ( );
    }

    private Sprite GetSpriteByName ( string name )
    {
        if ( currentLevelData.backSprite.name == name )
            return currentLevelData.backSprite;

        return currentLevelData.cardSprites.Find ( s => s.name == name );
    }

    public List<TileSaveData> GetTileSaveData ( )
    {
        List<TileSaveData> data = new List<TileSaveData> ( );
        foreach ( Tile tile in allTiles )
            data.Add ( tile.GetSaveData ( ) );
        return data;
    }
}
