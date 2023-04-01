using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class LuzEfeito : MonoBehaviour
{
    public bool apenasLuz;
    public NavMeshAgent guarda;
    public float tempoDesligado, velocidade;
    Light luz;
    bool desligado;
    Transform pj;
    // Start is called before the first frame update
    void Start()
    {
        luz = GetComponent<Light>();
        StartCoroutine(IluminacaoPiscando());
    }

    // Update is called once per frame
    void Update()
    {
        if (pj) guarda.SetDestination(pj.position);
    }
    IEnumerator IluminacaoPiscando()
    {
        desligado = false;
        float brilho = 80;
        while (brilho > 0f)
        {
            brilho -= Time.deltaTime * velocidade;
            luz.intensity = brilho;
            yield return null;
        }
        luz.intensity = brilho;
        desligado = true;
        yield return new WaitForSeconds(tempoDesligado);
        desligado = false;
        brilho = 0;
        while (brilho < 80f)
        {
            brilho += Time.deltaTime * velocidade;
            luz.intensity = brilho;
            yield return null;
        }
        luz.intensity = brilho;

        StartCoroutine(IluminacaoPiscando());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !desligado && !apenasLuz)
        {
            pj = other.transform;
            StartCoroutine(other.GetComponent<PlayerController>().Morte());
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && !desligado && !apenasLuz)
        {
            pj = other.transform;
            StartCoroutine(other.GetComponent<PlayerController>().Morte());

        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && !desligado && !apenasLuz)
        {
            pj = other.transform;
            StartCoroutine(other.GetComponent<PlayerController>().Morte());

        }
    }
}
