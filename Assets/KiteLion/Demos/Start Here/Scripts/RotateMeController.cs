using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateMeController : MonoBehaviour
{

    public float RotateSpeed = 6;
    public float ForwardSpeed = 20;

    float _sideInput = 0;
    float _forwardInput = 0;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        _sideInput = Input.GetAxis("Horizontal");
        _forwardInput = Input.GetAxis("Vertical");
    }

    private void FixedUpdate()
    {
        GetComponent<Rigidbody>().isKinematic = true;
        Vector3 previousRotation = new Vector3(0, GetComponent<Rigidbody>().rotation.eulerAngles.y, 0);
        Vector3 newRotation = new Vector3(0, previousRotation.y + _sideInput * RotateSpeed, 0);

        GetComponent<Rigidbody>().rotation = Quaternion.Euler(newRotation);
        GetComponent<Rigidbody>().velocity = transform.forward * ForwardSpeed * _forwardInput;

        GetComponent<Rigidbody>().isKinematic = false;
    }
}
