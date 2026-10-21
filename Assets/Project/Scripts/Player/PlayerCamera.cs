using Cinemachine;
using UnityEngine;

public class PlayerCamera : MonoBehaviour {

    [SerializeField] private CinemachineFreeLook _vcam;
    [SerializeField] private Transform _mainCamera;

    public Transform CameraTransform => _mainCamera;

    public Vector3 PlanarForward {
        get {
            Vector3 f = _mainCamera.forward;
            f.y = 0;
            return f.normalized;
        }
    }

    public Vector3 PlanarRight {
        get {
            Vector3 r = _mainCamera.right;
            r.y = 0;
            return r.normalized;
        }
    }
}
