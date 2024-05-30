using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SliderController : MonoBehaviour
{
    public Slider slider;
    public TMP_Text displayText;
    [SerializeField] 
    [Tooltip("Color for lowest slider value")]
    private Color lowValueColor = Color.blue; 
    [SerializeField] 
    [Tooltip("Color for highest slider value")]
    private Color highValueColor = Color.red;  
    [SerializeField] 
    [Tooltip("Animation time for slider value changes")]
    private float animationTime = 1.0f;  
    
    private int previousTextValue;
   
    private void Start()
    {
        if (slider == null)
        {
            slider = GetComponent<Slider>();
        }
        
        slider.value = 0;
        displayText.text = "0";
        UpdateSliderColor(slider.value);
    }
    
    public void AnimateSlider(float targetSliderPercentage, int targetTextValue)
    {
        StartCoroutine(AnimateSliderCoroutine(targetSliderPercentage, targetTextValue));
    }


    private IEnumerator AnimateSliderCoroutine(float targetSliderPercentage, int targetTextValue)
    {
        float timeElapsed = 0;
        float startValue = slider.value;
        int startTextValue = int.Parse(displayText.text);
        float targetSliderValue = targetSliderPercentage * (slider.maxValue - slider.minValue) + slider.minValue;

        while (timeElapsed < animationTime)
        {
            slider.value = Mathf.Lerp(startValue, targetSliderValue, timeElapsed / animationTime);
            UpdateSliderColor(slider.value);
            
            int newTextValue = (int)Mathf.Lerp(startTextValue, targetTextValue, timeElapsed / animationTime);
            displayText.text = newTextValue.ToString();

            timeElapsed += Time.deltaTime;
            yield return null;
        }

        slider.value = targetSliderValue;
        displayText.text = targetTextValue.ToString();
        UpdateSliderColor(slider.value);
    }
    
    private void UpdateSliderColor(float value)
    {
        float percentage = (value - slider.minValue) / (slider.maxValue - slider.minValue);
        slider.fillRect.GetComponent<Image>().color = Color.Lerp(lowValueColor, highValueColor, percentage);
    }
}
