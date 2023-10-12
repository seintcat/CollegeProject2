using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class C_ScreenFade : MonoBehaviour
{
    [SerializeField]
    RectTransform _transform;
    [SerializeField]
    Image image;
    float time, start;
    bool fade;
    Color color;
    [SerializeField]
    CommandCore m_command;
    [SerializeField]
    C_Sprites v_sprites;

    public M_ScreenFade data;
    public bool loaded;

    public void Make()
    {
        Function.SetRect(_transform);

        time = 0f;
        fade = false;
        loaded = true;
    }

    public void On(M_ScreenFade _data)
    {
        image.sprite = null;
        if (_data.sprites.Count > 0)
        {
            image.sprite = _data.sprites[0];
            v_sprites.Set(_data.sprites.ToArray(), _data.waitTime, _transform);
            v_sprites.Repeat();
        }
        image.type = Image.Type.Tiled;
        image.raycastTarget = true;

        color = _data.color;
        color.a = 0f;
        image.color = color;
        time = _data.fadeInTime;
        fade = true;
        start = time;

        gameObject.SetActive(true);
    }

    public void Off(M_ScreenFade _data)
    {
        if(_data.sprites.Count == 0)
            image.sprite = null;
        //if (_data.sprites.Count > 0)
        //{
        //    image.sprite = _data.sprites[0];
        //    v_sprites.Set(_data.sprites.ToArray(), _data.waitTime, _transform);
        //    v_sprites.Repeat();
        //}
        image.type = Image.Type.Tiled;
        image.raycastTarget = false;

        color = _data.color;
        color.a = 1f;
        image.color = color;
        time = _data.fadeOutTime;
        fade = false;
        start = time;

        gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        start -= Time.deltaTime;
        if (time == 0f)
        {
            color.a = 1f;
            image.color = color;
        }    
        else if (start > 0f)
        {
            if (fade)
                color.a += Time.deltaTime / time;
            else
                color.a -= Time.deltaTime / time;

            image.color = color;
        }
        else if(!fade)
        {
            image.raycastTarget = false;
            gameObject.SetActive(false);
        }
    }

    public IEnumerator Load(int sceneNum, string bundleName)
    {
        string path = Application.streamingAssetsPath + "/" + sceneNum + "/Fade/" + bundleName;
        loaded = false;

        AssetBundleCreateRequest request1 = AssetBundle.LoadFromFileAsync(path);
        while (!request1.isDone)
            yield return new WaitForSeconds(0.1f);

        AssetBundleRequest request2 = request1.assetBundle.LoadAssetAsync("info");
        while (!request2.isDone)
            yield return new WaitForSeconds(0.1f);

        List<CommandData> infos = CommandCore.Decode(m_command, request2.asset.ToString());
        Sprite[] sprites = null;
        float term = -1f;
        foreach (CommandData info in infos)
        {
            switch(info.command)
            {
                case 0:
                    float.TryParse(info.text, out data.color.r);
                    break;
                case 1:
                    float.TryParse(info.text, out data.color.g);
                    break;
                case 2:
                    float.TryParse(info.text, out data.color.b);
                    break;
                case 3:
                    float.TryParse(info.text, out data.color.a);
                    break;
                case 4:
                    float.TryParse(info.text, out data.fadeInTime);
                    break;
                case 5:
                    float.TryParse(info.text, out data.fadeOutTime);
                    break;
                case 6:
                    request2 = request1.assetBundle.LoadAssetWithSubAssetsAsync<Sprite>(info.text);
                    while (!request2.isDone)
                        yield return new WaitForSeconds(0.1f);

                    List<Sprite> list = new List<Sprite>();
                    foreach(Object obj in request2.allAssets)
                        list.Add((Sprite)obj);
                    sprites = list.ToArray();
                    break;
                case 7:
                    float.TryParse(info.text, out term);
                    break;
            }
        }
        data.waitTime = term;
        if(sprites != null)
            data.sprites = new List<Sprite>(sprites);

        loaded = true;
        yield return null;
    }
}
