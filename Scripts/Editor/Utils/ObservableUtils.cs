using UnityEditor;
using UnityEngine;

namespace FullCircleData.Editor.Editor.Utils
{
    public static class ObservableUtils
    {
        private static readonly Color connectedColor = new Color(0.15f, 0.63f, 0.15f);
        private static readonly Color disconnectedColor = new Color(0.2f, 0.2f, 0.2f);

        public static void DrawStatus(Rect position, bool connected)
        { 
            EditorGUI.DrawRect(
                new Rect(position.x - 16, position.y + 4, 2, 14),
                connected ? connectedColor : disconnectedColor
            );
        }
    }
}