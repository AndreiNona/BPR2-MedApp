using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class LectureManager : MonoBehaviour
{
    public List<LecturePrompt> prompts;
    public int currentIndex = 0;

    public TMP_Text mainTextUI;
    public TMP_Text subTextUI;
    public Image imageUI;
    public VideoPlayer videoPlayerUI;

    private void Start()
    {
        UpdateUI();
    }

    public void NextPrompt()
    {
        if (currentIndex < prompts.Count - 1)
        {
            currentIndex++;
            UpdateUI();
        }
    }

    public void PreviousPrompt()
    {
        if (currentIndex > 0)
        {
            currentIndex--;
            UpdateUI();
        }
    }

    void UpdateUI()
    {
        var currentPrompt = prompts[currentIndex];

        // Update main text
        mainTextUI.text = currentPrompt.mainText ?? "";
        mainTextUI.gameObject.SetActive(!string.IsNullOrEmpty(currentPrompt.mainText));

        // Update subtext
        subTextUI.text = currentPrompt.subText ?? "";
        subTextUI.gameObject.SetActive(!string.IsNullOrEmpty(currentPrompt.subText));

        // Update image
        if (currentPrompt.image != null)
        {
            imageUI.sprite = currentPrompt.image; imageUI.gameObject.SetActive(true);
        }
        else
            imageUI.gameObject.SetActive(false);

        // Update video
        if (currentPrompt.videoClip != null)
        {
            videoPlayerUI.clip = currentPrompt.videoClip; videoPlayerUI.gameObject.SetActive(true);
            videoPlayerUI.Play();
        }
        else
            videoPlayerUI.gameObject.SetActive(false);
    }
}
