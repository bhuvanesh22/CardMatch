using System.Collections;
using System.Collections.Generic;
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

    public int CurrentLevelIndex { get; private set; } = 0;

    private void Start ( )
    {
        Initialize ( );
    }

    void Initialize()
    {
        if ( allLevels == null || allLevels.Count <= 0 || gridManager == null || gameManager == null )
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

        StartLevel ( );
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
        LevelData dataToLoad = allLevels [ CurrentLevelIndex ];
        gridManager.GenerateGrid ( dataToLoad );
    }

    public void OnLevelCompleted()
    {
        if ( levelCompletetPanel != null )
            levelCompletetPanel.SetActive ( true );
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

        if ( gameCompletePanel != null )
            gameCompletePanel.SetActive ( true );
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

        CurrentLevelIndex = 0;
        StartLevel ( );
    }

    void OnClose( )
    {
        Application.Quit ( );
    }
}
