using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class C_EditingToken : MonoBehaviour
{
    TextMeshProUGUI text;
    List<GameObject> list;
    [SerializeField]
    GameObject p_token;
    C_TokenUI tokenUI;

    // Control readable.
    public bool[] directions
    {
        get
        {
            List<bool> dir = new List<bool>();
            foreach (GameObject d in list)
                dir.Add(d.activeSelf);
            dir.RemoveAt(0);

            return dir.ToArray();
        }
    }
    public int number
    {
        set
        {
            text.text = value.ToString();
        }
        get
        {
            int val = -1;
            if (int.TryParse(text.text, out val))
                return val;

            return val;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Boot(Transform root, Color color)
    {
        // Make setting before SetRect.
        // { load sprites }

        // Binding views.
        list = new List<GameObject>();
        list.Add(root.gameObject);
        root.gameObject.GetComponent<Image>().color = color;
        list.Add(list[0].transform.Find("Up").gameObject);
        list.Add(list[0].transform.Find("UpRight").gameObject);
        list.Add(list[0].transform.Find("Right").gameObject);
        list.Add(list[0].transform.Find("DownRight").gameObject);
        list.Add(list[0].transform.Find("Down").gameObject);
        list.Add(list[0].transform.Find("DownLeft").gameObject);
        list.Add(list[0].transform.Find("Left").gameObject);
        list.Add(list[0].transform.Find("UpLeft").gameObject);
        text = list[0].transform.Find("Text (TMP)").gameObject.GetComponent<TextMeshProUGUI>();

        // { set sprite at views }
        // { set image, color, etc }

        ResetValue();
        Function.SetRect(gameObject.GetComponent<RectTransform>());

        // Set result token prefab.
        GameObject t = Instantiate(p_token);
        t.transform.SetParent(transform);
        tokenUI = t.GetComponent<C_TokenUI>();
        tokenUI.Boot(color);
        t.SetActive(false);

        // {  }
        // {  }
    }

    void ResetValue()
    {
        // Initialize views.
        // { edit needed. for rules }
        for (int i = 0; i < 2; i++)
            list[i].SetActive(true);
        for (int i = 2; i < list.Count; i++)
            list[i].SetActive(false);
        text.gameObject.SetActive(true);
        // { check rules, set minimum cost }
        text.text = "1";
    }

    public void ToggleDirection(int index)
    {
        list[index + 1].SetActive(!list[index + 1].activeSelf);
    }

    public GameObject CopyToken()
    {
        Rect rect = list[0].GetComponent<Image>().sprite.rect;
        Texture2D texture = new Texture2D((int)rect.width, (int)rect.height);
        texture.filterMode = list[0].GetComponent<Image>().sprite.texture.filterMode;

        foreach (GameObject obj in list)
        {
            if (!obj.activeSelf)
                continue;

            Sprite s = obj.GetComponent<Image>().sprite;
            Texture2D tex = s.texture;
            rect = s.rect;
            for (int x = (int)rect.x; x < (rect.x + rect.width); x++)
                for (int y = (int)rect.y; y < (rect.y + rect.height); y++)
                    if(tex.GetPixel(x, y).a > 0)
                        texture.SetPixel(x - (int)rect.x, y - (int)rect.y, tex.GetPixel(x, y));
        }
        texture.Apply();

        GameObject ret = tokenUI.Copy(texture, directions, number);
        ret.SetActive(true);

        ResetValue();
        return ret;
    }

    public void Set(bool[] direction, int value)
    {
        for (int i = 1; i < list.Count; i++)
        {
            list[i].SetActive(direction[i - 1]);
        }
        number = value;
    }
}
