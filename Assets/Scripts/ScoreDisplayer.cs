using TMPro;
using UnityEngine;

public class ScoreDisplayer : MonoBehaviour
{
    private TextMeshProUGUI tmPro;

    private void Awake()
    {
        tmPro = GetComponent<TextMeshProUGUI>();
    }

    public void SetScore(int score)
    {
        tmPro.text = score.ToString();
    }
}
