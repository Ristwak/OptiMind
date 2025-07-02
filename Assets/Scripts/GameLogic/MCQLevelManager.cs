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
    public TextMeshProUGUI feedbackText;
    public TextMeshProUGUI questionCounter;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timerText;
    public Button submitButton;

    [Header("Options Container")]
    public GameObject optionButtonPrefab;
    public Transform optionsContainer;

    [Header("Game Settings")]
    public float timePerQuestion = 10f;
    public int scorePoints = 1;

    private List<MCQQuestion> questions = new List<MCQQuestion>();
    private int currentIndex = 0;
    private List<string> playerChoices = new List<string>();
    private Dictionary<string, Button> optionButtons = new Dictionary<string, Button>();
    private Coroutine timerCoroutine;
    private bool answered;
    private int score;

    void Start()
    {
        submitButton.onClick.AddListener(OnSubmitAnswer);
        submitButton.interactable = false;
        score = 0;
        StartCoroutine(LoadQuestionsFromStreamingAssets());
    }

    IEnumerator LoadQuestionsFromStreamingAssets()
    {
        string fileName = "Level01_AI_ML_MSQ_500_4opt.json";
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
        if (currentIndex >= questions.Count) return;

        answered = false;
        playerChoices.Clear();
        optionButtons.Clear();
        submitButton.interactable = false;

        // Update score and counter
        scoreText.text = $"Score: {score}";
        var q = questions[currentIndex];
        questionText.text = q.prompt;
        feedbackText.text = string.Empty;
        questionCounter.text = $"Q {currentIndex + 1} / {questions.Count}";
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

        // Start question timer
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
        if (!answered) ProcessAnswer();
    }

    void OnOptionToggled(string option, Button button)
    {
        if (playerChoices.Contains(option))
        {
            playerChoices.Remove(option);
            button.image.color = Color.white;
        }
        else
        {
            playerChoices.Add(option);
            button.image.color = Color.green;
        }
        submitButton.interactable = playerChoices.Count > 0;
    }

    void OnSubmitAnswer()
    {
        if (timerCoroutine != null) StopCoroutine(timerCoroutine);
        ProcessAnswer();
    }

    void ProcessAnswer()
    {
        answered = true;
        // Disable further input
        foreach (var btn in optionButtons.Values)
            btn.interactable = false;
        submitButton.interactable = false;

        // Evaluate
        var correctList = questions[currentIndex].correct;
        bool isCorrect = AreSelectionsEqual(playerChoices, correctList);
        if (isCorrect) score += scorePoints;

        feedbackText.text = isCorrect ? "✅ Correct!" : "❌ Wrong.";
        scoreText.text = $"Score: {score}";

        // Advance after delay
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
        if (currentIndex >= questions.Count)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("HomeScene");
        }
        else
        {
            DisplayQuestion();
        }
    }

    void ClearOptions()
    {
        foreach (Transform t in optionsContainer) Destroy(t.gameObject);
    }
}
