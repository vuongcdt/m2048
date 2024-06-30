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

        private int _indexMyScore;
        private BoardManager _boardManager;
        private const string YOUR_NAME = "You";

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
            var myScore = (int)_boardManager.highScore;
            var rankData = JsonUtility.FromJson<Utils.RankData>(Prefs.RankData);

            // SetRankScoreByDay(rankData);
            
            if (rankData is null)
            {
                return;
            }

            var chartScores = rankData.chartScores;
            var myChartScore = new Utils.ChartScore(myScore, YOUR_NAME);
            chartScores.Add(myChartScore);

            chartScores.Sort((a, b) => (int)(b.score - a.score));

            SetIndexChartScores(chartScores, myScore);

            for (var i = 0; i < chartItems.Length; i++)
            {
                chartItems[i].RenderChartUI(chartScores[i], myScore);
            }
        }

        private void SetRankScoreByDay(Utils.RankData rankData)
        {
            var chartScores = rankData.chartScores;
            for (var i = 0; i < chartScores.Count; i++)
            {
                chartScores[i].score *= 100000 / chartScores[i].score > Random.value ? Random.value / 5 + 1 : 1;
            }

            var dataSave = new Utils.RankData(chartScores, DateTime.Now.Date.ToString("d"));
            Prefs.RankData = JsonUtility.ToJson(dataSave);
        }

        private void SetIndexChartScores(List<Utils.ChartScore> chartScores, int myScore)
        {
            for (var i = 0; i < chartScores.Count; i++)
            {
                chartScores[i].index = i + 1;
            }
        }

        private void OnCloseBtnClick()
        {
            ModalContainer.Of(transform).Pop(true);
        }
    }
}