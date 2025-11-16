using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    int
        matchesFound = 0,
        turnsTaken = 0,
        totalPairsToFind = 0;

    [SerializeField] TMP_Text 
        scoreText,
        turnsText;

    [SerializeField] GameObject 
        levelCompletePanel;

    public void InitiaizeGame(int totalPairs)
    {
        totalPairsToFind = totalPairs;
        matchesFound = 0;
        turnsTaken = 0;

        if ( levelCompletePanel) levelCompletePanel.SetActive(false);

        UpdateUI ( );
    }

    public void OnMatchFound()
    {
        matchesFound++;
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
        if(levelCompletePanel) levelCompletePanel.SetActive (true);
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene ( SceneManager.GetActiveScene ( ).name );
    }
}
