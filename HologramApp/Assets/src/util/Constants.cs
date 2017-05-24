using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace de.ur.juergenhahn.ma
{
    public static class Constants
    {
        public static class UIConstants
        {
            public static float EDGE_OFFSET = 0.25f; //meters

            public static float DEFAULT_X_POSITION_MACHINE = 0.0f; //meters
            public static float DEFAULT_Y_POSITION_MACHINE = 0.85f; //meters
            public static float DEFAULT_Z_POSITION_MACHINE = 2.0f; //meters
            public static Vector3 MACHINE_DEFAULT_POSITION 
                = new Vector3(
                    DEFAULT_X_POSITION_MACHINE, 
                    DEFAULT_Y_POSITION_MACHINE, 
                    DEFAULT_Z_POSITION_MACHINE);

            public static float DEFAULT_X_POSITION_MACHINE_CONTRAST_PLANE = 0.0f; //meters
            public static float DEFAULT_Y_POSITION_MACHINE_CONTRAST_PLANE = DEFAULT_Y_POSITION_MACHINE; //meters
            public static float DEFAULT_Z_POSITION_MACHINE_CONTRAST_PLANE = 3.2f; //meters
            public static Vector3 MACHINE_CONTRAST_PLANE_DEFAULT_POSITION
                = new Vector3(
                    DEFAULT_X_POSITION_MACHINE_CONTRAST_PLANE,
                    DEFAULT_Y_POSITION_MACHINE_CONTRAST_PLANE,
                    DEFAULT_Z_POSITION_MACHINE_CONTRAST_PLANE);

            public static float DEFAULT_X_ROTATION_MACHINE_CONTRAST_PLANE = 0.0f; //meters
            public static float DEFAULT_Y_ROTATION_MACHINE_CONTRAST_PLANE = 180.0f; //meters
            public static float DEFAULT_Z_ROTATION_MACHINE_CONTRAST_PLANE = 0.0f; //meters
            public static Vector3 MACHINE_CONTRAST_PLANE_DEFAULT_ROTATION
                = new Vector3(
                    DEFAULT_X_ROTATION_MACHINE_CONTRAST_PLANE,
                    DEFAULT_Y_ROTATION_MACHINE_CONTRAST_PLANE,
                    DEFAULT_Z_ROTATION_MACHINE_CONTRAST_PLANE);


            public static float RESET_MACHINE_SELECTION_ROTATION_MODIFIER = 15.0f;

            public static string TAG_MACHINE = "Machine";
            public static float TAG_MACHINE_DEFAULT_X_ROTATION = -90.0f;
            public static float TAG_MACHINE_DEFAULT_Y_ROTATION = 90.0f;
            public static float TAG_MACHINE_DEFAULT_Z_ROTATION = 0.0f;
            public static Vector3 TAG_MACHINE_DEFAULT_ROTATION 
                = new Vector3(
                    TAG_MACHINE_DEFAULT_X_ROTATION, 
                    TAG_MACHINE_DEFAULT_Y_ROTATION, 
                    TAG_MACHINE_DEFAULT_Z_ROTATION);

            public static float TAG_MACHINE_DEFAULT_X_SCALE = 0.01f;
            public static float TAG_MACHINE_DEFAULT_Y_SCALE = 0.01f;
            public static float TAG_MACHINE_DEFAULT_Z_SCALE = 0.01f;
            public static Vector3 TAG_MACHINE_DEFAULT_SCALE 
                = new Vector3(
                    TAG_MACHINE_DEFAULT_X_SCALE, 
                    TAG_MACHINE_DEFAULT_Y_SCALE, 
                    TAG_MACHINE_DEFAULT_Z_SCALE);

            public static int RESET_MACHINE_INDEX = -1;

            public static string SHADER_STANDARD = "Standard";
            public static string LAYER_UI = "UI";
            public static string LAYER_DEFAULT = "Default";
            public static string CONTRAST_PLANE_NAME = "ContrastPlane";
            public static string SHADER_COLOR_IDENTIFIER = "_Color";

            public static Color COLOR_CONTRAST_PLANE = new Color(0.0f, 0.0f, 0.0f, 0.4f);
        }

        public static class ExceptionConstants
        {
            public static string UNSUPPORTED_MESHTYPE = "Desired MeshType not yet supported";
        }
    }
}

