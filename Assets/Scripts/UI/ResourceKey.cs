﻿using ZBase.UnityScreenNavigator.Core.Views;

namespace UI
{
    public static class ResourceKey
    {
        private const string PrefabFormat = "prefab_{0}";
        private const string LoadingScreenPrefabName = "screen_loading";
        private const string HomeScreenPrefabName = "screen_home";
        private const string PlayScreenPrefabName = "screen_game_play";
        private const string ShopScreenPrefabName = "screen_shop";
        private const string SettingsModalPrefabName = "modal_settings";
        private const string CharacterModalPrefabName = "modal_character";
        private const string ShopItemGridSheetPrefabName = "sheet_shop_item_grid";
        private const string CharacterModalImageSheetPrefabName = "sheet_character_modal_image";
        private const string CharacterImageModalPrefabName = "modal_character_image";
        private const string LoadingActivityPrefabName = "activity_loading";
        private const string ShopItemPrefabName = "control_shop_item";

        private const string CharacterImageFormat = "tex_character_{0:D3}_{1}";
        private const string CharacterThumbnailFormat = "tex_character_thumb_{0:D3}_{1}";

        public static string LoadingScreenPrefab()
        {
            return string.Format(PrefabFormat, LoadingScreenPrefabName);
        }

        public static string HomeScreenPrefab()
        {
            return string.Format(PrefabFormat, HomeScreenPrefabName);
        }

        public static ViewOptions PlayScreenPrefab()
        {
            return string.Format(PrefabFormat, PlayScreenPrefabName);
        }
        
        public static string ShopScreenPrefab()
        {
            return string.Format(PrefabFormat, ShopScreenPrefabName);
        }

        public static string SettingsModalPrefab()
        {
            return string.Format(PrefabFormat, SettingsModalPrefabName);
        }

        public static string CharacterModalPrefab()
        {
            return string.Format(PrefabFormat, CharacterModalPrefabName);
        }

        public static string ShopItemGridSheetPrefab()
        {
            return string.Format(PrefabFormat, ShopItemGridSheetPrefabName);
        }

        public static string CharacterModalImageSheetPrefab()
        {
            return string.Format(PrefabFormat, CharacterModalImageSheetPrefabName);
        }

        public static string CharacterImageModalPrefab()
        {
            return string.Format(PrefabFormat, CharacterImageModalPrefabName);
        }

        public static string CharacterSprite(int characterId, int rank)
        {
            return string.Format(CharacterImageFormat, characterId, rank);
        }

        public static string CharacterThumbnailSprite(int characterId, int rank)
        {
            return string.Format(CharacterThumbnailFormat, characterId, rank);
        }

        public static string LoadingActivity()
        {
            return string.Format(PrefabFormat, LoadingActivityPrefabName);
        }

        public static string ShopItemControlPrefab()
        {
            return string.Format(PrefabFormat, ShopItemPrefabName);
        }

    }
}