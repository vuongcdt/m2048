using System;
using System.Collections.Generic;
using System.Linq;
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
        [SerializeField] private List<ChartItem> chartItems;

        private BoardManager _boardManager;
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
            var charts = GetCharts();

            for (var i = 0; i < 3; i++)
            {
                chartItems[i].SetScore(charts[i].score);
            }

            for (var i = 3; i < chartItems.Count; i++)
            {
                chartItems[i].SetIndex(charts[i].index);
                chartItems[i].SetScore(charts[i].score);
            }
        }

        private void OnCloseBtnClick()
        {
            ModalContainer.Of(transform).Pop(true);
        }

        private List<Utils.ChartScore> GetCharts()
        {
            List<Utils.ChartScore> charts = new();
            var myScore = (int)_boardManager.highScore;

            for (int i = 0; i < 7; i++)
            {
                var random = Random.Range(0, myScore * 3);
                charts.Add(new Utils.ChartScore(random));
            }
            charts.Add(new Utils.ChartScore(myScore));

            charts.Sort((a, b) => b.score - a.score);
            Debug.Log(string.Join(", ", charts.Select(e => e.score)));
            for (var i = 0; i < charts.Count; i++)
            {
                if (charts[i].score == myScore)
                {
                    _indexMyScore = i + 1;
                }

                charts[i].index = i + 1;
            }

            return charts;
        }
    }
}