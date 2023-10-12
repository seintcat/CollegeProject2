using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class C_Menu : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T) && Input.GetKey(KeyCode.LeftControl))
        {
            // { set player index }
            FindObjectOfType<C_Essential>().StartPipeline_Game(true, 0);
        }
    }

    public void On()
    {
        gameObject.SetActive(true);
    }
}
