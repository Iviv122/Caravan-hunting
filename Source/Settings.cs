using UnityEngine;
using Verse;

namespace CaravanHunting
{
    class ModSettings : Verse.ModSettings
    {
        public int HoursRequired = 4;
        public float ButcheryEfficiencyMod = 0.7f;
        public bool NeedRangeWeapon = true;

        public float VisibilityModifier = 1.2f;
        public float MoveMultiplier = 0.8f;
        private Vector2 scrollPos = Vector2.zero;

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Values.Look(ref ButcheryEfficiencyMod, "ButcheryEfficiencyMod", 0.7f);
            Scribe_Values.Look(ref HoursRequired, "HoursRequired", 4);
            Scribe_Values.Look(ref NeedRangeWeapon, "NeedRangeWeapon", true);
            Scribe_Values.Look(ref MoveMultiplier, "MoveMultiplier", 0.8f);
            Scribe_Values.Look(ref VisibilityModifier, "VisibilityModifier", 1.2f);
        }

        public void DoWindowContents(Rect wrect)
        {
            var viewRect = new Rect(0f, 0f, wrect.width - 20f, 500f);
            Widgets.BeginScrollView(wrect, ref scrollPos, viewRect);
            var listing = new Listing_Standard();
            listing.Begin(viewRect);

            listing.Label("Hours required: " + HoursRequired);
            HoursRequired = Mathf.RoundToInt(listing.Slider(HoursRequired, 1f, 24f));

            listing.Gap();

            listing.Label("Butchery efficiency modifier: " + ButcheryEfficiencyMod.ToString("0.00"));
            ButcheryEfficiencyMod = listing.Slider(ButcheryEfficiencyMod, 0.1f, 2f);

            listing.Gap();

            listing.Label("Movespeed while hunting: " + MoveMultiplier.ToString("0.00"));
            MoveMultiplier = listing.Slider(MoveMultiplier, 0.1f, 2f);

            listing.Gap();

            listing.Label("Visibility while hunting: " + VisibilityModifier.ToString("0.00"));
            VisibilityModifier = listing.Slider(VisibilityModifier, 0.1f, 2f);

            listing.Gap();

            listing.CheckboxLabeled("Require a ranged weapon", ref NeedRangeWeapon, "When enabled, hunting requires the pawn to have a ranged weapon.");

            listing.GapLine();
            if (listing.ButtonText("Reset to defaults"))
            {
                HoursRequired = 4;
                ButcheryEfficiencyMod = 0.7f;
                MoveMultiplier = 0.8f;
                NeedRangeWeapon = true;
                VisibilityModifier = 1.2f;
            }

            listing.End();

            Widgets.EndScrollView();

            Write();
        }
    }
}
