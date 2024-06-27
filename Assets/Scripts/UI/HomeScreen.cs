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
        [SerializeField] private Button newGameButton;
 
        private BoardManager _boardManager;
        private UIManager _uiManager;
        public override UniTask Initialize(Memory<object> args)
        {
            _boardManager = BoardManager.Instance;
            _uiManager = UIManager.Instance;
            
            playButton.onClick.RemoveAllListeners();
            playButton.onClick.AddListener(OnClickPlay);
            settingButton.onClick.RemoveAllListeners();
            settingButton.onClick.AddListener(OnSettingsBtnClick);
            newGameButton.onClick.RemoveAllListeners();
            newGameButton.onClick.AddListener(OnNewGameBtnClick);
            
            return UniTask.CompletedTask;
        }

        private void OnNewGameBtnClick()
        {
            _boardManager.isPlaying = true;
            _boardManager.isClearData = true;
            _uiManager.ResetGame();
            
            ScreenContainer.Of(transform).Push(new ScreenOptions(ResourceKey.PlayScreenPrefab(),false));
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