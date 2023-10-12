using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Map", menuName = "Model/Map", order = 1)]
public class M_Map : ScriptableObject
{
    public Texture2D map_data, skin_map, skin_cursor;
    public Material material_map, material_cursor;
    public int atlasSizeInBlocks_map, atlasSizeInBlocks_cursor;
    // map > black, void
    // cursor > normal
    public List<int> skinOrder_map, skinOrder_cursor;

    // { need user color }
    public List<Color> player_color;
}
