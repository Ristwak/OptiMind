using UnityEngine;
using TMPro;

/// <summary>
/// Populates the About panel with detailed game info for OptiMind (multi-correct logic game).
/// </summary>
public class AboutGameManager : MonoBehaviour
{
    [Header("UI Reference")]
    [Tooltip("Assign the TMP_Text component for the About section.")]
    public TMP_Text aboutText;

    void Start()
    {
        if (aboutText == null)
        {
            Debug.LogError("AboutGameManager: 'aboutText' reference is missing.");
            return;
        }

        aboutText.text =
            "<b>About OptiMind</b>\n" +
            "OptiMind is a multi-select reasoning challenge designed to sharpen your conceptual understanding of AI, logic, and machine learning.\n\n" +

            "<b>What’s the Game About?</b>\n" +
            "Each level presents complex, real-world inspired questions with more than one correct answer.\n" +
            "Your task: analyze, infer, and select all valid answers — not just the obvious one.\n\n" +

            "<b>Thinking Beyond A or B</b>\n" +
            "OptiMind rewards clarity of thought, pattern recognition, and the ability to hold multiple possibilities in mind.\n" +
            "It simulates the kind of multi-path decision-making required in real AI design and evaluation.\n\n" +

            "<b>What You'll Learn</b>\n" +
            "• How to handle ambiguous and abstract problems\n" +
            "• Where classical logic meets probabilistic reasoning\n" +
            "• The challenge of choosing all valid answers — not just the best one\n\n" +

            "<b>Designed for AI/ML Learners</b>\n" +
            "Built for students and enthusiasts in AI, ML, and cognitive science, OptiMind helps you:\n" +
            "• Strengthen logical foundations\n" +
            "• Apply reasoning across multiple options\n" +
            "• Spot false positives and eliminate distractors\n\n" +

            "<b>Ready to Think Differently?</b>\n" +
            "This isn’t guesswork — it’s insight under pressure.\n" +
            "Explore. Select. Reflect.\n" +
            "Welcome to OptiMind.";
    }
}
