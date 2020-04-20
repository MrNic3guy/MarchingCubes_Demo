using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float WalkSpeed = 1;
    public float speedH = 3.0f;
    public float speedV = 3.0f;

    private float yaw = 0.0f;
    private float pitch = 0.0f;
    public Camera Camera;

    private CharacterController charcController;
    // Start is called before the first frame update
    void Start()
    {
        charcController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {

        // Handle camera rotation
        yaw += speedH * Input.GetAxis("Mouse X");
        pitch -= speedV * Input.GetAxis("Mouse Y");
        Camera.transform.eulerAngles = new Vector3(pitch, yaw, 0.0f);

        // Moving direction
        Vector3 moveDirection = 
            Camera.transform.forward * Input.GetAxis("Vertical") +
            Camera.transform.right * Input.GetAxis("Horizontal");

        moveDirection *= WalkSpeed * Time.deltaTime;

        // Apply gravity
        moveDirection.y = -1f;

        // Move
        charcController.Move(moveDirection);

        // Handle Mouse Input
        if (Input.GetMouseButton(1))
        {
            Ray ray = new Ray(Camera.transform.position, Camera.transform.forward);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {


            }
        }

        if (Input.GetMouseButton(0))
        {
            Ray ray = new Ray(Camera.transform.position, Camera.transform.forward);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                Chunk chunk = ChunkManager.Instance.AddVoxel(hit.point + hit.normal, 1);
                chunk.Rebuild();
                Debug.DrawLine(hit.point, hit.point + hit.normal, Color.white, 10000);
                Debug.Log(hit.point);

            }
        }

    }
}
