using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingAverage
{
    private PoseMemory.poseData[] previousStates;

    private int numTrackedStates;
    private int indexNextState = 0;

    public MovingAverage(int _numTrackedStates)
    {
        numTrackedStates = _numTrackedStates;

        previousStates = new PoseMemory.poseData[numTrackedStates];
    }

    public void calculateCurrentStateAverage(ref PoseMemory.poseData current)
    {
        Vector3 posTotal = new Vector3(0.0f, 0.0f, 0.0f);
        int numStates = 0;
        previousStates[indexNextState] = current;

        int i = indexNextState;

        do
        {
            numStates++;
            posTotal += previousStates[i].pos;

            i = CustomModulo(i - 1, numTrackedStates);
        }
        while (i != indexNextState);

        posTotal /= numStates;

        current.pos = posTotal;

        int previousIndex = CustomModulo(indexNextState - 1, numTrackedStates);

        current.rot = Quaternion.Slerp(current.rot, previousStates[i].rot, 0.5f);

        indexNextState = (indexNextState + 1) % numTrackedStates;
    }

    public void Reset()
    {
        previousStates = new PoseMemory.poseData[numTrackedStates];
        indexNextState = 0;
    }

    private int CustomModulo(int x, int m)
    {
        return (x % m < 0) ? (x % m + m) : (x % m);
    }
}
