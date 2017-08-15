using System.Collections;
using System.Collections.Generic;
using UnityEngine;

{
    public interface IDebug
    {
        void DebugVisualizeAABB(Vector3[] aabb, Color color);
    }
}
