using System.Collections.Generic;
using Spine.Unity;

namespace Toge.Battle
{
    public static class SpineAnimResolver
    {
        private static readonly Dictionary<string, string> SkinAlias = new Dictionary<string, string>
        {
            { "cat_punk", "catpunk" },
            { "wolf", "catpunk" },
            { "default", "catpunk" },
        };

        public static void Resolve(SkeletonAnimation spine, string fallbackSkin,
            out string idle, out string attack, out string die)
        {
            idle = ResolveClip(spine, fallbackSkin, "idle", "idle");
            attack = ResolveClip(spine, fallbackSkin, "attack001", "attack001");
            die = ResolveClip(spine, fallbackSkin, "die", "die");
        }

        public static string ResolveClip(SkeletonAnimation spine, string fallbackSkin, string action, string fallback)
        {
            if (spine == null || spine.Skeleton == null) return fallback;

            string skin = spine.Skeleton.Skin != null ? spine.Skeleton.Skin.Name : fallbackSkin;
            if (string.IsNullOrEmpty(skin)) return fallback;
            if (SkinAlias.TryGetValue(skin, out string alias)) skin = alias;

            string head = skin + "_" + action + "_";
            foreach (Spine.Animation a in spine.Skeleton.Data.Animations)
                if (a.Name.StartsWith(head)) return a.Name;
            return fallback;
        }
    }
}
