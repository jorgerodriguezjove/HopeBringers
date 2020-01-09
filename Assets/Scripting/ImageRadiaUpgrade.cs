using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageRadiaUpgrade : MonoBehaviour
{
    float lerpTime = 1f;
    float currentLerpTime;
    float perc;

    Image image;

    bool doAnim;

    public void SetImageFill(bool _shouldDoAnimation)
    {
        image = GetComponent<Image>();
        doAnim = _shouldDoAnimation;

        if (_shouldDoAnimation)
        {
            image.fillAmount = 0f;
        }
        else
        {
            image.fillAmount = 1f;
        }
    }

    private void Update()
    {
        if (doAnim)
        {
            currentLerpTime += Time.deltaTime;
            if (currentLerpTime > lerpTime)
            {
                currentLerpTime = lerpTime;
            }

            perc = currentLerpTime / lerpTime;

            image.fillAmount = Mathf.Lerp(0, 1, perc);
        }
    }
}
