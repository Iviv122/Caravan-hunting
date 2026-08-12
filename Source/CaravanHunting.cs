using System.Collections.Generic;
using RimWorld;
using RimWorld.Planet;
using Verse;
using UnityEngine;

namespace CaravanHunting
{
    public class CaravanHunting : WorldObjectComp
    {
        public float progress = 0;
        public bool isHunting = false;
        public const float TicksToFinish = 4*GenDate.TicksPerHour;

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref isHunting, "allowNightTravel", false);
        }

        public void ResetProgress(){
            progress = 0;
        }

        public override IEnumerable<Gizmo> GetCaravanGizmos(Caravan caravan)
        {
            if (Find.WorldSelector.SingleSelectedObject == this.parent && this.parent != null && this.parent.Faction != null && this.parent.Faction == Faction.OfPlayerSilentFail)
            {
                var cmdAllowNightTravel = new Command_Toggle
                {
                    isActive = () => isHunting,
                    toggleAction = () => {
                        isHunting = !isHunting;
                        ResetProgress();
                    },
                    defaultLabel = "Hunt along way",
                    defaultDesc = "Your colonists will hunt while wander, it will increase their visibility and slow them down",
                    Order = 199f,
                    icon = ContentFinder<Texture2D>.Get("UI/Icon/Hunting", true),
                };
                yield return cmdAllowNightTravel;
            }
            yield break;
        }

        public override void CompTick()
        {
            progress += 1;
        }
        public string GetProgress(){
            return (100*progress/TicksToFinish).ToString();
        }

    }
}
