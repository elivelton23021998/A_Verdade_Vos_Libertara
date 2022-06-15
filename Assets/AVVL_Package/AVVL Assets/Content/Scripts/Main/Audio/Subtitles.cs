using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Subtitles : MonoBehaviour
{
    //Vari�veis do �udio
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] voiceovers;

    //Vari�veis do texto
    [SerializeField] private TextMeshProUGUI subtitleText;
    [SerializeField] private string[] subtitles;

    //Impede o usu�rio de iniciar a coroutine at� que ela tenha terminado ou parado
    private bool canPlay = true;

    //Guarda a refer�ncia da inst�ncia atual da coroutine
    private IEnumerator coroutine;

    private void Start()
    {
        
    }

    public void PlayDialogue()
    {
        if (canPlay)
        {
            canPlay = false;
            //Passa a inst�ncia atual da coroutine para o coroutineAtual
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
        audioSource.Stop();
        //Desabilita o texto
        subtitleText.gameObject.SetActive(false);
        //Permite que a coroutine possa iniciar
        canPlay = true;
        
    }

    private IEnumerator PlaySpeech()
    {
        for (int i = 0; i < voiceovers.Length; i++)
        {
            //Passa a dublagem atual para o Audio Source e toca
            audioSource.clip = voiceovers[i];
            audioSource.Play();

            //Faz o texto da legenda aparecer e passa a legenda atual para o texto
            subtitleText.gameObject.SetActive(true);
            subtitleText.text = subtitles[i];

            //Espera o audio da dublagem atual acabar
            yield return new WaitForSeconds(voiceovers[i].length);
            //Esconde o texto da dublagem
            subtitleText.gameObject.SetActive(false);
        }
        //Permite que a coroutine possa iniciar
        canPlay = true;
        gameObject.SetActive(false);
    }
}