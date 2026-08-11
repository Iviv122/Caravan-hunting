using Verse;

namespace CaravanHunting
{
    public class Main : Mod
    {
        //internal static ModSettings Settings { get; set; }

        public Main(ModContentPack content) : base(content)
        {
            Log.Message("Hello World! It had loaded YOHO");
            //Settings = GetSettings<ModSettings>();
            //HarmonyPatcher.RunEarlyPatches();
        }

        /*
        public override void DoSettingsWindowContents(UnityEngine.Rect inRect)
        {
            base.DoSettingsWindowContents(inRect);
            GetSettings<ModSettings>().DoWindowContents(inRect);
        }

        public override string SettingsCategory()
        {
            return "Caravan Adventures";
        }
        */
    }
}
