using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class PauseModal : MonoBehaviour
    {
        [SerializeField] private Button continueButton;
        [SerializeField] private Button homeButton;
        [SerializeField] private Button newGameButton;
        [SerializeField] private Slider sliderMusic;
        [SerializeField] private Slider sliderSfx;

        private BoardManager _boardManager;

        public void Start()
        {
            Initialize();
        }

        public void Initialize()
        {
            Observer.On(Constants.EventKey.PAUSE_POPUP, e => ShowPausePopup());
            _boardManager = BoardManager.Instance;

            continueButton.onClick.RemoveAllListeners();
            continueButton.onClick.AddListener(OnContinueBtnClick);

            newGameButton.onClick.RemoveAllListeners();
            newGameButton.onClick.AddListener(OnNewGameBtnClick);

            homeButton.onClick.RemoveAllListeners();
            homeButton.onClick.AddListener(OnHomeBtnClick);

            SetVolumeUI();
            gameObject.SetActive(false);
        }

        private void ShowPausePopup()
        {
            gameObject.SetActive(true);
        }

        private void OnNewGameBtnClick()
        {
            Prefs.HighScore = _boardManager.highScore;
            _boardManager.isPlaying = true;

            gameObject.SetActive(false);
            Observer.Emit(Constants.EventKey.SAVE_VOLUMN, new SaveVolumeEvent(sliderMusic.value, sliderSfx.value));
            Observer.Emit(Constants.EventKey.RESET_GAME);
        }

        private void OnHomeBtnClick()
        {
            _boardManager.isPlaying = false;

            Observer.Emit(Constants.EventKey.SAVE_VOLUMN, new SaveVolumeEvent(sliderMusic.value, sliderSfx.value));
            gameObject.SetActive(false);
            Observer.Emit(Constants.EventKey.HOME_SCREEN);
        }

        private void OnContinueBtnClick()
        {
            _boardManager.isPlaying = true;

            gameObject.SetActive(false);
            Observer.Emit(Constants.EventKey.SAVE_VOLUMN, new SaveVolumeEvent(sliderMusic.value, sliderSfx.value));
        }

        private void SetVolumeUI()
        {
            if (Mathf.Approximately(Prefs.VolumeMusic, -1) || Mathf.Approximately(Prefs.VolumeSfx, -1))
            {
                sliderMusic.value = Constants.Volume.VOLUME_DEFAULT;
                sliderSfx.value = Constants.Volume.VOLUME_DEFAULT;
            }
            else
            {
                sliderMusic.value = Prefs.VolumeMusic;
                sliderSfx.value = Prefs.VolumeSfx;
            }
        }

        public void OnChangeVolumeMusic()
        {
            Observer.Emit(Constants.EventKey.SET_VOLUMN_MUSIC, sliderMusic.value);
        }

        public void OnChangeVolumeSFX()
        {
            Observer.Emit(Constants.EventKey.SET_VOLUMN_SOUND_SHOOT, sliderSfx.value);
        }
    }
}