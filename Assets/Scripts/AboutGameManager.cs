using UnityEngine;
using TMPro;

/// <summary>
/// Populates the About panel with detailed game info for OptiMind.
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
            "OptiMind is an interactive decision-making simulator where you tackle complex questions alongside a simulated AI model. Both you and the AI make choices — but only one of you might be right.\n\n" +

            "<b>What’s the Game About?</b>\n" +
            "Each level presents you with challenging multi-option questions that reflect real-world logic, ambiguity, and abstract reasoning.\n" +
            "Your goal? Pick the best option. Meanwhile, the AI 'thinks' too — based on probability, not certainty.\n\n" +

            "<b>AI That Thinks (Sort of)</b>\n" +
            "The AI doesn’t always get it right. It calculates, weighs options, and selects what it 'believes' is most probable.\n" +
            "You’ll see how machine learning deals with uncertainty — and how it differs from human logic.\n\n" +

            "<b>Compare Minds</b>\n" +
            "• Your answer vs AI’s choice\n" +
            "• Observe when the AI guesses wrong — and when it gets it right\n" +
            "• Analyze patterns in your own reasoning vs an adaptive system\n\n" +

            "<b>What You'll Learn</b>\n" +
            "• Foundations of probabilistic AI reasoning\n" +
            "• How machines evaluate multiple options\n" +
            "• Decision-making under uncertainty\n" +
            "• The contrast between intuition and statistical choice\n\n" +

            "<b>Designed for AI/ML Learners</b>\n" +
            "OptiMind is ideal for AI/ML students and curious minds who want to:\n" +
            "• Understand how algorithms 'choose'\n" +
            "• Develop sharper logical intuition\n" +
            "• Recognize cognitive biases — human and machine\n\n" +

            "<b>Ready to Compete?</b>\n" +
            "Dive into a mind duel. Outthink the AI.\n" +
            "Decode its patterns. Sharpen yours.\n" +
            "Welcome to OptiMind.";
    }
}
