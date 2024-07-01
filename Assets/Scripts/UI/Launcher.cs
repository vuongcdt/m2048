using System.Collections;
using Cysharp.Threading.Tasks;
using ZBase.UnityScreenNavigator.Core;
using ZBase.UnityScreenNavigator.Core.Modals;
using ZBase.UnityScreenNavigator.Core.Screens;
using ZBase.UnityScreenNavigator.Core.Views;
using ZBase.UnityScreenNavigator.Core.Windows;

namespace UI
{
    public class Launcher : UnityScreenNavigatorLauncher
    {
        private static WindowContainerManager ContainerManager { get; set; }

        private ScreenContainer screenContainer;
        private ModalContainer modalContainer;

        protected override void OnAwake()
        {
            base.OnAwake();
            ContainerManager = this;
        }

        protected override void OnPostCreateContainers()
        {
            base.OnPostCreateContainers();
            screenContainer = ContainerManager.Find<ScreenContainer>(ContainerKey.Screens);
            modalContainer = ModalContainer.Find(ContainerKey.Modals);

            PreloadingScreen();
            ShowLoadingPage().Forget();
        }

        private void PreloadingScreen()
        {
            screenContainer.Preload(ResourceKey.LoadingScreenPrefab());
            screenContainer.Preload(ResourceKey.HomeScreenPrefab());
            screenContainer.Preload(ResourceKey.PlayScreenPrefab());
            
            modalContainer.Preload(ResourceKey.RankingRewardsPrefab());
            modalContainer.Preload(ResourceKey.PauseModalPrefab());
            modalContainer.Preload(ResourceKey.GameOverModalPrefab());

        }

        private async UniTaskVoid ShowLoadingPage()
        {
            var options = new ViewOptions(ResourceKey.LoadingScreenPrefab(), false, loadAsync: false);
            await screenContainer.PushAsync(options);
            Invoke(nameof(ShowHomePage), 1);
        }

        private async UniTaskVoid ShowHomePage()
        {
            var options = new ViewOptions(ResourceKey.HomeScreenPrefab(), false, loadAsync: false);
            await screenContainer.PushAsync(options);
        }
    }
}