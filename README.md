# 🧠 OptiMind – A Multi-Select Logic Challenge Game

**OptiMind** is an educational game designed for postgraduate students and AI/ML enthusiasts. It challenges players to make decisions under pressure by selecting **multiple correct options** from complex questions within a time limit — all while sharpening their logical reasoning and conceptual understanding of AI topics.

## 🎯 Objective

Each round presents a multiple-select question with 4 options. Some or all may be correct.  
Players must choose all valid answers **before the timer runs out**. Points are awarded only for completely correct selections — partial credit is not given.

## 🧩 Features

- ✅ Multi-correct answer support (MSQ format)
- ⏱️ Question timer with auto-submission
- 🧮 Score tracking across rounds
- 🔀 Questions are shuffled randomly from a JSON file
- 📂 Questions are loaded dynamically from the **StreamingAssets** folder
- 🎬 When no questions remain, a **Coming Soon** screen is shown
- ⏹️ Exit panel accessible anytime via the Android back button (`Escape`)

## 📂 Project Structure

Assets/
│
├── Scripts/
│ ├── MCQLevelManager.cs # Core game logic
│ └── JsonHelper.cs # Utility for parsing array-based JSON
│
├── UI/
│ ├── OptionButtonPrefab.prefab # Toggle-able option buttons
│ ├── Panels/
│ │ ├── GamePanel
│ │ ├── ExitPanel
│ │ └── ComingSoonPanel
│
├── StreamingAssets/
│ └── Level01_AI_ML_MSQ_500_4opt_HindiPartial.json


## 🧠 Educational Value

**OptiMind** trains:
- Critical thinking
- AI/ML domain knowledge
- Decision-making under time constraints
- Recognition of multi-factor solutions

Ideal for:
- AI/ML postgraduates
- Logic puzzle enthusiasts
- Competitive quiz formats

## 🛠️ How to Add New Questions

1. Create a new `.json` file inside `StreamingAssets/`
2. Use the following format:

```json
[
  {
    "id": "Q1",
    "prompt": "Which of the following are regularization techniques in ML?",
    "options": ["L1 Regularization", "L2 Regularization", "Dropout", "AdaBoost"],
    "correct": ["L1 Regularization", "L2 Regularization", "Dropout"]
  },
  ...
]
