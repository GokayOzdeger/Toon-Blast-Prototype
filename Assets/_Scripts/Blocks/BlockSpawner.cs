using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockSpawner
{
    private uint[] BlockSpawnRequests;

    private uint _collumnCount;

    public BlockSpawner(uint collumnCount)
    {
        _collumnCount = collumnCount;
        FlushRequests();
    }

    private void FlushRequests()
    {
        BlockSpawnRequests = new uint[_collumnCount];
    }

    public void AddBlockSpawnReqeust(int collumnIndex)
    {
        BlockSpawnRequests[collumnIndex] += 1;
    }
}
