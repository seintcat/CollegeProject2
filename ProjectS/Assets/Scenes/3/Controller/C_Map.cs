using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C_Map : MonoBehaviour
{
    [SerializeField]
    MeshRenderer meshRenderer;
    [SerializeField]
    MeshFilter meshFilter;

    // Horizontal and vertical cell counts are must be same.
    int atlasSizeInBlocks;
    // Using for UV, when making mesh with code.
    float normalizedBlockTextureSize
    {
        get { return 1f / (float)atlasSizeInBlocks; }
    }

    Texture2D texture_map;
    Dictionary<Vector2Int, C_Unit> v_units;
    M_Map m_map;

    public bool loaded;

    [SerializeField]
    GameObject p_unit;
    [SerializeField]
    float offset_unit;

    public int unitCount
    {
        get { return v_units.Keys.Count; }
    }

    private void Awake()
    {
        loaded = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Set(M_Map _m_map, int _player_index)
    {
        loaded = false;

        List<Vector3> vertices = new List<Vector3>();
        List<Color> colors = new List<Color>();
        List<int> triangles = new List<int>();
        int vertexIndex = 0;
        List<Vector3> normals = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();

        m_map = _m_map;
        v_units = new Dictionary<Vector2Int, C_Unit>();
        texture_map = m_map.map_data;
        atlasSizeInBlocks = m_map.atlasSizeInBlocks_map;

        for (int x = 0; x < texture_map.width; x++)
            for (int y = 0; y < texture_map.height; y++)
            {
                Vector3 offset = new Vector3(x, y, 0);

                triangles.Add(vertexIndex);
                triangles.Add(vertexIndex + 1);
                triangles.Add(vertexIndex + 2);
                triangles.Add(vertexIndex + 2);
                triangles.Add(vertexIndex + 1);
                triangles.Add(vertexIndex + 3);

                for (int i = 0; i < 4; i++)
                {
                    vertices.Add(CubeMeshData.verts_startsZero[CubeMeshData.vectorTris[0, i]] + offset);
                    colors.Add(new Color(255, 255, 255, 255));
                    normals.Add(CubeMeshData.faceChecks[0]);
                }

                Color32 pix = texture_map.GetPixel(x, y);

                int atlasID = m_map.skinOrder_map[pix.r] % (atlasSizeInBlocks * atlasSizeInBlocks);
                float _y = atlasID / atlasSizeInBlocks * normalizedBlockTextureSize;
                float _x = atlasID % atlasSizeInBlocks * normalizedBlockTextureSize;

                uvs.Add(new Vector2(_x, _y));
                uvs.Add(new Vector2(_x, _y + normalizedBlockTextureSize));
                uvs.Add(new Vector2(_x + normalizedBlockTextureSize, _y));
                uvs.Add(new Vector2(_x + normalizedBlockTextureSize, _y + normalizedBlockTextureSize));

                vertexIndex += 4;
            }

        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();

        mesh.subMeshCount = 2;
        mesh.SetTriangles(triangles.ToArray(), 0);

        mesh.uv = uvs.ToArray();

        mesh.colors = colors.ToArray();

        mesh.normals = normals.ToArray();
        mesh.RecalculateNormals();

        meshRenderer.material = _m_map.material_map;
        meshFilter.mesh = mesh;

        loaded = true;
    }

    public bool SpawnUnit(Vector2 pos, int player_index, C_TokenUI token)
    {
        Vector2Int posInt = new Vector2Int((int)pos.x, (int)pos.y);
        if (v_units.ContainsKey(posInt))
            return false;

        Vector3 spawnPos = new Vector3(pos.x, pos.y, -offset_unit);
        v_units.Add(posInt, Instantiate(p_unit).GetComponent<C_Unit>());
        v_units[posInt].Boot(this, spawnPos, player_index, token, m_map);

        return true;
    }
    public bool SpawnUnit(Vector2 pos, int player_index, M_Unit token)
    {
        Vector2Int posInt = new Vector2Int((int)pos.x, (int)pos.y);
        if (v_units.ContainsKey(posInt))
            return false;

        Vector3 spawnPos = new Vector3(pos.x, pos.y, -offset_unit);
        v_units.Add(posInt, Instantiate(p_unit).GetComponent<C_Unit>());
        v_units[posInt].Boot(this, spawnPos, player_index, token, m_map);

        return true;
    }

    public Vector2 GetEmptyCell()
    {
        List<Vector2> listEmpty = new List<Vector2>();

        for (int x = 0; x < m_map.map_data.width; x++)
            for (int y = 0; y < m_map.map_data.height; y++)
            {
                // {  == 0 >> need to read other data(kind of rule file) }
                if (m_map.map_data.GetPixel(x, y).r == 0 && !v_units.ContainsKey(new Vector2Int(x, y)))
                    listEmpty.Add(new Vector2(x, y));
            }

        if (listEmpty.Count < 1)
            return new Vector2(-1, -1);

        return listEmpty[Random.Range(0, (listEmpty.Count - 1))];
    }

    public void Attack(int direction, int number, int playerindex, Vector2 pos)
    {
        switch (direction)
        {
            case 0:
                pos.y += 1; 
                break;
            case 1:
                pos.x += 1;
                pos.y += 1;
                break;
            case 2:
                pos.x += 1;
                break;
            case 3:
                pos.x += 1;
                pos.y -= 1;
                break;
            case 4:
                pos.y -= 1;
                break;
            case 5:
                pos.x -= 1;
                pos.y -= 1;
                break;
            case 6:
                pos.x -= 1;
                break;
            case 7:
                pos.x -= 1;
                pos.y += 1;
                break;
        }

        if (v_units.ContainsKey(new Vector2Int((int)pos.x, (int)pos.y)))
            v_units[new Vector2Int((int)pos.x, (int)pos.y)].Defence(direction, number, playerindex);
    }

    public List<Vector2Int> CalculateScore()
    {
        List<Vector2Int> val = new List<Vector2Int>();
        for(int i = 0; i < m_map.player_color.Count; i++)
            val.Add(new Vector2Int(i, 0));

        foreach(C_Unit unit in v_units.Values)
        {
            Vector2Int edit = val[unit.player_index];
            edit.y = val[unit.player_index].y + 1;
            val[unit.player_index] = edit;
        }

        return val;
    }
}
