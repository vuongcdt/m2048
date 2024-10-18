using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ZBase.UnityScreenNavigator.Core.Modals;

namespace UI
{
    public class GamePlayScreen : MonoBehaviour
    {
        [SerializeField] internal TMP_Text scoreText;
        [SerializeField] internal TMP_Text highScoreText;
        [SerializeField] internal TMP_Text nextSquareText;
        [SerializeField] internal Image background;
        [SerializeField] private GameObject comboPrefab;
        [SerializeField] private Text comboText;
        [SerializeField] private Button pauseBtn;
        [SerializeField] private GridLayoutGroup gridLayoutGroup;
        [SerializeField] private TMP_Text width;
        [SerializeField] private TMP_Text height;

        private Camera _cameraMain;
        private UIManager _uiManager;
        private BoardManager _boardManager;

        public void Start()
        {
            Initialize();
        }

        public void Initialize()
        {
            Observer.On(Constants.EventKey.GAME_PLAY_SCREEN, e => ShowGamePlayScreen());
            Observer.On(Constants.EventKey.COMBO, e => ShowCombo());
            _cameraMain = Camera.main;
            _uiManager = UIManager.Instance;
            _boardManager = BoardManager.Instance;

            // SetLayout();

            pauseBtn.onClick.RemoveAllListeners();
            pauseBtn.onClick.AddListener(OnPauseBtnClick);

            comboPrefab.SetActive(false);

            _uiManager.SetScoreUI(this);
            _boardManager.SetNextSquareValue(this);

            gameObject.SetActive(false);
        }

        private void OnPauseBtnClick()
        {
            _boardManager.isPlaying = false;
            // var options = new ModalOptions(ResourceKey.PauseModalPrefab());
            // ModalContainer.Find(ContainerKey.Modals).Push(options);
            Observer.Emit(Constants.EventKey.PAUSE_POPUP);
        }

        public void ShowGameOverPopup()
        {
            // var options = new ModalOptions(ResourceKey.GameOverModalPrefab());
            // ModalContainer.Find(ContainerKey.Modals).Push(options);
            Observer.Emit(Constants.EventKey.GAME_OVER_POPUP);
        }

        public void ShowCombo()
        {
            comboText.text = string.Format(Constants.FomatText.COMBO_TEXT_FORMAT, _uiManager.comboCount);
            var targetWorldPos = new Vector2(_uiManager.comboPos.x, _uiManager.comboPos.y - 1.3f);
            comboPrefab.transform.position = _cameraMain.WorldToScreenPoint(targetWorldPos);
            comboPrefab.SetActive(true);
            StartCoroutine(DeActiveComboIE());
        }

        private IEnumerator DeActiveComboIE()
        {
            yield return new WaitForSeconds(1);
            comboPrefab.SetActive(false);
        }

        public void SetNextSquare()
        {
            nextSquareText.text = Utils.GetText(_boardManager.nextSquareValue);
            if (background != null)
            {
                background.color = Utils.GetColor(_boardManager.nextSquareValue);
            }
        }

        public void SetScore()
        {
            scoreText.text = _boardManager.score.ToString(Constants.FomatText.FORMAT_SCORE);
            highScoreText.text = _boardManager.highScore.ToString(Constants.FomatText.FORMAT_SCORE);
        }

        public void ShowGamePlayScreen()
        {
            gameObject.SetActive(true);
        }
    }
}