using RimWorld;
using Verse;
namespace CaravanHunting
{
    [StaticConstructorOnStartup]
    public static class InitPatches
    {
        static InitPatches()
        {
            Log.Message("Hello From Patcher!!");
            PatchAddCaravanDecisionsComp();
        }


        private static void PatchAddCaravanDecisionsComp()
        {
            var caravanDef = WorldObjectDefOf.Caravan;
            if (caravanDef != null)
            {
                caravanDef.comps.Add(
                    new WorldObjectCompProperties()
                    {
                        compClass = typeof(CaravanHunting)
                    }
                );
            }
        }
    }
}
