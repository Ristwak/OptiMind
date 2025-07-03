using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.IO;

[System.Serializable]
public class MCQQuestion
{
    public string id;
    public string prompt;
    public List<string> options;
    public List<string> correct;
}

public static class JsonHelper
{
    [System.Serializable]
    private class Wrapper<T>
    {
        public T[] Items;
    }

    public static T[] FromJson<T>(string json)
    {
        string wrapped = "{\"Items\":" + json + "}";
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(wrapped);
        return wrapper.Items;
    }
}

public class MCQLevelManager : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI questionText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timerText;

    [Header("Options Container")]
    public GameObject optionButtonPrefab;
    public Transform optionsContainer;

    [Header("Game Settings")]
    public float timePerQuestion = 10f;
    public int scorePoints = 1;

    [Header("Panels")]
    public GameObject comingSoonPanel;
    public GameObject exitPanel;

    private List<MCQQuestion> questions = new List<MCQQuestion>();
    private int currentIndex = 0;
    private List<string> playerChoices = new List<string>();
    private Dictionary<string, Button> optionButtons = new Dictionary<string, Button>();
    private Coroutine timerCoroutine;
    private bool answered;
    private int score;

    void Start()
    {
        score = 0;
        comingSoonPanel.SetActive(false);
        exitPanel.SetActive(false);
        StartCoroutine(LoadQuestionsFromStreamingAssets());
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!exitPanel.activeSelf)
                exitPanel.SetActive(true);
        }

        if (exitPanel.activeSelf)
        {
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = 1f;
        }
    }

    IEnumerator LoadQuestionsFromStreamingAssets()
    {
        string fileName = "Level01_AI_ML_MSQ_500_4opt_HindiPartial.json";
        string filePath = Path.Combine(Application.streamingAssetsPath, fileName);
        string json = "";

#if UNITY_ANDROID
        using (var request = UnityEngine.Networking.UnityWebRequest.Get(filePath))
        {
            yield return request.SendWebRequest();
            if (request.result != UnityEngine.Networking.UnityWebRequest.Result.Success)
            {
                Debug.LogError("Failed to load JSON on Android: " + request.error);
                yield break;
            }
            json = request.downloadHandler.text;
        }
#else
        if (File.Exists(filePath))
        {
            json = File.ReadAllText(filePath);
        }
        else
        {
            Debug.LogError("File not found at: " + filePath);
            yield break;
        }
#endif

        MCQQuestion[] loaded = JsonHelper.FromJson<MCQQuestion>(json);
        if (loaded == null || loaded.Length == 0)
        {
            Debug.LogError("No questions found or invalid JSON.");
            ShowComingSoon();
            yield break;
        }

        questions = new List<MCQQuestion>(loaded);
        ShuffleQuestions();
        DisplayQuestion();
    }

    void ShuffleQuestions()
    {
        for (int i = 0; i < questions.Count; i++)
        {
            int j = Random.Range(i, questions.Count);
            var tmp = questions[i];
            questions[i] = questions[j];
            questions[j] = tmp;
        }
    }

    void DisplayQuestion()
    {
        if (currentIndex >= questions.Count)
        {
            ShowComingSoon();
            return;
        }

        answered = false;
        playerChoices.Clear();
        optionButtons.Clear();

        scoreText.text = $"vad {score}";
        var q = questions[currentIndex];
        questionText.text = q.prompt;
        timerText.text = Mathf.CeilToInt(timePerQuestion).ToString();

        ClearOptions();

        foreach (var opt in q.options)
        {
            var btnGO = Instantiate(optionButtonPrefab, optionsContainer);
            var label = btnGO.GetComponentInChildren<TextMeshProUGUI>();
            var button = btnGO.GetComponent<Button>();

            label.text = opt;
            button.interactable = true;
            button.onClick.AddListener(() => OnOptionToggled(opt, button));
            optionButtons[opt] = button;
        }

        if (timerCoroutine != null) StopCoroutine(timerCoroutine);
        timerCoroutine = StartCoroutine(QuestionTimer());
    }

    IEnumerator QuestionTimer()
    {
        float remaining = timePerQuestion;
        while (remaining > 0f && !answered)
        {
            remaining -= Time.deltaTime;
            timerText.text = Mathf.CeilToInt(remaining).ToString();
            yield return null;
        }

        if (!answered)
        {
            ProcessAnswer(); // auto-submit
        }
    }

    void OnOptionToggled(string option, Button button)
    {
        if (answered) return;

        if (playerChoices.Contains(option))
        {
            playerChoices.Remove(option);
            button.image.color = Color.white;
        }
        else
        {
            playerChoices.Add(option);
            button.image.color = Color.black;
        }
    }

    void ProcessAnswer()
    {
        answered = true;

        foreach (var btn in optionButtons.Values)
            btn.interactable = false;

        var correctList = questions[currentIndex].correct;
        bool isCorrect = AreSelectionsEqual(playerChoices, correctList);
        if (isCorrect) score += scorePoints;

        scoreText.text = $"vad {score}";
        StartCoroutine(AutoAdvance());
    }

    bool AreSelectionsEqual(List<string> a, List<string> b)
    {
        if (a.Count != b.Count) return false;
        foreach (var val in a) if (!b.Contains(val)) return false;
        return true;
    }

    IEnumerator AutoAdvance()
    {
        yield return new WaitForSeconds(2f);
        currentIndex++;
        DisplayQuestion();
    }

    void ClearOptions()
    {
        foreach (Transform t in optionsContainer) Destroy(t.gameObject);
    }

    void ShowComingSoon()
    {
        comingSoonPanel.SetActive(true);
    }

    // Called from exit panel buttons
    public void OnExitYes()
    {
        Application.Quit();
        Debug.Log("Game closed.");
    }

    public void OnExitNo()
    {
        exitPanel.SetActive(false);
    }
}
