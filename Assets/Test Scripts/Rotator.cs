using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    [SerializeField]
    Vector3 _rotationAxis = new Vector3(0, 90, 0);

    Transform _myTransform = null;

    // Start is called before the first frame update
    void Start()
    {
        _myTransform = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        _myTransform.Rotate(_rotationAxis * Time.deltaTime, Space.Self);
    }
}
