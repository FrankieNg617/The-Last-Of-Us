using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VectorNormalize : MonoBehaviour
{
    public Vector3 inputVector;

    // Start is called before the first frame update
    void Start()
    {
        inputVector = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        inputVector.x = Input.GetAxis("Horizontal");
        inputVector.z = Input.GetAxis("Vertical");
        inputVector = Vector3.ClampMagnitude(inputVector, 1f);
    }
}
