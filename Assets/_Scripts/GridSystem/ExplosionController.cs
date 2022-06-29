using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionController
{
    private GridController _gridController;
    

    public bool TryStartExplosion()
    {
        if (_gridController.GridInAction) return false;
        _gridController.GridInAction = true;



        return true;
    }
}
