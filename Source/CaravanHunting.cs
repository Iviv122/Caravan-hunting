using System.Collections.Generic;
using RimWorld;
using RimWorld.Planet;
using Verse;
using UnityEngine;
using System.Linq;

namespace CaravanHunting
{
    public class CaravanHunting : WorldObjectComp
    {
        public float progress = 0;
        public bool isHunting = false;
        public const int TicksToFinish = 4 * GenDate.TicksPerHour;

        public Caravan Caravan;

        // ui
        public override IEnumerable<Gizmo> GetCaravanGizmos(Caravan caravan)
        {
            GetCaravan();
            if (Find.WorldSelector.SingleSelectedObject == this.parent && this.parent != null && this.parent.Faction != null && this.parent.Faction == Faction.OfPlayerSilentFail)
            {
                var cmdAllowNightTravel = new Command_Toggle
                {
                    isActive = () => isHunting,
                    toggleAction = () =>
                    {
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
        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref isHunting, "allowNightTravel", false);
        }
        private void GetCaravan(){
            if(Caravan != null){
                return;
            }
            if(parent is Caravan){
                Caravan = parent as Caravan;
            }
        }
        // game logic
        public override void CompTick()
        {
            if (progress == TicksToFinish)
            {
                if(Caravan == null){
                    GetCaravan();
                }
                if(Caravan != null){
                    GiveItems(Caravan);
                }
                progress = 0;
            }
            progress += 1;
        }
        public string GetProgress()
        {
            return (100 * (float)progress / TicksToFinish).ToString();
        }
        public void GiveItems(Caravan caravan)
        {
            var animal = caravan.Biome.AllWildAnimals
            .Where(i => i.RaceProps.baseBodySize >= 1f)
            .OrderByDescending(caravan.Biome.CommonalityOfAnimal)
            .RandomElement();


            ThingDef leather = animal.RaceProps.leatherDef ?? ThingDefOf.Leather_Plain;
            Thing leather_res = ThingMaker.MakeThing(leather);
            leather_res.stackCount = (int)
            (
                animal.race.GetStatValueAbstract(StatDefOf.LeatherAmount)
                * Find.Storyteller.difficulty.butcherYieldFactor
            );
            CaravanInventoryUtility.GiveThing(caravan, leather_res);

            ThingDef meat = animal.RaceProps.meatDef ?? ThingDefOf.Cow.race.meatDef;
            Thing meat_res = ThingMaker.MakeThing(meat);
            meat_res.stackCount = (int)animal.race.GetStatValueAbstract(StatDefOf.MeatAmount);
            CaravanInventoryUtility.GiveThing(caravan, meat_res);

        }

        public void ResetProgress()
        {
            progress = 0;
        }

    }
}
