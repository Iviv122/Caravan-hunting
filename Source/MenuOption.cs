using System.Collections.Generic;
using RimWorld;
using RimWorld.Planet;
using Verse;
using UnityEngine;

namespace CaravanHunting
{
    public class CompCaravanDecisions : WorldObjectComp
    {
        public bool isHunting = false;

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref isHunting, "allowNightTravel", false);
        }

        public override IEnumerable<Gizmo> GetCaravanGizmos(Caravan caravan)
        {
            if (Find.WorldSelector.SingleSelectedObject == this.parent && this.parent != null && this.parent.Faction != null && this.parent.Faction == Faction.OfPlayerSilentFail)
            {
                var cmdAllowNightTravel = new Command_Toggle
                {
                    isActive = () => isHunting,
                    toggleAction = () => isHunting = !isHunting,
                    defaultLabel = "Hunt along way",
                    defaultDesc = "Your colonists will hunt while wander, it will increase their visibility and slow them down",
                    Order = 199f,
                    icon = ContentFinder<Texture2D>.Get("UI/Icon/Hunting", true),
                };
                yield return cmdAllowNightTravel;
            }
            yield break;
        }


    }
}
