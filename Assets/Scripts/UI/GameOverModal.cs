using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class GameOverModal : MonoBehaviour
    {
        [SerializeField] private Button replayButton;

        private UIManager _uiManager;
        private BoardManager _boardManager;

        public void Start()
        {
            Initialize();
        }

        public void Initialize()
        {
            Observer.On(Constants.EventKey.GAME_OVER_POPUP, e => ShowGameOverPopup());

            _uiManager = UIManager.Instance;
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
            _uiManager.ResetGame();
        }


        public void ShowGameOverPopup()
        {
            gameObject.SetActive(true);
        }
    }
}