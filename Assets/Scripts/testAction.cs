using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class testAction : MonoBehaviour
{
    private void Start()
    {
        DontDestroyOnLoad(this);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(new Vector3(Input.GetAxisRaw("Horizontal") * Time.deltaTime, 0, Input.GetAxisRaw("Vertical") * Time.deltaTime));

        if (Input.GetKeyDown(KeyCode.Space))
        {
            changeScene();
        }
    }

    private void changeScene()
    {
        if (SceneManager.GetActiveScene().name == "Test1")
        {
            SceneManager.LoadScene("Test2");
        }
        else if (SceneManager.GetActiveScene().name == "Test2")
        {
            SceneManager.LoadScene("Test1");
        }
    }
}

