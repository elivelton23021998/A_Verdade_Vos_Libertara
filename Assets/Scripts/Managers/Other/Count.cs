using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class Count : MonoBehaviour
{

    public float tempo;
    public string nomeCena;
    // Start is called before the first frame update
    void Start()
    {
        if (nomeCena == "Menu") PlayerPrefs.DeleteAll();
        StartCoroutine (Game ());
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return)) SceneManager.LoadScene(nomeCena);
        if (Input.GetKey(KeyCode.Escape)) SceneManager.LoadScene(nomeCena);
        if (Input.GetKey(KeyCode.Space)) SceneManager.LoadScene(nomeCena);
    }
    IEnumerator Game()
    {
        yield return new WaitForSeconds(tempo);
        SceneManager.LoadScene(nomeCena);
    }
}
