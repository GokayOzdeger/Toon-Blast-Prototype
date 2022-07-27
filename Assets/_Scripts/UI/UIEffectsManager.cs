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
    
    [SerializeField] private RectTransform onGridUnderUiLayerParent;
    [SerializeField] private RectTransform overEverythingLayerParent;

    [SerializeField] private GameObject flyingSpritePrefab;
    [SerializeField] private GameObject flyingTextPrefab;
    
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
}
