using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using ZBase.UnityScreenNavigator.Core.Screens;
using Screen = ZBase.UnityScreenNavigator.Core.Screens.Screen;

namespace UI
{
    public class HomeScreen : Screen
    {
        [SerializeField] private Button playButton;

        private BoardManager _boardManager;
        public override UniTask Initialize(Memory<object> args)
        {
            _boardManager = BoardManager.Instance;
            playButton.onClick.RemoveAllListeners();
            playButton.onClick.AddListener(OnClickPlay);
            
            return UniTask.CompletedTask;
        }
        
        private void OnClickPlay()
        {
            _boardManager.isPlaying = true;
            ScreenContainer.Of(transform).Push(new ScreenOptions(ResourceKey.PlayScreenPrefab(),false));
        }
    }
}