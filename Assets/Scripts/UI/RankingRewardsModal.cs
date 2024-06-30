using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using ZBase.UnityScreenNavigator.Core.Modals;

namespace UI
{
    public class RankingRewardsModal : Modal
    {
        [SerializeField] private Button closeButton;
        [SerializeField] private ChartItem[] chartItems;

        public override UniTask Initialize(Memory<object> args)
        {
            closeButton.onClick.RemoveAllListeners();
            closeButton.onClick.AddListener(OnCloseBtnClick);

            RenderChartsUI();

            return UniTask.CompletedTask;
        }

        private void RenderChartsUI()
        {
            var rankData = JsonUtility.FromJson<Utils.RankData>(Prefs.RankData);
            if (rankData is null)
            {
                return;
            }
            Debug.Log(Prefs.RankData);
            var chartScores = rankData.chartScores;
            // if (chartScores is null)
            // {
            //     return;
            // }

            for (var i = 0; i < chartItems.Length; i++)
            {
                chartItems[i].RenderChartUI(chartScores[i], rankData.highScore);
            }
        }

        private void OnCloseBtnClick()
        {
            ModalContainer.Of(transform).Pop(true);
        }
    }
}