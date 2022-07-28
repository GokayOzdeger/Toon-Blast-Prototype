using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;
using DG.Tweening;
using System;
using UnityEngine.UI;

public class UIEffectsManager : AutoSingleton<UIEffectsManager>
{
    public enum CanvasLayer
    {
        OverGridUnderUI,
        OverEverything,
    }

    [BHeader("References")]
    [SerializeField] private RectTransform onGridUnderUiLayerParent;
    [SerializeField] private RectTransform overEverythingLayerParent;
    [BHeader("Prefabs")]
    [SerializeField] private GameObject flyingSpritePrefab;
    [SerializeField] private GameObject flyingTextPrefab;
    [BHeader("Point References")]
    [SerializeField] private List<RectTransform> pointReferenceRects = new List<RectTransform>();
    
    public void CreateCurvyFlyingSprite(Sprite sprite, Vector2 spriteSize, Vector2 spawnPos, Vector2 targetPos, CanvasLayer layer, Action onComplete = null)
    {
        GameObject flyingObj = ObjectPooler.Instance.Spawn(flyingSpritePrefab.name, spawnPos);
        flyingObj.transform.SetParent(GetLayerParent(layer));
        flyingObj.GetComponent<RectTransform>().sizeDelta = spriteSize;
        flyingObj.GetComponent<Image>().sprite = sprite;
        Tween flyingTween = TweenHelper.CurvingMoveTo(flyingObj.transform, targetPos, onComplete, 1f, .2f, Ease.InOutCubic, Ease.InBack);
        flyingTween.onComplete += () => flyingObj.GetComponent<PoolObject>().GoToPool();
    }

    public void CreateLinearFlyingSprite(Sprite sprite, Vector2 spriteSize, Vector2 spawnPos, Vector2 targetPos, CanvasLayer layer, Action onComplete = null)
    {
        GameObject flyingObj = ObjectPooler.Instance.Spawn(flyingSpritePrefab.name, spawnPos);
        flyingObj.transform.SetParent(GetLayerParent(layer));
        flyingObj.GetComponent<RectTransform>().sizeDelta = spriteSize;
        flyingObj.GetComponent<Image>().sprite = sprite;
        Tween flyingTween = TweenHelper.LinearMoveTo(flyingObj.transform, targetPos, onComplete);
        flyingTween.onComplete += () => flyingObj.GetComponent<PoolObject>().GoToPool();
    }

    public void CreatePassingByFlyingText(string text, float fontSize, Vector2 spawnPos, Vector2 waitingPos, Vector2 targetPos, CanvasLayer layer, float moveDuration, float waitDuration, Action onComplete = null)
    {
        GameObject flyingObj = ObjectPooler.Instance.Spawn(flyingTextPrefab.name, spawnPos);
        flyingObj.transform.SetParent(GetLayerParent(layer));
        TMPro.TMP_Text textComponent = flyingObj.GetComponent<TMPro.TMP_Text>();
        textComponent.text = text;
        textComponent.fontSize = fontSize;
        Tween flyingTween = TweenHelper.PassingBy(flyingObj.transform, spawnPos, waitingPos, targetPos, moveDuration, waitDuration, onComplete);
        flyingTween.onComplete += () => flyingObj.GetComponent<PoolObject>().GoToPool();
    }

    public RectTransform GetLayerParent(CanvasLayer layer)
    {
        switch (layer)
        {
            case CanvasLayer.OverGridUnderUI:
                return onGridUnderUiLayerParent;
            case CanvasLayer.OverEverything:
                return overEverythingLayerParent;
        }
        return null;
    }

    public Vector2 GetReferencePointByName(string referenceName)
    {
        foreach (RectTransform rect in pointReferenceRects)
        {
            if (rect.name != referenceName) continue;
            return rect.position;
        }
        Debug.LogError("Reference point with Name not found: " + referenceName);
        return Vector2.zero;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        foreach (RectTransform rect in pointReferenceRects)
        {
            if (rect == null) continue;
            Gizmos.DrawSphere(rect.position, 50f);
            Extensions.drawString(rect.name, rect.position);
        }
    }
}
