using RimWorld;

namespace CaravanHunting
{
    [DefOf]
    public static class MyDefsOf
    {
        public static StatDef ButcheryFleshEfficiency;
        static MyDefsOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(MyDefsOf));
        }
    }
}
