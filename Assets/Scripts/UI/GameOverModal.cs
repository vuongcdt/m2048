using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class GameOverModal : MonoBehaviour
    {
        [SerializeField] private Button replayButton;

        private BoardManager _boardManager;

        public void Start()
        {
            Initialize();
        }

        public void Initialize()
        {
            Observer.On(Constants.EventKey.GAME_OVER_POPUP, e => ShowGameOverPopup());

            _boardManager = BoardManager.Instance;
            replayButton.onClick.RemoveAllListeners();
            replayButton.onClick.AddListener(OnReplayBtnClick);

            gameObject.SetActive(false);
        }

        private void OnReplayBtnClick()
        {
            gameObject.SetActive(false);

            if (Prefs.HighScore < _boardManager.highScore)
            {
                Prefs.HighScore = _boardManager.highScore;
            }
            
            Observer.Emit(Constants.EventKey.RESET_GAME);
        }


        public void ShowGameOverPopup()
        {
            gameObject.SetActive(true);
        }
    }
}