using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
        private BoardManager _boardManager;

        public void Start()
        {
            Initialize();
        }

        public void Initialize()
        {
            Observer.On(Constants.EventKey.GAME_PLAY_SCREEN, e => ShowGamePlayScreen());
            Observer.On(Constants.EventKey.COMBO, e => ShowCombo(e));
            Observer.On(Constants.EventKey.SCORE, e => SetScore(e));
            Observer.On(Constants.EventKey.NEXT_SQUARE, e => SetNextSquare(e));

            _cameraMain = Camera.main;
            _boardManager = BoardManager.Instance;

            pauseBtn.onClick.RemoveAllListeners();
            pauseBtn.onClick.AddListener(OnPauseBtnClick);

            comboPrefab.SetActive(false);

            gameObject.SetActive(false);
        }

        private void OnPauseBtnClick()
        {
            _boardManager.isPlaying = false;
            Observer.Emit(Constants.EventKey.PAUSE_POPUP);
        }

        public void ShowCombo(object data)
        {
            var comboData = (ComboEvent)data;
            comboText.text = string.Format(Constants.FomatText.COMBO_TEXT_FORMAT, comboData.count);
            var targetWorldPos = new Vector2(comboData.pos.x, comboData.pos.y - 1.3f);
            comboPrefab.transform.position = _cameraMain.WorldToScreenPoint(targetWorldPos);
            comboPrefab.SetActive(true);
            StartCoroutine(DeActiveComboIE());
        }

        private IEnumerator DeActiveComboIE()
        {
            yield return new WaitForSeconds(1);
            comboPrefab.SetActive(false);
        }

        public void SetNextSquare(object data)
        {
            var nextSquare = (float)data;
            nextSquareText.text = Utils.GetText(nextSquare);
            if (background != null)
            {
                background.color = Utils.GetColor(nextSquare);
            }
        }

        public void SetScore(object data)
        {
            var scoreData = (ScoreDataEvent)data;
            scoreText.text = scoreData.score.ToString(Constants.FomatText.FORMAT_SCORE);
            highScoreText.text = scoreData.highScore.ToString(Constants.FomatText.FORMAT_SCORE);
        }

        public void ShowGamePlayScreen()
        {
            gameObject.SetActive(true);
            Observer.Emit(Constants.EventKey.SCORE, new ScoreDataEvent(_boardManager.score, _boardManager.highScore));
            Observer.Emit(Constants.EventKey.NEXT_SQUARE, _boardManager.nextSquareValue);
        }
    }
}