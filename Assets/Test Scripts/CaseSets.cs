using UnityEngine;
using TMPro;

public class CaseSets : MonoBehaviour
{
    public enum clocationType { Greenhouse, Office, Workshop, None };
    public clocationType cvictimPlace = clocationType.None;

    public int[] cwitnesses = null;

    public enum cwSizeTypes { Small, Medium, Large, None };
    public cwSizeTypes[] cweaponSizeTypes = null;

    public enum cwColorTypes { Red, Orange, Yellow, Green, Blue, Purple, Pink, Brown, White, Grey, Black, None };
    public cwColorTypes[] cweaponColorTypes = null;

    //public TextMeshProUGUI[] caseText;

    public string[] caseTextDescription;

    public MeshRenderer photo;

    public Material[] locationTex;
    public Material[] sizeTex;
    public Material[] colorTex;
    public Material[] noWitnessTex;


    public void SetText()
    {
        Material[] matsToSet = photo.materials;

        if (cvictimPlace != clocationType.None)
        {
            //caseText[0] = transform.GetComponentInChildren<TextMeshProUGUI>();

            //caseText[0].text = "<size=80%>Location:<size=150%>" + "\n" + cvictimPlace;
            caseTextDescription[0] = "Location: " + cvictimPlace;

            matsToSet[1] = locationTex[(int)cvictimPlace];
        }
        else
        {
            if (cwitnesses != null)
            {
                if (cwitnesses.Length == 1)
                {
                    //caseText[0] = transform.GetComponentInChildren<TextMeshProUGUI>();

                    if (cweaponSizeTypes != null)
                    {
                        //caseText[0].text = "<size=80%>Witness:<size=200%>" + "\n" + cweaponSizeTypes[0] + "\n" + "Weapon";
                        caseTextDescription[0] = "Witness: " + cweaponSizeTypes[0] + " Weapon";

                        matsToSet[1] = sizeTex[(int)cweaponSizeTypes[0]];
                    }
                    else
                    {
                        if (cweaponColorTypes != null)
                        {
                            //caseText[0].text = "<size=80%>Witness:<size=200%>" + "\n" + cweaponColorTypes[0] + "\n" + "Weapon";
                            caseTextDescription[0] = "Witness: " + cweaponColorTypes[0] + " Weapon";

                            matsToSet[1] = colorTex[(int)cweaponColorTypes[0]];
                        }
                        else
                        {
                            //caseText[0].text = "<size=80%>Witness:<size=200%>" + "\n" + "Nothing";
                            caseTextDescription[0] = "Witness: Nothing";

                            matsToSet[1] = noWitnessTex[0];
                        }
                    }
                }
                else
                {
                    if (cweaponSizeTypes == null)
                    {
                        //Debug.Log("Size is null");
                    }
                    if (cweaponColorTypes == null)
                    {
                        //Debug.Log("Color is null");
                    }

                    if (cweaponSizeTypes == null)
                    {
                        if (cweaponColorTypes == null)
                        {
                            //caseText[0].text = "<size=80%>Witness:<size=200%>" + "\n" + "Nothing";
                            //caseText[1].text = "<size=80%>Witness:<size=200%>" + "\n" + "Nothing";

                            caseTextDescription[0] = "Witness: Nothing";
                            caseTextDescription[1] = "Witness: Nothing";

                            matsToSet[1] = noWitnessTex[0];
                            matsToSet[2] = noWitnessTex[0];
                        }
                        else if (cweaponColorTypes != null)
                        {
                            int counter = 0;

                            foreach (cwColorTypes w in cweaponColorTypes)
                            {
                                if (w == cwColorTypes.None)
                                {
                                    //caseText[counter].text = "<size=80%>Witness:<size=200%>" + "\n" + "Nothing";
                                    caseTextDescription[counter] = "Witness: Nothing";

                                    matsToSet[counter + 1] = noWitnessTex[0];
                                }
                                else
                                {
                                    //caseText[counter].text = "<size=80%>Witness:<size=200%>" + "\n" + cweaponColorTypes[counter] + "\n" + "Weapon";
                                    caseTextDescription[counter] = "Witness: " + cweaponColorTypes[counter] + " Weapon";

                                    matsToSet[counter + 1] = colorTex[(int)cweaponColorTypes[counter]];
                                }

                                counter++;
                            }
                        }
                        
                    }
                    else if (cweaponSizeTypes != null)
                    {
                        if (cweaponColorTypes == null)
                        {
                            int counter = 0;

                            foreach (cwSizeTypes w in cweaponSizeTypes)
                            {
                                if (w == cwSizeTypes.None)
                                {
                                    //caseText[counter].text = "<size=80%>Witness:<size=200%>" + "\n" + "Nothing";
                                    caseTextDescription[counter] = "Witness: Nothing";

                                    matsToSet[counter + 1] = noWitnessTex[0];
                                }
                                else
                                {
                                    //caseText[counter].text = "<size=80%>Witness:<size=200%>" + "\n" + cweaponSizeTypes[counter] + "\n" + "Weapon";
                                    caseTextDescription[counter] = "Witness: " + cweaponSizeTypes[counter] + " Weapon";

                                    matsToSet[counter + 1] = sizeTex[(int)cweaponSizeTypes[counter]];
                                }

                                counter++;
                            }
                        }
                        else if (cweaponColorTypes != null)
                        {
                            int counter = 0;
                            foreach (cwColorTypes w in cweaponColorTypes)
                            {
                                if (w == cwColorTypes.None)
                                {
                                    if (cweaponSizeTypes[counter] == cwSizeTypes.None)
                                    {
                                        //caseText[counter].text = "<size=80%>Witness:<size=200%>" + "\n" + "Nothing";
                                        caseTextDescription[counter] = "Witness: Nothing";

                                        matsToSet[counter + 1] = noWitnessTex[0];
                                    }
                                    else if (cweaponSizeTypes[counter] != cwSizeTypes.None)
                                    {
                                        //caseText[counter].text = "<size=80%>Witness:<size=200%>" + "\n" + cweaponSizeTypes[counter] + "\n" + "Weapon";
                                        caseTextDescription[counter] = "Witness: " + cweaponSizeTypes[counter] + " Weapon";

                                        matsToSet[counter + 1] = sizeTex[(int)cweaponSizeTypes[counter]];
                                    }
                                }
                                else if (w != cwColorTypes.None)
                                {
                                    //caseText[counter].text = "<size=80%>Witness:<size=200%>" + "\n" + cweaponColorTypes[counter] + "\n" + "Weapon";
                                    caseTextDescription[counter] = "Witness: " + cweaponColorTypes[counter] + " Weapon";

                                    matsToSet[counter + 1] = colorTex[(int)cweaponColorTypes[counter]];
                                }

                                counter++;
                            }
                        }
                    }
                }
            }
            else
            {
                //caseText = new TextMeshProUGUI[1];
                //caseText[0] = transform.GetComponentInChildren<TextMeshProUGUI>();

                //caseText[0].text = "<size=150%>No\nWitnesses";
                caseTextDescription[0] = "No Witnesses";

                matsToSet[1] = noWitnessTex[1];
            }
        }

        photo.materials = matsToSet;

        /*
        foreach (TextMeshProUGUI t in caseText)
        {
            t.text.Replace("\n", "\n");
        }
        */
    }
}
