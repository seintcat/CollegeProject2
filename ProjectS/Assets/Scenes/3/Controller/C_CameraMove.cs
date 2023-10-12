using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C_CameraMove : MonoBehaviour
{
    public float speed, zoomMin, zoomSpeed;

    [SerializeField]
    Transform cameraTransform;
    Vector2 mapsize;

    private void Awake()
    {
        mapsize = new Vector2();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = cameraTransform.position;

        if (Input.GetKey("w"))
            pos.y += speed * Time.deltaTime;
        if (Input.GetKey("a"))
            pos.x -= speed * Time.deltaTime;
        if (Input.GetKey("s"))
            pos.y -= speed * Time.deltaTime;
        if (Input.GetKey("d"))
            pos.x += speed * Time.deltaTime;

        if (pos.x > mapsize.x)
            pos.x = mapsize.x;
        if (pos.y > mapsize.y)
            pos.y = mapsize.y;
        if (pos.x < 0)
            pos.x = 0;
        if (pos.y < 0)
            pos.y = 0;

        Camera.main.orthographicSize = Camera.main.orthographicSize - Input.GetAxis("zoom") * zoomSpeed * Time.deltaTime;
        if (Camera.main.orthographicSize < zoomMin)
            Camera.main.orthographicSize = zoomMin;

        cameraTransform.position = pos;
    }

    public void SetMapSize(Vector2 size)
    {
        mapsize = size;
        mapsize.x = mapsize.x++;
        mapsize.y = mapsize.y++;
    }
}
