using Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    [SerializeField] List<LevelData> allLevels;

    [SerializeField] GameObject levelCompletetPanel, gameCompletePanel;
    [SerializeField] Button nextLevelButton, restartButton, reloadLevelButton, closeButton;

    [SerializeField] GridManager gridManager;
    [SerializeField] GameManager gameManager;
    [SerializeField] MatchProcessor matchProcessor;

    [SerializeField] bool isQuitting = false;

    public int CurrentLevelIndex { get; private set; } = 0;

    private void Start ( )
    {
        Initialize ( );
    }

    void Initialize()
    {
        if ( allLevels == null || allLevels.Count <= 0 || gridManager == null || gameManager == null || matchProcessor == null )
            return;

        if ( nextLevelButton != null )
            nextLevelButton.onClick.AddListener ( LoadNextLevel );

        if ( restartButton != null )
            restartButton.onClick.AddListener ( RestartGame );

        if ( reloadLevelButton != null )
            reloadLevelButton.onClick.AddListener ( RestartCurrentLevel );

        if ( closeButton != null )
            closeButton.onClick.AddListener ( OnClose );

        if ( levelCompletetPanel != null )
            levelCompletetPanel.SetActive ( false );
        if ( gameCompletePanel != null )
            gameCompletePanel.SetActive ( false );

        GameData savedData = SaveSystem.LoadGame ( );
        if ( savedData != null )
        {
            if ( savedData.currentLevelIndex >= allLevels.Count )
            {
                Debug.LogWarning ( "Saved level index is out of bounds (levels may have been removed). Starting new game." );
                SaveSystem.DeleteSaveFile ( );
                gameManager.LoadPersistentScores ( 0, 0 ); // Reset scores
                StartLevel ( );
                return; // Exit Start()
            }

            Debug.Log ( "Loading saved game..." );
            CurrentLevelIndex = savedData.currentLevelIndex;

            gameManager.LoadPersistentScores ( savedData.currentRunScore, savedData.highScore );

            InLevelState stateToLoad = savedData.currentLevelState;
            if ( stateToLoad != null )
            {
                LevelData levelAsset = allLevels [ CurrentLevelIndex ];
                int expectedTiles = 0;
                foreach ( var row in levelAsset.gridLayout )
                    expectedTiles += row.tiles.Count ( t => t == TileType.Tappable );

                if ( stateToLoad.tileData.Count != expectedTiles )
                {
                    Debug.LogWarning ( "Save data tile count mismatch (LevelData may have changed)! Discarding in-progress level state." );
                    stateToLoad = null; // Discard the state, but keep score
                }
            }

            gridManager.GenerateGrid ( allLevels [ CurrentLevelIndex ], savedData.currentLevelState );
        }
        else
        {
            // --- NEW GAME ---
            Debug.Log ( "Starting new game..." );
            gameManager.LoadPersistentScores ( 0, 0 ); // Reset scores
            StartLevel ( );
        }
    }

    void StartLevel ( )
    {
        if(CurrentLevelIndex >= allLevels.Count )
        {
            if( gameCompletePanel != null )
                gameCompletePanel.SetActive ( true );
            gridManager.ClearGrid ( );
            return;
        }
        if ( CurrentLevelIndex == 0 )
            gameManager.ResetCurrentRunScore ( );
        LevelData dataToLoad = allLevels [ CurrentLevelIndex ];
        gridManager.GenerateGrid ( dataToLoad, null );
    }

    public void OnLevelCompleted()
    {
        if ( levelCompletetPanel != null )
            levelCompletetPanel.SetActive ( true );
        SaveProgressOnly ( );
    }

    void LoadNextLevel()
    {
        gridManager.ClearGrid ( );
        CurrentLevelIndex++;

        if(levelCompletetPanel != null) levelCompletetPanel .SetActive ( false );

        if (CurrentLevelIndex < allLevels.Count) 
        {
            StartLevel ( );
            return;
        }

        gameManager.FinalizeScore ( );
        if ( gameCompletePanel != null )
            gameCompletePanel.SetActive ( true );
        SaveProgressOnly ( );
    }

    void RestartCurrentLevel()
    {
        if ( levelCompletetPanel != null )
            levelCompletetPanel.SetActive ( false );
        if ( gameCompletePanel != null )
            gameCompletePanel.SetActive ( false );

        gridManager.ClearGrid ( );
        StartLevel ( );
    }

    void RestartGame()
    {
        if ( gameCompletePanel != null )
            gameCompletePanel.SetActive ( false );

        SaveSystem.DeleteSaveFile ( );
        CurrentLevelIndex = 0;
        StartLevel ( );
    }

    public void SaveInProgressLevel ( )
    {
        if ( isQuitting )
            return; 

        Debug.Log ( "Saving in-progress level..." );
        GameData data = new GameData ( );

        // Get state from all managers
        data.currentLevelIndex = CurrentLevelIndex;
        (data.currentRunScore, data.highScore) = gameManager.GetPersistentScores ( );
        data.currentLevelState = new InLevelState ( );
        (data.currentLevelState.matchesFound, data.currentLevelState.turnsTaken) = gameManager.GetLevelState ( );
        data.currentLevelState.tileData = gridManager.GetTileSaveData ( );

        SaveSystem.SaveGame ( data );
    }

    void SaveProgressOnly ( )
    {
        Debug.Log ( "Saving progress..." );
        GameData data = new GameData ( );
        data.currentLevelIndex = CurrentLevelIndex;
        (data.currentRunScore, data.highScore) = gameManager.GetPersistentScores ( );
        data.currentLevelState = null; 

        SaveSystem.SaveGame ( data );
    }

    void OnApplicationPause ( bool pauseStatus )
    {
        if ( pauseStatus )
            SaveInProgressLevel ( );
    }

    void OnApplicationQuit ( )
    {
        isQuitting = true;
        SaveInProgressLevel ( );
    }

    void OnClose( )
    {
        Application.Quit ( );
    }
}
