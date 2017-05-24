using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace de.ur.zollnergroup.juergenhahn.ma
{
    public interface IDebug
    {
        void DebugVisualizeAABB(Vector3[] aabb, Color color);
    }
}
