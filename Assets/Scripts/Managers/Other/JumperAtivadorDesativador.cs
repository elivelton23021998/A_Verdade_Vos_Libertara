using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumperAtivadorDesativador : MonoBehaviour
{
    Electricity luz;
    public GameObject[] objs;
    bool ctrl;

    // Start is called before the first frame update
    void Start()
    {
        if(GetComponent<Electricity>()) luz = GetComponent<Electricity>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GetComponent<Electricity>())
        {
            if (!luz.isPoweredOn)
            {
                ctrl = true;
                StartCoroutine(Switch());
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !ctrl)
        {
            ctrl = true;
            StartCoroutine(Switch());
            
        }
    }

    private IEnumerator Switch()
    {
        for (int i = 0; i < objs.Length; i++)
        {
            if (!objs[i].activeSelf) objs[i].SetActive(true);
            else objs[i].SetActive(false);
        }
        this.enabled = false;
        yield return null;
    }
}
