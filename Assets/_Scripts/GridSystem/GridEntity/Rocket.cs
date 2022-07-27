using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;
using Utilities;

public class Rocket : FallingGridEntity
{
    [SerializeField] RocketDirection direction;
    public void OnClickRocket()
    {
        if (!_gridController.GridInterractable) return;
        GameManager.Instance.CurrentLevel.MovesController.ClickedPowerUp();
        DestroyBlocksGridEvent destroyEvent = new DestroyBlocksGridEvent(EntityDestroyTypes.DestroyedByMatch);
        destroyEvent.StartEvent(_gridController, new List<Rocket>() { this });
    }

    public override void DestoryEntity(EntityDestroyTypes destroyType)
    {
        Debug.Log("Destroy Rocker: " + gameObject.name);
        StartExplosion();
        base.DestoryEntity(destroyType);
    }

    private void StartExplosion()
    {
        DestroyBlocksOneByOneGridEvent destroyEvent1 = new DestroyBlocksOneByOneGridEvent(EntityDestroyTypes.DestroyedByPowerUp);
        DestroyBlocksOneByOneGridEvent destroyEvent2 = new DestroyBlocksOneByOneGridEvent(EntityDestroyTypes.DestroyedByPowerUp);
        
        List<IGridEntity> entitiesInDirection1 = null;
        List<IGridEntity> entitiesInDirection2 = null;
        
        switch (direction)
        {
            case RocketDirection.Horizontal:
                entitiesInDirection1 = _gridController.GetEntitiesTowardsLeft(GridCoordinates);
                entitiesInDirection2 = _gridController.GetEntitiesTowardsRight(GridCoordinates);
                CreateHorizontalVisiualRockets();
                break;
            case RocketDirection.Vertical:
                entitiesInDirection1 = _gridController.GetEntitiesTowardsUp(GridCoordinates);
                entitiesInDirection2 = _gridController.GetEntitiesTowardsDown(GridCoordinates);
                CreateVerticalVisiualRockets();
                break;
        }

        destroyEvent1.StartEvent(_gridController, entitiesInDirection1);
        destroyEvent2.StartEvent(_gridController, entitiesInDirection2);
    }

    private void CreateHorizontalVisiualRockets()
    {
        Vector2 rocketSize = GetComponent<RectTransform>().sizeDelta;
        GameObject effectLeft = ObjectPooler.Instance.Spawn((EntityType as RocketTypeDefinition).RocketExplodeAnimPrefab.name, transform.position);
        GameObject effectRight = ObjectPooler.Instance.Spawn((EntityType as RocketTypeDefinition).RocketExplodeAnimPrefab.name, transform.position);
        RectTransform layerParent = UIEffectsManager.Instance.GetLayerParent(UIEffectsManager.CanvasLayer.OverEverything);

        effectRight.transform.Rotate(0, 0, 180);
        effectLeft.transform.SetParent(layerParent);
        effectRight.transform.SetParent(layerParent);
    }

    private void CreateVerticalVisiualRockets()
    {
        Vector2 rocketSize = GetComponent<RectTransform>().sizeDelta;
        Debug.Log($"RocketRefs: {EntityType.GetType()}");
        GameObject effectUp = ObjectPooler.Instance.Spawn((EntityType as RocketTypeDefinition).RocketExplodeAnimPrefab.name, transform.position);
        GameObject effectDown = ObjectPooler.Instance.Spawn((EntityType as RocketTypeDefinition).RocketExplodeAnimPrefab.name, transform.position);
        RectTransform layerParent = UIEffectsManager.Instance.GetLayerParent(UIEffectsManager.CanvasLayer.OverEverything);

        effectUp.transform.Rotate(0, 0, 90);
        effectDown.transform.Rotate(0, 0, 270);

        effectDown.GetComponent<RectTransform>().sizeDelta = rocketSize;
        effectUp.GetComponent<RectTransform>().sizeDelta = rocketSize;

        effectUp.transform.SetParent(layerParent);
        effectDown.transform.SetParent(layerParent);
    }

    private enum RocketDirection
    {
        Horizontal,
        Vertical
    }
}
