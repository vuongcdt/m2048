using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using ZBase.UnityScreenNavigator.Core.Modals;

namespace UI
{
    public class SettingsModal:Modal
    {
        [SerializeField] private Button closeButton;
        
        public override UniTask Initialize(Memory<object> args)
        {
            
            closeButton.onClick.RemoveAllListeners();
            closeButton.onClick.AddListener(OnCloseBtnClick);
            
            return UniTask.CompletedTask;
        }

        private void OnCloseBtnClick()
        {
            ModalContainer.Of(transform).Pop(true);
        }
    }
}