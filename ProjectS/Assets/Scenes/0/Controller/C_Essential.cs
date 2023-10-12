using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class C_Essential : MonoBehaviour
{
    static C_Essential instance;

    static M_ScreenFade mFade
    {
        set
        {
            if (instance != null)
                instance.m_fade = value;
        }
    }
    [SerializeField]
    M_ScreenFade m_fade;
    [SerializeField]
    C_ScreenFade v_fade;

    [SerializeField]
    C_Loading c_loading;

    [SerializeField]
    CommandCore m_command_root, m_command_style;

    List<CommandData> m_root, m_style;
    List<string> m_style_list;

    [SerializeField]
    C_Menu c_menu;

    IEnumerator scenemove;
    C_GameHeader gameHeader;

    private void Awake()
    {
        if (instance != null)
            Destroy(gameObject);

        v_fade.Make();
        m_style_list = new List<string>();
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(StartPipeLine_TitleLoad());
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();
    }

    public void FadeIn(M_ScreenFade _m_fade)
    {
        v_fade.On(_m_fade);
    }

    public void FadeOut(M_ScreenFade _m_fade)
    {
        v_fade.Off(_m_fade);
    }

    IEnumerator StartPipeLine_TitleLoad()
    {
        FadeIn(m_fade);
        yield return Time.deltaTime;

        // Read root.
        AssetBundleCreateRequest request1 = AssetBundle.LoadFromFileAsync(Application.streamingAssetsPath + "/root");
        while (!request1.isDone)
            yield return new WaitForSeconds(0.1f);

        AssetBundleRequest request2 = request1.assetBundle.LoadAssetAsync("root");
        while (!request2.isDone)
            yield return new WaitForSeconds(0.1f);

        m_root = CommandCore.Decode(m_command_root, request2.asset.ToString());
        request1 = AssetBundle.LoadFromFileAsync(Application.streamingAssetsPath + "/Style/" + m_root[0].text);
        while (!request1.isDone)
            yield return new WaitForSeconds(0.1f);

        request2 = request1.assetBundle.LoadAssetAsync("info");
        while (!request2.isDone)
            yield return new WaitForSeconds(0.1f);

        m_style = CommandCore.Decode(m_command_style, request2.asset.ToString());

        // Make loading.
        StartCoroutine(c_loading.Load(0, m_style[0].text));
        while (!c_loading.IsReady(0, m_style[0].text))
            yield return new WaitForSeconds(0.1f);

        c_loading.Set(0, m_style[0].text);
        FadeOut(m_fade);

        // Pre-Loading works.
        c_loading.Set(0, m_style[0].text, "Loading StyleSheet...", 0.1f); 
        DirectoryInfo di = new DirectoryInfo(Application.streamingAssetsPath + "/Style/");
        foreach (FileInfo File in di.GetFiles())
            if (File.Extension.ToLower() == "")
                m_style_list.Add(File.Name);

        // Load complete
        yield return new WaitForSeconds(m_fade.fadeOutTime);
        StartCoroutine(v_fade.Load(0, m_style[1].text));
        while (!v_fade.loaded)
            yield return new WaitForSeconds(0.1f);

        FadeIn(v_fade.data);
        yield return new WaitForSeconds(v_fade.data.fadeInTime);

        // Scene move.
        DontDestroyOnLoad(gameObject);
        SceneManager.LoadScene(1);
        yield return new WaitForFixedUpdate();
        SceneManager.MoveGameObjectToScene(gameObject, SceneManager.GetSceneByBuildIndex(1));

        c_loading.Off();
        c_menu.On();
        FadeOut(v_fade.data);
        yield return new WaitForSeconds(v_fade.data.fadeOutTime);

        // 

        yield return null;
        //bundle.Unload(true);
    }

    // { test function }
    public void StartPipeline_Game(bool debugMap, int player_index)
    {
        if (scenemove == null)
        {
            if (debugMap)
                scenemove = Pipeline_Game(4, player_index);
            else
                scenemove = Pipeline_Game(3, player_index);

            StartCoroutine(scenemove);
        }
    }
    IEnumerator Pipeline_Game(int index, int player_index)
    {
        // { check fade data }
        FadeIn(v_fade.data);
        yield return new WaitForSeconds(v_fade.data.fadeInTime);

        DontDestroyOnLoad(gameObject);
        SceneManager.LoadScene(index);

        yield return new WaitForFixedUpdate();
        SceneManager.MoveGameObjectToScene(gameObject, SceneManager.GetSceneByBuildIndex(index));

        gameHeader = GameObject.Find("V_GameHeader").GetComponent<C_GameHeader>();
        // { get map data }
        // { check player index is possible }
        gameHeader.Boot(player_index, this);

        FadeOut(v_fade.data);
        yield return new WaitForSeconds(v_fade.data.fadeOutTime);

        Pipeline_Game_End();
    }
    void Pipeline_Game_End()
    {
        StopCoroutine(scenemove);
        scenemove = null;
    }

    // { incomplete }
    public bool CheckTurn(int player_index)
    {
        return true;
    }
}
