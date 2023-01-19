using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalCameraHandler : MonoBehaviour
{
    public Transform cameraAnchor;
    Camera localCamera;

    Vector2 viewInput;

    float camRotX = 0;
    float camRotY = 0;

    NetworkCharacterControllerPrototype networkCharacterControllerPrototype;

    private void Awake()
    {
        localCamera = GetComponent<Camera>();
        networkCharacterControllerPrototype = GetComponentInParent<NetworkCharacterControllerPrototype>();
    }

    // Start is called before the first frame update
    void Start()
    {
        if (localCamera.enabled)
            localCamera.transform.parent = null;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (cameraAnchor == null)
            return;

        if (!localCamera.enabled)
            return;

        // move cam to player
        localCamera.transform.position = cameraAnchor.position;

        camRotX += viewInput.y * Time.deltaTime * networkCharacterControllerPrototype.viewUpDownRotationSpeed;
        camRotX = Mathf.Clamp(camRotX, -90, 90);

        camRotY += viewInput.x * Time.deltaTime * networkCharacterControllerPrototype.rotationSpeed;

        localCamera.transform.rotation = Quaternion.Euler(camRotX, camRotY, 0);
    }

    public void SetViewInputVector(Vector2 viewInput)
    {
        this.viewInput = viewInput;
    }
}
