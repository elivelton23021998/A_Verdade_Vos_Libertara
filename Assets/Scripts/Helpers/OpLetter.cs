using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OpLetter : MonoBehaviour
{
    //BoxCollider col;
    public float tempo;
    public Image imagem;
    // Start is called before the first frame update
    void Start()
    {
        //col = GetComponent<BoxCollider>();
        
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(Lettering(other.gameObject));

           // col.enabled = false;
        }
    }
    IEnumerator Lettering(GameObject pj)
    {
       
        pj.GetComponent<PlayerController>().enabled = false;

        Color cor = imagem.color;
        cor.a = 0;
        while (cor.a < 0.9f)
        {
            cor.a += Time.deltaTime;
            imagem.color = cor;
            yield return null;
        }
        imagem.color = cor;
        yield return new WaitForSeconds(tempo);
        cor.a = 1;
        while (cor.a > 0f)
        {
            cor.a -= Time.deltaTime;
            imagem.color = cor;
            yield return null;
        }
        imagem.color = cor;

        pj.GetComponent<PlayerController>().enabled = true;
        gameObject.SetActive(false);

    }
}
