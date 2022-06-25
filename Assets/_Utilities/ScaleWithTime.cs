using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;
public class ScaleWithTime : MonoBehaviour
{
    [SerializeField] private Vector2 timeRange;
    [SerializeField] private AnimationCurve shrinkCurve;

    public bool IsEnabled { get; set; } = false;
    private Vector3 originalScale;
    private float counter = 0;
    private float time;
    private void Start()
    {
        time = Random.Range(timeRange.x, timeRange.y);
        originalScale = transform.localScale;
    }

    private void FixedUpdate()
    {
        if (!IsEnabled) return;
        counter += Time.fixedDeltaTime;
        transform.localScale = originalScale * (1-shrinkCurve.Evaluate(counter/time));
    }

    public void SetTimeRange(Vector2 newRange)
    {
        timeRange = newRange;
    }
}
