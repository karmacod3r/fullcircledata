using UnityEngine;

namespace FullCircleData.Extensions
{
    public static class CanvasExtensions
    {
        public static Vector2 WorldToCanvas(this Canvas canvas,
            Vector3 world_position,
            Camera camera = null)
        {
            camera = camera ?? canvas.worldCamera;
            if (camera == null)
            {
                return Vector2.zero;
            }
 
            var viewportPosition = camera.WorldToViewportPoint(world_position);
            var canvasRect = canvas.GetComponent<RectTransform>();
 
            return new Vector2((viewportPosition.x * canvasRect.sizeDelta.x) - (canvasRect.sizeDelta.x * 0.5f),
                (viewportPosition.y * canvasRect.sizeDelta.y) - (canvasRect.sizeDelta.y * 0.5f)) * canvas.scaleFactor;
        }
    }
    
}