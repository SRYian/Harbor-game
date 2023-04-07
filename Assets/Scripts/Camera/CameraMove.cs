using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraMove : MonoBehaviour
{

    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    private CinemachineComponentBase componentBase;
    [SerializeField] private Transform target;
    [Range(0.0f, 10.0f)]
    [SerializeField] private float maxzoomOut = 10f;
    [Range(0.0f, 10.0f)]
    [SerializeField] private float maxzoomIn = 2f;
    [Range(0.0f, 10.0f)]
    [SerializeField] private float sensitivity = 2f;
    private float cameraDistance = 1f;
    
    // Start is called before the first frame update
    void Start()
    {
        virtualCamera = GetComponent<CinemachineVirtualCamera>();
        componentBase = virtualCamera.GetCinemachineComponent(CinemachineCore.Stage.Body);
    }

    // Update is called once per frame
    void Update()
    {
       
        cameraDistance = Input.GetAxis("Mouse ScrollWheel") * -sensitivity;
        if (Input.GetAxis("Mouse ScrollWheel")!=0)
        {
            if (virtualCamera.m_Lens.OrthographicSize > maxzoomOut)
            {
                virtualCamera.m_Lens.OrthographicSize = maxzoomOut;
            }
            if (virtualCamera.m_Lens.OrthographicSize < maxzoomIn)
            {
                virtualCamera.m_Lens.OrthographicSize = maxzoomIn;
            }
            virtualCamera.m_Lens.OrthographicSize += cameraDistance;
        }
        
    }
}
