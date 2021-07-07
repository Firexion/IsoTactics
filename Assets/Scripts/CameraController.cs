using System.Collections;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float zoomSpeed;
    public float panSpeed;
    public float panBuffer = 50.0f;
    public float rotateSpeed;

    [SerializeField] private Camera mainCamera;

    private Plane _plane;

    // Start is called before the first frame update
    private void Start()
    {
        var mapCenter = new Vector3(0, 0, 0);
        _plane = new Plane(Vector3.up, Vector3.zero);
        transform.LookAt(mapCenter);
    }

    // Update is called once per frame
    void Update()
    {
        HandleZoom();
        // HandlePan();
        HandleRotation();
    }

    private void HandleZoom()
    {
        var scrollValue = Input.mouseScrollDelta.y;
        if (scrollValue == 0.0) return;
        var newSize = mainCamera.orthographicSize - scrollValue;
        mainCamera.orthographicSize = Mathf.Clamp(newSize, 3.0f, 20.0f);
    }

    private Vector3 GetCenter()
    {
        var transform1 = transform;
        var ray = new Ray(transform1.position, transform1.forward);
        return _plane.Raycast(ray, out var distance) ? ray.GetPoint(distance) : Vector3.zero;
    }

    private void HandleRotation()
    {
        if (Input.GetButtonDown("Rotate"))
        {
            StartCoroutine(nameof(RotateObject));
        }
    }

    private IEnumerator RotateObject()
    {
        var center = GetCenter();
        var dir = Input.GetAxis("Rotate");
        for (var i = 1; i <= 90; i++)
        {
            new WaitForSeconds(60f);
            var dToCenter = transform.position - center;
            var newRot = Quaternion.Euler(new Vector3(0, dir, 0));
            var dDir = newRot * dToCenter;
            transform.position = center + dDir;
            transform.LookAt(center);
            yield return new WaitForSeconds(rotateSpeed);
        }
    }

    private void HandlePan()
    {
        Vector2 mousePos = Input.mousePosition;
        var dRight = transform.right.XZ();
        var dUp = transform.up.XZ();
        if (mousePos.x < panBuffer)
        {
            transform.position -= dRight * Time.deltaTime * panSpeed;
        }
        else if (mousePos.x > Screen.width - panBuffer)
        {
            transform.position += dRight * Time.deltaTime * panSpeed;
        }

        if (mousePos.y < panBuffer)
        {
            transform.position -= dUp * Time.deltaTime * panSpeed;
        }
        else if (mousePos.y > Screen.height - panBuffer)
        {
            transform.position += dUp * Time.deltaTime * panSpeed;
        }
    }
}