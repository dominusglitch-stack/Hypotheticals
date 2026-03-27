using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Credits : MonoBehaviour
{
    public RectTransform creditsContainer;
    public GameObject creditOn;
    public float finalScrollPos;
    public float creditsDuration;

    private Vector3 initialScrollPos;
    public bool creditsStarted;
    public bool creditsPaused = true;

    private void Start()
    {
        initialScrollPos = creditsContainer.localPosition;
    }

    public void PlayCredits(int whoPressed)
    {
        if (whoPressed == 0)
        {
            if (!creditsStarted)
            {
                StartCoroutine(CreditRoll());
                creditsStarted = true;
            }

            creditsPaused = !creditsPaused;

            if (creditsPaused)
            {
                creditOn.SetActive(false);
            }
            else
            {
                creditOn.SetActive(true);
            }
        }
        else
        {
            creditsPaused = true;

            if (creditsPaused)
            {
                creditOn.SetActive(false);
            }
            else
            {
                creditOn.SetActive(true);
            }
        }

        
    }

    IEnumerator CreditRoll()
    {
        float timer = 0;

        while (timer < creditsDuration)
        {
            if (!creditsPaused)
            {
                Vector3 containerPos = initialScrollPos;

                containerPos.y = Mathf.Lerp(initialScrollPos.y, finalScrollPos, timer / creditsDuration);

                creditsContainer.localPosition = containerPos;

                timer += Time.deltaTime;
            }

            yield return null;
        }

        creditsStarted = false;
    }
}
