using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartAudioTrigger : MonoBehaviour
{
    public bool onStart;
  //  public float tempo; 
   // bool check;
   // AudioSource som;
    // Start is called before the first frame update
    void Start()
    {
        //som = GetComponent<AudioSource>();
        if (onStart)
        {
            ativar();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ativar();

        }
    }
    //IEnumerator antiBug()
    //{
    //    yield return new WaitForSeconds(tempo);
    // //   check = false;
    //   // StartCoroutine(antiBug());
    //}
    private void ativar()
    {
        GetComponent<Subtitles>().PlayDialogue();
        
    }
}
