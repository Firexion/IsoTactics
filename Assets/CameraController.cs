using System.Collections;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float ZoomSpeed;
    public float panSpeed;
    public float PanBuffer = 50.0f;
    public float rotateSpeed;

    private Plane _Plane;

    // Start is called before the first frame update
    void Start()
    {
        Vector3 mapCenter = new Vector3(0, 0, 0);
        _Plane = new Plane(Vector3.up, Vector3.zero);
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
        float scrollValue = Input.mouseScrollDelta.y;
        if (scrollValue != 0.0)
        {
            float newSize = Camera.main.orthographicSize - scrollValue;
            Camera.main.orthographicSize = Mathf.Clamp(newSize, 3.0f, 20.0f);
        }
    }

    private Vector3 GetCenter()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        if (_Plane.Raycast(ray, out float distance))
        {
            return ray.GetPoint(distance);
        }

        return Vector3.zero;
        
    }

    private void HandleRotation()
    {
       if (Input.GetButtonDown("Rotate"))
        {
            StartCoroutine("RotateObject");
        }
    }

    IEnumerator RotateObject()
    {
        Vector3 center = GetCenter();
        float dir = Input.GetAxis("Rotate");
        for (int i = 1; i <= 90; i++)
        {
            new WaitForSeconds(60f);
            Vector3 dToCenter = transform.position - center;
            Quaternion newRot = Quaternion.Euler(new Vector3(0, dir, 0));
            Vector3 dDir = newRot * dToCenter;
            transform.position = center + dDir;
            transform.LookAt(center);
            yield return new WaitForSeconds(rotateSpeed);
        }
    }

    private void HandlePan()
    {
        Vector2 mousePos = Input.mousePosition;
        Vector3 dRight = transform.right.XZ();
        Vector3 dUp = transform.up.XZ();
        if (mousePos.x < PanBuffer)
        {
            transform.position -= dRight * Time.deltaTime * panSpeed;
        } 
        else if (mousePos.x > Screen.width - PanBuffer)
        {
            transform.position += dRight * Time.deltaTime * panSpeed;
        }
        
        if (mousePos.y < PanBuffer)
        {
            transform.position -= dUp * Time.deltaTime * panSpeed;
        }
        else if (mousePos.y > Screen.height - PanBuffer)
        {
            transform.position += dUp * Time.deltaTime * panSpeed;
        }
    }

}
