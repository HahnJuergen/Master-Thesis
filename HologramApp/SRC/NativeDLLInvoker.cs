using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.InteropServices;
using de.ur.zollnergroup.juergenhahn.ma;
using System;

public class NativeDLLInvoker : Singleton<NativeDLLInvoker>
{
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public delegate void PrintDelegate(string str);

    [DllImport("ComputerVisionProcessingDLL", EntryPoint = "initialize")]
    public static extern int initialize_native(int width, int height, float targetWidth, float targetHeight);

    [DllImport("ComputerVisionProcessingDLL", EntryPoint = "process")]
    public unsafe static extern int process_native(byte * imageData, float * out_rvec, float * out_tvec, int detectionAccuracy);

    [DllImport("ComputerVisionProcessingDLL", EntryPoint = "set_debug_print")]
    public static extern int set_debug_print_native([MarshalAs(UnmanagedType.FunctionPtr)] PrintDelegate function);

    public void initializeNative(int width, int height, float targetWidth, float targetHeight)
    {
        initialize_native(width, height, targetWidth, targetHeight);
    }

    public unsafe int processNative(byte * imageData, float * out_rvec, float * out_tvec, int detectionAccuracy)
    {
        return process_native(imageData, out_rvec, out_tvec, detectionAccuracy);
    }

    public void setDebugPrintNative()
    {

        PrintDelegate printDebug = (value) =>
        {
            Debug.Log("CV_DLL: " + value);
        };

        set_debug_print_native(printDebug);
    }    
}
