using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    int
        matchesFound = 0,
        turnsTaken = 0,
        totalPairsToFind = 0;

    [SerializeField] TMP_Text 
        matchesText,
        turnsText,
        scoreText, 
        highScoreText, 
        levelText;

    [SerializeField] LevelManager 
        levelManager;

    int currentRunScore = 0, highScore = 0;

    public void InitializeGame( int totalPairs, int loadedMatches, int loadedTurns )
    {
        totalPairsToFind = totalPairs;
        matchesFound = loadedMatches;
        turnsTaken = loadedTurns;

        UpdateUI ( );
    }

    public void LoadPersistentScores ( int score, int high )
    {
        currentRunScore = score;
        highScore = high;
        UpdateUI ( );
    }

    public void ResetCurrentRunScore ( )
    {
        currentRunScore = 0;
        UpdateUI ( );
    }

    public void OnMatchFound()
    {
        matchesFound++;
        turnsTaken++;
        currentRunScore ++;

        UpdateUI ( );
        CheckForLevelComplete ( );
    }

    public void OnMismatch()
    {
        turnsTaken++;
        UpdateUI ( );
    }

    void UpdateUI()
    {
        if ( matchesText )
            matchesText.text = matchesFound.ToString ( );

        if ( turnsText )
            turnsText.text = turnsTaken.ToString ( );

        if ( levelText )
            levelText.text = levelManager.CurrentLevelIndex.ToString ( );

        if ( scoreText != null )
            scoreText.text = $"Score\n{currentRunScore}";

        if ( highScoreText != null )
            highScoreText.text = $"High Score\n{highScore}";
    }

    void CheckForLevelComplete()
    {
        if ( matchesFound != totalPairsToFind )
            return;

        Debug.Log ( "Level Complete" );
        if ( levelManager != null )
            levelManager.OnLevelCompleted ( );
    }

    public void FinalizeScore ( )
    {
        if ( currentRunScore > highScore )
        {
            highScore = currentRunScore;
            UpdateUI ( );
        }
    }

    public (int matches, int turns) GetLevelState ( )
    {
        return (matchesFound, turnsTaken);
    }

    public (int score, int high) GetPersistentScores ( )
    {
        return (currentRunScore, highScore);
    }
}
