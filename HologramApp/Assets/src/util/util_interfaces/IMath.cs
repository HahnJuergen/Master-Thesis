﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace de.ur.zollnergroup.juergenhahn.ma
{
    public interface IMath
    {
        bool IsEven(int x);
        bool IsEven(long x);

        float CalculateLengthXOfParentFromWorldCoordinatesFromSortedTransform(Transform t, float offset);

        Vector3[] ToAABBPoints(GameObject go); // axis aligned bounding box
    }
}