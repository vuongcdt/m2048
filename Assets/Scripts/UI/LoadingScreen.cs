using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class LoadingScreen : MonoBehaviour
    {
        [SerializeField] private Image loadingMask;

        public void Start()
        {
            Initialize();
        }

        public void Initialize()
        {
            DOVirtual
            .Float(0, 1, 1, value => SetLoading(value))
            .OnComplete(() =>
            {
                gameObject.SetActive(false);
                Observer.Emit(Constants.EventKey.HOME_SCREEN);
            });
        }

        public void SetLoading(float value)
        {
            loadingMask.fillAmount = value;
        }
    }
}