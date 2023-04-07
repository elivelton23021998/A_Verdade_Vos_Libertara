using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class LuzEfeito : MonoBehaviour
{
    public bool apenasLuz, luzPiscando;
    public NavMeshAgent guarda;
    public float tempoDesligado, velocidade;
    Light luz;
    bool desligado;
    Transform pj;
    float brilhoMax, velAtual;

    // Start is called before the first frame update
    void Start()
    {
        velAtual = velocidade;
        brilhoMax = GetComponent<Light>().intensity;
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

        float brilho = brilhoMax;
        if (!luzPiscando)
        {
            brilho = 80f;
            velocidade = Random.Range(velAtual / 2, velAtual);
        }
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

        float max = brilhoMax;
        if (!luzPiscando)
        {
            brilho = 80f;
            velocidade = Random.Range(velAtual / 2, velAtual);
        }

        while (brilho < max)
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
