using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utilities
{
    [CreateAssetMenu(fileName = "NewPoolPack", menuName = "ScriptableObjects/PoolPack", order = 5)]
    public class PoolPack : ScriptableObject
    {
        public List<Pool> pools;
    }
}
