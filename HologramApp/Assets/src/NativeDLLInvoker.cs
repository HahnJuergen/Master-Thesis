using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.InteropServices;

public class NativeDLLInvoker : MonoBehaviour
{
    [DllImport("TestCPPLibraryDLL", EntryPoint = "test_divide_f")]
    public static extern float StraightFromDLLF(float a, float b);

    [DllImport("TestCPPLibraryDLL", EntryPoint = "test_divide_i")]
    public static extern int StraightFromDLLI(int a, int b);

    void Start ()
    {
        ShowDLLMessage();
	}

    void ShowDLLMessage()
    {
        float nDivResult = StraightFromDLLF(2, 5);
        int nDivResultI = StraightFromDLLI(20, 5);
    }
}
