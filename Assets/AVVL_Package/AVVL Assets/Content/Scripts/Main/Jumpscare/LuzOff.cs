using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LuzOff : MonoBehaviour
{
    GameObject[] luzes;
    // Start is called before the first frame update
    void Start()
    {
        luzes = GameObject.FindGameObjectsWithTag("LuzesLaterais");
        for (int i = 0; i < luzes.Length; i++)
        {
            luzes[i].SetActive(false);
        }
        StartCoroutine(Off());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private IEnumerator Off()
    {
        yield return new WaitForSeconds(3);
        GetComponent<BoxCollider>().enabled = false;
    }
}
