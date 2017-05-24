using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace de.ur.juergenhahn.ma
{
    public class Utility : Singleton<Utility>, IMath, IDebug, IMesh
    {
        protected Utility() { }

        public static IMath Math
        {
            get
            {
                return Instance;
            }
        }

        public static IDebug Debug
        {
            get
            {
                return Instance;
            }
        }

        public static IMesh Mesh
        {
            get
            {
                return Instance;
            }
        }

        bool IMath.IsEven(int x)
        {
            return ((x & 1) == 0);
        }

        bool IMath.IsEven(long x)
        {
            return ((x & 1L) == 0L);
        }

        float IMath.CalculateLengthXOfParentFromWorldCoordinatesFromSortedTransform(Transform t, float offset)
        {
            if (t.name == "MachineLine")
            {
                return CalculateMachineLineLengthFromWorldCoordinatesXFromSortedTransform(t, offset);
            }

            throw new InvalidOperationException("Transform does not have Components: MachineHandler");
        }

        Vector3[] IMath.ToAABBPoints(GameObject go)
        {
            Vector3 p1 = go.GetComponent<Renderer>().bounds.min;
            Vector3 p2 = go.GetComponent<Renderer>().bounds.max;
            Vector3 p3 = new Vector3(p1.x, p1.y, p2.z);
            Vector3 p4 = new Vector3(p1.x, p2.y, p1.z);
            Vector3 p5 = new Vector3(p2.x, p1.y, p1.z);
            Vector3 p6 = new Vector3(p1.x, p2.y, p2.z);
            Vector3 p7 = new Vector3(p2.x, p1.y, p2.z);
            Vector3 p8 = new Vector3(p2.x, p2.y, p1.z);

            return new Vector3[]
            {
                p1, p2, p3, p4, p5, p6, p7, p8
            };
        }        

        void IDebug.DebugVisualizeAABB(Vector3[] AABB, Color color)
        {                
            UnityEngine.Debug.DrawLine(AABB[5], AABB[1], color, 2, false);            
            UnityEngine.Debug.DrawLine(AABB[1], AABB[7], color, 2, false);            
            UnityEngine.Debug.DrawLine(AABB[7], AABB[3], color, 2, false);            
            UnityEngine.Debug.DrawLine(AABB[3], AABB[5], color, 2, false);            
            UnityEngine.Debug.DrawLine(AABB[2], AABB[6], color, 2, false);            
            UnityEngine.Debug.DrawLine(AABB[6], AABB[4], color, 2, false);            
            UnityEngine.Debug.DrawLine(AABB[4], AABB[0], color, 2, false);
            UnityEngine.Debug.DrawLine(AABB[0], AABB[2], color, 2, false);            
            UnityEngine.Debug.DrawLine(AABB[5], AABB[2], color, 2, false);
            UnityEngine.Debug.DrawLine(AABB[1], AABB[6], color, 2, false);            
            UnityEngine.Debug.DrawLine(AABB[7], AABB[4], color, 2, false);
            UnityEngine.Debug.DrawLine(AABB[3], AABB[0], color, 2, false);                  
        }

        Mesh IMesh.GenerateMesh(MeshType mt, float width, float height)
        {
            if (mt == MeshType.Plane)
                return GeneratePlaneMesh(width, height, name);

            throw new ArgumentException(Constants.ExceptionConstants.UNSUPPORTED_MESHTYPE);            
        }

        GameObject IMesh.MeshToGameObject(Mesh m, MeshType mt, string shaderName, Color color, string layerName, string name, Transform parent)
        {
            if (mt == MeshType.Plane)
                return GameObjectFromPlaneMesh(m, shaderName, color, layerName, name, parent);            

            throw new ArgumentException(Constants.ExceptionConstants.UNSUPPORTED_MESHTYPE);
        }

        private GameObject GameObjectFromPlaneMesh(Mesh m, string shaderName, Color color, string layerName, string name, Transform parent)
        {           
            GameObject go = new GameObject(name);

            MeshFilter mf = (MeshFilter)go.AddComponent(typeof(MeshFilter));
            mf.mesh = m;

            MeshRenderer mr = go.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
            mr.material.shader = Shader.Find(shaderName);

            if(Constants.UIConstants.SHADER_STANDARD == shaderName)
                mr.material.SetColor(Constants.UIConstants.SHADER_COLOR_IDENTIFIER, color);    
            else if(shaderName == "Custom/Sillhouette")
            {
                mr.material.SetColor("_OutlineColor", new Color(0, 255, 255));
                mr.material.SetFloat("_Outline", 0.005f);
            }

            go.transform.Rotate(Constants.UIConstants.MACHINE_CONTRAST_PLANE_DEFAULT_ROTATION);
            go.layer = LayerMask.NameToLayer(layerName);
            go.GetComponent<Renderer>().receiveShadows = false;
            go.GetComponent<Renderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

            go.transform.position = Constants.UIConstants.MACHINE_CONTRAST_PLANE_DEFAULT_POSITION;

            go.transform.parent = parent;

            return go;
        }

        private Mesh GeneratePlaneMesh(float width, float height, string name)
        {
            Mesh m = new Mesh();

            m.name = name;

            m.vertices = new Vector3[]
            {
                new Vector3(-width / 2.0f, -height, 1.01f),
                new Vector3(width / 2.0f, -height, 1.01f),
                new Vector3(width / 2.0f, height, 1.01f),
                new Vector3(-width / 2.0f, height, 1.01f)
            };

            m.uv = new Vector2[]
            {
                new Vector2(0, 0),
                new Vector2(0, 1),
                new Vector2(1, 1),
                new Vector2(1, 0)
            };

            m.triangles = new int[] { 0, 1, 2, 0, 2, 3 };

            m.RecalculateNormals();

            return m;
        }

        private float CalculateMachineLineLengthFromWorldCoordinatesXFromSortedTransform(Transform tr, float offset)
        {
            float length = 0.0f;

            foreach (Transform t in tr)
                length += t.GetComponent<Machine>().LowerFrontLineLengthX;

            length += (offset * (tr.childCount - 1)) - tr.GetChild(0).GetComponent<Machine>().LowerFrontLineLengthX;

            return length;
        }
    }

    public static class HasComponentChecker
    {
        public static bool HasComponent<T>(this GameObject go) where T : Component
        {
            return (go.GetComponent<T>() != null);
        }
    }   
}
