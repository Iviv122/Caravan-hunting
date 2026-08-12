using System.Text;
using HarmonyLib;
using RimWorld.Planet;
using Verse;

namespace CaravanHunting{
    public static class HuntingPatcher{
        public static void ApplyPatches()
        {

            // left corner text
            var TravelOrgDesc = AccessTools.Method(typeof(Caravan), nameof(Caravan.GetInspectString));
            var TravelNewDesc = new HarmonyMethod(typeof(HuntingPatcher).GetMethod(nameof(DescPostFix)));
            HarmonyPatcher.harmony.Patch(TravelOrgDesc, null, TravelNewDesc);

            Log.Message("applied Hunting patch");
        }
        public static void DescPostFix(ref string __result, Caravan __instance) {
            if (__result != null) {
                var cmp = __instance.GetComponent<CaravanHunting>();
                if (cmp != null) {
                    StringBuilder build = new StringBuilder(__result);
                    __result = build.Append(cmp.isHunting ? $"\nHunting right now: {cmp.GetProgress()}%" : "").ToString();
                }
            }
        }
    }
}
