using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class InteractText : MonoBehaviour
{
    public PickUpTest grabScript;
    public Camera playerCam;
    public GameObject grabTextObj;
    public TextMeshProUGUI grabText;
    public TextMeshProUGUI detailsText;
    public GameObject detailsTextObj;
    public GameObject grabTextPanel;
    public GameObject detailsTextPanel;
    public Rigidbody isMoving = null;
    public ObjectAttributes objA = null;
    public CaseSets objB = null;

    public GameObject tempObjectLooking;

    public string sizeText;
    public string colorText;
    public string typeText;
    public string categoryText;

    private bool isLooking = false;
    private bool detailPressed = false;
    private bool detailOn = false;

    public CaseGenerator pageNumber;

    public InputActionReference detailAction;
    private void OnEnable()
    {
        detailAction.action.Enable();
    }

    private void OnDisable()
    {
        detailAction.action.Disable();
    }

    private void Update()
    {
        if (grabScript.heldObj == null)
        {
            RaycastHit hit;
            if (Physics.Raycast(playerCam.transform.position, playerCam.transform.TransformDirection(Vector3.forward), out hit, grabScript.pickUpRange))
            {
                if (hit.transform.gameObject.GetComponent<Rigidbody>() != null)
                {
                    isMoving = hit.transform.gameObject.GetComponent<Rigidbody>();

                    if (isMoving.linearVelocity == Vector3.zero)
                    {
                        if (hit.transform.gameObject.tag == "canPickup" && isLooking == false)
                        {
                            objA = hit.transform.gameObject.GetComponent<ObjectAttributes>();
                            objB = hit.transform.gameObject.GetComponent<CaseSets>();

                            SpawnText(hit.transform.gameObject);

                            if (detailOn)
                            {
                                SpawnDetails();
                            }

                            isLooking = true;

                            tempObjectLooking = hit.transform.gameObject;
                        }
                        else if (hit.transform.gameObject.tag == "canPickup" && isLooking == true)
                        {
                            if (tempObjectLooking == hit.transform.gameObject) 
                            {
                                //Debug.Log("True");
                                if (detailAction.action.WasPressedThisFrame())
                                {
                                    //Debug.Log("Pressed");
                                    if (hit.transform.gameObject.GetComponent<ObjectAttributes>())
                                    {
                                        detailPressed = true;
                                        SpawnDetails();
                                    }
                                }
                            }
                            else
                            {
                                objA = hit.transform.gameObject.GetComponent<ObjectAttributes>();
                                objB = hit.transform.gameObject.GetComponent<CaseSets>();

                                SpawnText(hit.transform.gameObject);

                                SpawnDetails();

                                tempObjectLooking = hit.transform.gameObject;
                            }
                        }
                    }
                    else
                        DespawnText();
                }
                else
                    DespawnText();

            }
            else
                DespawnText();
        }
        else
            DespawnText();
    }

    private void SpawnText(GameObject interactable)
    {
        transform.position = interactable.transform.position;
        transform.LookAt(playerCam.transform);

        if (objA != null)
        {
            if (!SettingsScript.instance.hardMode)
            {
                if (objA.objSize == 1f)
                    sizeText = " inch (";
                else
                    sizeText = " inches (";

                if (objA.objColorTypes.Length > 1)
                {
                    colorText = "Colors: " + objA.objColorTypes[0];

                    for (int i = 1; i < objA.objColorTypes.Length; i++)
                        colorText += ", " + objA.objColorTypes[i];
                }
                else
                    colorText = ("Color: " + objA.objColorTypes[0]);

                if (objA.objDamageTypes.Length > 1)
                {
                    typeText = "Types: " + objA.objDamageTypes[0];

                    for (int i = 1; i < objA.objDamageTypes.Length; i++)
                        typeText += ", " + objA.objDamageTypes[i];
                }
                else
                    typeText = ("Type: " + objA.objDamageTypes[0]);

                if (objA.objCategoryTypes.Length > 1)
                {
                    categoryText = "Places Found: " + objA.objCategoryTypes[0];

                    for (int i = 1; i < objA.objCategoryTypes.Length; i++)
                        categoryText += ", " + objA.objCategoryTypes[i];
                }
                else
                    categoryText = ("Places Found: " + objA.objCategoryTypes[0]);

                grabText.text = ("<size=130%>" + objA.objName + "<size=100%>" + "\n" + "Size: " + objA.objSize.ToString() + sizeText + objA.objSizeType + ")" + "\n" + colorText + "\n" + typeText + "\n" + categoryText + "\n" + "\n" + "<size=70%>Q for details");
            }
            else
            {
                grabText.text = ("<size=130%>" + objA.objName + "\n" + "\n" + "<size=70%>Q for details");
            }

            if(SettingsScript.instance.tutorialMode && !objA.tutorialSafe && pageNumber.pageNumber < 2)
            {
                grabText.text = ("<size=130%>" + objA.objName + "\n" + "\n" + "<size=70%>Q for details");
            }
        }
        else if (objB != null)
        {
            if (!SettingsScript.instance.hardMode)
            {
                grabText.text = objB.caseTextDescription[0];

                if (objB.caseTextDescription.Length == 2)
                {
                    grabText.text += "\n+\n" + objB.caseTextDescription[1];
                }

                grabText.text += "\n\n<size=80%>E to Interact";
            }
            else
            {
                grabText.text = "E to Interact";
            }
                
        }
        
        grabText.text.Replace("\n", "\n");
        grabTextObj.SetActive(true);
        grabTextPanel.SetActive(true);
    }

    private void SpawnDetails()
    {
        if (objA != null)
        {
            if(detailPressed)
            {
                detailsText.text = ("\"" + objA.objDescription + "\"");
                detailsTextObj.SetActive(!detailsTextObj.activeSelf);
                detailsTextPanel.SetActive(!detailsTextPanel.activeSelf);
                detailPressed = false;
                if(detailsTextPanel.activeSelf)
                {
                    detailOn = true;
                }
                else
                {
                    detailOn = false;
                }
            }
            else
            {
                detailsText.text = ("\"" + objA.objDescription + "\"");
                if(detailOn)
                {
                    detailsTextObj.SetActive(true);
                    detailsTextPanel.SetActive(true);
                }
            }
        }
    }

    private void DespawnText()
    {
        grabTextObj.SetActive(false);
        grabTextPanel.SetActive(false);
        detailsTextObj.SetActive(false);
        detailsTextPanel.SetActive(false);
        isLooking = false;
    }
}
