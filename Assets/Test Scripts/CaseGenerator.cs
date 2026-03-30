using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Unity.Cinemachine;
using TMPro;

public class CaseGenerator : MonoBehaviour
{
    public int seed;
    public int amountOfCases;

    public enum personSex {Male, Female, Unknown};
    public personSex victimSex;

    public float height;
    public int weight;
    public int bmi;
    public int age;

    public enum locationType {Greenhouse, Office, Workshop, None};
    public locationType victimPlace;

    public enum woundType {Trauma, Puncture, Laceration}
    public woundType victimWound;

    public enum woundSeverity {Shallow, Partial, Deep, None}
    public woundSeverity victimSeverity;

    public int[] witnesses;
    //public int witnessLength;
    //public int witnessSight;
    //public int witnessSight2;
    //public int wSawOne;
    //public int w2SawOne;

    public enum wSizeTypes {Small, Medium, Large, None};
    public wSizeTypes[] weaponSizeTypes;

    public enum wColorTypes {Red, Orange, Yellow, Green, Blue, Purple, Pink, Brown, White, Grey, Black, None};
    public wColorTypes[] weaponColorTypes;

    public GameObject caseBox;
    public BoardTrigger[] boardTriggers;

    public bool cSize = false;
    public bool cDamage = false;
    public bool[] cColor = null;
    public bool cColorSingle = false;
    public bool cLocation = false;
    public bool cWitness = false;

    public TextMeshProUGUI caseText;
    public string woundText;
    public string witnessText;
    public string witnessSawText = "";
    public string witnessWallText = "None";
    public string weaponText;
    public TextMeshProUGUI[] boardTexts;
    public TextMeshProUGUI wallTextCaption; 
    public GameObject[] winOrLoseText;

    public bool caseSolvedStarted = false;
    public bool caseFailedStarted = false;

    public GameObject leaveCheck;

    public int[,] generatedCases;
    public int caseNumber = -1;

    public InputActionReference previousAction;
    public InputActionReference forwardAction;
    public InputActionReference menuAction;
    public GameObject menuScreen;
    public GameObject settingsScreen;
    public CinemachineInputAxisController playerCamera;
    public Example playerMovement;
    public PickUpTest playerPickup;

    public int pageNumber = 0;

    public GameObject[] witnessPrefabs;
    public GameObject locationPrefab;
    public Transform spawner;

    public GameObject[] weaponsToCheck;
    public GameObject[] locationsToCheck;
    public GameObject[] witnessesToCheck;
    public List<GameObject> updateWitnesses = new List<GameObject>();
    public List<GameObject> allItemsToCheck = new List<GameObject>();
    public List<int>[] solutionNumbers;
    public List<bool>[] solutionActivity;
    public bool hasLost = false;
    public bool hasWon = false;
    public int hasLostIndex;
    public bool hasBeatenThisSeed = false;
    public GameObject loadingPanel;

    public GameObject[] lights;
    public AudioSource[] allAmbientAudio;
    public AudioSource universalSound;
    public AudioClip[] universalSounds;

    public GameObject[] evidenceBoxs;
    public Material evidenceBoxMat;
    public Material evidenceBoxLose;

    public GameObject[] tutorialBoxes;

    public Radio radio;

    private void OnEnable()
    {
        previousAction.action.Enable();
        forwardAction.action.Enable();
        menuAction.action.Enable();
    }

    private void OnDisable()
    {
        previousAction.action.Disable();
        forwardAction.action.Disable();
        menuAction.action.Disable();
    }

    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        menuScreen.SetActive(false);
        settingsScreen.SetActive(false);

        generatedCases = new int[amountOfCases, 12];
        locationsToCheck = new GameObject[amountOfCases];
        witnessesToCheck = new GameObject[amountOfCases];
        solutionNumbers = new List<int>[amountOfCases];
        solutionActivity = new List<bool>[amountOfCases];

        for (int i = 0; i < amountOfCases; i++)
        {
            solutionNumbers[i] = new List<int>();
            solutionActivity[i] = new List<bool>();
            //Debug.Log(solutionNumbers[i] + "exists");
        }

        if(SettingsScript.instance.tutorialMode)
        {
            foreach(GameObject box in tutorialBoxes)
            {
                box.SetActive(true);
            }
        }

        if (PlayerPrefs.HasKey("hasBeatenThisSeed"))
        {
            if (PlayerPrefs.GetInt("hasBeatenThisSeed") == 0)
            {
                hasBeatenThisSeed = false;
            }
            else if (PlayerPrefs.GetInt("hasBeatenThisSeed") == 1)
            {
                hasBeatenThisSeed = true;
            }

            if(SettingsScript.instance.tutorialMode)
            {
                PlayerPrefs.SetInt("caseSeed", 348486610);
                seed = 348486610;
                PlayerPrefs.SetInt("hasBeatenThisSeed", 0);
                Random.InitState(seed);
            }
            else
            {
                if (hasBeatenThisSeed)
                {
                    seed = (int)Random.Range(int.MinValue, int.MaxValue);
                    Random.InitState(seed);
                    PlayerPrefs.SetInt("caseSeed", seed);
                    PlayerPrefs.SetInt("hasBeatenThisSeed", 0);
                }
                else
                {
                    seed = PlayerPrefs.GetInt("caseSeed");
                    Random.InitState(seed);
                }
            }
        }
        else
        {
            if (SettingsScript.instance.tutorialMode)
            {
                PlayerPrefs.SetInt("caseSeed", 348486610);
                seed = 348486610;
                PlayerPrefs.SetInt("hasBeatenThisSeed", 0);
                Random.InitState(seed);
            }
            else
            {
                seed = (int)Random.Range(int.MinValue, int.MaxValue);
                Random.InitState(seed);
                PlayerPrefs.SetInt("caseSeed", seed);
                PlayerPrefs.SetInt("hasBeatenThisSeed", 0);
            }
            
        }

        SettingsScript.instance.LoadSeed();

        GameSceneManager.instance.SetCaseSeed(seed);

        if (PlayerPrefs.HasKey("casesCompleted"))
        {
            GameSceneManager.instance.SetCasesCompleted(PlayerPrefs.GetInt("casesCompleted"));
        }
        else
        {
            PlayerPrefs.SetInt("casesCompleted", 0);
            GameSceneManager.instance.SetCasesCompleted(PlayerPrefs.GetInt("casesCompleted"));
        }

        for (int i = 0; i < amountOfCases; i++)
        {
            CreateCase();
            evidenceBoxs[i].SetActive(true);
        }

        CreatePrefabs();

        CheckCasePossibility();

