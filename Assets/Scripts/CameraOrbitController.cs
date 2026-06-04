using UnityEngine;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class CameraOrbitController : MonoBehaviour
{
    [Header("Punto central de la cámara")]
    public Vector3 focusPoint = Vector3.zero;

    [Header("Movimiento")]
    public float orbitSpeed = 90f;
    public float zoomSpeed = 10f;
    public float panSpeed = 10f;
    public float verticalSpeed = 7f;

    [Header("Límites de distancia")]
    public float minDistance = 8f;
    public float maxDistance = 45f;

    private float yaw = 0f;
    private float pitch = 38f;
    private float distance = 30f;

    private void Start()
    {
        Vector3 offset =
            transform.position -
            focusPoint;

        distance =
            Mathf.Clamp(
                offset.magnitude,
                minDistance,
                maxDistance
            );

        ActualizarCamara();
    }

    private void Update()
    {
        ProcesarOrbita();
        ProcesarZoom();
        ProcesarDesplazamiento();

        ActualizarCamara();
    }

    private void ProcesarOrbita()
    {
        bool rightMousePressed;
        Vector2 mouseDelta;

#if ENABLE_INPUT_SYSTEM
        rightMousePressed =
            Mouse.current != null &&
            Mouse.current.rightButton.isPressed;

        mouseDelta =
            Mouse.current != null
                ? Mouse.current.delta.ReadValue()
                : Vector2.zero;
#else
        rightMousePressed =
            Input.GetMouseButton(1);

        mouseDelta =
            new Vector2(
                Input.GetAxis("Mouse X"),
                Input.GetAxis("Mouse Y")
            ) * 12f;
#endif

        if (!rightMousePressed)
        {
            return;
        }

        yaw +=
            mouseDelta.x *
            orbitSpeed *
            0.01f;

        pitch -=
            mouseDelta.y *
            orbitSpeed *
            0.01f;

        pitch =
            Mathf.Clamp(
                pitch,
                15f,
                80f
            );
    }

    private void ProcesarZoom()
    {
        float scrollValue;

#if ENABLE_INPUT_SYSTEM
        scrollValue =
            Mouse.current != null
                ? Mouse.current.scroll.ReadValue().y
                : 0f;

        scrollValue *= 0.01f;
#else
        scrollValue =
            Input.mouseScrollDelta.y;
#endif

        distance -=
            scrollValue *
            zoomSpeed;

        distance =
            Mathf.Clamp(
                distance,
                minDistance,
                maxDistance
            );
    }

    private void ProcesarDesplazamiento()
    {
        Vector3 movement =
            Vector3.zero;

#if ENABLE_INPUT_SYSTEM
        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed)
            {
                movement += Vector3.forward;
            }

            if (Keyboard.current.sKey.isPressed)
            {
                movement += Vector3.back;
            }

            if (Keyboard.current.aKey.isPressed)
            {
                movement += Vector3.left;
            }

            if (Keyboard.current.dKey.isPressed)
            {
                movement += Vector3.right;
            }

            if (Keyboard.current.qKey.isPressed)
            {
                movement += Vector3.down;
            }

            if (Keyboard.current.eKey.isPressed)
            {
                movement += Vector3.up;
            }
        }
#else
        if (Input.GetKey(KeyCode.W))
        {
            movement += Vector3.forward;
        }

        if (Input.GetKey(KeyCode.S))
        {
            movement += Vector3.back;
        }

        if (Input.GetKey(KeyCode.A))
        {
            movement += Vector3.left;
        }

        if (Input.GetKey(KeyCode.D))
        {
            movement += Vector3.right;
        }

        if (Input.GetKey(KeyCode.Q))
        {
            movement += Vector3.down;
        }

        if (Input.GetKey(KeyCode.E))
        {
            movement += Vector3.up;
        }
#endif

        if (movement == Vector3.zero)
        {
            return;
        }

        Vector3 horizontalForward =
            transform.forward;

        horizontalForward.y = 0f;
        horizontalForward.Normalize();

        Vector3 horizontalRight =
            transform.right;

        horizontalRight.y = 0f;
        horizontalRight.Normalize();

        Vector3 worldMovement =
            horizontalForward *
            movement.z +
            horizontalRight *
            movement.x +
            Vector3.up *
            movement.y;

        focusPoint +=
            worldMovement.normalized *
            panSpeed *
            Time.deltaTime;
    }

    private void ActualizarCamara()
    {
        Quaternion rotation =
            Quaternion.Euler(
                pitch,
                yaw,
                0f
            );

        Vector3 direction =
            rotation *
            Vector3.back;

        transform.position =
            focusPoint +
            direction *
            distance;

        transform.LookAt(
            focusPoint
        );
    }
}