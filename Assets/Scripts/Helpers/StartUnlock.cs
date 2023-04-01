using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartUnlock : MonoBehaviour
{
    public Transform camPivo;
    public Animator animator;
    public float tempoDaPrimeiraCinematica;
    [SerializeField] int forcada;
    bool struggleBK;

    bool ativar;
    
    // Start is called before the first frame update
    void Start()
    {
        
        GetComponent<PlayerController>().enabled = false;

        /*
        animator.SetBool("Andando", false);
        animator.SetBool("Agachado", true);
        animator.SetBool("Correndo", false);
        animator.SetBool("AndandoAgachado", false);
        */
        StartCoroutine(Inicio());
    }

    // Update is called once per frame
    void Update()
    {
      if (ativar) Libertação();
      else
        {
            GetComponent<PlayerController>().enabled = false;
        }
    }

    void Libertação()
    {
        if (forcada > 9)
        {
            GetComponent<PlayerController>().enabled = true;
           
            this.enabled = false;
        }
       

        if ( !struggleBK && Input.GetKeyDown(KeyCode.A))
        {
            struggleBK = true;
            camPivo.Translate(-1, 0, 0);
            //transform.position = Vector3.MoveTowards(transform.position, fechado, 12 * Time.deltaTime);
        }

        if (struggleBK && Input.GetKeyDown(KeyCode.D))
        {
            camPivo.Translate(1, 0, 0);
            forcada++;
            struggleBK = false;
        }

       
    }

    IEnumerator Inicio()
    {

        yield return new WaitForSeconds(tempoDaPrimeiraCinematica);
        ativar = true;

    }
}
