using DeconstructorMod.Customs;
using Kitchen;
using KitchenData;
using KitchenLib;
using KitchenLib.Customs;
using KitchenLib.Event;
using KitchenLib.References;
using KitchenLib.Utils;
using KitchenMods;
using PreferenceSystem;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.VFX;

// Namespace should have "Kitchen" in the beginning
namespace KitchenDeconstructor
{
    public class Mod : BaseMod, IModSystem
    {
        // GUID must be unique and is recommended to be in reverse domain name notation
        // Mod Name is displayed to the player and listed in the mods menu
        // Mod Version must follow semver notation e.g. "1.2.3"
        public const string MOD_GUID = "Madvion.PlateUp.DeconstructorMod";
        public const string MOD_NAME = "DeconstructorMod";
        public const string MOD_VERSION = "0.1.5";
        public const string MOD_AUTHOR = "Madvion";
        public const string MOD_GAMEVERSION = ">=1.1.4";
        // Game version this mod is designed for in semver
        // e.g. ">=1.1.3" current and all future
        // e.g. ">=1.1.3 <=1.2.3" for all from/until

        // Boolean constant whose value depends on whether you built with DEBUG or RELEASE mode, useful for testing
#if DEBUG
        public const bool DEBUG_MODE = true;
#else
        public const bool DEBUG_MODE = false;
#endif
        internal static Appliance BlueprintCabinet => GetExistingGDO<Appliance>(ApplianceReferences.BlueprintCabinet);
        public static PreferenceSystemManager PrefManager;
        internal static Appliance Deconstructor => GetModdedGDO<Appliance, Deconstructor>();
        internal static Process DeconstructProcess => GetModdedGDO<Process, DeconstructProcess>();
        public static string RETURN_MONEY_ID = "ReturnMoneyOnDeconstruct";
        public static string RETURN_MONEY_PERCENTAGE_ID = "ReturnMoneyOnDeconstructPercentage";


        public static AssetBundle Bundle;

        public Mod() : base(MOD_GUID, MOD_NAME, MOD_AUTHOR, MOD_VERSION, MOD_GAMEVERSION, Assembly.GetExecutingAssembly()) { }

        protected override void OnInitialise()
        {
            LogWarning($"{MOD_GUID} v{MOD_VERSION} in use!");
        }

        private void AddGameData()
        {
            LogInfo("Attempting to register game data...");

            AddGameDataObject<Deconstructor>();
            AddGameDataObject<DeconstructProcess>();
            PrefManager = new PreferenceSystemManager(MOD_GUID, MOD_NAME);
            CreatePreferences();
            LogInfo("Done loading game data.");
        }

        protected override void OnUpdate()
        {
        }

        protected override void OnPostActivate(KitchenMods.Mod mod)
        {
            // TODO: Uncomment the following if you have an asset bundle.
            // TODO: Also, make sure to set EnableAssetBundleDeploy to 'true' in your ModName.csproj

            LogInfo("Attempting to load asset bundle...");
            Bundle = mod.GetPacks<AssetBundleModPack>().SelectMany(e => e.AssetBundles).First();

            Bundle.LoadAllAssets<VisualEffect>();
            Bundle.LoadAllAssets<VisualEffectAsset>();
            Bundle.LoadAllAssets<AudioClip>();
            Bundle.LoadAllAssets<TMP_SpriteAsset>();
            var spriteAsset = Bundle.LoadAsset<TMP_SpriteAsset>("cogpngwhitescaled");
            Material m = new Material(TMP_Settings.defaultSpriteAsset.material);
            spriteAsset.material = m;
            spriteAsset.material.mainTexture = Bundle.LoadAsset<Texture2D>("cogpngwhitescaledTex");
            TMP_Settings.defaultSpriteAsset.fallbackSpriteAssets.Add(spriteAsset);
            
            LogInfo("Done loading asset bundle.");
            // Register custom GDOs
            AddGameData();
            // Perform actions when game data is built
            Events.BuildGameDataEvent += delegate (object s, BuildGameDataEventArgs args)
            {
                GetExistingGDO<Appliance>(ApplianceReferences.BlueprintCabinet).Upgrades.Add(Deconstructor);
            };
        }

        private void CreatePreferences()
        {
            string[] strings;
            PrefManager
                .AddLabel("Deconstructor")
                .AddLabel("Return money on deconstruct : ")
                .AddOption(RETURN_MONEY_ID, false, new bool[] { false, true }, new string[] { "Disabled", "Enabled" })
                .AddLabel("Money Returned : ")
                .AddOption(RETURN_MONEY_PERCENTAGE_ID, 100, Utils.GenerateIntArray("0|100|1", out strings, postfix: "%"), strings)
                .AddSpacer()
                .AddSpacer();

            PrefManager.RegisterMenu(PreferenceSystemManager.MenuType.MainMenu);
            PrefManager.RegisterMenu(PreferenceSystemManager.MenuType.PauseMenu);




        }

        private static T1 GetModdedGDO<T1, T2>() where T1 : GameDataObject
        {
            return (T1)GDOUtils.GetCustomGameDataObject<T2>().GameDataObject;
        }
        private static T GetExistingGDO<T>(int id) where T : GameDataObject
        {
            return (T)GDOUtils.GetExistingGDO(id);
        }
        internal static T Find<T>(int id) where T : GameDataObject
        {
            return (T)GDOUtils.GetExistingGDO(id) ?? (T)GDOUtils.GetCustomGameDataObject(id)?.GameDataObject;
        }

        internal static T Find<T, C>() where T : GameDataObject where C : CustomGameDataObject
        {
            return GDOUtils.GetCastedGDO<T, C>();
        }

        internal static T Find<T>(string modName, string name) where T : GameDataObject
        {
            return GDOUtils.GetCastedGDO<T>(modName, name);
        }
        #region Logging
        public static void LogInfo(string _log) { Debug.Log($"[{MOD_NAME}] " + _log); }
        public static void LogWarning(string _log) { Debug.LogWarning($"[{MOD_NAME}] " + _log); }
        public static void LogError(string _log) { Debug.LogError($"[{MOD_NAME}] " + _log); }
        public static void LogInfo(object _log) { LogInfo(_log.ToString()); }
        public static void LogWarning(object _log) { LogWarning(_log.ToString()); }
        public static void LogError(object _log) { LogError(_log.ToString()); }
        #endregion
    }
}