        SetCase(pageNumber);
    }


    private void Update()
    {
        if (menuAction.action.WasPressedThisFrame())
        {
            if (menuScreen.activeSelf)
            {
                menuScreen.SetActive(false);
                playerPickup.enabled = true;
                playerCamera.enabled = true;
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
                playerMovement.enabled = true;
                radio.isPaused = false;
            }
            else
            {
                menuScreen.SetActive(true);
                playerPickup.enabled = false;
                playerCamera.enabled = false;
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                playerMovement.enabled = false;
                radio.isPaused = true;
            }
        }

        /*
        if (previousAction.action.WasPressedThisFrame())
        {
            pageNumber = Mathf.Max(pageNumber - 1, 0);
            SetCase(pageNumber);
        }

        if (forwardAction.action.WasPressedThisFrame())
        {
            pageNumber = Mathf.Min(pageNumber + 1, 4);
            SetCase(pageNumber);
        }
        */
    }
    

    private void CreateCase()
    {
        caseNumber++;

        //Reset Witness Saw Text
        witnessSawText = "";
        //Reset Witnesses
        if (witnesses != null)
            witnesses = null;
        //Reset Witness Types
        weaponSizeTypes = null;
        weaponColorTypes = null;
        cColor = null;


        victimSex = (personSex)Random.Range(0, 3);
        victimPlace = (locationType)Random.Range(0, 3);

        if (victimSex == personSex.Male)
        {
            height = Random.Range(54, 78);
        }    
        else if (victimSex == personSex.Female)
        {
            height = Random.Range(50, 74);
        }
        else
        {
            height = Random.Range(50, 78);
        }

        bmi = Random.Range(17, 27);

        weight = (bmi * (int)(Mathf.Pow(height, 2))) / 703;

        age = Random.Range(18, 50);

        victimWound = (woundType)Random.Range(0, 3);
        victimSeverity = (woundSeverity)Random.Range(0, 4);

        witnesses = new int[Random.Range(0, 3)];

        if (witnesses.Length == 0)
            witnessSawText = "";
        else
        {
            for (int i = 0; i < witnesses.Length; i++)
            {
                witnesses[i] = Random.Range(0, 3);

                if (witnesses[i] == 0)
                {
                    witnesses[i] = Random.Range(0, 3);
                }
            }
        }

        /*
        if (witnesses == 0)
        {
            witnessSight = 0;
            witnessSight2 = 0;
        }   
        else if (witnesses == 1)
        {
            witnessSight = Random.Range(0, 2);
            witnessSight2 = 0;
        }
        else
        {
            witnessSight = Random.Range(0, 1);
            witnessSight2 = Random.Range(0, 1);
        }

        if (witnessSight == 1 && witnessSight2 == 1)
        {
            wSawOne = Random.Range(1, 2);

            if (wSawOne == 1)
                w2SawOne = 2;
            else
                w2SawOne = 1;

        }
        else if (witnessSight == 1 || witnessSight2 == 1)
        {
            if (witnessSight == 1)
            {
                wSawOne = Random.Range(1, 2);
                w2SawOne = 0;
            }
            else
            {
                wSawOne = 0;
                w2SawOne = Random.Range(1, 2);
            }
        }
        else if (witnessSight == 2)
        {
            weaponSizeType = (wSizeTypes)Random.Range(0, 2);
            WeaponColorType = (wColorTypes)Random.Range(0, 10);
        }
        else
        {
            weaponSizeType = wSizeTypes.None;
            WeaponColorType = wColorTypes.None;
        }
        */

        WitnessSaw();

        StoreCase(caseNumber);
    }

    private void CreatePrefabs()
    {
        //Create location prefabs
        for (int i = 0; i < amountOfCases; i++)
        {
            GameObject location = Instantiate(locationPrefab, spawner);
            CaseSets newLocationSets = location.GetComponent<CaseSets>();
            newLocationSets.cvictimPlace = (CaseSets.clocationType) generatedCases[i, 4];
            newLocationSets.cwitnesses = null;
            newLocationSets.cweaponSizeTypes = null;
            newLocationSets.cweaponColorTypes = null;
            newLocationSets.SetText();
            locationsToCheck[i] = location;
        }

        for (int i = 0; i < amountOfCases; i++)
        {
            if (generatedCases[i, 7] == 0)
            {
                GameObject witness = Instantiate(witnessPrefabs[0], spawner);
                CaseSets newWitnessSets = witness.GetComponent<CaseSets>();

                newWitnessSets.cvictimPlace = CaseSets.clocationType.None;
                newWitnessSets.cwitnesses = null;
                newWitnessSets.cweaponSizeTypes = null;
                newWitnessSets.cweaponColorTypes = null;

                newWitnessSets.SetText();

                witnessesToCheck[i] = witness;
            }
            else if (generatedCases[i, 7] == 1)
            {
                GameObject witness = Instantiate(witnessPrefabs[0], spawner);
                CaseSets newWitnessSets = witness.GetComponent<CaseSets>();

                newWitnessSets.cvictimPlace = CaseSets.clocationType.None;
                newWitnessSets.cwitnesses = new int[1];
                newWitnessSets.cweaponSizeTypes = new CaseSets.cwSizeTypes[1];
                newWitnessSets.cweaponColorTypes = new CaseSets.cwColorTypes[1];

                if (generatedCases[i, 8] != 4)
                {
                    newWitnessSets.cweaponSizeTypes[0] = (CaseSets.cwSizeTypes) generatedCases[i, 8];
                    newWitnessSets.cweaponColorTypes = null;
                }
                if (generatedCases[i, 10] != 12)
                {
                    newWitnessSets.cweaponColorTypes[0] = (CaseSets.cwColorTypes)generatedCases[i, 10];
                    newWitnessSets.cweaponSizeTypes = null;
                }
                else
                {
                    newWitnessSets.cweaponSizeTypes = null;
                    newWitnessSets.cweaponColorTypes = null;
                }

                newWitnessSets.SetText();
                witnessesToCheck[i] = witness;
            }
            else
            {
                GameObject witness = Instantiate(witnessPrefabs[1], spawner);
                CaseSets newWitnessSets = witness.GetComponent<CaseSets>();

                newWitnessSets.cvictimPlace = CaseSets.clocationType.None;
                newWitnessSets.cwitnesses = new int[2];
                newWitnessSets.cweaponSizeTypes = new CaseSets.cwSizeTypes[2];
                newWitnessSets.cweaponColorTypes = new CaseSets.cwColorTypes[2];

                int counter = 0;

                for (int c = 8; c < 10; c++)
                {
                    if (generatedCases[i,c] != 4)
                    {
                        newWitnessSets.cweaponSizeTypes[counter] = (CaseSets.cwSizeTypes)generatedCases[i, c];
                    }
                    else
                    {
                        newWitnessSets.cweaponSizeTypes[counter] = CaseSets.cwSizeTypes.None;
                    }

                    counter++;
                }

                if (newWitnessSets.cweaponSizeTypes[0] != CaseSets.cwSizeTypes.None && newWitnessSets.cweaponSizeTypes[1] != CaseSets.cwSizeTypes.None)
                {
                    if (newWitnessSets.cweaponSizeTypes[0] != newWitnessSets.cweaponSizeTypes[1])
                    {
                        newWitnessSets.cweaponSizeTypes[1] = newWitnessSets.cweaponSizeTypes[0];
                    }
                }

                if (newWitnessSets.cweaponSizeTypes[0] == CaseSets.cwSizeTypes.None && newWitnessSets.cweaponSizeTypes[1] == CaseSets.cwSizeTypes.None)
                {
                    newWitnessSets.cweaponSizeTypes = null;
                }

                    counter = 0;

                for (int c = 10; c < 12; c++)
                {
                    if (generatedCases[i, c] != 12)
                    {
                        newWitnessSets.cweaponColorTypes[counter] = (CaseSets.cwColorTypes)generatedCases[i, c];
                    }
                    else
                    {
                        newWitnessSets.cweaponColorTypes[counter] = CaseSets.cwColorTypes.None;
                    }

                    counter++;
                }

                if (newWitnessSets.cweaponColorTypes[0] == CaseSets.cwColorTypes.None && newWitnessSets.cweaponColorTypes[1] == CaseSets.cwColorTypes.None)
                {
                    newWitnessSets.cweaponColorTypes = null;
                }

                newWitnessSets.SetText();
                witnessesToCheck[i] = witness;
            }
        }

    }

    private void WitnessSaw()
    {
        int witness = 0;
        int witnessSees = 0;

        if (witnesses != null)
        {
            if (witnesses.Length != 0)
            {
                if (witnesses.Length == 1)
                {
                    foreach (int i in witnesses)
                    {
                        if (i == 2)
                        {
                            witness++;

                            weaponColorTypes = new wColorTypes[1];
                            //cColor = new bool[1];

                            weaponColorTypes[witnessSees] = (wColorTypes)Random.Range(0, 11);

                            witnessSawText += "Witness " + witness.ToString() + " Saw: " + weaponColorTypes[witnessSees] + " Colored Weapon" + "\n";

                        }
                        else if (i == 1)
                        {
                            witness++;

                            weaponSizeTypes = new wSizeTypes[1];

                            if (victimSeverity != woundSeverity.None)
                                weaponSizeTypes[witnessSees] = (wSizeTypes)(int)victimSeverity;
                            else
                                weaponSizeTypes[witnessSees] = (wSizeTypes)Random.Range(0, 3);

                            if (witness == 2)
                            {
                                if (witnesses[0] == 1 && witnesses[1] == 1)
                                {
                                    weaponSizeTypes[1] = weaponSizeTypes[0];
                                }
                            }

                            witnessSawText += "Witness " + witness.ToString() + " Saw: " + weaponSizeTypes[witnessSees] + " Sized Weapon" + "\n";
                        }
                        else
                        {
                            witness++;

                            if (weaponColorTypes != null)
                            {
                                weaponColorTypes[witnessSees] = wColorTypes.None;
                            }

                            if (weaponSizeTypes != null)
                            {
                                weaponSizeTypes[witnessSees] = wSizeTypes.None;
                            }

                            witnessSawText += "Witness " + witness.ToString() + " Saw: Nothing" + "\n";
                        }
                    }
                }
                else
                {
                    foreach (int i in witnesses)
                    {
                        if (i == 2)
                        {
                            witness++;

                            if (weaponColorTypes == null && witness == 2)
                            {
                                weaponColorTypes = new wColorTypes[2];
                                weaponColorTypes[0] = wColorTypes.None;
                            }
                            else if (weaponColorTypes == null && witness != 2)
                            {
                                weaponColorTypes = new wColorTypes[2];
                            }

                            if (weaponSizeTypes != null)
                            {
                                weaponSizeTypes[witnessSees] = wSizeTypes.None;
                            }

                            if (cColor == null)
                            {
                                //cColor = new bool[1];
                            }

                            weaponColorTypes[witnessSees] = (wColorTypes)Random.Range(0, 11);

                            witnessSawText += "Witness " + witness.ToString() + " Saw: " + weaponColorTypes[witnessSees] + " Colored Weapon" + "\n";

                            witnessSees++;

                        }
                        else if (i == 1)
                        {
                            witness++;

                            if (weaponSizeTypes == null && witness == 2)
                            {
                                weaponSizeTypes = new wSizeTypes[2];
                                weaponSizeTypes[0] = wSizeTypes.None;
                            }
                            else if (weaponSizeTypes == null && witness != 2)
                            {
                                weaponSizeTypes = new wSizeTypes[2];
                            }

                            if (weaponColorTypes != null)
                            {
                                weaponColorTypes[witnessSees] = wColorTypes.None;
                            }

                            if (victimSeverity != woundSeverity.None)
                                weaponSizeTypes[witnessSees] = (wSizeTypes)(int)victimSeverity;
                            else
                                weaponSizeTypes[witnessSees] = (wSizeTypes)Random.Range(0, 3);

                            if (witness == 2)
                            {
                                if (witnesses[witnessSees] == 1 && witnesses[witnessSees] == 1)
                                {
                                    weaponSizeTypes[witnessSees] = weaponSizeTypes[witnessSees];
                                }
                            }

                            witnessSawText += "Witness " + witness.ToString() + " Saw: " + weaponSizeTypes[witnessSees] + " Sized Weapon" + "\n";

                            witnessSees++;
                        }
                        else
                        {
                            witness++;

                            if (weaponColorTypes != null)
                            {
                                weaponColorTypes[witnessSees] = wColorTypes.None;
                            }

                            if (weaponSizeTypes != null)
                            {
                                weaponSizeTypes[witnessSees] = wSizeTypes.None;
                            }

                            witnessSawText += "Witness " + witness.ToString() + " Saw: Nothing" + "\n";

                            witnessSees++;
                        }
                    }
                }
            }
            else
            {
                weaponSizeTypes = null;
                weaponColorTypes = null;
            }
        }
        else
        {
            weaponSizeTypes = null;
            weaponColorTypes = null;
            witnessSawText = "";
        }


        
        

        /*
        if (wSawOne != 0 && w2SawOne != 0)
        {
            WSaw();
        }
        else if (wSawOne != 0 || w2SawOne != 0)
        {
            WSaw();
        }
        else
        {
            WSaw();
        }
        */
    }

    private void WitnessTextUpdate()
    {
        int witness = 0;
        int witnessSees = 0;

        if (witnesses != null)
        {
            if (witnesses.Length == 1)
            {
                foreach (int i in witnesses)
                {
                    if (weaponColorTypes != null)
                    {
                        if (weaponColorTypes[witnessSees] != wColorTypes.None)
                        {
                            witness++;
                            witnessSawText += "Witness " + witness.ToString() + " Saw: " + weaponColorTypes[witnessSees] + " Colored Weapon" + "\n";
                            witnessWallText = weaponColorTypes[witnessSees].ToString();
                        }
                        else
                        {
                            witness++;
                            witnessSawText += "Witness " + witness.ToString() + " Saw: Nothing" + "\n";
                            witnessWallText = "Saw Nothing";
                        }
                    }
                    else if (weaponSizeTypes != null)
                    {
                        if (weaponSizeTypes[witnessSees] != wSizeTypes.None)
                        {
                            witness++;
                            witnessSawText += "Witness " + witness.ToString() + " Saw: " + weaponSizeTypes[witnessSees] + " Sized Weapon" + "\n";
                            witnessWallText = weaponSizeTypes[witnessSees].ToString();
                        }
                        else
                        {
                            witness++;
                            witnessSawText += "Witness " + witness.ToString() + " Saw: Nothing" + "\n";
                            witnessWallText = "Saw Nothing";
                        }
                        
                    }
                    else
                    {
                        witness++;
                        witnessSawText += "Witness " + witness.ToString() + " Saw: Nothing" + "\n";
                        witnessWallText = "Saw Nothing";
                    }
                }

                
            }
            else
            {
                foreach (int i in witnesses)
                {
                    if (weaponColorTypes != null)
                    {
                        if (weaponColorTypes[witnessSees] != wColorTypes.None)
                        {
                            witness++;
                            witnessSawText += "Witness " + witness.ToString() + " Saw: " + weaponColorTypes[witnessSees] + " Colored Weapon" + "\n";

                            if (witness > 1)
                            {
                                witnessWallText += " & " + weaponColorTypes[witnessSees];
                            }
                            else
                            {
                                witnessWallText = weaponColorTypes[witnessSees].ToString();
                            }

                            witnessSees++;
                        }
                        else
                        {
                            witness++;
                            witnessSawText += "Witness " + witness.ToString() + " Saw: Nothing" + "\n";

                            if (witness > 1)
                            {
                                witnessWallText += " & Saw Nothing";
                            }
                            else
                            {
                                witnessWallText = "Saw Nothing";
                            }

                            witnessSees++;
                        }
                    }
                    else if (weaponSizeTypes != null)
                    {
                        if (weaponSizeTypes[witnessSees] != wSizeTypes.None)
                        {
                            witness++;
                            witnessSawText += "Witness " + witness.ToString() + " Saw: " + weaponSizeTypes[witnessSees] + " Sized Weapon" + "\n";

                            if (witness > 1)
                            {
                                witnessWallText += " & " + weaponSizeTypes[witnessSees];
                            }
                            else
                            {
                                witnessWallText = weaponSizeTypes[witnessSees].ToString();
                            }

                            witnessSees++;
                        }
                        else
                        {
                            witness++;
                            witnessSawText += "Witness " + witness.ToString() + " Saw: Nothing" + "\n";

                            if (witness > 1)
                            {
                                witnessWallText += " & Saw Nothing";
                            }
                            else
                            {
                                witnessWallText = "Saw Nothing";
                            }

                            witnessSees++;
                        }
                    }
                    else
                    {
                        witness++;
                        witnessSawText += "Witness " + witness.ToString() + " Saw: Nothing" + "\n";

                        if (witness > 1)
                        {
                            witnessWallText += " & Saw Nothing";
                        }
                        else
                        {
                            witnessWallText = "Saw Nothing";
                        }

                        witnessSees++;
                    }
                }
            }
        }
        else
        {
            witnessSawText = "";
            witnessWallText = "No Witnesses";
        }
    }

        /*
        private void WSaw()
        {
            if (wSawOne != 0 && w2SawOne != 0)
            {
                if (wSawOne == 1)
                    weaponSizeType = (wSizeTypes)Random.Range(0, 2);
                else if (wSawOne == 2)
                    WeaponColorType = (wColorTypes)Random.Range(0, 10);

                if (w2SawOne == 1)
                    weaponSizeType = (wSizeTypes)Random.Range(0, 2);
                else if (w2SawOne == 2)
                    WeaponColorType = (wColorTypes)Random.Range(0, 10);
            }
            else
            {
                if (wSawOne == 1)
                {
                    weaponSizeType = (wSizeTypes)Random.Range(0, 2);
                    WeaponColorType = wColorTypes.None;
                }
                else if (wSawOne == 2)
                {
                    WeaponColorType = (wColorTypes)Random.Range(0, 10);
                    weaponSizeType = wSizeTypes.None;
                }
                else if (w2SawOne == 1)
                {
                    weaponSizeType = (wSizeTypes)Random.Range(0, 2);
                    WeaponColorType = wColorTypes.None;
                }
                else if (w2SawOne == 2)
                {
                    WeaponColorType = (wColorTypes)Random.Range(0, 10);
                    weaponSizeType = wSizeTypes.None;
                }
                else
                {
                    weaponSizeType = wSizeTypes.None;
                    WeaponColorType = wColorTypes.None;
                }
            }

        }
        */

    private void UpdateText()
    {
        witnessSawText = "";
        witnessWallText = "None";

        WitnessTextUpdate();

        string heightText = ((int)height / 12).ToString() + "\'" + ((int)height % 12).ToString() + "\"";

        if (witnesses != null)
        {
            witnessText = "Witnesses: " + witnesses.Length.ToString();
        }
        else
        {
            witnessText = "Witnesses: 0";
        }

        if (victimSeverity != woundSeverity.None)
            woundText = victimSeverity + " " + victimWound;
        else
            woundText = victimWound.ToString();

        string sizeText = "";
        string typeText = "";

        switch (victimSeverity)
        {
            case woundSeverity.Shallow:
                sizeText = "Sounds like a small weapon did this";
                break;
            case woundSeverity.Partial:
                sizeText = "Sounds like a medium sized weapon did this";
                break;
            case woundSeverity.Deep:
                sizeText = "Sounds like a large weapon did this";
                break;
            case woundSeverity.None:
                sizeText = "Sounds like any size weapon will count";
                break;
        }

        switch (victimWound)
        {
            case woundType.Trauma:
                typeText = "Seems like something that can bludgeon";
                break;
            case woundType.Puncture:
                typeText = "Seems like something that can pierce";
                break;
            case woundType.Laceration:
                typeText = "Seems like something that can slash";
                break;
        }

        caseText.text = "Gender: " + victimSex + "\n" + "Height: " + heightText + "\n" + "Weight: " + weight.ToString() + "\n" + "Age: " + age.ToString() + "\n" + "BodyFound: " + victimPlace + "\n" + "Wound: " + woundText + "\n" + witnessText + "\n" + witnessSawText;
        caseText.text.Replace("\n", "\n");

        boardTexts[0].text = "Case " + (pageNumber + 1).ToString();
        boardTexts[1].text = "Gender: " + victimSex + "\n" + "Height: " + heightText + "\n" + "Weight: " + weight.ToString() + "\n" + "Age: " + age.ToString();
        boardTexts[2].text = "Cases Completed: " + pageNumber.ToString() + "/" + amountOfCases;
        boardTexts[4].text = "Wound: " + woundText;
        boardTexts[5].text = sizeText;
        boardTexts[6].text = typeText;
        boardTexts[7].text = weaponText + " + " + victimPlace + " + " + witnessWallText;

        if (SettingsScript.instance.tutorialMode)
        {
            if(pageNumber == 0)
            {
                boardTexts[4].text = "Wound: Knife Stab Wounds";
                boardTexts[5].text = "Sounds like a knife did this";
                boardTexts[6].text = "Definitely a knife";
            }
            if(pageNumber == 1)
            {
                boardTexts[4].text = "Wound: Gunshot Wounds";
                boardTexts[5].text = "Sounds like a gun";
                boardTexts[6].text = "Positively a gun";
            }
        }
    }

    public void SizeCheck(ObjectAttributes check)
    {
        if (victimSeverity != woundSeverity.None)
        {
            if ((int)check.objSizeType == ((int)victimSeverity))
            {
                //Debug.Log(weaponText + " is " + check.objSizeType.ToString() + " &  Victim Wound is " + victimSeverity);
                cSize = true;
                boardTriggers[0].correctWeapon = check.gameObject;
                boardTriggers[0].tempWeapon = null;
            }
            else
            {
                cSize = false;
                boardTriggers[0].correctWeapon = null;
                boardTriggers[0].tempWeapon = check.gameObject;
            }
        }
        else
        {
            if (weaponSizeTypes != null)
            {
                if (weaponSizeTypes.Length == 2)
                {
                    if ((int)check.objSizeType == ((int)weaponSizeTypes[0]) || (int)check.objSizeType == ((int)weaponSizeTypes[1]))
                    {
                        /*
                        if ((int)check.objSizeType == ((int)weaponSizeTypes[0]))
                        {
                            Debug.Log(weaponText + " is " + check.objSizeType.ToString() + " &  Witness Wound is " + weaponSizeTypes[0]);
                        }
                        else
                        {
                            Debug.Log(weaponText + " is " + check.objSizeType.ToString() + " &  Witness Wound is " + weaponSizeTypes[1]);
                        }
                        */
                        
                        cSize = true;
                        boardTriggers[0].correctWeapon = check.gameObject;
                        boardTriggers[0].tempWeapon = null;
                    }
                    else
                    {
                        cSize = false;
                        boardTriggers[0].correctWeapon = null;
                        boardTriggers[0].tempWeapon = check.gameObject;
                    }
                }
                else
                {
                    if ((int)check.objSizeType == ((int)weaponSizeTypes[0]))
                    {
                        //Debug.Log(weaponText + " is " + check.objSizeType.ToString() + " &  Witness Wound is " + weaponSizeTypes[0]);

                        cSize = true;
                        boardTriggers[0].correctWeapon = check.gameObject;
                        boardTriggers[0].tempWeapon = null;
                    }
                    else
                    {
                        cSize = false;
                        boardTriggers[0].correctWeapon = null;
                        boardTriggers[0].tempWeapon = check.gameObject;
                    }
                }
                
            }
            else
            {
                //Debug.Log("Weapon size types is null");

                cSize = true;
                boardTriggers[0].correctWeapon = check.gameObject;
                boardTriggers[0].tempWeapon = null;
            }
        }
    }

    public void DamageTypeCheck(ObjectAttributes check)
    {
        if (check.objDamageTypes.Length > 1)
        {
            for (int i = 0; i < check.objDamageTypes.Length; i++)
            {
                if ((int)check.objDamageTypes[i] == ((int)victimWound))
                {
                    cDamage = true;
                    boardTriggers[0].correctWeapon = check.gameObject;
                    boardTriggers[0].tempWeapon = null;

                    i += 10;
                }
                else
                {
                    cDamage = false;
                    boardTriggers[0].correctWeapon = null;
                    boardTriggers[0].tempWeapon = check.gameObject;
                }
            }
        }
        else
        {
            if ((int)check.objDamageTypes[0] == ((int)victimWound))
            {
                cDamage = true;
                boardTriggers[0].correctWeapon = check.gameObject;
                boardTriggers[0].tempWeapon = null;
            }
            else
            {
                cDamage = false;
                boardTriggers[0].correctWeapon = null;
                boardTriggers[0].tempWeapon = check.gameObject;
            }
        }
    }

    public void ColorCheck(ObjectAttributes check)
    {
        if (weaponColorTypes != null)
        {
            //Debug.Log("Case 1 weaponColorTypes lenght is " + weaponColorTypes.Length);

            if (check.objColorTypes.Length > 1)
            {
                if (cColor != null)
                {
                    cColor[0] = false;
                    cColor[1] = false;

                    for (int c = 0; c < cColor.Length; c++)
                    {
                        for (int i = 0; i < check.objColorTypes.Length; i++)
                        {
                            if (weaponColorTypes.Length == 1)
                            {
                                if ((int)check.objColorTypes[i] == ((int)weaponColorTypes[0]) && weaponColorTypes[0] != wColorTypes.None)
                                {
                                    cColor[c] = true;

                                    //Debug.Log((int)check.objColorTypes[i] + " == " + ((int)weaponColorTypes[0]));

                                    break;
                                }
                                else
                                {
                                    cColor[c] = false;
                                }

                                if (weaponColorTypes[0] == wColorTypes.None)
                                {
                                    cColor[c] = true;
                                    break;
                                }
                            }
                            else
                            {
                                if ((int)check.objColorTypes[i] == ((int)weaponColorTypes[c]) && weaponColorTypes[c] != wColorTypes.None)
                                {
                                    cColor[c] = true;

                                    //Debug.Log((int)check.objColorTypes[i] + " == " + ((int)weaponColorTypes[c]));

                                    break;
                                }
                                else
                                {
                                    cColor[c] = false;
                                }

                                if (weaponColorTypes[c] == wColorTypes.None)
                                {
                                    cColor[c] = true;
                                    break;
                                }
                            }
                        }
                    }
                }
                else
                {
                    cColor = new bool[2];

                    for (int c = 0; c < cColor.Length; c++)
                    {
                        for (int i = 0; i < check.objColorTypes.Length; i++)
                        {
                            if (weaponColorTypes.Length == 1)
                            {
                                if ((int)check.objColorTypes[i] == ((int)weaponColorTypes[0]) && weaponColorTypes[0] != wColorTypes.None)
                                {
                                    cColor[c] = true;

                                    //Debug.Log((int)check.objColorTypes[i] + " == " + ((int)weaponColorTypes[0]));

                                    break;
                                }
                                else
                                {
                                    cColor[c] = false;
                                }

                                if (weaponColorTypes[0] == wColorTypes.None)
                                {
                                    cColor[c] = true;
                                    break;
                                }
                            }
                            else
                            {
                                if ((int)check.objColorTypes[i] == ((int)weaponColorTypes[c]) && weaponColorTypes[c] != wColorTypes.None)
                                {
                                    cColor[c] = true;

                                    //Debug.Log((int)check.objColorTypes[i] + " == " + ((int)weaponColorTypes[c]));

                                    break;
                                }
                                else
                                {
                                    cColor[c] = false;
                                }

                                if (weaponColorTypes[c] == wColorTypes.None)
                                {
                                    cColor[c] = true;
                                    break;
                                }
                            }

                        }
                    }

                }
                
            }
            else
            {
                if (weaponColorTypes.Length == 1)
                {
                    if ((int)check.objColorTypes[0] == ((int)weaponColorTypes[0]) && weaponColorTypes[0] != wColorTypes.None)
                    {
                        cColorSingle = true;
                        boardTriggers[0].correctWeapon = check.gameObject;
                        boardTriggers[0].tempWeapon = null;
                    }
                    else
                    {
                        cColorSingle = false;
                        boardTriggers[0].correctWeapon = null;
                        boardTriggers[0].tempWeapon = check.gameObject;
                    }

                    if (weaponColorTypes[0] == wColorTypes.None)
                    {
                        cColorSingle = true;
                        boardTriggers[0].correctWeapon = check.gameObject;
                        boardTriggers[0].tempWeapon = null;
                    }
                }
                else
                {
                    if (cColor != null)
                    {
                        cColor[0] = false;
                        cColor[1] = false;
                        for (int w = 0; w < weaponColorTypes.Length; w++)
                        {
                            if ((int)check.objColorTypes[0] == ((int)weaponColorTypes[w]) && weaponColorTypes[w] != wColorTypes.None)
                            {
                                cColor[w] = true;
                            }
                            else
                            {
                                cColor[w] = false;
                            }

                            if (weaponColorTypes[w] == wColorTypes.None)
                            {
                                cColor[w] = true;
                            }
                        }
                    }
                    else
                    {
                        cColor = new bool[2];
                        for (int w = 0; w < weaponColorTypes.Length; w++)
                        {
                            if ((int)check.objColorTypes[0] == ((int)weaponColorTypes[w]) && weaponColorTypes[0] != wColorTypes.None)
                            {
                                cColor[w] = true;
                            }
                            else
                            {
                                cColor[w] = false;
                            }

                            if (weaponColorTypes[w] == wColorTypes.None)
                            {
                                cColor[w] = true;
                            }
                        }
                    }
                    
                }
            }
            
        }
        else
        {
            cColorSingle = true;
            boardTriggers[0].correctWeapon = check.gameObject;
            boardTriggers[0].tempWeapon = null;
        }
    }

    public void LocationCheck(ObjectAttributes check)
    {
        if (victimPlace != locationType.None)
        {
            if (check.objCategoryTypes.Length > 1)
            {
                for (int i = 0; i < check.objCategoryTypes.Length; i++)
                {
                    if ((int)check.objCategoryTypes[i] == ((int)victimPlace))
                    {
                        cLocation = true;
                        boardTriggers[0].correctWeapon = check.gameObject;
                        boardTriggers[0].tempWeapon = null;

                        i += 10;
                    }
                    else
                    {
                        cLocation = false;
                        boardTriggers[0].correctWeapon = null;
                        boardTriggers[0].tempWeapon = check.gameObject;
                    }
                }
            }
            else
            {
                if ((int)check.objCategoryTypes[0] == ((int)victimPlace))
                {
                    cLocation = true;
                    boardTriggers[0].correctWeapon = check.gameObject;
                    boardTriggers[0].tempWeapon = null;
                }
                else
                {
                    cLocation = false;
                    boardTriggers[0].correctWeapon = null;
                    boardTriggers[0].tempWeapon = check.gameObject;
                }
            }
        }
        else
        {
            cLocation = false;
            boardTriggers[0].correctWeapon = null;
            boardTriggers[0].tempWeapon = check.gameObject;
        }
       
    }

    public bool IsColorTrue()
    {
        if (cColor != null)
        {
            if (cColor[0] && cColor[1])
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            if (cColorSingle == true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    public void StoreCase(int storedCaseNumber)
    {
        generatedCases[storedCaseNumber, 0] = (int)victimSex;
        generatedCases[storedCaseNumber, 1] = (int)height;
        generatedCases[storedCaseNumber, 2] = weight;
        generatedCases[storedCaseNumber, 3] = age;
        generatedCases[storedCaseNumber, 4] = (int)victimPlace;
        generatedCases[storedCaseNumber, 5] = (int)victimWound;
        generatedCases[storedCaseNumber, 6] = (int)victimSeverity;
        generatedCases[storedCaseNumber, 7] = witnesses.Length;

        if (witnesses == null)
        {
            generatedCases[storedCaseNumber, 8] = 4;
            generatedCases[storedCaseNumber, 9] = 4;
            generatedCases[storedCaseNumber, 10] = 12;
            generatedCases[storedCaseNumber, 11] = 12;
        }
        else
        {
            if (weaponSizeTypes == null)
            {
                generatedCases[storedCaseNumber, 8] = 4;
                generatedCases[storedCaseNumber, 9] = 4;
                generatedCases[storedCaseNumber, 10] = 12;
                generatedCases[storedCaseNumber, 11] = 12;

                int rowNumber = 9;

                if (weaponColorTypes != null)
                {
                    foreach (wColorTypes w in weaponColorTypes)
                    {
                        rowNumber++;
                        generatedCases[storedCaseNumber, rowNumber] = (int)w;
                    }
                }
            }
            else if (weaponColorTypes == null)
            {
                generatedCases[storedCaseNumber, 8] = 4;
                generatedCases[storedCaseNumber, 9] = 4;
                generatedCases[storedCaseNumber, 10] = 12;
                generatedCases[storedCaseNumber, 11] = 12;

                int rowNumber = 7;

                foreach (wSizeTypes w in weaponSizeTypes)
                {
                    rowNumber++;
                    generatedCases[storedCaseNumber, rowNumber] = (int)w;
                }
            }
            else
            {
                generatedCases[storedCaseNumber, 8] = 4;
                generatedCases[storedCaseNumber, 9] = 4;
                generatedCases[storedCaseNumber, 10] = 12;
                generatedCases[storedCaseNumber, 11] = 12;

                int rowNumber = 7;

                foreach (wSizeTypes w in weaponSizeTypes)
                {
                    rowNumber++;
                    generatedCases[storedCaseNumber, rowNumber] = (int)w;
                }

                foreach (wColorTypes w in weaponColorTypes)
                {
                    rowNumber++;
                    generatedCases[storedCaseNumber, rowNumber] = (int)w;
                }
            }
        }

        for (int i = 0; i < generatedCases.GetLength(1); i++)
        {
            //Debug.Log("The value of Row " + storedCaseNumber + " Column " + i + " is " + generatedCases[storedCaseNumber, i]);
        }
        
    }

    public void SetCase(int caseToSet)
    {
        victimSeverity = woundSeverity.None;

        //Debug.Log(caseToSet.ToString());
        victimSex = (personSex) generatedCases[caseToSet, 0];
        height = generatedCases[caseToSet, 1];
        weight = generatedCases[caseToSet, 2];
        age = generatedCases[caseToSet, 3];
        victimWound = (woundType)generatedCases[caseToSet, 5];
        victimSeverity = (woundSeverity)generatedCases[caseToSet, 6];
        //Debug.Log("Case " + caseToSet + " is equal to " + victimSeverity);

        victimPlace = locationType.None;
        witnesses = null;
        weaponSizeTypes = null;
        weaponColorTypes = null;

        boardTriggers[0].tempWeapon = null;
        boardTriggers[0].correctWeapon = null;
        boardTriggers[1].correctLocation = null;
        boardTriggers[2].correctWitness = null;
        cSize = false;
        cDamage = false;
        cColor = null;
        cColorSingle = false;
        cLocation = false;
        cWitness = false;

        weaponText = "None";

        WitnessSaw();
        UpdateText();
    }

    public void UpdateCaseWeapon(GameObject weapon)
    {
        if (weapon != null)
        {
            ObjectAttributes check = weapon.gameObject.GetComponent<ObjectAttributes>();
            weaponText = check.objName;

            if (witnesses != null)
            {
                SizeCheck(check);
                ColorCheck(check);
            }
            else
            {
                cSize = false;
                cColorSingle = false;
                cColor = null;
            }

            DamageTypeCheck(check);
            LocationCheck(check);

            UpdateText();
        }
        else
        {
            weaponText = "None";
            cSize = false;
            cDamage = false;
            cColor = null;
            cColorSingle = false;
            cLocation = false;

            boardTriggers[0].correctWeapon = null;
            boardTriggers[0].tempWeapon = null;

            UpdateText();
        }
    }

    public void UpdateCaseLocation(GameObject location)
    {
        if (location != null)
        {
            CaseSets check = location.gameObject.GetComponent<CaseSets>();

            if (check.cvictimPlace != CaseSets.clocationType.None)
            {
                victimPlace = (locationType)check.cvictimPlace;
                boardTriggers[1].correctLocation = location;
            }
            else
            {
                victimPlace = locationType.None;
                boardTriggers[1].correctLocation = null;
            }
            
        }
        else
        {
            victimPlace = locationType.None;
            boardTriggers[1].correctLocation = null;

            UpdateText();
        }

        if (boardTriggers[0].correctWeapon != null)
        {
            UpdateCaseWeapon(boardTriggers[0].correctWeapon);
        }

        if (boardTriggers[0].tempWeapon != null)
        {
            UpdateCaseWeapon(boardTriggers[0].tempWeapon);
        }
        UpdateText();
    }

    public void UpdateCaseWitness(GameObject witnessObj)
    {
        if (witnessObj != null)
        {
            CaseSets check = witnessObj.gameObject.GetComponent<CaseSets>();

            if (witnesses == null)
            {
                if (check.cvictimPlace == CaseSets.clocationType.None && check.cwitnesses != null)
                {
                    witnesses = new int[check.cwitnesses.Length];
                    for (int i = 0; i < witnesses.Length; i++)
                    {
                        witnesses[i] = check.cwitnesses[i];
                    }

                    if (witnesses.Length == 1)
                    {
                        if (check.cweaponSizeTypes != null)
                        {
                            int counter = 0;
                            bool contradictory = false;
                            weaponSizeTypes = new wSizeTypes[1];
                            foreach (CaseSets.cwSizeTypes size in check.cweaponSizeTypes)
                            {
                                weaponSizeTypes[counter] = (wSizeTypes)size;

                                if (victimSeverity != woundSeverity.None && weaponSizeTypes[counter] != wSizeTypes.None)
                                {
                                    if (weaponSizeTypes[counter] != (wSizeTypes)victimSeverity)
                                    {
                                        contradictory = true;
                                    }
                                }

                                counter++;
                            }

                            if (contradictory)
                            {
                                witnesses = null;
                                weaponColorTypes = null;
                                weaponSizeTypes = null;
                                cWitness = false;
                                cColor = null;
                                boardTriggers[2].correctWitness = null;
                            }
                            else
                            {
                                cWitness = true;
                                boardTriggers[2].correctWitness = witnessObj;
                            }
                        }
                        else
                        {
                            if (check.cweaponColorTypes != null)
                            {
                                int counter = 0;
                                weaponColorTypes = new wColorTypes[check.cweaponColorTypes.Length];
                                foreach (CaseSets.cwColorTypes color in check.cweaponColorTypes)
                                {
                                    weaponColorTypes[counter] = (wColorTypes)color;

                                    counter++;
                                }

                                cWitness = true;
                                boardTriggers[2].correctWitness = witnessObj;
                            }
                            else
                            {
                                weaponColorTypes = null;
                                weaponSizeTypes = null;
                                cColor = null;
                                cWitness = true;
                                boardTriggers[2].correctWitness = witnessObj;
                            }
                        }
                    }
                    else
                    {
                        if (check.cweaponSizeTypes != null)
                        {
                            int counter = 0;
                            bool contradictory = false;
                            weaponSizeTypes = new wSizeTypes[2];

                            //string debugSize = "";
                            //string debugColor = "";

                            foreach (CaseSets.cwSizeTypes size in check.cweaponSizeTypes)
                            {
                                weaponSizeTypes[counter] = (wSizeTypes)size;

                                if (victimSeverity != woundSeverity.None && weaponSizeTypes[counter] != wSizeTypes.None)
                                {
                                    //debugSize = $"Witness {counter} saw size {weaponSizeTypes[counter]}";
                                    if (weaponSizeTypes[counter] != (wSizeTypes)victimSeverity)
                                    {
                                        contradictory = true;
                                    }
                                }

                                counter++;
                            }

                            if (check.cweaponColorTypes != null)
                            {
                                cColor = new bool[2];

                                counter = 0;
                                weaponColorTypes = new wColorTypes[check.cweaponColorTypes.Length];
                                foreach (CaseSets.cwColorTypes color in check.cweaponColorTypes)
                                {
                                    weaponColorTypes[counter] = (wColorTypes)color;

                                    /*
                                    if (weaponColorTypes[counter] != wColorTypes.None)
                                    {
                                        debugColor = $"Witness {counter} saw color {weaponColorTypes[counter]}";
                                    }
                                    */

                                    counter++;
                                }

                                //cWitness = true;
                                //boardTriggers[2].correctWitness = witnessObj;
                            }
                            else
                            {
                                //weaponColorTypes = null;
                                //weaponSizeTypes = null;
                                //cColor = null;
                                //cWitness = true;
                                //boardTriggers[2].correctWitness = witnessObj;
                            }

                            if (contradictory)
                            {
                                /*
                                if(debugSize != "" && debugColor != "")
                                {
                                    Debug.Log($"{debugSize} and {debugColor} is contradictory");
                                }
                                */
                        
                                witnesses = null;
                                weaponColorTypes = null;
                                weaponSizeTypes = null;
                                cWitness = false;
                                cColor = null;
                                boardTriggers[2].correctWitness = null;
                            }
                            else
                            {
                                /*
                                if (debugSize != "" && debugColor != "")
                                {
                                    Debug.Log($"{debugSize} and {debugColor}");
                                }
                                */

                                cWitness = true;
                                boardTriggers[2].correctWitness = witnessObj;
                                //Debug.Log("Weapon Size is not null");
                            }
                        }
                        else
                        {
                            if (check.cweaponColorTypes != null)
                            {
                                cColor = new bool[2];

                                int counter = 0;
                                weaponColorTypes = new wColorTypes[check.cweaponColorTypes.Length];
                                foreach (CaseSets.cwColorTypes color in check.cweaponColorTypes)
                                {
                                    weaponColorTypes[counter] = (wColorTypes)color;

                                    counter++;
                                }

                                cWitness = true;
                                boardTriggers[2].correctWitness = witnessObj;
                            }
                            else
                            {
                                weaponColorTypes = null;
                                weaponSizeTypes = null;
                                cColor = null;
                                cWitness = true;
                                boardTriggers[2].correctWitness = witnessObj;
                            }
                        }
                    }
                }
                else if (check.cvictimPlace == CaseSets.clocationType.None && check.cwitnesses == null)
                {
                    witnesses = new int[0];
                    cColor = null;
                    cColorSingle = true;
                    cSize = true;
                    cWitness = true;
                    boardTriggers[2].correctWitness = witnessObj;
                }
            }
        }
        else
        {
            witnesses = null;
            weaponColorTypes = null;
            weaponSizeTypes = null;
            cColor = null;
            boardTriggers[2].correctWitness = null;
        }

        if (boardTriggers[0].correctWeapon != null)
        {
            UpdateCaseWeapon(boardTriggers[0].correctWeapon);
        }

        if (boardTriggers[0].tempWeapon != null)
        {
            UpdateCaseWeapon(boardTriggers[0].tempWeapon);
        }
        UpdateText();
    }

    public void CheckCase()
    {
        if (cSize == true && cDamage == true && IsColorTrue() == true && cLocation == true && cWitness == true)
        {
            if (boardTriggers[0].correctWeapon != null && boardTriggers[1].correctLocation != null && boardTriggers[2].correctWitness != null && caseSolvedStarted == false)
            {
                GameObject[] items = { boardTriggers[0].correctWeapon, boardTriggers[1].correctLocation, boardTriggers[2].correctWitness };
                UpdateCaseProbablity(items);
                StartCoroutine(CaseSolved(items));

                caseSolvedStarted = true;
            }
        }
        else if (caseFailedStarted == false)
        {
            /*
            bool[] debugList = { cSize, cDamage, IsColorTrue(), cLocation, cWitness };
            string[] debugName = { "cSize", "cDamage", "IsColorTrue()", "cLocation", "cWitness" };

            int counter = 0;
            foreach (bool debug in debugList)
            {
                if (!debug)
                {
                    Debug.Log("Case is not solved because " + debugName[counter] + " is not true");
                }

                counter++;
            }
            */

            StartCoroutine(CaseUnsolved());

            caseFailedStarted = true;
        }
    }

    public void GiveUp(GameObject door)
    {
        if (hasLost || hasWon)
        {
            caseSolvedStarted = true;
            caseFailedStarted = true;
            //winOrLoseText[1].SetActive(true);
            BackToMainMenu();
        }
        else
        {
            leaveCheck.SetActive(true);

            door.tag = "Untagged";
        }

        universalSound.PlayOneShot(universalSounds[0]);
        
    }

    public void GiveUpYes()
    {
        caseSolvedStarted = true;
        caseFailedStarted = true;
        winOrLoseText[1].SetActive(true);
        BackToMainMenu();
    }

    public void BackToMainMenu()
    {
        Invoke("LoadMainMenu", 1.5f / 2);
    }

    public void QuitGame()
    {
        Invoke("QuitGameReal", 1.5f / 2);
    }

    protected void QuitGameReal()
    {
        Application.Quit();
    }

    private void LoadMainMenu()
    {
        SceneManager.LoadScene("Main Menu");
    }

    private void CreateNewPrefab(int pageNumber)
    {
        //Create location prefabs

        Destroy(locationsToCheck[pageNumber].gameObject);

        GameObject location = Instantiate(locationPrefab, spawner);
        CaseSets newLocationSets = location.GetComponent<CaseSets>();
        newLocationSets.cvictimPlace = (CaseSets.clocationType)generatedCases[pageNumber, 4];
        newLocationSets.cwitnesses = null;
        newLocationSets.cweaponSizeTypes = null;
        newLocationSets.cweaponColorTypes = null;
        newLocationSets.SetText();
        locationsToCheck[pageNumber] = location;

        if (generatedCases[pageNumber, 7] == 0)
        {
            Destroy(witnessesToCheck[pageNumber].gameObject);

            GameObject witness = Instantiate(witnessPrefabs[0], spawner);
            CaseSets newWitnessSets = witness.GetComponent<CaseSets>();

            newWitnessSets.cvictimPlace = CaseSets.clocationType.None;
            newWitnessSets.cwitnesses = null;
            newWitnessSets.cweaponSizeTypes = null;
            newWitnessSets.cweaponColorTypes = null;

            newWitnessSets.SetText();

            witnessesToCheck[pageNumber] = witness;
        }
        else if (generatedCases[pageNumber, 7] == 1)
        {
            Destroy(witnessesToCheck[pageNumber].gameObject);

            GameObject witness = Instantiate(witnessPrefabs[0], spawner);
            CaseSets newWitnessSets = witness.GetComponent<CaseSets>();

            newWitnessSets.cvictimPlace = CaseSets.clocationType.None;
            newWitnessSets.cwitnesses = new int[1];
            newWitnessSets.cweaponSizeTypes = new CaseSets.cwSizeTypes[1];
            newWitnessSets.cweaponColorTypes = new CaseSets.cwColorTypes[1];

            if (generatedCases[pageNumber, 8] != 4)
            {
                newWitnessSets.cweaponSizeTypes[0] = (CaseSets.cwSizeTypes)generatedCases[pageNumber, 8];
                newWitnessSets.cweaponColorTypes = null;
            }
            if (generatedCases[pageNumber, 10] != 12)
            {
                newWitnessSets.cweaponColorTypes[0] = (CaseSets.cwColorTypes)generatedCases[pageNumber, 10];
                newWitnessSets.cweaponSizeTypes = null;
            }
            else
            {
                newWitnessSets.cweaponSizeTypes = null;
                newWitnessSets.cweaponColorTypes = null;
            }

            newWitnessSets.SetText();
            witnessesToCheck[pageNumber] = witness;
        }
        else
        {
            Destroy(witnessesToCheck[pageNumber].gameObject);

            GameObject witness = Instantiate(witnessPrefabs[1], spawner);
            CaseSets newWitnessSets = witness.GetComponent<CaseSets>();

            newWitnessSets.cvictimPlace = CaseSets.clocationType.None;
            newWitnessSets.cwitnesses = new int[2];
            newWitnessSets.cweaponSizeTypes = new CaseSets.cwSizeTypes[2];
            newWitnessSets.cweaponColorTypes = new CaseSets.cwColorTypes[2];

            int counter = 0;

            for (int c = 8; c < 10; c++)
            {
                if (generatedCases[pageNumber, c] != 4)
                {
                    newWitnessSets.cweaponSizeTypes[counter] = (CaseSets.cwSizeTypes)generatedCases[pageNumber, c];
                }
                else
                {
                    newWitnessSets.cweaponSizeTypes[counter] = CaseSets.cwSizeTypes.None;
                }

                counter++;
            }

            if (newWitnessSets.cweaponSizeTypes[0] != CaseSets.cwSizeTypes.None && newWitnessSets.cweaponSizeTypes[1] != CaseSets.cwSizeTypes.None)
            {
                if (newWitnessSets.cweaponSizeTypes[0] != newWitnessSets.cweaponSizeTypes[1])
                {
                    newWitnessSets.cweaponSizeTypes[1] = newWitnessSets.cweaponSizeTypes[0];
                }
            }

            if (newWitnessSets.cweaponSizeTypes[0] == CaseSets.cwSizeTypes.None && newWitnessSets.cweaponSizeTypes[1] == CaseSets.cwSizeTypes.None)
            {
                newWitnessSets.cweaponSizeTypes = null;
            }

            counter = 0;

            for (int c = 10; c < 12; c++)
            {
                if (generatedCases[pageNumber, c] != 12)
                {
                    newWitnessSets.cweaponColorTypes[counter] = (CaseSets.cwColorTypes)generatedCases[pageNumber, c];
                }
                else
                {
                    newWitnessSets.cweaponColorTypes[counter] = CaseSets.cwColorTypes.None;
                }

                counter++;
            }

            if (newWitnessSets.cweaponColorTypes[0] == CaseSets.cwColorTypes.None && newWitnessSets.cweaponColorTypes[1] == CaseSets.cwColorTypes.None)
            {
                newWitnessSets.cweaponColorTypes = null;
            }

            newWitnessSets.SetText();
            witnessesToCheck[pageNumber] = witness;
        }
    }

    public bool CheckCaseTest()
    {
        if (cSize == true && cDamage == true && IsColorTrue() == true && cLocation == true && cWitness == true)
        {
            if (boardTriggers[0].correctWeapon != null && boardTriggers[1].correctLocation != null && boardTriggers[2].correctWitness != null)
            {
                /*
                if (cColor != null)
                {
                    Debug.Log("case 1 is true because cColor is true");
                }
                else
                {
                    Debug.Log("Case 1 is true because cColorSingle is true");
                }
                */

                return true;
            }

            return false;
        }

        /*
        bool[] debugList = { cSize, cDamage, IsColorTrue(), cLocation, cWitness };
        string[] debugName = { "cSize", "cDamage", "IsColorTrue()", "cLocation", "cWitness" };

        int counter = 0;
        foreach (bool debug in debugList)
        {
            if (!debug)
            {
                Debug.Log("Case is not solved because " + debugName[counter] + " is not true");
            }

            counter++;
        }
        */

        return false;
    }

    private IEnumerator CheckCaseProbabilityMoreThanOnce()
    {
        foreach (AudioSource noise in allAmbientAudio)
        {
            noise.Stop();
        }

        playerPickup.enabled = false;
        playerCamera.enabled = false;
        playerMovement.enabled = false;

        int caseSolutionCount = 0;
        bool impossibleWitness = true;

        while (impossibleWitness)
        {
            for (int cas = 0; cas < amountOfCases; cas++)
            {
                solutionNumbers[cas].Clear();
                solutionActivity[cas].Clear();
            }

            for (int cas = 0; cas < amountOfCases; cas++)
            {
                for (int wep = 0; wep < weaponsToCheck.Length; wep++)
                {
                    for (int loc = 0; loc < locationsToCheck.Length; loc++)
                    {
                        for (int wit = 0; wit < witnessesToCheck.Length; wit++)
                        {
                            SetCase(cas);

                            boardTriggers[0].tempWeapon = null;
                            UpdateCaseWeapon(weaponsToCheck[wep]);
                            UpdateCaseWitness(witnessesToCheck[wit]);
                            UpdateCaseLocation(locationsToCheck[loc]);

                            if (CheckCaseTest())
                            {
                                solutionNumbers[cas].Add(allItemsToCheck.IndexOf(weaponsToCheck[wep]));
                                solutionNumbers[cas].Add(allItemsToCheck.IndexOf(witnessesToCheck[wit]));
                                solutionNumbers[cas].Add(allItemsToCheck.IndexOf(locationsToCheck[loc]));
                                solutionActivity[cas].Add(true);

                                caseSolutionCount++;

                                
                                //Debug.Log("Case " + (cas + 1) + " can be solved with: " + allItemsToCheck.IndexOf(weaponsToCheck[wep]) + ", " + allItemsToCheck.IndexOf(witnessesToCheck[wit]) + ", " + allItemsToCheck.IndexOf(locationsToCheck[loc]));
                                /*
                                string debugCaseTextW = witnessesToCheck[wit].GetComponent<CaseSets>().caseTextDescription[0];
                                if (witnessesToCheck[wit].GetComponent<CaseSets>().caseTextDescription.Length == 2)
                                {
                                    debugCaseTextW += " " + witnessesToCheck[wit].GetComponent<CaseSets>().caseTextDescription[1];
                                }

                                string debugCaseTextL = locationsToCheck[loc].GetComponent<CaseSets>().caseTextDescription[0];
                                if (locationsToCheck[loc].GetComponent<CaseSets>().caseTextDescription.Length == 2)
                                {
                                    debugCaseTextL += " " + locationsToCheck[loc].GetComponent<CaseSets>().caseTextDescription[1];
                                }

                                Debug.Log("Case " + (cas + 1) + " can be solved with: " + weaponsToCheck[wep] + ", " + debugCaseTextW + ", " + debugCaseTextL);
                                */
                            }
                        }
                    }
                }

                //Debug.Log("Case " + (cas + 1) + " has " + caseSolutionCount + " solutions, and " + solutionNumbers[cas].Count + " numbers in there");
                caseSolutionCount = 0;
                yield return null;
            }

            bool[] impossibleWitnessDetector = new bool[amountOfCases];

            for (int cas = 0; cas < amountOfCases; cas++)
            {
                for (int wit = 0; wit < witnessesToCheck.Length; wit++)
                {
                    if (solutionNumbers[cas].Contains(allItemsToCheck.IndexOf(witnessesToCheck[wit])))
                    {
                        impossibleWitnessDetector[wit] = true;
                    }
                }
            }

            yield return null;

            int firstIndex = 0;
            int nextIndex = 0;
            bool firstIndexSet = false;
            bool uniqueWeapon = false;

            bool impossibleWitnessThere = false;

            for (int cas = 0; cas < amountOfCases; cas++)
            {
                for (int wep = 0; wep < solutionNumbers[cas].Count; wep += 3)
                {
                    if (!firstIndexSet)
                    {
                        firstIndex = solutionNumbers[cas][wep];
                        firstIndexSet = true;
                    }
                    else
                    {
                        nextIndex = solutionNumbers[cas][wep];
                        if (nextIndex != firstIndex)
                        {
                            uniqueWeapon = true;
                            //Debug.Log("Case " + (cas + 1) + " weapon is unique");
                            break;
                        }
                    }

                    //Debug.Log("Case " + (cas + 1) + " weapon is " + wep);

                    if (wep == solutionNumbers[cas].Count - 3 && !uniqueWeapon)
                    {
                        //Debug.Log("Case " + (cas + 1) + " has only 1 weapon for their solution");
                        caseNumber = cas - 1;

                        //Debug.Log("1 Location Case is " + cas);

                        int locationIndex = weaponsToCheck.Length + witnessesToCheck.Length + cas;
                        int witnessIndex = weaponsToCheck.Length + cas;
                        //Debug.Log("1 locations to check is equal to " + locationsToCheck[cas]);
                        //Debug.Log("1 locationIndex is " + locationIndex + " & the true value is " + allItemsToCheck.IndexOf(locationsToCheck[cas]));
                        //Debug.Log("Location case " + cas + " is equal to " + locationsToCheck[cas]);
                        //GameObject locationToDestroy = locationsToCheck[cas].gameObject;
                        //Destroy(locationToDestroy);
                        //Destroy(witnessesToCheck[cas].gameObject);

                        //yield return null;

                        //Debug.Log("Location case " + cas + " is equal to " + locationsToCheck[cas]);
                        //Destroy(allItemsToCheck[locationIndex]);
                        //Destroy(allItemsToCheck[witnessIndex]);
                        CreateCase();
                        CreateNewPrefab(caseNumber);
                        allItemsToCheck[locationIndex] = locationsToCheck[cas];
                        //Debug.Log("1 allItemsToCheck location is equal to " + allItemsToCheck[locationIndex]);
                        allItemsToCheck[witnessIndex] = witnessesToCheck[cas];
                        impossibleWitnessThere = true;
                    }
                }

                firstIndex = 0;
                nextIndex = 0;
                firstIndexSet = false;
                uniqueWeapon = false;
            }

            yield return null;

            int[] witnessSawOneWep = new int[amountOfCases];
            int[] witnessOnlyWep = new int[amountOfCases];

            for (int cas = 0; cas < amountOfCases; cas++)
            {
                for (int wep = 0; wep < solutionNumbers[cas].Count; wep += 3)
                {
                    if (!firstIndexSet && solutionNumbers[cas][wep + 1] == allItemsToCheck.IndexOf(witnessesToCheck[cas]))
                    {
                        firstIndex = solutionNumbers[cas][wep];
                        firstIndexSet = true;
                    }
                    else
                    {
                        if (solutionNumbers[cas][wep + 1] == allItemsToCheck.IndexOf(witnessesToCheck[cas]))
                        {
                            nextIndex = solutionNumbers[cas][wep];
                            if (nextIndex != firstIndex)
                            {
                                uniqueWeapon = true;
                                //Debug.Log("Case " + (cas + 1) + " weapon is unique");
                                break;
                            }
                        }
                    }

                    //Debug.Log("Case " + (cas + 1) + " weapon is " + wep);

                    if (wep == solutionNumbers[cas].Count - 3 && !uniqueWeapon)
                    {
                        witnessSawOneWep[cas] = allItemsToCheck.IndexOf(witnessesToCheck[cas]);
                        witnessOnlyWep[cas] = firstIndex;

                        for (int firstWit = 0; firstWit < amountOfCases; firstWit++)
                        {
                            if (witnessSawOneWep[firstWit] != 0 && witnessSawOneWep[firstWit] != witnessSawOneWep[cas])
                            {
                                if (witnessOnlyWep[firstWit] == witnessOnlyWep[cas])
                                {
                                    witnessSawOneWep[cas] = 0;
                                    witnessOnlyWep[cas] = 0;

                                    //Debug.Log("Case " + (cas + 1) + " witness only has 1 weapon associated and it's the same weapon");
                                    caseNumber = cas - 1;

                                    //Debug.Log("1 Location Case is " + cas);

                                    int locationIndex = weaponsToCheck.Length + witnessesToCheck.Length + cas;
                                    int witnessIndex = weaponsToCheck.Length + cas;
                                    //Debug.Log("1 locations to check is equal to " + locationsToCheck[cas]);
                                    //Debug.Log("1 locationIndex is " + locationIndex + " & the true value is " + allItemsToCheck.IndexOf(locationsToCheck[cas]));
                                    //Destroy(locationsToCheck[cas].gameObject);
                                    //Destroy(witnessesToCheck[cas].gameObject);
                                    //Destroy(allItemsToCheck[locationIndex]);
                                    //Destroy(allItemsToCheck[witnessIndex]);
                                    CreateCase();
                                    CreateNewPrefab(caseNumber);
                                    allItemsToCheck[locationIndex] = locationsToCheck[cas];
                                    //Debug.Log("1 allItemsToCheck location is equal to " + allItemsToCheck[locationIndex]);
                                    allItemsToCheck[witnessIndex] = witnessesToCheck[cas];
                                    impossibleWitnessThere = true;
                                }
                            }
                        }
                    }
                }


                firstIndex = 0;
                nextIndex = 0;
                firstIndexSet = false;
                uniqueWeapon = false;
            }

            witnessSawOneWep = null;
            witnessOnlyWep = null;

            yield return null;

            for (int cas = 0; cas < amountOfCases; cas++)
            {
                if (solutionNumbers[cas].Count == 0)
                {
                    //Debug.Log("Case " + (cas + 1) + " has no solutions already");
                    caseNumber = cas - 1;

                    //Debug.Log("2 Location Case is " + cas);

                    int locationIndex = weaponsToCheck.Length + witnessesToCheck.Length + cas;
                    int witnessIndex = weaponsToCheck.Length + cas;
                    //Debug.Log("2 locations to check is equal to " + locationsToCheck[cas]);
                    //Debug.Log("2 locationIndex is " + locationIndex + " & the true value is " + allItemsToCheck.IndexOf(locationsToCheck[cas]));
                    //Debug.Log("Location case " + cas + " is equal to " + locationsToCheck[cas]);
                    //GameObject locationToDestroy = locationsToCheck[cas].gameObject;
                    //Destroy(locationToDestroy);
                    //Destroy(witnessesToCheck[cas].gameObject);

                    //yield return null;

                    //Debug.Log("Location case " + cas + " is equal to " + locationsToCheck[cas]);
                    //Destroy(allItemsToCheck[locationIndex]);
                    //Destroy(allItemsToCheck[witnessIndex]);
                    CreateCase();
                    CreateNewPrefab(caseNumber);
                    allItemsToCheck[locationIndex] = locationsToCheck[cas];
                    //Debug.Log("2 allItemsToCheck location is equal to " + allItemsToCheck[locationIndex]);
                    allItemsToCheck[witnessIndex] = witnessesToCheck[cas];
                    impossibleWitnessThere = true;
                }
            }

            yield return null;

            int dupeCount = 0;
            for (int cas = 0; cas < amountOfCases; cas++)
            {
                for (int nextCas = 0; nextCas < amountOfCases; nextCas++)
                {
                    if (solutionNumbers[cas].Count == solutionNumbers[nextCas].Count)
                    {
                        dupeCount++;

                    }

                    if (dupeCount == 3)
                    {
                        //Debug.Log("Case " + (nextCas + 1) + " has the same amount of solutions as Case " + (cas + 1));
                        caseNumber = nextCas - 1;

                        //Debug.Log("location case number is equal to " + caseNumber);

                        //Debug.Log("3 Location Case is " + cas);

                        int locationIndex = weaponsToCheck.Length + witnessesToCheck.Length + cas;
                        int witnessIndex = weaponsToCheck.Length + cas;
                        //Debug.Log("3 locations to check is equal to " + locationsToCheck[cas]);
                        //Debug.Log("3 locationIndex is " + locationIndex + " & the true value is " + allItemsToCheck.IndexOf(locationsToCheck[cas]));
                        //Debug.Log("Location case " + cas + " is equal to " + locationsToCheck[cas]);
                        //GameObject locationToDestroy = locationsToCheck[cas].gameObject;
                        //Destroy(locationToDestroy);
                        //Destroy(witnessesToCheck[cas].gameObject);

                        //yield return null;

                        //Debug.Log("Location case " + cas + " is equal to " + locationsToCheck[cas]);
                        //Destroy(allItemsToCheck[locationIndex]);
                        //Destroy(allItemsToCheck[witnessIndex]);
                        CreateCase();

                        //Debug.Log("location case number is equal to " + caseNumber);

                        CreateNewPrefab(caseNumber);
                        allItemsToCheck[locationIndex] = locationsToCheck[cas];
                        //Debug.Log("3 allItemsToCheck location is equal to " + allItemsToCheck[locationIndex]);
                        allItemsToCheck[witnessIndex] = witnessesToCheck[cas];
                        impossibleWitnessThere = true;
                    }
                }

                dupeCount = 0;
            }

            yield return null;

            for (int i = 0; i < amountOfCases; i++)
            {
                if (!impossibleWitnessDetector[i])
                {
                    //Debug.Log("Case " + (i + 1) + " has an impossible witness");
                    caseNumber = i - 1;

                    //Debug.Log("4 Location Case is " + i);

                    int locationIndex = weaponsToCheck.Length + witnessesToCheck.Length + i;
                    int witnessIndex = weaponsToCheck.Length + i;
                    //Debug.Log("4 locations to check is equal to " + locationsToCheck[i]);
                    //Debug.Log("4 locationIndex is " + locationIndex + " & the true value is " + allItemsToCheck.IndexOf(locationsToCheck[i]));
                    //Debug.Log("Location case " + i + " is equal to " + locationsToCheck[i]);
                    //GameObject locationToDestroy = locationsToCheck[i].gameObject;
                    //Destroy(locationToDestroy);
                    //Destroy(witnessesToCheck[i].gameObject);

                    //Debug.Log("Location case " + i + " is equal to " + locationsToCheck[i]);
                    //Destroy(allItemsToCheck[locationIndex]);
                    //Destroy(allItemsToCheck[witnessIndex]);
                    //yield return null;

                    CreateCase();
                    CreateNewPrefab(caseNumber);
                    allItemsToCheck[locationIndex] = locationsToCheck[i];
                    //Debug.Log("4 allItemsToCheck location is equal to " + allItemsToCheck[locationIndex]);
                    allItemsToCheck[witnessIndex] = witnessesToCheck[i];
                    impossibleWitnessThere = true;
                }
            }

            yield return null;

            
            int locationCounter = 0;
            for (int location = weaponsToCheck.Length + witnessesToCheck.Length; location < allItemsToCheck.Count; location++)
            {
                allItemsToCheck[location] = locationsToCheck[locationCounter];
                locationCounter++;
            }

            int witnessCounter = 0;
            for (int witness = weaponsToCheck.Length; witness < weaponsToCheck.Length + witnessesToCheck.Length; witness++)
            {
                allItemsToCheck[witness] = witnessesToCheck[witnessCounter];
                witnessCounter++;
            }

            yield return null;
            

            if (!impossibleWitnessThere)
            {
                impossibleWitness = false;
            }
        }

        boardTriggers[0].tempWeapon = null;
        boardTriggers[0].correctWeapon = null;
        boardTriggers[1].correctLocation = null;
        boardTriggers[2].correctWitness = null;
        cSize = false;
        cDamage = false;
        cColor = null;
        cColorSingle = false;
        cLocation = false;
        cWitness = false;
        witnesses = null;

        foreach (AudioSource noise in allAmbientAudio)
        {
            noise.Play();
        }

        UpdateCaseLocation(null);
        UpdateCaseWeapon(null);
        UpdateCaseWitness(null);

        loadingPanel.SetActive(false);

        playerPickup.enabled = true;
        playerCamera.enabled = true;
        playerMovement.enabled = true;

        GameSceneManager.instance.StartTime();

        SetCase(pageNumber);

        menuScreen.SetActive(false);
        settingsScreen.SetActive(false);

        radio.isPaused = false;

        updateWitnesses.AddRange(witnessesToCheck);
    }

    private void CheckCasePossibility()
    {
        // First set is weapons, then witnesses, then locations
        allItemsToCheck.AddRange(weaponsToCheck);
        allItemsToCheck.AddRange(witnessesToCheck);
        allItemsToCheck.AddRange(locationsToCheck);
        int caseSolutionCount = 0;
        bool impossibleWitness = true;
        int whileCounter = 0;

        radio.isPaused = true;

        while (impossibleWitness)
        {
            if (whileCounter == 1)
            {
                impossibleWitness = false;
                //Debug.Log("Hammer Time");
                StartCoroutine(CheckCaseProbabilityMoreThanOnce());
                loadingPanel.SetActive(true);
                whileCounter++;
                break;
            }

            for (int cas = 0; cas < amountOfCases; cas++)
            {
                solutionNumbers[cas].Clear();
                solutionActivity[cas].Clear();
            }

            for (int cas = 0; cas < amountOfCases; cas++)
            {
                for (int wep = 0; wep < weaponsToCheck.Length; wep++)
                {
                    for (int loc = 0; loc < locationsToCheck.Length; loc++)
                    {
                        for (int wit = 0; wit < witnessesToCheck.Length; wit++)
                        {
                            SetCase(cas);

                            boardTriggers[0].tempWeapon = null;
                            UpdateCaseWeapon(weaponsToCheck[wep]);
                            UpdateCaseLocation(locationsToCheck[loc]);
                            UpdateCaseWitness(witnessesToCheck[wit]);

                            if (CheckCaseTest())
                            {
                                solutionNumbers[cas].Add(allItemsToCheck.IndexOf(weaponsToCheck[wep]));
                                solutionNumbers[cas].Add(allItemsToCheck.IndexOf(witnessesToCheck[wit]));
                                solutionNumbers[cas].Add(allItemsToCheck.IndexOf(locationsToCheck[loc]));
                                solutionActivity[cas].Add(true);

                                caseSolutionCount++;

                                /*
                                string debugCaseTextW = witnessesToCheck[wit].GetComponent<CaseSets>().caseTextDescription[0];
                                if (witnessesToCheck[wit].GetComponent<CaseSets>().caseTextDescription.Length == 2)
                                {
                                    debugCaseTextW += " " + witnessesToCheck[wit].GetComponent<CaseSets>().caseTextDescription[1];
                                }

                                string debugCaseTextL = locationsToCheck[loc].GetComponent<CaseSets>().caseTextDescription[0];
                                if (locationsToCheck[loc].GetComponent<CaseSets>().caseTextDescription.Length == 2)
                                {
                                    debugCaseTextL += " " + locationsToCheck[loc].GetComponent<CaseSets>().caseTextDescription[1];
                                }

                                Debug.Log("Case " + (cas + 1) + " can be solved with: " + weaponsToCheck[wep] + ", " + debugCaseTextW + ", " + debugCaseTextL);
                                */
                                
                            }
                        }
                    }
                }

                //Debug.Log("Case " + (cas + 1) + " has " + caseSolutionCount + " solutions, and " + solutionNumbers[cas].Count + " numbers in there");
                caseSolutionCount = 0;
            }

            bool[] impossibleWitnessDetector = new bool[amountOfCases];

            for (int cas = 0; cas < amountOfCases; cas++)
            {
                for (int wit = 0; wit < witnessesToCheck.Length; wit++)
                {
                    if (solutionNumbers[cas].Contains(allItemsToCheck.IndexOf(witnessesToCheck[wit])))
                    {
                        impossibleWitnessDetector[wit] = true;
                    }
                }
            }

            int firstIndex = 0;
            int nextIndex = 0;
            bool firstIndexSet = false;
            bool uniqueWeapon = false;

            bool impossibleWitnessThere = false;

            for (int cas = 0; cas < amountOfCases; cas++)
            {
                for (int wep = 0; wep < solutionNumbers[cas].Count; wep += 3)
                {
                    if(!firstIndexSet)
                    {
                        firstIndex = solutionNumbers[cas][wep];
                        firstIndexSet = true;
                    }
                    else
                    {
                        nextIndex = solutionNumbers[cas][wep];
                        if (nextIndex != firstIndex)
                        {
                            uniqueWeapon = true;
                            //Debug.Log("Case " + (cas + 1) + " weapon is unique");
                            break;
                        }
                    }

                    //Debug.Log("Case " + (cas + 1) + " weapon is " + wep);

                    if (wep == solutionNumbers[cas].Count - 3 && !uniqueWeapon)
                    {
                        //Debug.Log("Case " + (cas + 1) + " has only 1 weapon for their solution");
                        caseNumber = cas - 1;

                        //Debug.Log("1 Location Case is " + cas);

                        int locationIndex = weaponsToCheck.Length + witnessesToCheck.Length + cas;
                        int witnessIndex = weaponsToCheck.Length + cas;
                        //Debug.Log("1 locations to check is equal to " + locationsToCheck[cas]);
                        //Debug.Log("1 locationIndex is " + locationIndex + " & the true value is " + allItemsToCheck.IndexOf(locationsToCheck[cas]));
                        //Destroy(locationsToCheck[cas].gameObject);
                        //Destroy(witnessesToCheck[cas].gameObject);
                        //Destroy(allItemsToCheck[locationIndex]);
                        //Destroy(allItemsToCheck[witnessIndex]);
                        CreateCase();
                        CreateNewPrefab(caseNumber);
                        allItemsToCheck[locationIndex] = locationsToCheck[cas];
                        //Debug.Log("1 allItemsToCheck location is equal to " + allItemsToCheck[locationIndex]);
                        allItemsToCheck[witnessIndex] = witnessesToCheck[cas];
                        impossibleWitnessThere = true;
                    }
                }

                firstIndex = 0;
                nextIndex = 0;
                firstIndexSet = false;
                uniqueWeapon = false;
            }

            int[] witnessSawOneWep = new int[amountOfCases];
            int[] witnessOnlyWep = new int[amountOfCases];

            for (int cas = 0; cas < amountOfCases; cas++)
            {
                for (int wep = 0; wep < solutionNumbers[cas].Count; wep += 3)
                {
                    if (!firstIndexSet && solutionNumbers[cas][wep + 1] == allItemsToCheck.IndexOf(witnessesToCheck[cas]))
                    {
                        firstIndex = solutionNumbers[cas][wep];
                        firstIndexSet = true;
                    }
                    else
                    {
                        if (solutionNumbers[cas][wep + 1] == allItemsToCheck.IndexOf(witnessesToCheck[cas]))
                        {
                            nextIndex = solutionNumbers[cas][wep];
                            if (nextIndex != firstIndex)
                            {
                                uniqueWeapon = true;
                                //Debug.Log("Case " + (cas + 1) + " weapon is unique");
                                break;
                            }
                        }
                    }

                    //Debug.Log("Case " + (cas + 1) + " weapon is " + wep);

                    if (wep == solutionNumbers[cas].Count - 3 && !uniqueWeapon)
                    {
                        witnessSawOneWep[cas] = allItemsToCheck.IndexOf(witnessesToCheck[cas]);
                        witnessOnlyWep[cas] = firstIndex;

                        for (int firstWit = 0; firstWit < amountOfCases; firstWit++)
                        {
                            if (witnessSawOneWep[firstWit] != 0 && witnessSawOneWep[firstWit] != witnessSawOneWep[cas])
                            {
                                if (witnessOnlyWep[firstWit] == witnessOnlyWep[cas])
                                {
                                    witnessSawOneWep[cas] = 0;
                                    witnessOnlyWep[cas] = 0;

                                    //Debug.Log("Case " + (cas + 1) + " witness only has 1 weapon associated and it's the same weapon");
                                    caseNumber = cas - 1;

                                    //Debug.Log("1 Location Case is " + cas);

                                    int locationIndex = weaponsToCheck.Length + witnessesToCheck.Length + cas;
                                    int witnessIndex = weaponsToCheck.Length + cas;
                                    //Debug.Log("1 locations to check is equal to " + locationsToCheck[cas]);
                                    //Debug.Log("1 locationIndex is " + locationIndex + " & the true value is " + allItemsToCheck.IndexOf(locationsToCheck[cas]));
                                    //Destroy(locationsToCheck[cas].gameObject);
                                    //Destroy(witnessesToCheck[cas].gameObject);
                                    //Destroy(allItemsToCheck[locationIndex]);
                                    //Destroy(allItemsToCheck[witnessIndex]);
                                    CreateCase();
                                    CreateNewPrefab(caseNumber);
                                    allItemsToCheck[locationIndex] = locationsToCheck[cas];
                                    //Debug.Log("1 allItemsToCheck location is equal to " + allItemsToCheck[locationIndex]);
                                    allItemsToCheck[witnessIndex] = witnessesToCheck[cas];
                                    impossibleWitnessThere = true;
                                }
                            }
                        }
                    }
                }


                firstIndex = 0;
                nextIndex = 0;
                firstIndexSet = false;
                uniqueWeapon = false;
            }

            witnessSawOneWep = null;
            witnessOnlyWep = null;

            for (int cas = 0; cas < amountOfCases; cas++)
            {
                if (solutionNumbers[cas].Count == 0)
                {
                    //Debug.Log("Case " + (cas + 1) + " has no solutions already");
                    caseNumber = cas - 1;
                    //Debug.Log("1 Location Case is " + cas);

                    int locationIndex = weaponsToCheck.Length + witnessesToCheck.Length + cas;
                    int witnessIndex = weaponsToCheck.Length + cas;
                    //Debug.Log("1 locations to check is equal to " + locationsToCheck[cas]);
                    //Debug.Log("1 locationIndex is " + locationIndex + " & the true value is " + allItemsToCheck.IndexOf(locationsToCheck[cas]));
                    //Destroy(locationsToCheck[cas].gameObject);
                    //Destroy(witnessesToCheck[cas].gameObject);
                    //Destroy(allItemsToCheck[locationIndex]);
                    //Destroy(allItemsToCheck[witnessIndex]);
                    CreateCase();
                    CreateNewPrefab(caseNumber);
                    allItemsToCheck[locationIndex] = locationsToCheck[cas];
                    //Debug.Log("1 allItemsToCheck location is equal to " + allItemsToCheck[locationIndex]);
                    allItemsToCheck[witnessIndex] = witnessesToCheck[cas];
                    impossibleWitnessThere = true;
                }
            }

            int dupeCount = 0;
            for (int cas = 0; cas < amountOfCases; cas++)
            {
                for (int nextCas = 0; nextCas < amountOfCases; nextCas++)
                {
                    if (solutionNumbers[cas].Count == solutionNumbers[nextCas].Count)
                    {
                        dupeCount++;
                        
                    }

                    if (dupeCount == 3)
                    {
                        //Debug.Log("Case " + (nextCas + 1) + " has the same amount of solutions as Case " + (cas + 1));
                        caseNumber = nextCas - 1;
                        //Debug.Log("1 Location Case is " + cas);

                        int locationIndex = weaponsToCheck.Length + witnessesToCheck.Length + cas;
                        int witnessIndex = weaponsToCheck.Length + cas;
                        //Debug.Log("1 locations to check is equal to " + locationsToCheck[cas]);
                        //Debug.Log("1 locationIndex is " + locationIndex + " & the true value is " + allItemsToCheck.IndexOf(locationsToCheck[cas]));
                        //Destroy(locationsToCheck[cas].gameObject);
                        //Destroy(witnessesToCheck[cas].gameObject);
                        //Destroy(allItemsToCheck[locationIndex]);
                        //Destroy(allItemsToCheck[witnessIndex]);
                        CreateCase();
                        CreateNewPrefab(caseNumber);
                        allItemsToCheck[locationIndex] = locationsToCheck[cas];
                        //Debug.Log("1 allItemsToCheck location is equal to " + allItemsToCheck[locationIndex]);
                        allItemsToCheck[witnessIndex] = witnessesToCheck[cas];
                        impossibleWitnessThere = true;
                    }
                }

                dupeCount = 0;
            }

            for (int i = 0; i < amountOfCases; i++)
            {
                if (!impossibleWitnessDetector[i])
                {
                    //Debug.Log("Case " + (i + 1) + " has an impossible witness");
                    caseNumber = i - 1;
                    //Debug.Log("1 Location Case is " + i);

                    int locationIndex = weaponsToCheck.Length + witnessesToCheck.Length + i;
                    int witnessIndex = weaponsToCheck.Length + i;
                    //Debug.Log("1 locations to check is equal to " + locationsToCheck[i]);
                    //Debug.Log("1 locationIndex is " + locationIndex + " & the true value is " + allItemsToCheck.IndexOf(locationsToCheck[i]));
                    //Debug.Log("Location case " + i + " is equal to " + locationsToCheck[i]);
                    //Destroy(locationsToCheck[i].gameObject);
                    //Debug.Log("Location case " + i + " is equal to " + locationsToCheck[i]);
                    //Destroy(witnessesToCheck[i].gameObject);
                    //Destroy(allItemsToCheck[locationIndex]);
                    //Destroy(allItemsToCheck[witnessIndex]);
                    CreateCase();
                    CreateNewPrefab(caseNumber);
                    allItemsToCheck[locationIndex] = locationsToCheck[i];
                    //Debug.Log("1 allItemsToCheck location is equal to " + allItemsToCheck[locationIndex]);
                    allItemsToCheck[witnessIndex] = witnessesToCheck[i];
                    impossibleWitnessThere = true;
                }
            }

            int locationCounter = 0;
            for (int location = weaponsToCheck.Length + witnessesToCheck.Length; location < allItemsToCheck.Count; location++)
            {
                allItemsToCheck[location] = locationsToCheck[locationCounter];
                locationCounter++;
            }

            int witnessCounter = 0;
            for (int witness = weaponsToCheck.Length; witness < weaponsToCheck.Length + witnessesToCheck.Length; witness++)
            {
                allItemsToCheck[witness] = witnessesToCheck[witnessCounter];
                witnessCounter++;
            }

            if (!impossibleWitnessThere)
            {
                impossibleWitness = false;
            }

            whileCounter++;
        }

        if (whileCounter == 1)
        { 
            GameSceneManager.instance.StartTime();
            boardTriggers[0].tempWeapon = null;
            boardTriggers[0].correctWeapon = null;
            boardTriggers[1].correctLocation = null;
            boardTriggers[2].correctWitness = null;
            cSize = false;
            cDamage = false;
            cColor = null;
            cColorSingle = false;
            cLocation = false;
            cWitness = false;
            witnesses = null;

            foreach (AudioSource noise in allAmbientAudio)
            {
                noise.Play();
            }

            UpdateCaseLocation(null);
            UpdateCaseWeapon(null);
            UpdateCaseWitness(null);

            loadingPanel.SetActive(false);

            playerPickup.enabled = true;
            playerCamera.enabled = true;
            playerMovement.enabled = true;

            SetCase(pageNumber);

            menuScreen.SetActive(false);
            settingsScreen.SetActive(false);

            radio.isPaused = false;

            updateWitnesses.AddRange(witnessesToCheck);
        }
    }

    private void UpdateCaseProbablity(GameObject[] items)
    {
        int threeStepCounter = 0;
        int witnessIndex = 0;
        int locationIndex = 0;
        int weaponIndex = 0;
        bool hasLostLocal = true;
        bool firstLost = false;
        int caseSolutionCount = 0;
        int byThree = 1;

        for (int i = 0; i < witnessesToCheck.Length; i++)
        {
            if (witnessesToCheck[i] == items[2])
            {
                witnessIndex = allItemsToCheck.IndexOf(witnessesToCheck[i]);
                updateWitnesses.Remove(witnessesToCheck[i]);
                break;
            }
        }

        for (int i = 0; i < weaponsToCheck.Length; i++)
        {
            if (weaponsToCheck[i] == items[0])
            {
                weaponIndex = allItemsToCheck.IndexOf(weaponsToCheck[i]);
                break;
            }
        }

        for (int i = 0; i < locationsToCheck.Length; i++)
        {
            if (locationsToCheck[i] == items[1])
            {
                locationIndex = allItemsToCheck.IndexOf(locationsToCheck[i]);
                break;
            }
        }

        for (int cas = 0; cas < amountOfCases; cas++)
        {
            for (int i = 0; i < solutionNumbers[cas].Count; i++)
            {
                //Debug.Log("Step 1 count");
                if (solutionNumbers[cas][i] == witnessIndex)
                {
                    //Debug.Log("Step 2 count");
                    solutionNumbers[cas][i - 1] = 1000;
                    solutionNumbers[cas][i] = 1000;
                    solutionNumbers[cas][i + 1] = 1000;
                    solutionActivity[cas][threeStepCounter] = false;
                }

                if (solutionNumbers[cas][i] == weaponIndex)
                {
                    solutionNumbers[cas][i] = 1000;
                    solutionNumbers[cas][i + 1] = 1000;
                    solutionNumbers[cas][i + 2] = 1000;
                    solutionActivity[cas][threeStepCounter] = false;
                }

                if (solutionNumbers[cas][i] == locationIndex)
                {
                    solutionNumbers[cas][i - 2] = 1000;
                    solutionNumbers[cas][i - 1] = 1000;
                    solutionNumbers[cas][i] = 1000;
                    solutionActivity[cas][threeStepCounter] = false;
                }

                if (byThree == 3)
                {
                    //Debug.Log("Step 3 count");
                    //threeStepCounter = Mathf.Min(threeStepCounter++, solutionActivity[cas].Count - 1);
                    threeStepCounter++;
                    //Debug.Log("i is " + i + " and threeStepCounter is " + threeStepCounter);
                    byThree = 0;
                }

                byThree++;
            }

            threeStepCounter = 0;
        }

        bool[] impossibleWitnessDetector = new bool[amountOfCases - pageNumber - 1];

        for (int cas = pageNumber + 1; cas < amountOfCases; cas++)
        {
            for (int wit = 0; wit < updateWitnesses.Count; wit++)
            {
                if (solutionNumbers[cas].Contains(allItemsToCheck.IndexOf(updateWitnesses[wit])))
                {
                    impossibleWitnessDetector[wit] = true;
                }
            }
        }

        /*
        foreach (bool yes in impossibleWitnessDetector)
        {
            if(yes)
            {
                Debug.Log("We're so back");
            }
            else
            {
                Debug.Log("It's so over");
            }
        }
        */

        for (int cas = pageNumber + 1; cas < amountOfCases; cas++)
        {
            hasLostLocal = true;

            for (int i = 0; i < solutionActivity[cas].Count; i++)
            {
                if (solutionActivity[cas][i] == true)
                {
                    hasLostLocal = false;
                    caseSolutionCount++;
                }

                if (!impossibleWitnessDetector[cas - pageNumber - 1])
                {
                    hasLostLocal = true;
                }

                if (i == solutionActivity[cas].Count - 1 && hasLostLocal == true && firstLost == false)
                {
                    if (!impossibleWitnessDetector[cas - pageNumber - 1])
                    {
                        UpdateCaseWitness(updateWitnesses[cas - pageNumber - 1]);
                        wallTextCaption.text = "Cause:";
                        boardTexts[7].text = $"( {witnessWallText} ) no longer associates to anything";
                    }
                    else
                    {
                        woundType failVictimWound = (woundType)generatedCases[cas, 5];
                        woundSeverity failVictimSeverity = (woundSeverity)generatedCases[cas, 6];
                        string failWoundText = "";

                        if (failVictimSeverity != woundSeverity.None)
                            failWoundText = failVictimSeverity + " " + failVictimWound;
                        else
                            failWoundText = failVictimWound.ToString();

                        wallTextCaption.text = $"Case {cas} Wound:";
                        boardTexts[7].text = woundText;
                    }

                    hasLost = true;
                    hasLostIndex = cas;
                    firstLost = true;
                }
            }

            //Debug.Log("Case " + (cas + 1) + " now has " + caseSolutionCount + " solutions");

            caseSolutionCount = 0;
        }
    }

    public void HasLost()
    {
        universalSound.PlayOneShot(universalSounds[2]);
        lights[0].SetActive(false);
        lights[1].SetActive(false);
        lights[6].SetActive(false);
        lights[7].SetActive(false);
        GameObject evidenceLostAt = evidenceBoxs[hasLostIndex];
        MeshRenderer evidenceLostAtMesh = evidenceLostAt.GetComponent<MeshRenderer>();
        evidenceLostAtMesh.material = evidenceBoxLose;
        GameSceneManager.instance.music.enabled = false;
        GameSceneManager.instance.StopTime();
    }

    public void HasWon()
    {
        if(SettingsScript.instance.tutorialMode)
        {
            PlayerPrefs.SetInt("tutorial", 0);
        }

        universalSound.PlayOneShot(universalSounds[1]);
        lights[0].SetActive(false);
        PlayerPrefs.SetInt("hasBeatenThisSeed", 1);
        int caseCompleted = PlayerPrefs.GetInt("casesCompleted");
        caseCompleted++;
        PlayerPrefs.SetInt("casesCompleted", caseCompleted);
        GameSceneManager.instance.SetCasesCompleted(caseCompleted);
        GameSceneManager.instance.StopTime();
        if (GameSceneManager.instance.timer < GameSceneManager.instance.bestTimer && GameSceneManager.instance.bestTimer != 0 && GameSceneManager.instance.timer != 0)
        {
            PlayerPrefs.SetFloat("bestTime", GameSceneManager.instance.timer);
            GameSceneManager.instance.bestTimer = GameSceneManager.instance.timer;
            GameSceneManager.instance.SetBestTimeText();
        }
        else if (GameSceneManager.instance.bestTimer == 0)
        {
            PlayerPrefs.SetFloat("bestTime", GameSceneManager.instance.timer);
            GameSceneManager.instance.bestTimer = GameSceneManager.instance.timer;
            GameSceneManager.instance.SetBestTimeText();
        }
    }

    public void Lights()
    {
        if(hasWon)
        {
            lights[2].SetActive(true);
        }
        else if (hasLost)
        {
            lights[5].SetActive(true);
        }
    }

    public IEnumerator CaseSolved(GameObject[] items)
    {
        cSize = false;
        cDamage = false;
        cColor = null;
        cColorSingle = false;
        cLocation = false;

        boardTriggers[0].correctWeapon = null;
        boardTriggers[1].correctLocation = null;
        boardTriggers[2].correctWitness = null;

        if (pageNumber == amountOfCases - 1)
        {
            boardTexts[2].text = "Cases Completed: " + amountOfCases + "/" + amountOfCases;
            winOrLoseText[0].SetActive(true);
            GameObject evidenceBox = evidenceBoxs[pageNumber];
            MeshRenderer evidenceBoxMesh = evidenceBox.GetComponent<MeshRenderer>();
            evidenceBoxMesh.material = evidenceBoxMat;
            hasWon = true;
            HasWon();
            Invoke("Lights", 0.95f);
        }
        else
        {
            if (hasLost)
            {
                GameObject evidenceBox = evidenceBoxs[pageNumber];
                MeshRenderer evidenceBoxMesh = evidenceBox.GetComponent<MeshRenderer>();
                evidenceBoxMesh.material = evidenceBoxMat;
                winOrLoseText[1].SetActive(true);
                HasLost();
                Invoke("Lights", 1.91f);
            }
            else
            {
                GameObject evidenceBox = evidenceBoxs[pageNumber];
                MeshRenderer evidenceBoxMesh = evidenceBox.GetComponent<MeshRenderer>();
                evidenceBoxMesh.material = evidenceBoxMat;
                pageNumber = Mathf.Min(pageNumber + 1, amountOfCases - 1);
                SetCase(pageNumber);
            }
        }

        foreach (GameObject item in items)
        {
            item.layer = 9;
            //Collider weaponCollider = item.GetComponent<Collider>();
            //weaponCollider.enabled = false;
        }

        float timer = 0;
        while (timer <= 4f)
        {
            foreach (GameObject item in items)
            {
                item.transform.position = Vector3.Lerp(item.transform.position, caseBox.transform.position, timer / 50f);
                item.transform.localScale = Vector3.Lerp(item.transform.localScale, Vector3.zero, timer / 1000f);
            }

            timer += Time.deltaTime;

            yield return null;
        }

        foreach (GameObject item in items)
        {
            item.SetActive(false);
        }

        if(!hasLost)
        {
            caseSolvedStarted = false;
        }
    }

    public IEnumerator CaseUnsolved()
    {
        MeshRenderer color = caseBox.GetComponent<MeshRenderer>();
        Color originalColor = color.material.color;

        float timer = 0;
        while (timer <= 1f)
        {
            color.material.color = Color.Lerp(color.material.color, Color.red, timer / 10f);

            timer += Time.deltaTime;

            yield return null;
        }

        timer = 0;
        while (timer <= 1f)
        {
            color.material.color = Color.Lerp(color.material.color, originalColor, timer / 10f);

            timer += Time.deltaTime;

            yield return null;
        }

        caseFailedStarted = false;
    }
}
