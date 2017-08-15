using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class CVTest : MonoBehaviour
{    
    private WebCamDevice[] devices;

    private WebCamTexture webcam;

    private Texture2D output;

    private Color[] data;

    private float targetWidth;
    private float targetHeight;

    public float[] rotationAsQuaternion = new float[4];
    public float[] translation_vector = new float[3];

    public int detectionAccuracy = 1;

    public event Action onDetectionSuccess;
    public event Action onDetectionFail;

    public void Initialize(float targetWidth, float targetHeight)
    {
        this.targetWidth = targetWidth;
        this.targetHeight = targetHeight;

        NativeDLLInvoker.Instance.setDebugPrintNative();

        devices = WebCamTexture.devices;

        webcam = new WebCamTexture(devices[0].name);
        webcam.Play();

        output = new Texture2D(webcam.width, webcam.height, TextureFormat.RGB24, false);

        this.transform.GetChild(0).gameObject.GetComponent<Renderer>().material.mainTexture = output;

        data = new Color[webcam.width * webcam.height];

        NativeDLLInvoker.Instance.initializeNative(webcam.width, webcam.height, targetWidth, targetHeight);

        StartCoroutine(HandleWebCamCapture());
    }

    private IEnumerator HandleWebCamCapture()
    {
        for (;;)
        {
            yield return null;

            ProcessWebCamImage();
        }
    }

    private unsafe void ProcessWebCamImage()
    {
        if (data != null)
        {            
            data = webcam.GetPixels();

            output.SetPixels(data);

            byte[] raw = output.GetRawTextureData();  

            handleCVProcessing(raw);   
        }
    }

    private unsafe void handleCVProcessing(byte[] raw)
    {
        fixed (byte* pRaw = raw) { fixed (float* out_rvec = rotationAsQuaternion) { fixed (float* out_tvec = translation_vector)
        {
            switch(NativeDLLInvoker.Instance.processNative(pRaw, out_rvec, out_tvec, detectionAccuracy))
            {
                case 0: onDetectionSuccess.Invoke(); break;
                case 1: onDetectionFail.Invoke(); break;
                case 2: NativeDLLInvoker.Instance.initializeNative(webcam.width, webcam.height, targetWidth, targetHeight); return;
            }
        }}}

        output.LoadRawTextureData(raw);
        output.Apply();
    }
}

