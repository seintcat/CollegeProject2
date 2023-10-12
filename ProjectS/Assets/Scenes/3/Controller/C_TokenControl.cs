using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class C_TokenControl : MonoBehaviour
{
    C_UI v_ui;
    C_EditingToken v_token;
    List<Toggle> list;
    Button v_submit, v_ready;
    TMP_InputField input;
    List<bool> dir_toggles;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Boot(C_UI ui, Transform pos)
    {
        v_ui = ui;
        list = new List<Toggle>();
        dir_toggles = new List<bool>();

        // Find buttons, inputs.
        list.Add(pos.Find("Up").gameObject.GetComponent<Toggle>());
        list.Add(pos.Find("UpRight").gameObject.GetComponent<Toggle>());
        list.Add(pos.Find("Right").gameObject.GetComponent<Toggle>());
        list.Add(pos.Find("DownRight").gameObject.GetComponent<Toggle>());
        list.Add(pos.Find("Down").gameObject.GetComponent<Toggle>());
        list.Add(pos.Find("DownLeft").gameObject.GetComponent<Toggle>());
        list.Add(pos.Find("Left").gameObject.GetComponent<Toggle>());
        list.Add(pos.Find("UpLeft").gameObject.GetComponent<Toggle>());
        input = pos.Find("InputField (TMP)").gameObject.GetComponent<TMP_InputField>();

        // Bind events.
        for (int i = 0; i < list.Count; i++)
        {
            list[i].onValueChanged = new Toggle.ToggleEvent();
            list[i].onValueChanged.AddListener(DirectionChangeHandler);
            dir_toggles.Add(false);
        }
        input.onValueChanged = new TMP_InputField.OnChangeEvent();
        input.onValueChanged.AddListener(NumberChangeHandler);

        v_submit = pos.Find("Submit").gameObject.GetComponent<Button>();
        v_submit.onClick = new Button.ButtonClickedEvent();
        v_submit.onClick.AddListener(SubmitHandler);

        v_ready = pos.Find("Ready").gameObject.GetComponent<Button>();
        v_ready.onClick = new Button.ButtonClickedEvent();
        v_ready.onClick.AddListener(ReadyHandler);
    }

    public void Set(C_EditingToken token)
    {
        v_token = token;

        // Synchronize token and view.
        bool[] dir = v_token.directions;
        for (int i = 0; i < dir.Length; i++)
        {
            list[i].SetIsOnWithoutNotify(dir[i]);
            dir_toggles[i] = dir[i];
        }

        input.text = v_token.number.ToString();
    }

    void DirectionChangeHandler(bool tmp)
    {
        int i;
        for (i = 0; i < list.Count; i++)
            if (list[i].isOn != dir_toggles[i])
                break;

        CheckDirectionChange(i);
    }
    void CheckDirectionChange(int index)
    {
        int dir_count = 0, value = 0;
        Toggle toggle = list[index];
        foreach (Toggle t in list)
            if (t.isOn)
                dir_count++;

        int.TryParse(input.text, out value);
        value *= dir_count;

        if (v_ui.Produce_CheckValue(value))
            v_token.ToggleDirection(index);
        else
            toggle.SetIsOnWithoutNotify(!toggle.isOn);

        dir_toggles[index] = toggle.isOn;
    }

    void NumberChangeHandler(string num)
    {
        int number = v_token.number, value = 0;
        if (int.TryParse(num, out number))
        {
            // { check value min, max rule }

            foreach (Toggle t in list)
                if (t.isOn)
                    value++;

            value *= number;
            if (v_ui.Produce_CheckValue(value))
                v_token.number = number;
        }
        input.SetTextWithoutNotify(v_token.number.ToString());
    }

    void SubmitHandler()
    {
        int dir_count = 0, value = 0;
        foreach (Toggle t in list)
            if (t.isOn)
                dir_count++;

        int.TryParse(input.text, out value);
        value *= dir_count;

        if (!v_ui.Produce_CheckValue(value))
            return;

        GameObject token = v_token.CopyToken();
        v_ui.Produce_AddToken(token);

        Set(v_token);
        NumberChangeHandler(input.text);
    }

    void ReadyHandler()
    {
        v_ui.Produce_Ready();
    }
}
