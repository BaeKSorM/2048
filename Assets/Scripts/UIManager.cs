using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;
using System;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public GameObject endingPanel;
    public Button reStartButton;
    public Button quitButton;
    public int score;
    public int bestScore;
    public TMP_Text scoreText;
    public TMP_Text bestScoreText;
    public PlayData playData;
    public string jsonData;
    public string path;
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        string path = Path.Combine(Application.dataPath, "Scripts/PlayData.json");
        string jsonData = File.ReadAllText(path);
        playData = JsonUtility.FromJson<PlayData>(jsonData);
        bestScore = playData.bestScore;
    }
    void Start()
    {
        scoreText = GameObject.Find("ScoreText").GetComponent<TMP_Text>();
        bestScoreText = GameObject.Find("BestScoreText").GetComponent<TMP_Text>();
        bestScoreText.text = playData.bestScore.ToString();
        endingPanel = GameObject.Find("EndingPanel");
        reStartButton = endingPanel.transform.Find("ReStartButton").GetComponent<Button>();
        quitButton = endingPanel.transform.Find("QuitButton").GetComponent<Button>();
        reStartButton.onClick.AddListener(() => ReStart());
        quitButton.onClick.AddListener(() => Quit());
        endingPanel.SetActive(false);
    }
    public void ScoreUp(int earnedPoint)
    {
        score += earnedPoint;
        scoreText.text = score.ToString();
        if (score > bestScore)
        {
            bestScore = score;
            bestScoreText.text = bestScore.ToString();
            playData.bestScore = bestScore;
            jsonData = JsonUtility.ToJson(playData);
            path = Path.Combine(Application.dataPath, "Scripts/PlayData.json");
            File.WriteAllText(path, jsonData);
        }
    }

    [ContextMenu("ResetScore")]
    public void ResetScore()
    {
        playData.bestScore = 0;
        jsonData = JsonUtility.ToJson(playData);
        path = Path.Combine(Application.dataPath, "Scripts/PlayData.json");
        File.WriteAllText(path, jsonData);
    }
    void ReStart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }
}
[Serializable]
public class PlayData
{
    public int bestScore;
}