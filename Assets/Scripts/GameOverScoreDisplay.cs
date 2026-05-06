using TMPro;
using UnityEngine;

public class GameOverScoreDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;

    private void Start()
    {
        if (scoreText != null)
            scoreText.text = $"Score: {ScoreManager.TotalScore}";
    }
}
