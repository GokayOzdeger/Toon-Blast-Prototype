using System.Collections.Generic;


public class FallingBlocksGroup
{
    public List<Block> FallingBlocks;
    public int FallDistance;

    public FallingBlocksGroup(List<Block> blocks, int fallDistance)
    {
        FallingBlocks = blocks;
        FallDistance = fallDistance;
    }
}

