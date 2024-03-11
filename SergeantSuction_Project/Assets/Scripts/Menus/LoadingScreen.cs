using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewBehaviourScript : MonoBehaviour
{
    [SerializeField]
    private Slider loadingSlider;

    public void OnEnable()
    {
        loadingSlider.value = 0f;
    }

    public void UpdateSlider(float progress)
    {
        float progressPerc = Mathf.Clamp01(progress / .9f);
        loadingSlider.value = progressPerc;
    }
}
