using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] protected Image _imageFade = null;
    [SerializeField] protected float _fadeSpeed = 1.5f;

    [SerializeField] protected GameObject settingsPanel;
    [SerializeField] protected GameObject spotLight;

    [SerializeField] protected AudioSource menuMusic;

    //protected AudioSource _audioSource;
    //protected float _audioVolume;

    protected void Start()
    {
        Application.targetFrameRate = 120;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        _imageFade.color = new Color(0, 0, 0, 1);

        //_audioSource = GetComponent<AudioSource>();
        //_audioVolume = _audioSource.volume;

        menuMusic.Play();
        StartCoroutine(Fade(true, _fadeSpeed));
        Invoke("TurnOnSpotLight", 3.75f);
    }

    IEnumerator Fade(bool fadeIn, float speed)
    {
        Color targetColor = fadeIn ? new Color(0, 0, 0, 0) : new Color(0, 0, 0, 1);
        Color sourceColor = fadeIn ? new Color(0, 0, 0, 1) : new Color(0, 0, 0, 0);

        TurnOffSettings();

        float timer = 0;
        while (timer <= speed)
        {
            _imageFade.color = Color.Lerp(sourceColor, targetColor, timer / speed);

            //if (fadeIn)
            //{
               // _audioSource.volume = Mathf.Lerp(0, _audioVolume, timer / speed);
            //}
            //else
            //{
                //_audioSource.volume = Mathf.Lerp(_audioVolume, 0, timer / speed);
            //}

            timer += Time.deltaTime;

            yield return null;
        }

        _imageFade.color = targetColor;
    }

    public void NewGame()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        StartCoroutine(Fade(false, _fadeSpeed / 2));

        Invoke("LoadGameScene", _fadeSpeed / 2);
    }

    protected void LoadGameScene()
    {
        SceneManager.LoadScene("Main Scene");
    }

    public void QuitGame()
    {
        //Cursor.visible = false;
        //Cursor.lockState = CursorLockMode.Locked;

        StartCoroutine(Fade(false, _fadeSpeed / 2));

        Invoke("QuitGameReal", _fadeSpeed / 2);
    }

    protected void QuitGameReal()
    {
        Application.Quit();
    }

    public void TurnOffSettings()
    {
        settingsPanel.SetActive(false);
    }

    public void TurnOnSpotLight()
    {
        spotLight.SetActive(true);
    }
}
