using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class C_Sprites : MonoBehaviour
{
    Sprite[] sprites;
    float waitTime;
    Image image;
    int index;
    IEnumerator enumerator;
    [SerializeField]
    RectTransform rect;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Set(Sprite[] _sprites, float _waitTime, RectTransform _rect = null)
    {
        if (enumerator != null)
            StopCoroutine(enumerator);

        index = 0;
        sprites = _sprites;
        waitTime = _waitTime;
        
        if(_rect != null)
        {
            transform.SetParent(_rect);
            image = _rect.gameObject.GetComponent<Image>();
        }
        else
        {
            transform.SetParent(rect);
            image = rect.gameObject.GetComponent<Image>();
        }
        image.sprite = sprites[index];
    }

    private void OnEnable()
    {
        if (enumerator != null)
            StartCoroutine(enumerator);
    }

    private void OnDisable()
    {
        if (enumerator != null)
            StopCoroutine(enumerator);
    }

    public void Show(int _index = 0)
    {
        index = _index;
        image.sprite = sprites[index];
    }

    public void Repeat(int _index = 0)
    {
        Show(_index);

        if (waitTime > 0)
        {
            enumerator = _Repeat();
            if (gameObject.activeSelf)
                StartCoroutine(enumerator);
        }
    }
    IEnumerator _Repeat()
    {
        while(true)
        {
            yield return new WaitForSeconds(waitTime);
            index++;
            if (sprites.Length <= index)
                index = 0;

            image.sprite = sprites[index];
        }
    }

    public void Play()
    {
        Show(0);

        if (waitTime > 0)
        {
            enumerator = _Play();
            if (gameObject.activeSelf)
                StartCoroutine(enumerator);
        }
    }
    IEnumerator _Play()
    {
        while (sprites.Length > index)
        {
            yield return new WaitForSeconds(waitTime);

            index++;
            image.sprite = sprites[index];
        }

        StopCoroutine(enumerator);
        enumerator = null;
    }
}
