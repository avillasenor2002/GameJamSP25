using System.Collections;
using UnityEngine;
using TMPro;

public class GameTimerManager : MonoBehaviour
{
    public float gameDuration = 60f; // Total game time in seconds
    public TextMeshProUGUI timerText;

    public TimedHazardSpawner hazardSpawner; // Reference to the spawner
    public float updateInterval = 1f; // How often to update timer and hazard difficulty

    private float remainingTime;

    void Start()
    {
        remainingTime = gameDuration;
        StartCoroutine(CountdownTimer());
    }

    IEnumerator CountdownTimer()
    {
        while (remainingTime > 0)
        {
            yield return new WaitForSeconds(updateInterval);
            remainingTime -= updateInterval;
            UpdateTimerDisplay();

            // Adjust hazard difficulty
            AdjustHazardDifficulty();
        }

        EndGame();
    }

    void UpdateTimerDisplay()
    {
        int seconds = Mathf.CeilToInt(remainingTime);
        timerText.text = $"Time: {seconds}";
    }

    void AdjustHazardDifficulty()
    {
        if (hazardSpawner == null) return;

        float percentRemaining = remainingTime / gameDuration;

        // Increase frequency and number of hazards as time decreases
        hazardSpawner.spawnInterval = Mathf.Lerp(0.5f, 5f, percentRemaining); // From 5s to 0.5s
        hazardSpawner.maxHazardsPerWave = Mathf.Clamp((int)Mathf.Lerp(5, 1, percentRemaining), 1, 5);
    }

    void EndGame()
    {
        timerText.text = "Time: 0\nGAME OVER";
        hazardSpawner.StopSpawning();
        StartCoroutine(slowsDownEnd());

    }

    IEnumerator slowsDownEnd()
    {
        //Some kind of visual indication like fade out
        yield return new WaitForSeconds(3f);
        SceneSequencing.instance.EndSceneLoad();
    }

}
