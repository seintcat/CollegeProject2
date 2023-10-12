using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class C_Loading : MonoBehaviour
{
    Dictionary<string, GameObject> loadedView;
    Dictionary<string, AssetBundle> loadedBundle;
    Dictionary<string, loadType> loadedType;
    Dictionary<string, bool> isReady;

    TextMeshProUGUI v_text;
    RectTransform v_progress;
    [SerializeField]
    CommandCore m_command;

    [SerializeField]
    GameObject v_sprites;

    public enum loadType
    {
        Vertical,
        Horizontal,
    }

    private void Awake()
    {
        loadedView = new Dictionary<string, GameObject>();
        loadedBundle = new Dictionary<string, AssetBundle>();
        loadedType = new Dictionary<string, loadType>();
        isReady = new Dictionary<string, bool>();
    }

    public IEnumerator Load(int sceneNum,  string bundleName)
    {
        GameObject view = null;
        string path = Path(sceneNum, bundleName);
        isReady.Add(path, false);

        // Check bundle already loaded.
        if (loadedBundle.ContainsKey(path))
            view = loadedView[path];
        else
        {
            // Load bundle and make view.
            AssetBundleCreateRequest request1 = AssetBundle.LoadFromFileAsync(path);
            while (!request1.isDone)
                yield return new WaitForSeconds(0.1f);

            AssetBundle bundle = request1.assetBundle;
            if (bundle == null)
            {
                Debug.Log("Failed to load AssetBundle!");
                yield return null;
            }

            AssetBundleRequest request2 = bundle.LoadAssetAsync("info");
            while (!request2.isDone)
                yield return new WaitForSeconds(0.1f);

            List<CommandData> infos = CommandCore.Decode(m_command, request2.asset.ToString());
            string prefabName = "";
            Sprite[] sprites = null;
            float term = -1f;
            TMP_FontAsset font = null;
            foreach (CommandData info in infos)
            {
                if (info.command == 0)
                    prefabName = info.text;
                else if (info.command == 1)
                    switch (info.text)
                    {
                        case "Vertical":
                            loadedType.Add(path, loadType.Vertical);
                            break;
                        case "Horizontal":
                            loadedType.Add(path, loadType.Horizontal);
                            break;
                    }
                else if (info.command == 2)
                {
                    request2 = bundle.LoadAssetWithSubAssetsAsync<Sprite>(info.text);
                    while (!request2.isDone)
                        yield return new WaitForSeconds(0.1f);

                    sprites = (Sprite[])request2.allAssets;
                }
                else if (info.command == 3)
                    float.TryParse(info.text, out term);
                else if (info.command == 4)
                {
                    request2 = bundle.LoadAssetAsync(info.text);
                    while (!request2.isDone)
                        yield return new WaitForSeconds(0.1f);

                    font = (TMP_FontAsset)request2.asset;
                }
            }

            request2 = bundle.LoadAssetAsync(prefabName);
            while (!request2.isDone)
                yield return new WaitForSeconds(0.1f);

            view = Instantiate((GameObject)request2.asset);
            view.transform.SetParent(gameObject.transform);

            RectTransform rect = view.GetComponent<RectTransform>();
            Function.SetRect(rect);

            loadedBundle.Add(path, bundle);
            loadedView.Add(path, view);

            if(sprites != null)
            {
                //C_Sprites i_sprites = Instantiate(v_sprites).GetComponent<C_Sprites>();
                C_Sprites i_sprites = view.AddComponent<C_Sprites>();
                i_sprites.Set(sprites, term, rect);
                i_sprites.Repeat();
            }

            if (font != null)
            {
                TextMeshProUGUI[] _v_text = view.GetComponentsInChildren<TextMeshProUGUI>();
                foreach (TextMeshProUGUI component in _v_text)
                {
                    if (component.gameObject.name == "Text")
                    {
                        component.font = font;
                        break;
                    }
                }
            }
        }

        view.SetActive(false);

        isReady[path] = true;
    }

    public void Set(int sceneNum, string bundleName, string text = "Now loading...", float progress = 0f)
    {
        if (!gameObject.activeSelf)
            gameObject.SetActive(true);

        string path = Path(sceneNum, bundleName);
        GameObject view = null;
        if (!loadedBundle.ContainsKey(path))
            Load(sceneNum, bundleName);

        view = loadedView[path];
        
        if (!view.activeSelf)
        {
            v_text = null;
            v_progress = null;

            // Off all other loading screen.
            foreach (GameObject obj in loadedView.Values)
                obj.SetActive(false);

            TextMeshProUGUI[] _v_text = view.GetComponentsInChildren<TextMeshProUGUI>();
            foreach (TextMeshProUGUI component in _v_text)
            {
                if (component.gameObject.name == "Text")
                {
                    v_text = component;
                    break;
                }
            }

            RectTransform[] _v_progress = view.GetComponentsInChildren<RectTransform>();
            foreach (RectTransform component in _v_progress)
            {
                if (component.gameObject.name == "Progress")
                {
                    v_progress = component;
                    break;
                }
            }
        }

        if(v_text != null)
            v_text.text = text;
        if (v_progress != null)
        {
            Vector2 anchorMax = v_progress.anchorMax, anchorMin = v_progress.anchorMin, offsetMax = v_progress.offsetMax, offsetMin = v_progress.offsetMin;
            switch (loadedType[path])
            {
                case loadType.Vertical:
                    anchorMin.y = 0f;
                    anchorMax.y = progress;
                    offsetMin.y = 0f;
                    offsetMax.y = 0f;
                    break;
                case loadType.Horizontal:
                    anchorMin.x = 0f;
                    anchorMax.x = progress;
                    offsetMin.x = 0f;
                    offsetMax.x = 0f;
                    break;
            }
            v_progress.anchorMin = anchorMin;
            v_progress.anchorMax = anchorMax;
            v_progress.offsetMin = offsetMin;
            v_progress.offsetMax = offsetMax;
        }

        view.SetActive(true);
    }

    public bool IsReady(int sceneNum, string bundleName)
    {
        return isReady[Path(sceneNum, bundleName)];
    }

    string Path(int sceneNum, string bundleName)
    {
        return Application.streamingAssetsPath + "/" + sceneNum + "/Loading/" + bundleName;
    }

    public void Off()
    {
        gameObject.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
