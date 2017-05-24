using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class FillingLevelBar : MonoBehaviour 
{
    public float maxFillingLevel;
    public float fillingLevel;
    public Image currentFillingLevelBar;
    public Text ratioText;

    private bool isInitialized = false;
    private int id;

    public void Initialze(float fillingLevel, int id)
    {
        if(!isInitialized)
        {
            this.fillingLevel = fillingLevel;
            this.maxFillingLevel = fillingLevel;
            this.id = id;

            StartCoroutine(KeepCanvasPosition());

            this.isInitialized = true;
        }        
    }

    public void Deinitialize()
    {
        this.isInitialized = false;
    }

    private void UpdateFillingLevelBar()
    {
        if(isInitialized)
        {
            float ratio = this.fillingLevel / this.maxFillingLevel;

            this.currentFillingLevelBar.rectTransform.localScale = new Vector3(1.0f, ratio, 1.0f);
            this.ratioText.text = (ratio * 100.0f).ToString("0") + '%';
        }        
    }

    private IEnumerator KeepCanvasPosition()
    {
        for(;;)
        {
            yield return null;
        }
    }

    public void Fill(float amount)
    {
        if (this.isInitialized)
        {
            this.maxFillingLevel = fillingLevel + amount;
            this.fillingLevel = this.maxFillingLevel;

            UpdateFillingLevelBar();
        }
    }

    public void Deplete(float amount)
    {
        if (this.isInitialized)
        {
            this.fillingLevel = (this.fillingLevel - amount > 0.0f) ? this.fillingLevel - amount : 0.0f;

            UpdateFillingLevelBar();
        }
    }
}
