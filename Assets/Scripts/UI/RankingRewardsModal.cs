using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using ZBase.UnityScreenNavigator.Core.Modals;
using ZBase.UnityScreenNavigator.Core.Screens;

namespace UI
{
    public class RankingRewardsModal:Modal
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