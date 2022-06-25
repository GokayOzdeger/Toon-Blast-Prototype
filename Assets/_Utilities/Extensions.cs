using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Utilities
{
    [Serializable]
    public struct Cooldown
    {
        public float cooldown;
        private float timer;

        public bool IsReady
        {
            get { if (timer >= cooldown) { return true; } else { return false; } }
        }

        public Cooldown(float _cooldown)
        {
            cooldown = _cooldown;
            timer = _cooldown;
        }

        public void Step(float deltaTime)
        {
            if (timer < cooldown)
            {
                timer += deltaTime;
            }
        }

        public void EnterCooldown()
        {
            timer = 0;
        }
    }

    public static class Extensions
    {
        public static Vector2 ToVector2(this Vector3 vector)
        {
            return new Vector2(vector.x, vector.z);
        }

        /// <summary>
        /// Remaps input range to output range
        /// </summary>
        /// <param name="input">Input value</param>
        /// <param name="minInput">Minimum possible input value</param>
        /// <param name="maxInput">Maximum possible input value</param>
        /// <param name="minOutput">Minimum possible output value after scaling</param>
        /// <param name="maxOutput">Maximum possible output value after scaling</param>
        /// <returns>Scaled output</returns>
        public static float Remap(float input, float minInput, float maxInput, float minOutput, float maxOutput)
        {
            return (((maxOutput - minOutput) * (input - minInput)) / (maxInput - minInput)) + minOutput;
        }

        public static string AddDotsToNumber(string numberString)
        {
            string finalString = "";
            int counter = 0;
            Debug.Log(numberString);
            for (int i = numberString.Length-1; i > -1; i--)
            {
                Debug.Log(numberString[i]);
                if (counter == 3) { finalString = '.' + finalString; counter = 0; }
                finalString = numberString[i] + finalString;
                counter++;
            }
            return finalString;
        }

        /// <summary>
        /// Clamps an angle value to minimum and maximum values
        /// </summary>
        /// <param name="angle">input angle</param>
        /// <param name="min">minimum value to clamp</param>
        /// <param name="max">maximum value to clamp</param>
        /// <returns></returns>
        public static float ClampAngle(float angle, float min, float max)
        {
            if (angle < -360F)
                angle += 360F;
            if (angle > 360F)
                angle -= 360F;
            return Mathf.Clamp(angle, min, max);
        }

        /// <summary>
        /// Cuts string if over desired length
        /// </summary>
        /// <param name="text">text</param>
        /// <param name="maxLenght">max length</param>
        /// <param name="startIndex"> start index of cut operation</param>
        /// <returns></returns>
        public static string ClampString(string text, int maxLenght, int startIndex = 0)
        {
            if (text.Length > maxLenght) return text.Substring(startIndex, maxLenght);
            return text;
        }

        /// <summary>
        /// A clamp method that works well with 359 degree system
        /// </summary>
        /// <param name="angle"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>

        public static float ClampEulerAngles(float angle, float min, float max)
        {
            float realmin = Mathf.Min(min, max);
            float realmax = Mathf.Max(min, max);

            //make the angle positive and between 0-359
            if (angle < 0)
                angle = 360 + angle;
            else if (angle > 360)
            {
                angle = angle % 360;
            }

            if (angle >= realmin && angle <= realmax)
            {
                if (Mathf.Abs(min - angle) > Mathf.Abs(max - angle))
                {
                    return max;
                }
                return min;
            }
            else
            {
                return angle;
            }
        }

        /// <summary>
        /// Destroys all children of a parent transform
        /// </summary>
        /// <param name="parent">Parent transform</param>
        public static void DestroyAllChildren(Transform parent)
        {
            foreach (Transform child in parent)
                GameObject.Destroy(child.gameObject);
        }
        
        /// <summary>
        /// Extension version of DestroyAllChildren
        /// </summary>
        /// <param name="transform"></param>
        
        public static void RemoveAllChildren(this Transform transform)
        {
            DestroyAllChildren(transform);
        }

        /// <summary>
        /// Recursive search in all the children and subchildren
        /// </summary>
        /// <param name="parent">Parent transform</param>
        public static Transform FindInAllChildren(Transform parent, string childName)
        {
            foreach (Transform child in parent)
            {
                if (child.name == childName)
                {
                    return child;
                }
                else
                {
                    Transform found = FindInAllChildren(child, childName);
                    if (found != null)
                    {
                        return found;
                    }
                }
            }
            return null;
        }


        /// <summary>
        /// Checks if the pointer is on an UI object (Canvas) or not
        /// </summary>
        /// <param name="canvases">Canvas list to process</param>
        /// <param name="screenPosition">Clicked position</param>
        /// <returns></returns>
        public static bool IsPointerOverUIObject(List<Canvas> canvases, Vector2 screenPosition)
        {
            PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
            eventDataCurrentPosition.position = screenPosition;

            int resultCount = 0;

            foreach (Canvas canvas in canvases)
            {
                GraphicRaycaster uiRaycaster = canvas.gameObject.GetComponent<GraphicRaycaster>();
                List<RaycastResult> results = new List<RaycastResult>();
                uiRaycaster.Raycast(eventDataCurrentPosition, results);
                resultCount += results.Count;
            }

            return resultCount > 0;
        }

        /// <summary>
        /// Checks if the pointer is on an UI object (Canvas) or not
        /// </summary>
        /// <param name="canvas">Canvas list to process</param>
        /// <param name="screenPosition">Clicked position</param>
        /// <returns></returns>
        public static bool IsPointerOverUIObject(Canvas canvas, Vector2 screenPosition)
        {
            PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
            eventDataCurrentPosition.position = screenPosition;
            int resultCount = 0;

            GraphicRaycaster uiRaycaster = canvas.gameObject.GetComponent<GraphicRaycaster>();
            List<RaycastResult> results = new List<RaycastResult>();
            uiRaycaster.Raycast(eventDataCurrentPosition, results);
            resultCount += results.Count;

            return resultCount > 0;
        }

        /// <summary>
        /// Calculates a random chance and returns a bool
        /// </summary>
        /// <param name="percentage">Percentage of probability</param>
        /// <returns>true if the RNG chance occurs, false for vice versa</returns>
        public static bool CalculateRngChange(float percentage)
        {
            float rand = UnityEngine.Random.Range(0f, 100f);

            if (percentage == 100f || rand < percentage && percentage != 0)
                return true;
            
            return false;
        }

        /// <summary>
        /// Returns String by adding spaces before captials
        /// </summary>
        /// <param name="text"></param>
        /// <param name="preserveAcronyms"></param>
        /// <returns></returns>
        public static string AddSpacesToSentence(string text, bool preserveAcronyms)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;
            StringBuilder newText = new StringBuilder(text.Length * 2);
            newText.Append(text[0]);
            for (int i = 1; i < text.Length; i++)
            {
                if (char.IsUpper(text[i]))
                    if ((text[i - 1] != ' ' && !char.IsUpper(text[i - 1])) ||
                        (preserveAcronyms && char.IsUpper(text[i - 1]) &&
                         i < text.Length - 1 && !char.IsUpper(text[i + 1])))
                        newText.Append(' ');
                newText.Append(text[i]);
            }
            return newText.ToString();
        }


        private static System.Random rng = new System.Random();
        /// <summary>
        /// Random shuffle Extension for lists
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        /// <summary>
        /// A stack that deletes old elements when it exceeds its capacity
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public class HistoryStack<T>
        {
            private LinkedList<T> items = new LinkedList<T>();
            public List<T> Items => items.ToList();
            public int Count => items.Count;
            public int Capacity { get; }
            public HistoryStack(int capacity)
            {
                Capacity = capacity;
            }

            public void Clear()
            {
                items = new LinkedList<T>();
            }

            public void Push(T item)
            {
                // full
                if (items.Count == Capacity)
                {
                    // we should remove first, because some times, if we exceeded the size of the internal array
                    // the system will allocate new array.
                    items.RemoveFirst();
                    items.AddLast(item);
                }
                else
                {
                    items.AddLast(new LinkedListNode<T>(item));
                }
            }

            public T Pop()
            {
                if (items.Count == 0)
                {
                    return default;
                }
                var ls = items.Last;
                items.RemoveLast();
                return ls == null ? default : ls.Value;
            }
        }
        
        /// <summary>
        /// Makes the material opaque. Source: https://forum.unity.com/threads/change-rendering-mode-via-script.476437/#post-5235491
        /// </summary>
        /// <param name="material">Material to be made opaque.</param>
        public static void ToOpaqueMode(this Material material)
        {
            material.SetOverrideTag("RenderType", "");
            material.SetInt("_SrcBlend", (int) UnityEngine.Rendering.BlendMode.One);
            material.SetInt("_DstBlend", (int) UnityEngine.Rendering.BlendMode.Zero);
            material.SetInt("_ZWrite", 1);
            material.DisableKeyword("_ALPHATEST_ON");
            material.DisableKeyword("_ALPHABLEND_ON");
            material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            material.renderQueue = -1;
        }
   
        /// <summary>
        /// Makes the material transparent. Source: https://forum.unity.com/threads/change-rendering-mode-via-script.476437/#post-5235491
        /// </summary>
        /// <param name="material">Material to be made transparent.</param>
        public static void ToFadeMode(this Material material)
        {
            material.SetOverrideTag("RenderType", "Transparent");
            material.SetInt("_SrcBlend", (int) UnityEngine.Rendering.BlendMode.SrcAlpha);
            material.SetInt("_DstBlend", (int) UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            material.SetInt("_ZWrite", 0);
            material.DisableKeyword("_ALPHATEST_ON");
            material.EnableKeyword("_ALPHABLEND_ON");
            material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            material.renderQueue = (int) UnityEngine.Rendering.RenderQueue.Transparent;
        }

        /// <summary>
        /// Clones a list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="listToClone"></param>
        /// <returns></returns>
        public static IList<T> Clone<T>(this IList<T> listToClone) where T : ICloneable
        {
                return listToClone.Select(item => (T) item.Clone()).ToList();
        }
        
        ///<summary>Finds the index of the first item matching an expression in an enumerable.</summary>
        ///<param name="items">The enumerable to search.</param>
        ///<param name="predicate">The expression to test the items against.</param>
        ///<returns>The index of the first matching item, or -1 if no items match.</returns>
        public static int FindIndex<T>(this IEnumerable<T> items, Func<T, bool> predicate) {
            if (items == null) throw new ArgumentNullException("items");
            if (predicate == null) throw new ArgumentNullException("predicate");

            int retVal = 0;
            foreach (var item in items) {
                if (predicate(item)) return retVal;
                retVal++;
            }
            return -1;
        }
        ///<summary>Finds the index of the first occurrence of an item in an enumerable.</summary>
        ///<param name="items">The enumerable to search.</param>
        ///<param name="item">The item to find.</param>
        ///<returns>The index of the first matching item, or -1 if the item was not found.</returns>
        public static int IndexOf<T>(this IEnumerable<T> items, T item) { return items.FindIndex(i => EqualityComparer<T>.Default.Equals(item, i)); }
        
        /// <summary>
        /// Prevents the object to launch unity events
        /// </summary>
        /// <param name="ev"></param>
        public static void Mute( UnityEngine.Events.UnityEventBase ev )
        {
            int count = ev.GetPersistentEventCount();
            for ( int i = 0 ; i < count ; i++ )
            {
                ev.SetPersistentListenerState( i, UnityEngine.Events.UnityEventCallState.Off );
            }
        }
 
        /// <summary>
        /// Allows the object to launch unity events
        /// </summary>
        /// <param name="ev"></param>
        public static void Unmute( UnityEngine.Events.UnityEventBase ev )
        {
            int count = ev.GetPersistentEventCount();
            for ( int i = 0 ; i < count ; i++ )
            {
                ev.SetPersistentListenerState( i, UnityEngine.Events.UnityEventCallState.RuntimeOnly );
            }
        }
        
        /// <summary>
        /// Shorthand method to check if the value is in range
        /// </summary>
        /// <param name="item"></param>
        /// <param name="start">Start of the range</param>
        /// <param name="end">End of the range</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static bool IsBetween<T>(this T item, T start, T end)
        {
            return Comparer<T>.Default.Compare(item, start) >= 0
                   && Comparer<T>.Default.Compare(item, end) <= 0;
        }        
        
        /// <summary>
        /// Picks a random member of the list
        /// </summary>
        /// <param name="list"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T PickRandom<T>(this IList<T> list)
        {
            int pickedIndex = UnityEngine.Random.Range(0, list.Count);
            return list[pickedIndex];
        }
        
        public static Vector3 SampleParabola(Vector3 start, Vector3 end, float height, float t)
        {
            float parabolicT = t * 2 - 1;
            if (Mathf.Abs(start.y - end.y) < 0.1f)
            {
                //start and end are roughly level, pretend they are - simpler solution with less steps
                Vector3 travelDirection = end - start;
                Vector3 result = start + t * travelDirection;
                result.y += (-parabolicT * parabolicT + 1) * height;
                return result;
            }
            else
            {
                //start and end are not level, gets more complicated
                Vector3 travelDirection = end - start;
                Vector3 levelDirecteion = end - new Vector3(start.x, end.y, start.z);
                Vector3 right = Vector3.Cross(travelDirection, levelDirecteion);
                Vector3 up = Vector3.Cross(right, travelDirection);
                if (end.y > start.y) up = -up;
                Vector3 result = start + t * travelDirection;
                result += ((-parabolicT * parabolicT + 1) * height) * up.normalized;
                return result;
            }
        }

        public static void StopParticleEmission(this ParticleSystem particle)
        {
            List<ParticleSystem> particles = particle.GetComponentsInChildren<ParticleSystem>().ToList();
            particles.Add(particle);

            foreach (var particleSystem in particles)
            {
                var emissionModule = particleSystem.emission;
                emissionModule.enabled = false;
            }
        }

        /// <summary>
        /// Draws Debug Text
        /// </summary>
        /// <param name="text">text to display</param>
        /// <param name="worldPos">Text position</param>
        /// <param name="textColor">Text color</param>
        /// <returns></returns>
#if UNITY_EDITOR
        public static void drawString(string text, Vector3 worldPos, Color? textColor = null)
        {
            if (textColor.HasValue) UnityEditor.Handles.color = textColor.Value;
            UnityEditor.Handles.Label(worldPos, text);

        }

        public static void MoveToGround(Transform transform)
        {
            transform.position += new Vector3(0, 20, 0);
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, Mathf.Infinity, ~LayerMask.GetMask("CommandAreas")))
            {
                transform.position -= new Vector3(0, hit.distance, 0);
            }
            else
            {
                transform.position -= new Vector3(0, 20, 0);
            }
        }
#endif
    }
}