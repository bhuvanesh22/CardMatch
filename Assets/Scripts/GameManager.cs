using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    int
        matchesFound = 0,
        score = 0,
        turnsTaken = 0,
        totalPairsToFind = 0;

    [SerializeField] TMP_Text 
        scoreText,
        turnsText;

    [SerializeField] LevelManager 
        levelManager;

    public void InitiaizeGame(int totalPairs)
    {
        totalPairsToFind = totalPairs;
        if(levelManager.CurrentLevelIndex == 0) score = 0;
        matchesFound = 0;
        turnsTaken = 0;

        UpdateUI ( );
    }

    public void OnMatchFound()
    {
        matchesFound++;
        score++;
        turnsTaken++;

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
        if ( scoreText )
            scoreText.text = matchesFound.ToString ( );

        if ( turnsText )
            turnsText.text = turnsTaken.ToString ( ); 
    }

    void CheckForLevelComplete()
    {
        if ( matchesFound != totalPairsToFind )
            return;

        Debug.Log ( "Level Complete" );
        if ( levelManager != null )
            levelManager.OnLevelCompleted ( );
    }
}
