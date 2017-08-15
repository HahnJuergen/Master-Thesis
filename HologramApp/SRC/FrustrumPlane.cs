using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrustrumPlane : MonoBehaviour {

    public float fov = 60;
    //Camera aspect ratio
    public float aspect = 640.0f / 480.0f;

    // Use this for initialization
    void Start()
    {
        updateSize();
    }

    public void updateSize()
    {
        float distance = transform.localPosition.z;
        float frustrum_height = 2.0f * distance * Mathf.Tan(fov * 0.5f * Mathf.Deg2Rad);
        float frustrum_width = frustrum_height * aspect;

        Vector2 local_scale = transform.localScale;

        local_scale.x = frustrum_width;
        local_scale.y = frustrum_height;

        transform.localScale = local_scale;
    }
}
