using System;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using RimWorld.Planet;
using Verse;

namespace CaravanHunting
{
    public static class HuntingPatcher
    {
        public static void ApplyPatches()
        {

            // left corner text
            var TravelOrgDesc = AccessTools.Method(typeof(Caravan), nameof(Caravan.GetInspectString));
            var TravelNewDesc = new HarmonyMethod(typeof(HuntingPatcher).GetMethod(nameof(DescPostFix)));
            HarmonyPatcher.harmony.Patch(TravelOrgDesc, null, TravelNewDesc);

            var TravelOrgTickMove = AccessTools.PropertyGetter(typeof(Caravan), nameof(Caravan.TicksPerMove));
            var TravelNewTickMove = new HarmonyMethod(typeof(HuntingPatcher).GetMethod(nameof(GetTicksPermovePostFix)));
            HarmonyPatcher.harmony.Patch(TravelOrgTickMove, null, TravelNewTickMove);

            Log.Message("applied Hunting patch");
        }
        public static void DescPostFix(ref string __result, Caravan __instance)
        {
            if (__result != null)
            {
                var cmp = __instance.GetComponent<CaravanHunting>();
                if (cmp != null)
                {
                    StringBuilder build = new StringBuilder(__result);
                    __result = build.Append(cmp.isHunting ? $"\nHunting right now: {cmp.GetProgress()}% {cmp.hunters} colonists are hunting now" : "").ToString();
                }
            }
        }
        public static void GetTicksPermovePostFix(ref int __result, Caravan __instance)
        {
            var cmp = __instance.GetComponent<CaravanHunting>();
            if (cmp != null)
            {
                if (cmp.isHunting)
                {
                    Log.Message($"Old MoveSpeed: {__result}");
                    __result = (int)(__result / Main.Settings.MoveMultiplier);
                    Log.Message($"New MoveSpeed: {__result}");
                }
            }
        }
    }
}
