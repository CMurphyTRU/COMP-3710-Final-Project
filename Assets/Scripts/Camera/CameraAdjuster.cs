using UnityEngine;

public class CameraAdjuster : MonoBehaviour
{
    [SerializeField] public float zoom = 5f;
    [SerializeField] public bool followMe;
    [SerializeField] Camera mainCamera;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if(followMe)
        {
            mainCamera.transform.position = transform.position + Vector3.back;
        }
        if(mainCamera.orthographicSize != zoom)
        {
            mainCamera.orthographicSize = zoom;
        }
    }
}
