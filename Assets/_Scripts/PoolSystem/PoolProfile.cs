using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utilities
{
    [CreateAssetMenu(fileName = "NewPoolProfile", menuName = "ScriptableObjects/PoolProfile", order = 5)]
    public class PoolProfile : ScriptableObject
    {
        public List<PoolPack> poolPacks;
    }
}
