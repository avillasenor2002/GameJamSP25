using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EntryEnter : MonoBehaviour
{
    public TMP_InputField playerNameInput;
    public GameObject submitScoreButton;

    public Transform scoreEntryParent;
    public GameObject scoreEntryPrefab;
    public GameObject scoreboardContainer;
    public GameObject justRandomBlackTransparentBackground;

    private void Start()
    {
        scoreboardContainer.SetActive(false);
    }

    public void SubmitScore()
    {
        string playerName = playerNameInput.text.Trim();
        if (string.IsNullOrEmpty(playerName)) playerName = "?????";

        Ranking.NameAndScore updatedScore = new Ranking.NameAndScore
        {
            Score = Mathf.FloorToInt(Scores.instance.currentScore),
            Name = playerName
        };

        Ranking.instance.AcceptNewScore(updatedScore);
        Ranking.instance.SaveScores();
        UpdateScoreboard();

        playerNameInput.gameObject.SetActive(false);
        submitScoreButton.gameObject.SetActive(false);
        justRandomBlackTransparentBackground.gameObject.SetActive(false);
    }
    private void UpdateScoreboard()
    {
        foreach (Transform child in scoreEntryParent)
        {
            Destroy(child.gameObject);
        }

        int scoreCount = Ranking.instance.GetScoreCount();
        for (int i = 0; i < scoreCount; i++)
        {
            Ranking.NameAndScore score = Ranking.instance.GetScoreAt(i);
            GameObject entry = Instantiate(scoreEntryPrefab, scoreEntryParent);
            entry.GetComponent<ScoreBoard>().Setup(score, i + 1);
        }

        scoreboardContainer.SetActive(true);
    }

    public void ShowEntryUI()
    {
        playerNameInput.gameObject.SetActive(true);
        submitScoreButton.gameObject.SetActive(true);
        justRandomBlackTransparentBackground.gameObject.SetActive(true);
    }

}

