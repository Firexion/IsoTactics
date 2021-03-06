using System;
using System.Collections;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour, PlayerActions.ICameraActions
{
    public float zoomSpeed;
    public float panSpeed;
    public float panBuffer = 50.0f;
    
    [Range (0f, 100f)]
    public float rotateSpeed;
    
    public float speed;
    public float damping = 6.0f;


    private Vector3 _center;

    private Vector3 _origPosition;
    private bool _rotating = false;
    private bool _moving = false;
    private float _rotateAxis;

    private Camera _camera;

    private void Awake()
    {
        _camera = GetComponent<Camera>();
    }

    // Start is called before the first frame update
    private void Start()
    {
        InputController.playerActions.Camera.SetCallbacks(this);
        _center = new Vector3(0, 0, 0);
        transform.LookAt(_center);
        _origPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);
    }

    private void FixedUpdate()
    {
        CheckRotate();
    }


    private void CheckRotate()
    {
        if (_rotating) return;
        _rotateAxis = InputController.playerActions.Camera.Rotate.ReadValue<float>();
        if (_rotateAxis == 0f) return;
        StartCoroutine(nameof(RotateCoroutine));
    }


    private void SmoothMoveAndLookAt()
    {
        if (_rotating) return;
        var newPos = new Vector3(_center.x + _origPosition.x, transform.position.y, _center.z + _origPosition.z);
        _moving = Mathf.Abs(newPos.x - transform.position.x) > 0.01f ||
                  Mathf.Abs(newPos.z - transform.position.z) > 0.01f;
        if (!_moving) return;
        transform.position = Vector3.Lerp(transform.position, newPos, Time.deltaTime * damping);
    }

    private IEnumerator RotateCoroutine()
    {
        _rotating = true;
        for (var i = 1; i <= 90; i++)
        {
            new WaitForSeconds(60f);
            var dToCenter = transform.position - _center;
            var newRot = Quaternion.Euler(new Vector3(0, _rotateAxis, 0));
            var dDir = newRot * dToCenter;
            transform.position = _center + dDir;
            transform.LookAt(_center);
            var waitTime = (100 - rotateSpeed) * 0.0001f;
            yield return new WaitForSeconds(waitTime);
        }

        _origPosition = new Vector3(transform.position.x - _center.x, transform.position.y,
            transform.position.z - _center.z);
        _rotating = false;
    }

    public void Focus(Transform focusTransform)
    {
        _center = focusTransform.position;
        SmoothMoveAndLookAt();
    }

    public void OnRotate(InputAction.CallbackContext context)
    {
        if (_moving || _rotating) return;
        _rotateAxis = context.ReadValue<float>();
        StartCoroutine(nameof(RotateCoroutine));
    }

    public void OnZoom(InputAction.CallbackContext context)
    {
        var scrollValue = context.ReadValue<Vector2>().y;
        if (scrollValue == 0.0) return;
        scrollValue = scrollValue > 0.0f ? +zoomSpeed : -zoomSpeed;
        var newSize = _camera.orthographicSize - scrollValue;
        _camera.orthographicSize = Mathf.Clamp(newSize, 3.0f, 20.0f);
    }
}