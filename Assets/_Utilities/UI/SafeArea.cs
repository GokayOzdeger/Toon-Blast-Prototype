using UnityEngine;

namespace Utilities
{    
    [ExecuteAlways]
    public class SafeArea : MonoBehaviour
    {
        private float offsetMultiplier =.7f;

        private void Awake()
        {
            if(Application.isPlaying) ApplySafeArea();
            DeviceManager.Instance.DeviceOrientationChanged += ApplySafeArea;
        }

        private void OnDestroy()
        {
            DeviceManager.Instance.DeviceOrientationChanged -= ApplySafeArea;
        }

        private void GetSpecialOffsets(out Vector2 minOffset, out Vector2 maxOffset)
        {
            minOffset = Vector2.zero;
            maxOffset = Vector2.zero;

            float leftPadding = Screen.safeArea.xMin;
            float rightPadding =Screen.width - Screen.safeArea.xMax;

            DeviceOrientation myOrientation = DeviceManager.Instance.DeviceOrientate;
            
            if (DeviceManager.Instance.HasNotch)
            {
                if (myOrientation == DeviceOrientation.LandscapeRight)
                {
                    minOffset = new Vector2(-leftPadding * offsetMultiplier, 0) / Screen.width;
                }
                else
                {
                    maxOffset = new Vector2(rightPadding * offsetMultiplier, 0) / Screen.width;
                }
            }
            
        }

        private void ApplySafeArea()
        {
            var rectTransform = GetComponent<RectTransform>();

            var safeArea = Screen.safeArea;
            var anchorMin = safeArea.position;
            var anchorMax = anchorMin + safeArea.size;
            anchorMin.x /= Screen.width;
            anchorMin.y /= Screen.height;
            anchorMax.x /= Screen.width;
            anchorMax.y /= Screen.height;

            Vector2 minOffset = Vector2.zero;
            Vector2 maxOffset = Vector2.zero;
            GetSpecialOffsets(out minOffset, out maxOffset);

            if (Input.deviceOrientation != DeviceOrientation.Unknown)
            {
                Debug.Log(Input.deviceOrientation);
            }

            rectTransform.anchorMin = anchorMin + minOffset;
            rectTransform.anchorMax = anchorMax + maxOffset;
        }
    }
}