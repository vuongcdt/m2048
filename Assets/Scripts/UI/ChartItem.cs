using TMPro;
using UnityEngine;

namespace UI
{
    public class ChartItem : MonoBehaviour
    {
        [SerializeField] private TMP_Text indexText;
        [SerializeField] private TMP_Text scoreText;
        
        public void SetIndex(int index)
        {
            indexText.text = index.ToString();
        }
        public void SetScore(int score)
        {
            scoreText.text = score.ToString();
        }

    }
}