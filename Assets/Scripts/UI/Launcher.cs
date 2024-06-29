using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using ZBase.UnityScreenNavigator.Core;
using ZBase.UnityScreenNavigator.Core.Screens;
using ZBase.UnityScreenNavigator.Core.Views;
using ZBase.UnityScreenNavigator.Core.Windows;

namespace UI
{
    public class Launcher : UnityScreenNavigatorLauncher
    {
        private static WindowContainerManager ContainerManager { get; set; }
        private BoardManager _boardManager;
        
        protected override void OnAwake()
        {
            base.OnAwake();
            ContainerManager = this;
        }

        
        protected override void OnPostCreateContainers()
        {
            base.OnPostCreateContainers();
            _boardManager = BoardManager.Instance;
            ShowLoadingPage().Forget();
        }

        private async UniTaskVoid ShowLoadingPage()
        {
            var options = new ViewOptions(ResourceKey.LoadingScreenPrefab(), false, loadAsync: false);
            await ContainerManager.Find<ScreenContainer>(ContainerKey.Screens).PushAsync(options);
            Invoke(nameof(ShowHomePage),0.1f);
        }
        private async UniTaskVoid ShowHomePage()
        {
            var options = new ViewOptions(ResourceKey.HomeScreenPrefab(), false, loadAsync: false);
            await ContainerManager.Find<ScreenContainer>(ContainerKey.Screens).PushAsync(options);
        }
        
        public void PlayingGame()
        {
            _boardManager.isPlaying = true;
        }
    }
}