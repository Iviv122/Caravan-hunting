using System.Collections.Generic;
using RimWorld;
using RimWorld.Planet;
using Verse;
using UnityEngine;
using System.Linq;
using System;


// TODO: Settings
namespace CaravanHunting
{
    public class CaravanHunting : WorldObjectComp
    {
        public float progress = 0;
        public bool isHunting = false;
        public int TicksToFinish => Main.Settings.HoursRequired * GenDate.TicksPerHour;

        private Command_Toggle cmdAllowHuntnig = null;
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
            CountEfficiency();

            if (Find.WorldSelector.SingleSelectedObject == parent
                && parent != null
                && parent.Faction != null
                && parent.Faction == Faction.OfPlayerSilentFail)
            {
                cmdAllowHuntnig = new Command_Toggle
                {
                    isActive = () => isHunting,

                    toggleAction = () =>
                    {
                        isHunting = !isHunting;
                        ResetProgress();
                    },

                    defaultLabel = "Hunt along way",
                    defaultDesc = $"Your colonists will hunt while wandering. " +
                                  $"It will increase their visibility and slow them down.\n" +
                                  $"Butchery efficiency: {Math.Round(best_efficiency * Main.Settings.ButcheryEfficiencyMod, 2)}\n" +
                                  $"Hunters: {hunters}",

                    Order = 199f,

                    icon = ContentFinder<Texture2D>.Get(
                        "UI/Icons/Animal/Hunt",
                        true
                    ),

                    Disabled = hunters <= 0 || best_efficiency <= 0,
                    disabledReason = DisableReason(),
                };

                yield return cmdAllowHuntnig;
            }
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
                if (!Caravan.pawns[i].WorkTagIsDisabled(WorkTags.Violent)
                    && !Caravan.pawns[i].NonHumanlikeOrWildMan()
                    && HasRangedWeapon(Caravan.pawns[i])
                )
                {
                    hunters += 1;
                }
            }
            UpdateIsDisabled();
        }
        private void UpdateIsDisabled()
        {
            cmdAllowHuntnig?.Disabled = hunters <= 0 || Mathf.Approximately(best_efficiency, 0);
            cmdAllowHuntnig?.disabledReason = DisableReason();
        }
        private string DisableReason()
        {
            if (Mathf.Approximately(best_efficiency, 0))
            {
                return "No butchers with efficiency better than zero";
            }
            if (hunters == 0)
            {
                return "No hunters with ranged weapons";
            }

            return "Error";
        }
        private bool HasRangedWeapon(Pawn p)
        {
            if (!Main.Settings.NeedRangeWeapon)
            {
                return true;
            }
            if (p.equipment.Primary != null)
            {
                return p.equipment.Primary.def.IsRangedWeapon;
            }
            return false;

        }
        private void CountEfficiency()
        {
            best_efficiency = 0;
            if (Caravan == null)
            {
                return;
            }
            for (int i = 0; i < Caravan.pawns.Count; i++)
            {
                if (!Caravan.pawns[i].WorkTagIsDisabled(WorkTags.Cooking))
                {
                    best_efficiency = Mathf.Max(best_efficiency, Caravan.pawns[i].GetStatValue(MyDefsOf.ButcheryFleshEfficiency));
                }
            }
            UpdateIsDisabled();
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
            CountCombatans();
            CountEfficiency();
            if (!isHunting)
            {
                return;
            }
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
                    * Main.Settings.ButcheryEfficiencyMod
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
                    * Main.Settings.ButcheryEfficiencyMod
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
