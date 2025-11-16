using Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class GridManager : MonoBehaviour
{
    [field : SerializeField] public LevelData LevelData { get; private set; }

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

    private void Start ( )
    {
        GenerateGrid ( );
    }

    void GenerateGrid ( )
    {
        if ( LevelData == null
            || tilePrefab == null
            || matchProcessor == null
            || gameManager == null
            || gridContainer == null )
            return;

        int totalTappableTiles = 0;
        int numRows = LevelData.gridLayout.Count;
        int maxCols = 0;

        if ( numRows == 0 )
            return;

        foreach ( LevelRow row in LevelData.gridLayout )
        {
            if ( row.tiles.Count > maxCols )
                maxCols = row.tiles.Count;

            totalTappableTiles += row.tiles.Count ( t => t == TileType.Tappable );
        }

        if ( totalTappableTiles % 2 != 0 )
            return;

        int numPairs = totalTappableTiles / 2;
        if ( LevelData.cardSprites.Count < numPairs )
            return;

        List<Sprite> spritesForGrid = new ( );
        for ( int i = 0; i < numPairs; i++ )
        {
            spritesForGrid.Add(LevelData.cardSprites[i]);
            spritesForGrid.Add(LevelData.cardSprites[i]);
        }

        System.Random rng = new ( );
        List<Sprite> shuffledSprites = spritesForGrid.OrderBy( a => rng.Next()).ToList();

        int spriteIndex = 0;

        float totalGridWidth = ( maxCols * tileWidth ) + ( ( maxCols - 1 ) * spacing );
        float totalGridHeight = ( numRows * tileHeight ) + ( ( numRows - 1 ) * spacing );

        float startX = -totalGridWidth / 2f + tileWidth / 2f;
        float startY = totalGridHeight / 2f - tileHeight / 2f;

        for ( int y = 0; y < numRows; y++ )
        {
            LevelRow row = LevelData.gridLayout[y];
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
                        Sprite faceSprite = shuffledSprites [spriteIndex]; 
                        string spriteId = faceSprite.name;

                        tileComponent.Initialize ( spriteId, faceSprite, LevelData.backSprite, matchProcessor );
                        allTiles.Add ( tileComponent );

                        spriteIndex++;
                    }
                }
            }
        }
        gameManager.InitiaizeGame ( numPairs );
    }
}
