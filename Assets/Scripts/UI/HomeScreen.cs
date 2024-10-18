using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class HomeScreen : MonoBehaviour
    {
        [SerializeField] private Button playButton;
        [SerializeField] private Button rankingRewardsButton;

        private BoardManager _boardManager;

        public void Start()
        {
            Initialize();
        }

        public void Initialize()
        {
            Observer.On(Constants.EventKey.HOME_SCREEN, e => ShowHomeScreen());
            _boardManager = BoardManager.Instance;

            playButton.onClick.RemoveAllListeners();
            playButton.onClick.AddListener(OnClickPlay);

            rankingRewardsButton.onClick.RemoveAllListeners();
            rankingRewardsButton.onClick.AddListener(OnRankingRewardsBtnClick);

            gameObject.SetActive(false);
        }

        private void OnRankingRewardsBtnClick()
        {
            Observer.Emit(Constants.EventKey.RANKING_POPUP);
        }

        private void OnClickPlay()
        {
            _boardManager.isPlaying = true;
            gameObject.SetActive(false);
            Observer.Emit(Constants.EventKey.GAME_PLAY_SCREEN);
        }

        private void ShowHomeScreen()
        {
            gameObject.SetActive(true);
        }
    }
}