using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using ZBase.UnityScreenNavigator.Core.Modals;
using ZBase.UnityScreenNavigator.Core.Screens;
using Screen = ZBase.UnityScreenNavigator.Core.Screens.Screen;

namespace UI
{
    public class HomeScreen : Screen
    {
        [SerializeField] private Button playButton;
        [SerializeField] private Button settingButton;
 
        private BoardManager _boardManager;
        public override UniTask Initialize(Memory<object> args)
        {
            _boardManager = BoardManager.Instance;
            playButton.onClick.RemoveAllListeners();
            playButton.onClick.AddListener(OnClickPlay);
            settingButton.onClick.RemoveAllListeners();
            settingButton.onClick.AddListener(OnSettingsBtnClick);
            
            return UniTask.CompletedTask;
        }

        private void OnClickPlay()
        {
            _boardManager.isPlaying = true;
            ScreenContainer.Of(transform).Push(new ScreenOptions(ResourceKey.PlayScreenPrefab(),false));
        }    

        private void OnSettingsBtnClick()
        {
            var options = new ModalOptions(ResourceKey.SettingsModalPrefab());
            ModalContainer.Find(ContainerKey.Modals).Push(options);
        }
    }
}