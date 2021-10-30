using UnityEngine;
using System.Collections;

// Player Camera

public class ThirdPersonCamera : MonoBehaviour
{
    [SerializeField] float mouseSensitivity = 2.5f;               // How fast the camera will rotate
    [SerializeField] float distanceFromTarget = 2;                // How far the camera will stay away from the player
    [SerializeField] float rotationSmoothTime = 0.12f;            // How long it will take the camera to rotate
    [SerializeField] Transform target;                            // The player
    [SerializeField] Vector2 pitchMinMax = new Vector2(-40, 85);  // How far you can rotate the camera pitch
    [SerializeField] LayerMask collisionMask;                     // What the camera will collide with
    [SerializeField] float cameraSwitchSidesTime;                 // How long it takes for the camera to switch its position (left / right)

    float pitch;
    float yaw;
    Vector3 rotationSmoothVelocity;
    Vector3 currentRotation;

    float targetStartX;

    CameraCollisionHandler cameraCollisionHandler;

    void Start()
    {
        targetStartX = target.localPosition.x;

        cameraCollisionHandler = new CameraCollisionHandler(Camera.main, collisionMask);
        cameraCollisionHandler.UpdateCameraClipPoints(transform.position, transform.rotation, ref cameraCollisionHandler.adjustedCameraClipPoints);
        cameraCollisionHandler.UpdateCameraClipPoints(target.position, transform.rotation, ref cameraCollisionHandler.desiredCameraClipPoints);
    }

    // Calls the RotateCamera and MoveCamera methods. LateUpdate so that the players input has already been applied
    void LateUpdate()
    {
        RotateCamera();
        MoveCamera();
    }

    // Rotate the camera
    void RotateCamera()
    {
        yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
        pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;
        pitch = Mathf.Clamp(pitch, pitchMinMax.x, pitchMinMax.y);

        currentRotation = Vector3.SmoothDamp(currentRotation, new Vector3(pitch, yaw), ref rotationSmoothVelocity, rotationSmoothTime);
        transform.eulerAngles = currentRotation;
    }

    // Move the camera
    void MoveCamera()
    {
        cameraCollisionHandler.UpdateCameraClipPoints(transform.position, transform.rotation, ref cameraCollisionHandler.adjustedCameraClipPoints);
        cameraCollisionHandler.UpdateCameraClipPoints(target.position, transform.rotation, ref cameraCollisionHandler.desiredCameraClipPoints);

        Vector3 destination = target.position - transform.forward * (cameraCollisionHandler.CheckColliding(target.position) 
                              ? cameraCollisionHandler.GetAdjustedDistanceWithRayFrom(target.position) : distanceFromTarget);
        transform.position = destination;
    }

    // Starts the SwitchCameraSideCo coroutine. Called from an event hooked up in PlayerController Start
    public void SwitchCameraSide()
    {
        StopAllCoroutines();
        StartCoroutine(SwitchCameraSideCo());
    }

    // Switch the cameras position from one side of the player to the other via a lerp
    IEnumerator SwitchCameraSideCo()
    {
        Vector3 targetPosition = new Vector3(target.localPosition.x < 0 ? targetStartX : -targetStartX, target.localPosition.y, target.localPosition.z);
        float time = 0;
        Vector3 startPosition = target.localPosition;

        while (time < cameraSwitchSidesTime)
        {
            target.localPosition = Vector3.Lerp(startPosition, targetPosition, time / cameraSwitchSidesTime);
            time += Time.deltaTime;
            yield return null;
        }

        target.localPosition = targetPosition;
    }

    // Checks if the camera can see the player - handles collision, occlusion and shearing (or at least it should xD) | Base: Renaissance Coders (https://bit.ly/3lH9FLb)
    // Its kinda buggy atm, so a TODO is to fix it. However, camera stuff isn't in the spec so its low priority and I've just got it disabled atm
    class CameraCollisionHandler
    {
        public Vector3[] adjustedCameraClipPoints, desiredCameraClipPoints;

        Camera camera;
        LayerMask layerMask;

        public CameraCollisionHandler(Camera _camera, LayerMask _layerMask)
        {
            camera = _camera;
            layerMask = _layerMask;

            adjustedCameraClipPoints = new Vector3[5];
            desiredCameraClipPoints = new Vector3[5];
        }

        public void UpdateCameraClipPoints(Vector3 cameraPosition, Quaternion atRotation, ref Vector3[] intoArray)
        {
            if (!camera) return;

            intoArray = new Vector3[5];

            float z = camera.nearClipPlane;
            float y = Mathf.Tan(camera.fieldOfView / 3.41f) * z;
            float x = y * camera.aspect;

            intoArray[0] = (atRotation * new Vector3(-x, y, z)) + cameraPosition;    // Top left
            intoArray[1] = (atRotation * new Vector3(x, y, z)) + cameraPosition;     // Top right
            intoArray[2] = (atRotation * new Vector3(-x, -y, -z)) + cameraPosition;  // Bottom left
            intoArray[3] = (atRotation * new Vector3(x, -y, -z)) + cameraPosition;   // Bottom right
            intoArray[4] = cameraPosition - camera.transform.forward;                // Camera position (can multiply this to fix some close clipping issues, but then this causes other problems)
        }

        public bool CheckColliding(Vector3 targetPosition)
        {
            if (CollisionDetectedAtClipPoints(desiredCameraClipPoints, targetPosition)) return true;
            else return false;
        }

        bool CollisionDetectedAtClipPoints(Vector3[] clipPoints, Vector3 fromPosition)
        {
            for (int i = 0; i < clipPoints.Length; i++)
            {
                Ray ray = new Ray(fromPosition, clipPoints[i] - fromPosition);
                float distance = Vector3.Distance(clipPoints[i], fromPosition);
                if (Physics.Raycast(ray, distance, layerMask)) return true;
            }

            return false;
        }

        public float GetAdjustedDistanceWithRayFrom(Vector3 fromPosition)
        {
            float distance = -1;

            for (int i = 0; i < desiredCameraClipPoints.Length; i++)
            {
                Ray ray = new Ray(fromPosition, desiredCameraClipPoints[i] - fromPosition);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    if (distance == -1) distance = hit.distance;
                    else if (hit.distance < distance) distance = hit.distance;
                }
            }

            if (distance == -1) return 0;
            else return distance;
        }
    }
}