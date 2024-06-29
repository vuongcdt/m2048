using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using ZBase.UnityScreenNavigator.Core.Modals;
using Random = UnityEngine.Random;

namespace UI
{
    public class RankingRewardsModal : Modal
    {
        [SerializeField] private Button closeButton;
        [SerializeField] private ChartItem[] chartItems;
        [SerializeField] private TextAsset dataName;
        [SerializeField] private int maxData;

        private BoardManager _boardManager;
        private List<ChartItem> _chartItems;
        private int _indexMyScore;

        public override UniTask Initialize(Memory<object> args)
        {
            _boardManager = BoardManager.Instance;

            closeButton.onClick.RemoveAllListeners();
            closeButton.onClick.AddListener(OnCloseBtnClick);

            RenderChartsUI();

            return UniTask.CompletedTask;
        }

        private void RenderChartsUI()
        {
            var nameList = JsonUtility.FromJson<Utils.JsonHelper<string>>(dataName.text).data;

            List<Utils.ChartScore> chartScores = new();
            var myScore = (int)_boardManager.highScore;

            for (var i = 0; i < maxData; i++)
            {
                var random = Random.Range(0, myScore * 3);
                var randomName = Random.Range(0, nameList.Count);
                chartScores.Add(new Utils.ChartScore(random, nameList[randomName]));
            }

            var myChartScore = new Utils.ChartScore(myScore, "You");
            chartScores.Add(myChartScore);

            chartScores.Sort((a, b) => b.score - a.score);

            ChartItem endChartItem = null;

            for (var i = 0; i < chartScores.Count; i++)
            {
                if (chartScores[i].score == myScore)
                {
                    _indexMyScore = i + 1;
                }

                chartScores[i].index = i + 1;

                if (i >= 20)
                {
                    continue;
                }

                chartItems[i].RenderChartUI(chartScores[i]);
                endChartItem = chartItems[i];
            }

            if (_indexMyScore > 20)
            {
                endChartItem.RenderChartUI(myChartScore);
            }
        }

        private void OnCloseBtnClick()
        {
            ModalContainer.Of(transform).Pop(true);
        }
    }
}