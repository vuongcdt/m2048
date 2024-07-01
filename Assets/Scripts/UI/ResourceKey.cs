using ZBase.UnityScreenNavigator.Core.Views;

namespace UI
{
    public static class ResourceKey
    {
        private const string PrefabFormat = "prefab_{0}";
        private const string LoadingScreenPrefabName = "screen_loading";
        private const string HomeScreenPrefabName = "screen_home";
        private const string PlayScreenPrefabName = "screen_game_play";
        private const string SettingsModalPrefabName = "modal_settings";
        private const string PauseModalPrefabName = "modal_pause";
        private const string GameOverModalPrefabName = "modal_game_over";
        private const string RankingRewardsModalPrefabName = "modal_ranking_rewards";

        public static string LoadingScreenPrefab()
        {
            return string.Format(PrefabFormat, LoadingScreenPrefabName);
        }

        public static string HomeScreenPrefab()
        {
            return string.Format(PrefabFormat, HomeScreenPrefabName);
        }

        public static string PlayScreenPrefab()
        {
            return string.Format(PrefabFormat, PlayScreenPrefabName);
        }
        
        public static string SettingsModalPrefab()
        {
            return string.Format(PrefabFormat, SettingsModalPrefabName);
        }   
        
        public static string PauseModalPrefab()
        {
            return string.Format(PrefabFormat, PauseModalPrefabName);
        }

        public static string GameOverModalPrefab()
        {
            return string.Format(PrefabFormat, GameOverModalPrefabName);
        }

        public static string RankingRewardsPrefab()
        {
            return string.Format(PrefabFormat, RankingRewardsModalPrefabName);
        }
    }
}