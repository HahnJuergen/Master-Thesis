using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoseMemory
{
    public struct poseData
    {
        public Vector3 pos;
        public Quaternion rot;
    }

    //from this so thread: http://stackoverflow.com/questions/12933284/rodrigues-into-eulerangles-and-vice-versa
    public static poseData PoseData(float[] _tvec, float[] _rvec)
    {
        Vector3 rvec = new Vector3();
        poseData data = new poseData();

        data.pos.Set(_tvec[0], _tvec[1], _tvec[2]);

        rvec.Set(_rvec[0], _rvec[1], _rvec[2]);

        float theta = rvec.magnitude;
        rvec.Normalize();

        data.rot = Quaternion.AngleAxis(theta * Mathf.Rad2Deg, rvec);

        return data;
    }

    public static void PerformLowpassCheckPosRot(ref poseData current, poseData old, float threshPos, float threshRot)
    {
        if (old.pos == null || old.pos == null)
            return;

        float diffPos = (current.pos - old.pos).sqrMagnitude;
        float diffRot = Quaternion.Angle(current.rot, old.rot);

        if(diffPos < (threshPos * threshPos))        
            current.pos = old.pos;

        if (diffRot < threshRot)
            current.rot = old.rot;
    }
}
