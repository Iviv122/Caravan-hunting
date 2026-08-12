using Verse;
using HarmonyLib;

namespace CaravanHunting{
    [StaticConstructorOnStartup]
    public static class HarmonyPatcher{
        public static Harmony harmony;

        static HarmonyPatcher(){
            harmony = harmony ?? (harmony = new Harmony("iviv122.huntingPatcher"));
            HuntingPatcher.ApplyPatches();
        }
    }
}
