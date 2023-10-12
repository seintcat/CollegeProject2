using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class C_Unit : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI text;
    [SerializeField]
    MeshRenderer meshRenderer;
    [SerializeField]
    MeshFilter meshFilter;

    int index_now;
    C_Map v_map;
    M_Map m_map;
    bool[] dir;
    int number;

    public int player_index
    {
        get { return index_now; }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void Boot(C_Map map, Vector3 pos, int player_index, C_TokenUI token, M_Map mapdata)
    {
        v_map = map;
        index_now = player_index;
        m_map = mapdata;

        // Set text.
        TextMeshProUGUI font = token.transform.Find("Text").gameObject.GetComponent<TextMeshProUGUI>();
        text.color = font.color;
        text.fontMaterial = font.fontMaterial;
        text.fontMaterials = font.fontMaterials;
        text.fontSharedMaterial = font.fontSharedMaterial;
        text.fontSharedMaterials = font.fontSharedMaterials;
        text.fontStyle = font.fontStyle;
        text.fontWeight = font.fontWeight;
        text.styleSheet = font.styleSheet;
        text.textStyle = font.textStyle;

        // Set mesh.
        List<Vector3> vertices = new List<Vector3>();
        List<Color> colors = new List<Color>();
        List<int> triangles = new List<int>();
        List<Vector3> normals = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();

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

        uvs.Add(new Vector2(0, 0));
        uvs.Add(new Vector2(0, 1));
        uvs.Add(new Vector2(1, 0));
        uvs.Add(new Vector2(1, 1));

        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.subMeshCount = 2;
        mesh.SetTriangles(triangles.ToArray(), 0);
        mesh.uv = uvs.ToArray();
        mesh.colors = colors.ToArray();
        mesh.normals = normals.ToArray();
        mesh.RecalculateNormals();

        meshRenderer.material = m_map.material_map;
        meshFilter.mesh = mesh;

        meshRenderer.material.SetColor("BaseColor", m_map.player_color[index_now]);
        meshRenderer.material.SetTexture("Texture", token.texture);

        dir = token.directions;
        number = token.number;
        text.text = number.ToString();

        transform.position = pos;
        Destroy(token.gameObject);
        Attack();
    }

    public void Boot(C_Map map, Vector3 pos, int player_index, M_Unit token, M_Map mapdata)
    {
        v_map = map;
        index_now = player_index;
        m_map = mapdata;

        // { Set text. }
        text.color = new Color(0, 0, 0);

        // Set mesh.
        List<Vector3> vertices = new List<Vector3>();
        List<Color> colors = new List<Color>();
        List<int> triangles = new List<int>();
        List<Vector3> normals = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();

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
        
        uvs.Add(new Vector2(0, 0));
        uvs.Add(new Vector2(0, 1));
        uvs.Add(new Vector2(1, 0));
        uvs.Add(new Vector2(1, 1));

        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.subMeshCount = 2;
        mesh.SetTriangles(triangles.ToArray(), 0);
        mesh.uv = uvs.ToArray();
        mesh.colors = colors.ToArray();
        mesh.normals = normals.ToArray();
        mesh.RecalculateNormals();

        meshRenderer.material = m_map.material_map;
        meshFilter.mesh = mesh;

        int pixel_width = m_map.skin_map.width / m_map.atlasSizeInBlocks_map, pixel_height = m_map.skin_map.height / m_map.atlasSizeInBlocks_map;
        Texture2D texture = new Texture2D(pixel_width, pixel_height);
        texture.filterMode = m_map.skin_map.filterMode;
        List<Rect> rects = new List<Rect>();

        for (int y = 0; y < m_map.atlasSizeInBlocks_map; y++)
            for (int x = 0; x < m_map.atlasSizeInBlocks_map; x++)
                rects.Add(new Rect(x * pixel_width, y * pixel_height, pixel_width, pixel_height));

        rects.RemoveAt(0);
        rects.RemoveAt(0);
        rects.RemoveAt(0);

        for (int x = (int)rects[0].x; x < (rects[0].x + rects[0].width); x++)
            for (int y = (int)rects[0].y; y < (rects[0].y + rects[0].height); y++)
                texture.SetPixel(x - (int)rects[0].x, y - (int)rects[0].y, m_map.skin_map.GetPixel(x, y));
        rects.RemoveAt(0);

        for(int i = 0; i < token.direction.Length; i++)
            if(token.direction[i])
                for (int x = (int)rects[i].x; x < (rects[i].x + rects[i].width); x++)
                    for (int y = (int)rects[i].y; y < (rects[i].y + rects[i].height); y++)
                        if (m_map.skin_map.GetPixel(x, y).a > 0)
                            texture.SetPixel(x - (int)rects[i].x, y - (int)rects[i].y, m_map.skin_map.GetPixel(x, y));

        texture.Apply();

        meshRenderer.material.SetColor("BaseColor", m_map.player_color[index_now]);
        meshRenderer.material.SetTexture("Texture", texture);

        dir = token.direction;
        number = token.number;
        text.text = number.ToString();

        transform.position = pos;
        Attack();
    }

    public void Attack()
    {
        Vector2 pos = new Vector2((int)transform.position.x, (int)transform.position.y);
        if (dir[0])
            v_map.Attack(0, number, index_now, pos);
        if (dir[1])
            v_map.Attack(1, number, index_now, pos);
        if (dir[2])
            v_map.Attack(2, number, index_now, pos);
        if (dir[3])
            v_map.Attack(3, number, index_now, pos);
        if (dir[4])
            v_map.Attack(4, number, index_now, pos);
        if (dir[5])
            v_map.Attack(5, number, index_now, pos);
        if (dir[6])
            v_map.Attack(6, number, index_now, pos);
        if (dir[7])
            v_map.Attack(7, number, index_now, pos);
    }

    public void Defence(int direction, int _number, int playerindex)
    {
        if (playerindex == index_now)
            return;

        int defDir = -1;
        switch (direction)
        {
            case 0:
                defDir = 4;
                break;
            case 1:
                defDir = 5;
                break;
            case 2:
                defDir = 6;
                break;
            case 3:
                defDir = 7;
                break;
            case 4:
                defDir = 0;
                break;
            case 5:
                defDir = 1;
                break;
            case 6:
                defDir = 2;
                break;
            case 7:
                defDir = 3;
                break;
        }
        if (defDir == -1) 
            return;

        if (!dir[defDir] || number < _number)
        {
            index_now = playerindex;
            meshRenderer.material.SetColor("BaseColor", m_map.player_color[index_now]);
        }
    }
}
