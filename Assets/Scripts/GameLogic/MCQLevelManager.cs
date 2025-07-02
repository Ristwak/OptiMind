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
    public string correct;
}

[System.Serializable]
public class MCQLevelData
{
    public string title;
    public string format;
    public List<MCQQuestion> questions;
}

public class MCQLevelManager : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI questionText;
    public TextMeshProUGUI aiAnswerText;
    public TextMeshProUGUI feedbackText;
    public TextMeshProUGUI questionCounter;

    [Header("Options Container")]
    public GameObject optionButtonPrefab;
    public Transform optionsContainer;

    [Header("AI Settings")]
    public float aiMinThinkTime = 1.0f;
    public float aiMaxThinkTime = 3.0f;
    [Range(0f, 1f)]
    public float aiCorrectProbability = 0.8f;

    [Header("Auto Progression")]
    public float autoAdvanceDelay = 3.0f;

    private List<MCQQuestion> questions = new List<MCQQuestion>();
    private int currentIndex = 0;
    private string playerChoice;

    void Start()
    {
        StartCoroutine(LoadQuestionsFromStreamingAssets());
    }

    IEnumerator LoadQuestionsFromStreamingAssets()
    {
        string fileName = "Level01_MCQ.json";
        string filePath = Path.Combine(Application.streamingAssetsPath, fileName);
        string json = "";

#if UNITY_ANDROID
        using (UnityEngine.Networking.UnityWebRequest request = UnityEngine.Networking.UnityWebRequest.Get(filePath))
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

        MCQLevelData data = JsonUtility.FromJson<MCQLevelData>(json);
        if (data == null || data.questions == null || data.questions.Count == 0)
        {
            Debug.LogError("Invalid or empty JSON data.");
            yield break;
        }

        questions = new List<MCQQuestion>(data.questions);

        // Shuffle questions
        for (int i = 0; i < questions.Count; i++)
        {
            int j = Random.Range(i, questions.Count);
            var tmp = questions[i];
            questions[i] = questions[j];
            questions[j] = tmp;
        }

        DisplayQuestion();
    }

    void DisplayQuestion()
    {
        if (currentIndex >= questions.Count) return;

        var q = questions[currentIndex];
        questionText.text = q.prompt;
        aiAnswerText.text = "";
        feedbackText.text = "";
        questionCounter.text = $"Q {currentIndex + 1} / {questions.Count}";

        ClearOptions();

        foreach (var option in q.options)
        {
            var btnGO = Instantiate(optionButtonPrefab, optionsContainer);
            var label = btnGO.GetComponentInChildren<TextMeshProUGUI>();
            var button = btnGO.GetComponent<Button>();

            label.text = option;
            button.interactable = true;

            string chosen = option;
            button.onClick.AddListener(() => OnOptionSelected(chosen));
        }
    }

    void ClearOptions()
    {
        foreach (Transform child in optionsContainer)
        {
            Destroy(child.gameObject);
        }
    }

    void OnOptionSelected(string chosen)
    {
        playerChoice = chosen;

        foreach (Transform child in optionsContainer)
        {
            var btn = child.GetComponent<Button>();
            if (btn != null) btn.interactable = false;
        }

        aiAnswerText.text = "🧠 AI is thinking...";
        StartCoroutine(AIResponseCoroutine());
    }

    IEnumerator AIResponseCoroutine()
    {
        float thinkTime = Random.Range(aiMinThinkTime, aiMaxThinkTime);
        yield return new WaitForSeconds(thinkTime);

        var q = questions[currentIndex];
        string aiChoice;

        if (Random.value <= aiCorrectProbability)
        {
            aiChoice = q.correct;
        }
        else
        {
            List<string> wrongs = new List<string>(q.options);
            wrongs.Remove(q.correct);

            aiChoice = wrongs.Count > 0 ? wrongs[Random.Range(0, wrongs.Count)] : q.correct;
        }

        aiAnswerText.text = $"🧠 AI says: {aiChoice}";

        bool playerCorrect = (playerChoice == q.correct);
        bool aiCorrect = (aiChoice == q.correct);

        feedbackText.text =
            $"{(playerCorrect ? "✅ You: Correct" : "❌ You: Wrong")}\n" +
            $"{(aiCorrect ? "🤖✅ AI: Correct" : "🤖❌ AI: Wrong")}";

        StartCoroutine(AutoAdvanceToNextQuestion());
    }

    IEnumerator AutoAdvanceToNextQuestion()
    {
        yield return new WaitForSeconds(autoAdvanceDelay);
        OnNextQuestion();
    }

    public void OnNextQuestion()
    {
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
}