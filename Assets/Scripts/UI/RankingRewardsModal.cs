using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Unity.Profiling;
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

        private BoardManager _boardManager;
        private const string YOUR_NAME = "You";

        private static readonly ProfilerMarker PauseModelMaker = new("MyMaker.PauseModel");

        public override UniTask Initialize(Memory<object> args)
        {
            PauseModelMaker.Begin();
            
            _boardManager = BoardManager.Instance;
            closeButton.onClick.RemoveAllListeners();
            closeButton.onClick.AddListener(OnCloseBtnClick);
            RenderChartsUI();
            
            PauseModelMaker.End();

            return UniTask.CompletedTask;
        }

        private void RenderChartsUI()
        {
            var myScore = (int)_boardManager.highScore;
            var rankData = JsonUtility.FromJson<Utils.RankData>(Prefs.RankData);

            if (rankData is null)
            {
                return;
            }

            if (!DateTime.Now.Date.ToString(Constants.FomatText.SHORT_DATE_FORMAT).Equals(rankData.dateTimeString))
            {
                SetRankScoreByDay(rankData);
            }
            
            var chartScores = rankData.chartScores;
            var myChartScore = new Utils.ChartScore(myScore, YOUR_NAME);
            chartScores.Add(myChartScore);

            chartScores.Sort((a, b) => (int)(b.score - a.score));

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
                chartScores[i].score *= 100000 / chartScores[i].score > Random.value ? Random.value + 1 : 1;
            }

            var dataSave = new Utils.RankData(chartScores, DateTime.Now.Date.ToString(Constants.FomatText.SHORT_DATE_FORMAT));
            Prefs.RankData = JsonUtility.ToJson(dataSave);
        }

        private void OnCloseBtnClick()
        {
            ModalContainer.Of(transform).Pop(true);
        }
    }
}