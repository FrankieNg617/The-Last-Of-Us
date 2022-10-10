using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossHairTarget : MonoBehaviour
{
    [SerializeField] private GameObject player;
    Camera mainCamera;
    Ray ray;
    RaycastHit hitInfo;

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;    
    }

    // Update is called once per frame
    void Update()
    {
    //     //offset to cast ray based on the distance from the camera to the player
    //     float cameraDistanceOffset = Vector3.Distance(mainCamera.transform.position, player.transform.position);

    //     //cast a ray from the center of the camera plus the offset
    //     RaycastHit hitInfo;
    //     Physics.Raycast(mainCamera.transform.position + mainCamera.transform.forward * cameraDistanceOffset, mainCamera.transform.forward, out hitInfo);

    //     //visualize the ray cast from the camera
    //    // Debug.DrawLine(_camera.transform.position + _camera.transform.forward * cameraDistanceOffset, hitInfo.point, Color.blue);

    //     //set the crosshairTarget
    //     transform.position = hitInfo.point;


        ray.origin = mainCamera.transform.position;
        ray.direction = mainCamera.transform.forward;
        if (Physics.Raycast(ray, out hitInfo)) {
            transform.position = hitInfo.point;
        } else {
            transform.position = ray.origin + ray.direction * 1000.0f;  
        }
    }
}
