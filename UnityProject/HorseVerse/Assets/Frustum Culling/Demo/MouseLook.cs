using System;
using UnityEngine;
 
public class MouseLook : MonoBehaviour
{
    public float mouseSensitivity = 100.0f;
    public float clampAngle = 80.0f;

    private float rotY = 0.0f; // rotation around the up/y axis
    private float rotX = 0.0f; // rotation around the right/x axis


    void Start ()
    {
        Vector3 rot = transform.localRotation.eulerAngles;
        rotY = rot.y;
        rotX = rot.x;
        
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        cam = Camera.main;
        planes = GeometryUtility.CalculateFrustumPlanes(cam);
        if(obj != null)
            objCollider = obj.GetComponent<Collider>();
    }

    void Update ()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = -Input.GetAxis("Mouse Y");

        rotY += mouseX * mouseSensitivity * Time.deltaTime;
        rotX += mouseY * mouseSensitivity * Time.deltaTime;

        rotX = Mathf.Clamp(rotX, -clampAngle, clampAngle);

        Quaternion localRotation = Quaternion.Euler(rotX, rotY, 0.0f);
        transform.rotation = localRotation;

        if (objCollider != null)
        {
            if (GeometryUtility.TestPlanesAABB(planes, objCollider.bounds))
            {
                Debug.Log(obj.name + " has been detected!");
            }
            else
            {
                Debug.Log("Nothing has been detected");
            }
        }
    }

    Plane[] planes;
    Camera cam;
    [SerializeField] GameObject obj;
    Collider objCollider;
}