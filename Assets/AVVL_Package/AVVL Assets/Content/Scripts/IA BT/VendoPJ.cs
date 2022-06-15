using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VendoPJ : MonoBehaviour
{
    public BehaviourTree bt;
    Transform pj;
    public Material[] olho;
    public MeshRenderer plano;
    // Start is called before the first frame update
    void Start()
    {
       pj = GameObject.FindGameObjectWithTag("Player").transform;

        StartCoroutine (Virando());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private IEnumerator Virando()
    {
        plano.material = olho[bt.nivel];
        transform.LookAt(pj);
        yield return new WaitForSeconds(0.01f);
        StartCoroutine(Virando());
    }
}
