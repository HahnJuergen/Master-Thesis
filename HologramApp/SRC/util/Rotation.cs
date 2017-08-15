using System.Collections;
using System.Collections.Generic;
using UnityEngine;

{
    public class Rotation : MonoBehaviour
    {
        public enum axes
        {
            X, Y, Z
        }

        public axes m_Axes;

        private Vector3 m_Direction = new Vector3(0, 0, 0);
        private Coroutine RotationCoroutine;

        private IEnumerator RotationHandling()
        {
            for(;;)
            {
                transform.Rotate(m_Direction, Time.deltaTime * 50);

                yield return null;
            }
        }

        private void OnEnable()
        {
            switch (m_Axes)
            {
                case axes.X: m_Direction = Vector3.up; break;
                case axes.Y: m_Direction = Vector3.forward; break;
                case axes.Z: m_Direction = Vector3.right; break;
            }

            RotationCoroutine = StartCoroutine(RotationHandling());
        }

        private void OnDisable()
        {
            StopCoroutine(RotationCoroutine);
        }   
    }
}
