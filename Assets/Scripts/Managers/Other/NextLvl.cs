using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class NextLvl : MonoBehaviour
{
    public Image imagem;
    public string NomeDaFase;
    public GameObject fecharOlho;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (fecharOlho) fecharOlho.SetActive(true);
            else SceneManager.LoadScene(NomeDaFase);

            gameObject.SetActive(false);

        }
    }
    public void FecharOlho()
    {
        SceneManager.LoadScene(NomeDaFase);
    }
}
