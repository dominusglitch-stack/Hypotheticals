using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectAttributes : MonoBehaviour
{
    public string objName;
    public float objSize;

    public enum sizeTypes {Small, Medium, Large};
    public sizeTypes objSizeType;

    public enum damageTypes {Bludgeoning, Piercing, Slashing};
    public damageTypes[] objDamageTypes = new damageTypes[1];

    public enum colorTypes {Red, Orange, Yellow, Green, Blue, Purple, Pink, Brown, White, Grey, Black};
    public colorTypes[] objColorTypes = new colorTypes[1];

    public enum categoryTypes { Greenhouse, Office, Workshop };
    public categoryTypes[] objCategoryTypes = new categoryTypes[1];

    [TextArea(15, 20)]
    public string objDescription;

    public bool tutorialSafe;

    private void Start()
    {
        if (objSize <= 7f)
            objSizeType = sizeTypes.Small;
        else if (objSize <= 13f)
            objSizeType = sizeTypes.Medium;
        else
            objSizeType = sizeTypes.Large;
    }
}
