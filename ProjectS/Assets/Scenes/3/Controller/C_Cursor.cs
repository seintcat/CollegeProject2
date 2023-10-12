using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class C_Cursor : MonoBehaviour
{
    [SerializeField]
    GameObject view;
    [SerializeField]
    float offset_cursor;

    MeshFilter filter;
    MeshRenderer _renderer;
    Vector2Int mapSize;
    IEnumerator cursorMove;
    Vector3 mousepos;
    C_GameHeader v_gameheader;

    [HideInInspector]
    public Vector2 cursorPos;
    [HideInInspector]
    public bool loaded = false, forcePos = false;

    // Horizontal and vertical cell counts are must be same.
    int atlasSizeInBlocks;
    // Using for UV, when making mesh with code.
    float normalizedBlockTextureSize
    {
        get { return 1f / (float)atlasSizeInBlocks; }
    }

    private void Awake()
    {
        loaded = false;
        if (view != null)
        {
            filter = view.GetComponent<MeshFilter>();
            _renderer = view.GetComponent<MeshRenderer>();
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
            mousepos = Camera.main.ScreenPointToRay(Input.mousePosition).origin;

        // { cursor will move different when other rule }
        if (Input.GetKeyUp(KeyCode.Mouse0) 
            && mousepos != null 
            && mousepos == Camera.main.ScreenPointToRay(Input.mousePosition).origin 
            && EventSystem.current.IsPointerOverGameObject() == false 
            && view.activeSelf
            && v_gameheader.waiting)
        {
            v_gameheader.SpawnUnit(new Vector2((int)mousepos.x, (int)mousepos.y));
        }
    }

    public void Boot(M_Map m_map, C_GameHeader gameheader)
    {
        loaded = false;
        List<Vector3> vertices = new List<Vector3>();
        List<Color> colors = new List<Color>();
        List<int> triangles = new List<int>();
        List<Vector3> normals = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();

        v_gameheader = gameheader;
        atlasSizeInBlocks = m_map.atlasSizeInBlocks_map;

        triangles.Add(0);
        triangles.Add(1);
        triangles.Add(2);
        triangles.Add(2);
        triangles.Add(1);
        triangles.Add(3);

        for (int i = 0; i < 4; i++)
        {
            vertices.Add(CubeMeshData.verts_startsZero[CubeMeshData.vectorTris[0, i]]);
            colors.Add(new Color(255, 255, 255, 255));
            normals.Add(CubeMeshData.faceChecks[0]);
        }

        // { edit here  index }
        float _y = m_map.skinOrder_cursor[0] / atlasSizeInBlocks * normalizedBlockTextureSize;
        float _x = m_map.skinOrder_cursor[0] % atlasSizeInBlocks * normalizedBlockTextureSize;

        uvs.Add(new Vector2(_x, _y));
        uvs.Add(new Vector2(_x, _y + normalizedBlockTextureSize));
        uvs.Add(new Vector2(_x + normalizedBlockTextureSize, _y));
        uvs.Add(new Vector2(_x + normalizedBlockTextureSize, _y + normalizedBlockTextureSize));

        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();

        mesh.subMeshCount = 2;
        mesh.SetTriangles(triangles.ToArray(), 0);

        mesh.uv = uvs.ToArray();

        mesh.colors = colors.ToArray();

        mesh.normals = normals.ToArray();
        mesh.RecalculateNormals();


        gameObject.SetActive(true);
        mapSize = new Vector2Int(m_map.map_data.width, m_map.map_data.height);

        filter.mesh = mesh;
        _renderer.material = m_map.material_cursor;

        cursorMove = Point();
        StartCoroutine(cursorMove);

        loaded = true;
    }

    private IEnumerator Point()
    {
        while (true)
        {
            // Get mouse position, rotation.
            if(forcePos)
            {
                transform.position = new Vector3((int)(cursorPos.x - 0f), (int)(cursorPos.y - 0f), -offset_cursor);
            }
            else
            {
                Vector3 pos = Camera.main.ScreenPointToRay(Input.mousePosition).origin;
                transform.position = new Vector3((int)(pos.x - 0f), (int)(pos.y - 0f), -offset_cursor);
            }

            // Check cursor out.
            // { cursor will move different when other rule }
            if (transform.position.x < 0 || transform.position.x > (mapSize.x - 1) || transform.position.y < 0 || transform.position.y > (mapSize.y - 1))
                view.SetActive(false);
            else
                view.SetActive(true);

            yield return new WaitForSeconds(0.1f);

            //// When camera is Perspective.
            //float mult;
            //Vector3 pos = Camera.main.ScreenPointToRay(Input.mousePosition).origin, face = Camera.main.ScreenPointToRay(Input.mousePosition).direction
            //transform.position = new Vector3((int)(pos.x - 0f), (int)(pos.y - 0f), pos.z - offset);

            //// Calculate cusor on z=0.
            //mult = pos.z / face.z;
            //face *= mult;
            //pos -= face;
        }
    }

}
