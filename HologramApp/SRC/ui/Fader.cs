using System.Collections;
using System.Collections.Generic;
using UnityEngine;

{
    public class Fader : MonoBehaviour
    {
        public enum FadeMode
        {
            IN,
            OUT
        }

        public static IEnumerator Fade(GameObject go, FadeMode fm, float startAlpha, float targetAlpha, float duration)
        {

            Color c = go.GetComponent<Renderer>().material.color;
            c.a = startAlpha;
            go.GetComponent<Renderer>().material.color = c;

            float time = Time.deltaTime;
            float alpha = startAlpha;

            bool fadeCondition;

            do
            {
                float lerp = time / duration;

                alpha = Mathf.Lerp(startAlpha, targetAlpha, lerp);

                c.a = alpha;

                time += Time.deltaTime;

                go.GetComponent<Renderer>().material.color = c;

                if (fm == FadeMode.IN)
                {
                    fadeCondition = alpha < targetAlpha;
                    go.SetActive(true);
                }
                else
                {
                    fadeCondition = alpha > targetAlpha;
                }

                yield return null;
            }
            while (fadeCondition);

            if (fm == FadeMode.OUT)
                go.SetActive(false);
        }
    }
}