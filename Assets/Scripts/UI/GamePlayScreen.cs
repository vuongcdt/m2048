using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ZBase.UnityScreenNavigator.Core.Modals;
using ZBase.UnityScreenNavigator.Core.Screens;
using Screen = ZBase.UnityScreenNavigator.Core.Screens.Screen;

namespace UI
{
    public class GamePlayScreen : Screen
    {
        [SerializeField] internal TMP_Text scoreText;
        [SerializeField] internal TMP_Text highScoreText;
        [SerializeField] internal TMP_Text nextSquareText;
        [SerializeField] internal Image background;
        [SerializeField] private GameObject comboPrefab;
        [SerializeField] private Text comboText;
        [SerializeField] private GameObject gameOverPopup;
        [SerializeField] private Button backToHomeBtn;

        private Camera _cameraMain;
        private UIManager _uiManager;
        private BoardManager _boardManager;
        private const string FORMAT_SCORE = "0";

        private const string COMBO_TEXT_FORMAT = "Combo x{0}";
        // private void OnPostRender()
        // {
        //     _uiManager = UIManager.Instance;
        //     _boardManager = BoardManager.Instance;
        //     _uiManager.SetScoreUI();
        //     SetNextSquareValue();
        // }

        public override UniTask Initialize(Memory<object> args)
        {
            _uiManager = UIManager.Instance;
            backToHomeBtn.onClick.RemoveAllListeners();
            backToHomeBtn.onClick.AddListener(OnBackToHomeBtnClick);
            
            base.OnEnable();
            _cameraMain = Camera.main;
            _uiManager = UIManager.Instance;
            _boardManager = BoardManager.Instance;
            
            comboPrefab.SetActive(false);
            SetActiveGameOverPopup(false);
            _uiManager.SetScoreUI(this);
            _boardManager.SetNextSquareValue(this);
            return UniTask.CompletedTask;
        }

        private void OnBackToHomeBtnClick()
        {
            _boardManager.isPlaying = false;
            ScreenContainer.Of(transform).Pop(true);
        }

        // protected override void OnEnable()
        // {
        //     base.OnEnable();
        //     _cameraMain = Camera.main;
        //     _uiManager = UIManager.Instance;
        //     _boardManager = BoardManager.Instance;
        //     
        //     comboPrefab.SetActive(false);
        //     SetActiveGameOverPopup(false);
        //     _uiManager.SetScoreUI(this);
        //     _boardManager.SetNextSquareValue(this);
        // }

        public void SetActiveGameOverPopup(bool isActive)
        {
            gameOverPopup.SetActive(isActive);
        }
        
        public void ShowGameOverPopup()
        {
            var options = new ModalOptions(ResourceKey.GameOverModalPrefab());
            ModalContainer.Find(ContainerKey.Modals).Push(options);
        }

        public void ShowCombo()
        {
            comboText.text = string.Format(COMBO_TEXT_FORMAT, _uiManager.comboCount);
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
            background.color = Utils.GetColor(_boardManager.nextSquareValue);
            nextSquareText.text = Utils.GetText(_boardManager.nextSquareValue);
        }

        public void SetScore()
        {
            scoreText.text = _boardManager.score.ToString(FORMAT_SCORE);
            highScoreText.text = _boardManager.highScore.ToString(FORMAT_SCORE);
        }
    }
}