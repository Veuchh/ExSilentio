using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateOnAxis : MonoBehaviour
{
    [SerializeField] bool rotateX;
    [SerializeField] bool rotateY;
    [SerializeField] bool rotateZ;
    [SerializeField] float rotationSpeed = 1;

    bool isSpinningToWin = false;

    void Update()
    {
        Vector3 currentRotation = transform.localRotation.eulerAngles;

        transform.localRotation =
            Quaternion.Euler(
                currentRotation.x + (rotateX ? rotationSpeed * Time.deltaTime : 0),
                currentRotation.y + (rotateY ? rotationSpeed * Time.deltaTime : 0),
                currentRotation.z + (rotateZ ? rotationSpeed * Time.deltaTime : 0));
    }

    public void OnSpinToWin()
    {
        isSpinningToWin = !isSpinningToWin;
        rotationSpeed *= isSpinningToWin ? 100 : .01f;
    }
}
