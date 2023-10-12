using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class C_UI : MonoBehaviour
{
    M_Map m_map;

    // Produce phase UI.
    // { this must be hide in complete ver, load from file }
    [SerializeField]
    GameObject v_produce;
    [SerializeField]
    string text_cost_err, text_submit_token, text_default, text_not_ready;

    [SerializeField]
    GameObject p_token, p_token_control;
    TextMeshProUGUI v_cost_using, v_cost_remain, v_message, v_token_count;
    string v_cost_using_text, v_cost_remain_text, v_token_count_text;
    int cost, cost_submit, cost_using, token_index, token_max;
    C_EditingToken v_token;
    Transform v_editing_token_pos, v_token_list_pos;
    C_TokenControl v_control;
    Dictionary<int, C_TokenUI> v_token_list;

    // In-Game phase UI.
    // { this must be hide in complete ver, load from file }
    [SerializeField]
    GameObject v_in_game, v_score;

    Transform v_unit_pos, v_selected_pos;
    C_TokenUI selected_token;
    
    public C_TokenUI selected_unit
    {
        get { return selected_token; }
    }
    public int allCellCount;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // { edit needed }
    public void ProduceUI(M_Map _m_map, int _player_index)
    {
        m_map = _m_map;

        // { load produce ui }
        v_produce.SetActive(true);

        // Get text.
        List<TextMeshProUGUI> texts = new List<TextMeshProUGUI>(v_produce.GetComponentsInChildren<TextMeshProUGUI>());
        foreach(TextMeshProUGUI ugui in texts)
        {
            if (ugui.gameObject.name == "Cost_Remain")
            {
                v_cost_remain = ugui;
                v_cost_remain_text = ugui.text;
            }
            else if (ugui.gameObject.name == "Cost_Using")
            {
                v_cost_using = ugui;
                v_cost_using_text = ugui.text;
            }
            else if (ugui.gameObject.name == "Message")
                v_message = ugui;
            else if (ugui.gameObject.name == "Token_Count")
            {
                v_token_count = ugui;
                v_token_count_text = ugui.text;
            }
        }

        // Count cost remain.
        token_max = 0;
        for (int x = 0; x < m_map.map_data.width; x++)
            for (int y = 0; y < m_map.map_data.height; y++)
            {
                // {  == 0 >> need to read other data(kind of rule file) }
                if (m_map.map_data.GetPixel(x, y).r == 0)
                    token_max++;
            }
        allCellCount = token_max;
        token_max = (int)Math.Ceiling(((double)token_max / m_map.player_color.Count));
        cost = token_max * 20;

        cost_submit = 0;
        cost_using = 0;

        // { find view token and set view }
        v_token_list = new Dictionary<int, C_TokenUI>();
        token_index = -1;

        // Set editing token.
        Transform target = v_produce.transform.Find("View");
        v_editing_token_pos = target;
        target = v_editing_token_pos.transform.Find("Token");
        GameObject token = Instantiate(p_token);
        token.transform.SetParent(target);
        v_token = token.GetComponent<C_EditingToken>();
        v_token.Boot(target, m_map.player_color[_player_index]);

        // { add view token in deck and set cost }

        // { Binding controls. }
        target = v_produce.transform.Find("Control").Find("Content");
        v_control = Instantiate(p_token_control).GetComponent<C_TokenControl>();
        v_control.transform.SetParent(target);
        v_control.Boot(this, target);
        v_control.Set(v_token);

        // Set token list.
        v_token_list_pos = v_produce.transform.Find("Scroll View").Find("Viewport").Find("Content");
        TokenCount();

        CostShow();
        v_message.text = text_default;
    }

    void CostShow()
    {
        v_cost_remain.text = v_cost_remain_text + (cost - cost_using - cost_submit);
        v_cost_using.text = v_cost_using_text + cost_using;
    }

    public bool Produce_CheckValue(int value)
    {
        bool check = false;
        int calculate_cost = cost - cost_submit;

        if (value <= calculate_cost)
        {
            cost_using = value;
            check = true;
            v_message.text = text_default;
        }
        else
            v_message.text = text_cost_err;

        CostShow();

        return check;
    }

    public void Produce_AddToken(GameObject obj)
    {
        int _cost = 0;
        if (token_index < 0)
        {
            // Add new token.
            obj.transform.SetParent(v_token_list_pos);
            v_control.Set(v_token);
            Function.SetRect(obj.GetComponent<RectTransform>());
            C_TokenUI t = obj.GetComponentsInChildren<C_TokenUI>()[0];
            v_token_list.Add(v_token_list.Count, t);
            t.Set(this);

            _cost = obj.GetComponentsInChildren<C_TokenUI>()[0].cost;
        }
        else
        {
            // Edit token.
            obj.GetComponentsInChildren<C_TokenUI>()[0].Merge(v_token_list[token_index]);
            _cost = obj.GetComponentsInChildren<C_TokenUI>()[0].cost;
            Destroy(obj);
        }

        cost_submit += _cost;
        token_index = -1;
        CostShow();
        v_message.text = text_submit_token;
        TokenCount();
    }

    public void Produce_EditToken(C_TokenUI target)
    {
        int index = -1;

        // Find target index.
        if (v_token_list.ContainsValue(target))
            index = v_token_list.FirstOrDefault(x => x.Value == target).Key;

        // { Check index. }
        if (index < 0)
            return;

        // Cancel editing token.
        if (token_index > -1)
        {
            cost_submit += v_token_list[token_index].cost;
            //Debug.Log("-" + v_token_list[token_index].cost);
        }

        // Edit new token.
        token_index = index;
        cost_submit -= v_token_list[token_index].cost;
        cost_using = v_token_list[token_index].cost;
        //Debug.Log("+" + v_token_list[token_index].cost);
        v_token_list[token_index].Apply();
        v_control.Set(v_token);

        CostShow();
        v_message.text = text_default;
    }

    void TokenCount()
    {
        v_token_count.text = v_token_count_text + v_token_list.Values.Count + " / " + token_max;
    }

    public void Produce_Ready()
    {
        if(v_token_list.Values.Count >= token_max)
        {
            C_TokenUI[] tokens = v_token_list.Values.ToArray();
            List<Transform> token_pos = new List<Transform>();
            foreach (C_TokenUI t in tokens)
                token_pos.Add(t.Ready());

            InGameUI(token_pos);
            v_produce.SetActive(false);

            GameObject.Find("V_GameHeader").GetComponent<C_GameHeader>().Game();
        }
        else
            v_message.text = text_not_ready;
    }

    public void InGameUI(List<Transform> units)
    {
        // { load ingame ui }
        v_produce.SetActive(false);
        v_in_game.SetActive(true);

        v_unit_pos = v_in_game.transform.Find("Units").Find("Scroll View").Find("Viewport").Find("Content");
        v_selected_pos = v_in_game.transform.Find("Selected");
        foreach (Transform u in units)
            u.SetParent(v_unit_pos);

        //ContentSizeFitter fitter = v_unit_pos.gameObject.GetComponent<ContentSizeFitter>();
        //LayoutRebuilder.ForceRebuildLayoutImmediate(v_unit_pos.transform as RectTransform);
        //fitter.enabled = false;
    }

    public void InGame_Select(C_TokenUI token)
    {
        if (selected_token == token)
            return;

        if(selected_token != null)
            selected_token.transform.SetParent(v_unit_pos);

        selected_token = token;
        selected_token.transform.SetParent(v_selected_pos);
    }

    public void InGame_End(List<Vector2Int> score)
    {
        v_in_game.SetActive(false);

        Vector2Int winner = score[0];
        foreach (Vector2Int p in score)
            if (p.y > winner.y)
                winner = p;

        // { load score ui }
        TextMeshProUGUI text = v_score.transform.Find("Win").GetComponent<TextMeshProUGUI>();
        text.text = "Player " + (winner.x + 1) + text.text;
        text.color = m_map.player_color[winner.x];

        GameObject obj = v_score.transform.Find("Scroll View").Find("Viewport").Find("Content").Find("Text (TMP)").gameObject;
        foreach (Vector2Int p in score)
        {
            text = Instantiate(obj).GetComponent<TextMeshProUGUI>();
            text.text = "Player " + (p.x + 1) + text.text + p.y;
            text.color = m_map.player_color[p.x];
            text.transform.SetParent(obj.transform.parent);
        }
        Destroy(obj);

        v_score.SetActive(true);
    }
}
