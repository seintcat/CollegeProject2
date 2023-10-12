using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ScreenFade", menuName = "Model/ScreenFade", order = 1)]
public class M_ScreenFade : ScriptableObject
{
    public Color color;
    public List<Sprite> sprites;
    public float fadeInTime, fadeOutTime, waitTime;
}
