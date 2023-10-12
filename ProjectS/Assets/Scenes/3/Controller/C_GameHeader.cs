using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class C_GameHeader : MonoBehaviour
{
    public C_CameraMove cam;
    public C_Map c_map;
    public C_Cursor c_cursor;
    public C_UI c_ui;

    // { edit needed }
    [SerializeField]
    M_Map m_map;
    int player_index;
    C_Essential v_essential;
    Dictionary<int, List<M_Unit>> coms;
    IEnumerator spawn;

    public bool waiting
    {
        get { return spawn == null; }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Boot(int _player_index, C_Essential essential)
    {
        v_essential = essential;
        player_index = _player_index;
        cam.SetMapSize(new Vector2(m_map.map_data.width, m_map.map_data.height));
        c_ui.ProduceUI(m_map, _player_index);
    }

    public void Game()
    {
        LoadComputer();
        c_map.Set(m_map, player_index);
        c_cursor.Boot(m_map, this);
    }

    public void SpawnUnit(Vector2 pos)
    {
        spawn = SpawnPipeLine(pos);
        StartCoroutine(spawn);
    }
    public IEnumerator SpawnPipeLine(Vector2 pos)
    {
        if (c_ui.selected_unit == null || !v_essential.CheckTurn(player_index))
        {
            StopCoroutine(spawn);
            spawn = null;
            yield return new WaitForSeconds(1f);
        }

        bool val = c_map.SpawnUnit(pos, player_index, c_ui.selected_unit);
        if (!val)
        {
            StopCoroutine(spawn);
            spawn = null;
            yield return new WaitForSeconds(1f);
        }
        cam.transform.position = new Vector3(pos.x + 0.5f, pos.y + 0.5f, cam.transform.position.z);
        c_cursor.cursorPos = pos;
        c_cursor.forcePos = true;
        yield return new WaitForSeconds(1f);

        // Check game ends.
        if (c_map.unitCount >= c_ui.allCellCount)
        {
            c_ui.InGame_End(c_map.CalculateScore());
            c_cursor.gameObject.SetActive(false);
            StopCoroutine(spawn);
            spawn = null;
            yield return new WaitForSeconds(1f);
        }

        // { wait other computer or player }
        int enemyIndex = Random.Range(0, (coms[1].Count - 1));
        Vector2 enemyPos = c_map.GetEmptyCell();
        if (enemyPos.x < 0 || enemyPos.y < 0)
        {
            c_cursor.forcePos = false;
            StopCoroutine(spawn);
            spawn = null;
            yield return new WaitForSeconds(1f);
        }
        c_map.SpawnUnit(enemyPos, 1, coms[1][enemyIndex]);
        coms[1].RemoveAt(enemyIndex);
        c_cursor.cursorPos = enemyPos;
        cam.transform.position = new Vector3(enemyPos.x + 0.5f, enemyPos.y + 0.5f, cam.transform.position.z);

        yield return new WaitForSeconds(1f);
        c_cursor.forcePos = false;

        StopCoroutine(spawn);
        spawn = null;

        // Check game ends.
        if (c_map.unitCount >= c_ui.allCellCount)
        {
            c_ui.InGame_End(c_map.CalculateScore());
            c_cursor.gameObject.SetActive(false);
        }
        
        yield return new WaitForSeconds(1f);
    }

    // { incomplete }
    public void LoadComputer()
    {
        Dictionary<int, List<M_Unit>> comList = new Dictionary<int, List<M_Unit>>();
        // { edit needed }
        comList.Add(1, new List<M_Unit>());

        int cost = 0;
        for (int x = 0; x < m_map.map_data.width; x++)
            for (int y = 0; y < m_map.map_data.height; y++)
            {
                // {  == 0 >> need to read other data(kind of rule file) }
                if (m_map.map_data.GetPixel(x, y).r == 0)
                    cost++;
            }
        cost = (int)Math.Ceiling(((double)cost / m_map.player_color.Count));

        List<bool> dir;
        for (int i = 0; i < cost; i += 2)
        {
            dir = new List<bool>();
            dir.Add(true);
            dir.Add(false);
            dir.Add(true);
            dir.Add(false);
            dir.Add(true);
            dir.Add(false);
            dir.Add(true);
            dir.Add(false);
            comList[1].Add(new M_Unit(dir.ToArray(), 5));

            dir = new List<bool>();
            dir.Add(false);
            dir.Add(true);
            dir.Add(false);
            dir.Add(true);
            dir.Add(false);
            dir.Add(true);
            dir.Add(false);
            dir.Add(true);
            comList[1].Add(new M_Unit(dir.ToArray(), 5));
        }
        if(cost > 0)
        {
            dir = new List<bool>();
            dir.Add(true);
            dir.Add(false);
            dir.Add(true);
            dir.Add(false);
            dir.Add(true);
            dir.Add(false);
            dir.Add(true);
            dir.Add(false);
            comList[1].Add(new M_Unit(dir.ToArray(), 5));
        }
        coms = comList;
    }
}
