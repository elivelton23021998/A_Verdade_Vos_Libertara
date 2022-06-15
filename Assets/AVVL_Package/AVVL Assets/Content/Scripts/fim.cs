using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class fim : MonoBehaviour
{
    public Image end;
    bool check;
    DynamicObject porta;
    // Start is called before the first frame update
    void Start()
    {
        porta = GetComponent<DynamicObject>();
    }

    // Update is called once per frame
    void Update()
    {
        if (porta.useType == Type_Use.Normal)
        {
            StartCoroutine(Fim());
            
        }
    }
    public IEnumerator Fim()
    {
        if (!check)
        {
            check = true;
            Color cor = end.color;
            cor.a = 0;
            while (cor.a < 1f)
            {
                cor.a += Time.deltaTime;
                end.color = cor;
                yield return null;
            }
            end.color = cor;
            yield return new WaitForSeconds(0.5f);

            SceneManager.LoadScene("CenaFinal");

            // SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
