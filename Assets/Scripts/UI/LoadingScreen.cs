using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Screen = ZBase.UnityScreenNavigator.Core.Screens.Screen;

namespace UI
{
    public class LoadingScreen : Screen
    {
        [SerializeField] private Image loadingMask;

        public override UniTask Initialize(Memory<object> args)
        {
            return UniTask.CompletedTask;
        }

        public void SetLoading(float value)
        {
            loadingMask.fillAmount = value;
        }
    }
}