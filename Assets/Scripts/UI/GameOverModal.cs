using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using ZBase.UnityScreenNavigator.Core.Modals;

namespace UI
{
    public class GameOverModal:Modal
    {
        [SerializeField] private Button replayButton;

        private UIManager _uiManager;
        
        public override UniTask Initialize(Memory<object> args)
        {
            _uiManager = UIManager.Instance;
            replayButton.onClick.RemoveAllListeners();
            replayButton.onClick.AddListener(OnReplayBtnClick);
            
            return UniTask.CompletedTask;
        }

        private void OnReplayBtnClick()
        {
            ModalContainer.Of(transform).Pop(true);
            _uiManager.ResetGame();
        }
    }
}