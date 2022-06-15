using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogoStart : MonoBehaviour
{
    /*
     bool pausado;

     void Start()
     {
     }

     void Update()
     {

     }
     private void OnTriggerEnter(Collider other)
     {
         if (other.CompareTag("Player"))
         {
             StartCoroutine(Dialog());

         }
     }
     private void OnTriggerExit(Collider other)
     {
         if (other.CompareTag("Player"))
         {
             GetComponent<AudioSource>().Pause();
         }
     }

     IEnumerator Dialog()
     {
         if (!pausado) GetComponent<AudioSource>().Play();
         else GetComponent<AudioSource>().UnPause();
         Camera.main.transform.LookAt(transform);
         pausado = true;
         yield return null;
     }
    */
    int iAtual=0;
    bool pv;
    [SerializeField] private GameObject menu;

    //Variáveis do áudio
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] voiceovers;

    //Variáveis do texto
    [SerializeField] private TextMeshProUGUI subtitleText;
    [SerializeField] private string[] subtitles;

    //Impede o usuário de iniciar a coroutine até que ela tenha terminado ou parado
    private bool canPlay = true;

    //Guarda a referência da instância atual da coroutine
    private IEnumerator coroutine;

    public void PlayDialogue()
    {
        if (canPlay)
        {
            canPlay = false;
            //Passa a instância atual da coroutine para o coroutineAtual
            coroutine = PlaySpeech();
            //Inicia a coroutine
            StartCoroutine(coroutine);
        }
    }

    public void StopDialogue()
    {
        //Para a coroutine
        StopCoroutine(coroutine);
        //Para o audio
        GetComponent<AudioSource>().Stop();
        //Desabilita o texto
        subtitleText.gameObject.SetActive(false);
        //Permite que a coroutine possa iniciar
        canPlay = true;
    }

    private IEnumerator PlaySpeech()
    {
        
        for (int i = iAtual; i < voiceovers.Length; i++)
        {
            //Passa a dublagem atual para o Audio Source e toca
            audioSource.clip = voiceovers[i];
            audioSource.Play();
            iAtual = i;
            pv = true;
            //Faz o texto da legenda aparecer e passa a legenda atual para o texto
            subtitleText.gameObject.SetActive(true);
            subtitleText.text = subtitles[i];

            //Espera o audio da dublagem atual acabar
            yield return new WaitForSeconds(voiceovers[i].length);
            //Esconde o texto da dublagem
            subtitleText.gameObject.SetActive(false);
            
            
        }
        //Permite que a coroutine possa iniciar
        iAtual = 0;
        canPlay = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayDialogue();

        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
         
            StopDialogue();
            
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (menu.activeSelf)
            {
                audioSource.Pause();
            }
            else
            {
                audioSource.UnPause();
            }

        }
    }
}
