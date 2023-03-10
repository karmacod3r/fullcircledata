using UnityEngine;

namespace FullCircleData.Extensions
{
    public static class TransformExtension
    {
        public static void AnchoredPositionX(this RectTransform transform, float posX)
        {
            var p = transform.anchoredPosition;
            p.x = posX;
            transform.anchoredPosition = p;
        }

        public static void AnchoredPositionY(this RectTransform transform, float posY)
        {
            var p = transform.anchoredPosition;
            p.y = posY;
            transform.anchoredPosition = p;
        }

        public static void SizeDeltaX(this RectTransform transform, float sizeX)
        {
            var p = transform.sizeDelta;
            p.x = sizeX;
            transform.sizeDelta = p;
        }

        public static void SizeDeltaY(this RectTransform transform, float sizeY)
        {
            var p = transform.sizeDelta;
            p.y = sizeY;
            transform.sizeDelta = p;
        }

        public static void MoveLocalX(this Transform transform, float posX)
        {
            var p = transform.localPosition;
            p.x = posX;
            transform.localPosition = p;
        }

        public static void MoveLocalY(this Transform transform, float posY)
        {
            var p = transform.localPosition;
            p.y = posY;
            transform.localPosition = p;
        }

        public static void MoveLocalZ(this Transform transform, float posZ)
        {
            var p = transform.localPosition;
            p.z = posZ;
            transform.localPosition = p;
        }

        public static void LocalScaleX(this Transform transform, float posX)
        {
            var p = transform.localScale;
            p.x = posX;
            transform.localScale = p;
        }

        public static void LocalScaleY(this Transform transform, float posY)
        {
            var p = transform.localScale;
            p.y = posY;
            transform.localScale = p;
        }

        public static void LocalScaleZ(this Transform transform, float posZ)
        {
            var p = transform.localScale;
            p.z = posZ;
            transform.localScale = p;
        }

        public static void RotationX(this Transform transform, float rotX)
        {
            var p = transform.rotation.eulerAngles;
            p.x = rotX;
            transform.rotation = Quaternion.Euler(p);
        }

        public static void RotationY(this Transform transform, float rotY)
        {
            var p = transform.rotation.eulerAngles;
            p.y = rotY;
            transform.rotation = Quaternion.Euler(p);
        }

        public static void RotationZ(this Transform transform, float rotZ)
        {
            var p = transform.rotation.eulerAngles;
            p.z = rotZ;
            transform.rotation = Quaternion.Euler(p);
        }

        public static void LocalRotationX(this Transform transform, float rotX)
        {
            var p = transform.localRotation.eulerAngles;
            p.x = rotX;
            transform.localRotation = Quaternion.Euler(p);
        }

        public static void LocalRotationY(this Transform transform, float rotY)
        {
            var p = transform.localRotation.eulerAngles;
            p.y = rotY;
            transform.localRotation = Quaternion.Euler(p);
        }

        public static void LocalRotationZ(this Transform transform, float rotZ)
        {
            var p = transform.localRotation.eulerAngles;
            p.z = rotZ;
            transform.localRotation = Quaternion.Euler(p);
        }
    }
}