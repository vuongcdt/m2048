using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class ChartItem : MonoBehaviour
    {
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text scoreText;
        [SerializeField] private GameObject[] iconList;
        [SerializeField] private Image background;

        private Color _myColor = new (0.8396f, 0.0831f, 0.0831f, 1);
        
        private Color[] _colors = new Color[]
        {
            new(0.749f, 0.447f, 0.1725f, 1),
            new(0.398f, 0.3411f, 0.6313f, 1),
            new(0.5215f, 0.2313f, 0.2196f, 1),
            new(0.0117f, 0.247f, 0.6431f, 1)
        };

        public void RenderChartUI(Utils.ChartScore chartScore,float highScore)
        {
            scoreText.text = chartScore.score.ToString(Constants.FomatText.FORMAT_SCORE);
            nameText.text = chartScore.fullName;

            if (chartScore.index < 4)
            {
                background.color = _colors[chartScore.index - 1];
                iconList[chartScore.index - 1].SetActive(true);
            }
            else
            {
                background.color = _colors[^1];
                iconList[^1].SetActive(true);
                iconList[^1].GetComponentInChildren<TMP_Text>().text = chartScore.index.ToString();
                iconList[^1].transform.position = new Vector3(iconList[^1].transform.position.x + 2,
                    iconList[^1].transform.position.y);
            }

            
            
            if (Mathf.Approximately(chartScore.score ,highScore))
            {
                background.color = _myColor;
            }
        }
    }
}