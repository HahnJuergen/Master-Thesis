using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageTargetController : MonoBehaviour
{
    private CVTest detector;
    private Quaternion baseRotation;

    public GameObject markerObject;

    private GameObject quad;

    void Start ()
    {
        baseRotation = this.markerObject.transform.localRotation;

        detector = Camera.main.GetComponent<CVTest>();

        detector.onDetectionSuccess += OnDetectionSuccess;
        detector.onDetectionFail += OnDetectionFail;

        quad = GameObject.Instantiate(markerObject);

        Vector3 v = quad.transform.GetChild(0).GetComponent<Renderer>().bounds.size;

        detector.Initialize(v.x, v.y);
    }

    private void OnDetectionSuccess()
    {
        if(quad == null)
            quad = GameObject.Instantiate(markerObject, new Vector3(), Quaternion.Euler(0, 0 ,0), Camera.main.transform);

        quad.transform.parent = Camera.main.transform;

        Quaternion q = new Quaternion(detector.rotationAsQuaternion[0], detector.rotationAsQuaternion[1], detector.rotationAsQuaternion[2], detector.rotationAsQuaternion[3]);
        Vector3 position = new Vector3(detector.translation_vector[0], detector.translation_vector[1], -detector.translation_vector[2]);

        quad.transform.position = Camera.main.cameraToWorldMatrix.MultiplyPoint(position);
        quad.transform.rotation = Camera.main.transform.rotation * q * baseRotation;
    }

    private void OnDetectionFail()
    {
        if(quad != null)
        {
            GameObject.Destroy(quad);
            quad = null;
        }
    }    
}
