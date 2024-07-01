using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using ZBase.UnityScreenNavigator.Core.Modals;
using ZBase.UnityScreenNavigator.Core.Screens;
using Screen = ZBase.UnityScreenNavigator.Core.Screens.Screen;

namespace UI
{
    public class HomeScreen : Screen
    {
        [SerializeField] private Button playButton;
        [SerializeField] private Button rankingRewardsButton;

        private BoardManager _boardManager;
        private UIManager _uiManager;

        public override UniTask Initialize(Memory<object> args)
        {
            _boardManager = BoardManager.Instance;
            _uiManager = UIManager.Instance;

            playButton.onClick.RemoveAllListeners();
            playButton.onClick.AddListener(OnClickPlay);

            rankingRewardsButton.onClick.RemoveAllListeners();
            rankingRewardsButton.onClick.AddListener(OnRankingRewardsBtnClick);

            return UniTask.CompletedTask;
        }

        private void OnRankingRewardsBtnClick()
        {
            var options = new ModalOptions(ResourceKey.RankingRewardsPrefab());
            ModalContainer.Find(ContainerKey.Modals).Push(options);
        }

        private void OnClickPlay()
        {
            _boardManager.isPlaying = true;
            ScreenContainer.Of(transform).Push(new ScreenOptions(ResourceKey.PlayScreenPrefab()));
        }
    }
}