using UnityEngine;
using UnityEngine.UI;

namespace FullCircleData.Extensions
{
    public static class CanvasExtension
    {
        public static void Fade(this Image target, float alpha)
        {
            target.color = target.color.Fade(alpha);
        }
        
        public static void Fade(this RawImage target, float alpha)
        {
            target.color = target.color.Fade(alpha);
        }
        
        public static void Fade(this SpriteRenderer target, float alpha)
        {
            target.color = target.color.Fade(alpha);
        }
        
        private static Color Fade(this Color target, float alpha)
        {
            target.a = alpha;
            return target;
        }
    }
}