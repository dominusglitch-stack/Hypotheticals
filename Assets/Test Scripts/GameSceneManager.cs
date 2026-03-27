using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using TMPro;
using System;
using UnityEngine.Rendering.PostProcessing;

public class GameSceneManager : MonoBehaviour
{
    static GameSceneManager _instance = null;
    static public GameSceneManager instance
    {
        get
        {
            if (_instance == null)
                _instance = FindFirstObjectByType<GameSceneManager>();

            return _instance;
        }
    }

    public List<LineRenderer> redStrings;
    public List<Transform> stringLocations;
    public List<int> connectionCount;
    public List<GameObject> redStringObjs;
    public GameObject redStringPrefab;
    public float timer;
    public float bestTimer;
    public bool activeTimer;
    public TextMeshProUGUI bestTime;
    public TextMeshProUGUI caseSeed;
    public TextMeshProUGUI casesCompleted;

    public PostProcessVolume noirEffectMode;
    public ColorGrading noirEffect;

    public AudioSource music;

    [Header("Rope")]
    public int segments = 12;
    public float droop = 0.6f;
    public float maxSagPerMeter = 0.25f;
    public float ropeClearance = 0.04f;
    public Material redStringMat;

    public void Start()
    {
        timer = 0;
        StopTime();

        if(PlayerPrefs.HasKey("bestTime"))
        {
            bestTimer = PlayerPrefs.GetFloat("bestTime");
            //Debug.Log("Best time is " + PlayerPrefs.GetFloat("bestTime"));
        }
        else
        {
            bestTimer = 0;
            PlayerPrefs.SetFloat("bestTime", 0);
        }

        noirEffectMode.profile.TryGetSettings(out noirEffect);

        NoirMode(SettingsScript.instance.noirMode);

        SetBestTimeText();
    }

    public void Update()
    {
        TrackTime();
    }

    private void TrackTime()
    {
        if(activeTimer)
        {
            timer = timer + Time.deltaTime;
        }

        TimeSpan rtime = TimeSpan.FromSeconds(timer);

        if(bestTimer == 0)
        {
            //Debug.Log("bestTimer = " + bestTimer);
            bestTime.text = "Best Time: " + "\n" + rtime.Minutes.ToString() + ":" + rtime.Seconds.ToString() + ":" + rtime.Milliseconds.ToString();
        }
    }

    public void StartTime()
    {
        activeTimer = true;
    }

    public void StopTime()
    {
        activeTimer = false;
    }

    public void SetCaseSeed(int seed)
    {
        if(seed == 357764661)
        {
            caseSeed.text = "Case Seed:\nTutorial";
        }
        else
        {
            caseSeed.text = "Case Seed: " + "\n" + seed;
        }
    }

    public void SetCasesCompleted(int cases)
    {
        casesCompleted.text = "Cases Completed: " + "\n" + cases;
    }

    public void SetBestTimeText()
    {
        if (bestTimer != 0)
        {
            //Debug.Log("bestTimer = " + bestTimer);

            TimeSpan time = TimeSpan.FromSeconds(bestTimer);

            bestTime.text = "Best Time: " + "\n" + time.Minutes.ToString() + ":" + time.Seconds.ToString() + ":" + time.Milliseconds.ToString();
        }
    }

    public void NoirMode(bool active)
    {
        if (active)
        {
            noirEffect.saturation.value = -100;
        }
        else
        {
            noirEffect.saturation.value = 1;
        }
    }

    public void DrawRope(Transform other)
    {
        int check = stringLocations.IndexOf(other);

        if (check == -1)
        {
            //Debug.Log("String index is " + check);
        }
        else
        {
            int range = connectionCount[check];

            //Debug.Log("String range is " + range);

            int startString = 0;
            for (int i = 0; i < check; i++)
            {
                startString += connectionCount[i];
            }

            //Debug.Log("Startstring is " + startString);

            int trueRange = startString + range;

            int counter = startString;

            foreach (Transform t in stringLocations)
            {
                if (t.position != other.position)
                {
                    Vector3 a = other.position;
                    Vector3 b = t.position;

                    if (redStrings[counter].positionCount != segments + 1)
                        redStrings[counter].positionCount = segments + 1;

                    float dist = Vector3.Distance(a, b);
                    float effectiveDroop = Mathf.Min(droop, Mathf.Max(-2f, maxSagPerMeter * dist));

                    for (int i = 0; i <= segments; i++)
                    {
                        float g = i / (float)segments;

                        Vector3 p = Vector3.Lerp(a, b, g);

                        float sag = Mathf.Sin(g * Mathf.PI) * effectiveDroop;
                        p.y -= sag;

                        float minY = 0f + ropeClearance;
                        if (p.y < minY) p.y = minY;

                        redStrings[counter].SetPosition(i, p);
                    }

                    if (counter < trueRange)
                    {
                        counter++;
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }

        

        //redString.SetPosition(0, weaponNotes.position);
        //redString.SetPosition(1, other.gameObject.transform.position);
    }

    public void ManageStrings(Transform stringPos)
    {
        int counter = 0;

        foreach (Transform t in stringLocations)
        {
            if (t == stringPos)
            {
                connectionCount[counter] = stringLocations.Count - 1;
                //Debug.Log("connectionCount = " + connectionCount[counter]);
            }

            counter++;
        }

        int connectionLimit = 0;

        foreach(int i in connectionCount)
        {
            connectionLimit += i;
        }

        if (redStrings.Count < connectionLimit)
        {
            for (int i = redStrings.Count; i < connectionLimit; i++)
            {
                redStringObjs.Add(Instantiate(redStringPrefab));
                redStrings.Add(redStringObjs[i].GetComponent<LineRenderer>());
                redStrings[i].material = redStringMat;
                redStrings[i].startWidth = 0.01f;
            }

            //Debug.Log(redStrings.Count);
        }
        else if (redStrings.Count > connectionLimit)
        {
            //Debug.Log("Money");
            for (int i = redStrings.Count - 1; i >= connectionLimit; i--)
            {
                //Debug.Log("Swag");
                LineRenderer lineToRemove = redStrings[i];
                redStrings.Remove(lineToRemove);
                GameObject objToDestroy = redStringObjs[i];
                redStringObjs.Remove(objToDestroy);
                Destroy(objToDestroy);
            }
        }
    }
}
