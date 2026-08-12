using System.Collections.Generic;
using RimWorld;
using RimWorld.Planet;
using Verse;
using UnityEngine;
using System.Linq;
using System;

namespace CaravanHunting
{
    public class CaravanHunting : WorldObjectComp
    {
        public float progress = 0;
        public bool isHunting = false;
        public const int TicksToFinish = 4 * GenDate.TicksPerHour;

        public Caravan Caravan;

        public int hunters = 0;
        public float best_efficiency = 0;

        public override void Initialize(WorldObjectCompProperties props)
        {
            base.Initialize(props);
            GetCaravan();
        }
        public override void PostMapGenerate()
        {
            GetCaravan();
        }
        // ui
        public override IEnumerable<Gizmo> GetCaravanGizmos(Caravan caravan)
        {
            GetCaravan();
            CountCombatans();
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
                    Disabled = hunters <= 0,
                    disabledReason = "No colonists capable of violence",
                };
                yield return cmdAllowNightTravel;
            }
            yield break;
        }
        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref isHunting, "IsHunting", false);
            Scribe_Values.Look(ref progress, "huntingProgress", 0);
        }
        private void CountCombatans()
        {

            hunters = 0;
            if (Caravan == null)
            {
                return;
            }
            for (int i = 0; i < Caravan.pawns.Count; i++)
            {
                if (!Caravan.pawns[i].WorkTagIsDisabled(WorkTags.Violent) && !Caravan.pawns[i].NonHumanlikeOrWildMan())
                {
                    hunters += 1;
                    best_efficiency = Mathf.Max(best_efficiency, Caravan.pawns[i].GetStatValue(MyDefsOf.ButcheryFleshEfficiency));
                }
            }
        }
        private void GetCaravan()
        {
            if (parent is Caravan)
            {
                Caravan = parent as Caravan;
            }
        }
        // game logic
        public override void CompTick()
        {
            GetCaravan();
            if (Caravan == null)
            {
                return;
            }
            if (hunters <= 0)
            {
                return;
            }
            if (progress == TicksToFinish)
            {
                if (Caravan != null)
                {
                    GiveItems();
                }
                progress = 0;
            }
            if (!Caravan.NightResting)
            { // can't hunt while sleep
                progress += 1;
            }
        }
        public string GetProgress()
        {
            return (100 * (float)progress / TicksToFinish).ToString();
        }
        public void GiveItems()
        {
            if (Caravan == null)
            {
                return;
            }
            var animal = Caravan.Biome.AllWildAnimals
            .Where(i => i.RaceProps.baseBodySize >= 1f)
            .OrderByDescending(Caravan.Biome.CommonalityOfAnimal)
            .RandomElement();

            int val = UnityEngine.Random.Range(1, hunters);
            for (int i = 0; i < val; i++)
            {
                ThingDef leather = animal.RaceProps.leatherDef ?? ThingDefOf.Leather_Plain;
                Thing leather_res = ThingMaker.MakeThing(leather);
                leather_res.stackCount = GenMath.RoundRandom
                (
                    animal.race.GetStatValueAbstract(StatDefOf.LeatherAmount)
                    * Find.Storyteller.difficulty.butcherYieldFactor
                    * best_efficiency
                    * 0.7f // same as butcher spot
                );
                CaravanInventoryUtility.GiveThing(Caravan, leather_res);

                ThingDef meat = animal.RaceProps.meatDef ?? ThingDefOf.Cow.race.meatDef;
                Thing meat_res = ThingMaker.MakeThing(meat);
                meat_res.stackCount = (int)animal.race.GetStatValueAbstract(StatDefOf.MeatAmount);
                meat_res.stackCount = GenMath.RoundRandom
                (
                    animal.race.GetStatValueAbstract(StatDefOf.MeatAmount)
                    * Find.Storyteller.difficulty.butcherYieldFactor
                    * best_efficiency
                    * 0.7f // same as butcher spot
                );
                CaravanInventoryUtility.GiveThing(Caravan, meat_res);
            }



        }

        public void ResetProgress()
        {
            progress = 0;
        }

    }
}
