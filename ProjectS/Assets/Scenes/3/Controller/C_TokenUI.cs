using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class C_TokenUI : MonoBehaviour
{
    [SerializeField]
    Image back;
    [SerializeField]
    EventTrigger trigger;
    [SerializeField]
    TextMeshProUGUI number_text;
    bool[] dir;
    C_UI v_ui;
    static C_EditingToken editing;

    public int number
    {
        get
        {
            int val = -1;
            if (int.TryParse(number_text.text, out val))
                return val;

            return val;
        }
    }
    public bool[] directions
    {
        set { dir = value; }
        get { return dir; }
    }
    public int cost
    {
        get
        {
            int d = 0;
            foreach (bool b in dir)
                if (b)
                    d++;
            return number * d;
        }
    }
    public Texture2D texture
    {
        get { return back.sprite.texture; }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Boot(Color color)
    {
        TextMeshProUGUI font = transform.parent.parent.Find("Text (TMP)").gameObject.GetComponent<TextMeshProUGUI>();

        number_text.color = font.color;
        number_text.fontMaterial = font.fontMaterial;
        number_text.fontMaterials = font.fontMaterials;
        number_text.fontSharedMaterial = font.fontSharedMaterial;
        number_text.fontSharedMaterials = font.fontSharedMaterials;
        number_text.fontSize = font.fontSize;
        number_text.fontSizeMax = font.fontSizeMax;
        number_text.fontSizeMin = font.fontSizeMin;
        number_text.fontStyle = font.fontStyle;
        number_text.fontWeight = font.fontWeight;
        number_text.styleSheet = font.styleSheet;
        number_text.textStyle = font.textStyle;

        editing = transform.parent.gameObject.GetComponent<C_EditingToken>();
        back.color = color;
    }

    public GameObject Copy(Texture2D _texture, bool[] direction, int value)
    {
        GameObject obj = Instantiate(gameObject);
        obj.GetComponentsInChildren<C_TokenUI>()[0].Set(_texture, direction, value);
        return obj;
    }

    public void Set(Texture2D _texture, bool[] direction, int value)
    {
        back.sprite = Sprite.Create(_texture, new Rect(0, 0, _texture.width, _texture.height), new Vector2(0, 0));
        dir = direction;
        number_text.text = value.ToString();

        trigger.triggers.Clear();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerClick;
        entry.callback.AddListener((data) => { ClickEventHandler_Produce((PointerEventData)data); });
        trigger.triggers.Add(entry);
    }
    public void Set(C_UI ui)
    {
        v_ui = ui;
    }

    public void ClickEventHandler_Produce(PointerEventData data)
    {
        if (v_ui != null)
            v_ui.Produce_EditToken(this);
    }

    public void Merge(C_TokenUI target)
    {
        target.Set(back.sprite.texture, dir, number);
    }

    public void Apply()
    {
        editing.Set(dir, number);
    }

    public Transform Ready()
    {
        // Set click event.
        trigger.triggers.Clear();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerClick;
        entry.callback.AddListener((data) => { ClickEventHandler_InGame((PointerEventData)data); });
        trigger.triggers.Add(entry);

        gameObject.GetComponentsInChildren<AspectRatioFitter>()[0].aspectMode = AspectRatioFitter.AspectMode.HeightControlsWidth;

        return transform;
    }

    public void ClickEventHandler_InGame(PointerEventData data)
    {
        v_ui.InGame_Select(this);
    }

    //Function.SetRect(gameObject.GetComponent<RectTransform>());
}
