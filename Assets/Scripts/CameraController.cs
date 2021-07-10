using System.Collections;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float zoomSpeed;
    public float panSpeed;
    public float panBuffer = 50.0f;
    public float rotateSpeed;
    public float speed;
    public float damping = 6.0f;

    public bool slerp = true;

    [SerializeField] private Camera mainCamera;

    private Plane _plane;
    private Vector3 _center;

    private Vector3 _origPosition;
    private bool _rotating = false;    

    // Start is called before the first frame update
    private void Start()
    {
        _center = new Vector3(0, 0, 0);
        _plane = new Plane(Vector3.up, Vector3.zero);
        transform.LookAt(_center);
        _origPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);
    }

    // Update is called once per frame
    private void Update()
    {
        HandleZoom();
        // HandlePan();
        HandleRotation();
    }

    private void SmoothMoveAndLookAt()
    {
        if (_rotating) return;
        var newPos = new Vector3(_center.x + _origPosition.x, transform.position.y, _center.z + _origPosition.z);
        transform.position = Vector3.Lerp(transform.position, newPos, Time.deltaTime * damping);
    }

    private void HandleZoom()
    {
        var scrollValue = Input.mouseScrollDelta.y;
        if (scrollValue == 0.0) return;
        var newSize = mainCamera.orthographicSize - scrollValue;
        mainCamera.orthographicSize = Mathf.Clamp(newSize, 3.0f, 20.0f);
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
        _rotating = true;
        var dir = Input.GetAxis("Rotate");
        for (var i = 1; i <= 90; i++)
        {
            new WaitForSeconds(60f);
            var dToCenter = transform.position - _center;
            var newRot = Quaternion.Euler(new Vector3(0, dir, 0));
            var dDir = newRot * dToCenter;
            transform.position = _center + dDir;
            transform.LookAt(_center);
            yield return new WaitForSeconds(rotateSpeed);
        }
        _origPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        _rotating = false;
    }

    private IEnumerator Recenter()
    {
        yield return new WaitForSeconds(rotateSpeed);
    }

    private void HandlePan()
    {
        var dRight = transform.right.XZ();
        var dUp = transform.up.XZ();
        if (_center.x < panBuffer)
        {
            transform.position -= dRight * (Time.deltaTime * panSpeed);
        }
        else if (_center.x > Screen.width - panBuffer)
        {
            transform.position += dRight * (Time.deltaTime * panSpeed);
        }

        if (_center.y < panBuffer)
        {
            transform.position -= dUp * (Time.deltaTime * panSpeed);
        }
        else if (_center.y > Screen.height - panBuffer)
        {
            transform.position += dUp * (Time.deltaTime * panSpeed);
        }
    }

    public void Focus(Transform focusTransform)
    {
        _center = focusTransform.position;
        SmoothMoveAndLookAt();
    }
}