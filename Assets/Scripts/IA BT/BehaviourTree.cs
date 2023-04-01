using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
//using FMODUnity;


public class BehaviourTree : MonoBehaviour
{
    public Transform visao;
    public bool patrulhador;
    public Transform[] pontoDeDeslocamento;
    public float anguloMax, distanceRay;
    [HideInInspector] public float distancia;
    [HideInInspector] public int ponto = 0, nivel;//se ele tiver ponto de patrulha ele vai começar no ponto 0
    [HideInInspector] public GameObject jogador;
    [HideInInspector] public Rigidbody rb;
    [HideInInspector] public NavMeshAgent agente;
    [HideInInspector] public bool alvo, morreu,/* assobio,*/visto;

   
        

    //    [Tooltip("(true = patrulhar) O inimigo vai patruhar ou vai fazer um caminho fixo e parar ")]

   


    private void Awake()
    {

        jogador = GameObject.FindGameObjectWithTag("Player");
        agente = GetComponent<NavMeshAgent>();
        // anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        //StartCoroutine(Animacao());
        StartCoroutine(JogadorProximo());

            }

    private IEnumerator JogadorProximo()
    {
        for (int i = 1; i <= 5; i++)
        {
            if (Vector3.Distance(visao.position, jogador.transform.position) < distanceRay/i)
            {

                Vector3 alvo = jogador.transform.position - visao.position;
                if (Vector3.Angle(transform.forward, alvo) <= anguloMax/i)// se o angulo entre a visao da torreta e o caminho do plauer ser a msm
                {
                    Ray raio = new Ray(visao.position, alvo);
                    Debug.DrawRay(raio.origin, raio.direction * 10, Color.red);

                    RaycastHit hit;
                    if (Physics.Raycast(raio, out hit, distanceRay))
                    {
                        if (hit.transform == jogador.transform)
                        {
                            nivel = i;
                        }
                    }
                }
            }
        }
        yield return new WaitForSeconds(0.1f);// recurso de otimizacao
        StartCoroutine(JogadorProximo());
    }
    private void FixedUpdate()
    {
        if (nivel == 5) StartCoroutine(jogador.GetComponent<PlayerController>().Morte());

        if (patrulhador)
        {
            agente.SetDestination(pontoDeDeslocamento[ponto].position);
            transform.LookAt(pontoDeDeslocamento[ponto]);

            float dist = Vector3.Distance(transform.position, pontoDeDeslocamento[ponto].position);
            if (dist < 1)
            {
                AddIndex();
            }

        }
    }
    void AddIndex()
    {
        ponto++;
        if (ponto >= pontoDeDeslocamento.Length)
        {
            ponto = 0;
        }
    }
    //        private void FixedUpdate()
    //        {
    //    //      // if (visual != 0f) print (visual);

    //    //        //print(visto);

    //    //        //if (!morreu)
    //    //        //{
    //    //        //    if (rb.velocity == Vector3.zero)
    //    //        //    {
    //    //        //        anim.Play("Idle");
    //    //        //        agente.angularSpeed = 30;
    //    //        //    }
    //    //        //    else
    //    //        //    {
    //    //        //        anim.Play("WalkingGuarda");
    //    //        //        agente.angularSpeed = 120;
    //    //        //    }
    //    //        //}
    //    //        //else
    //    //        //{
    //    //        //    inimigo.enabled = false;
    //    //        //    anim.Play("Dead");

    //    //        //    GetComponent<CapsuleCollider>().enabled = false;
    //    //        //    GetComponent<Rigidbody>().isKinematic = true;
    //    //        //    this.enabled = false;
    //    //        //}


    //}
    //public IEnumerator Execute()
    //{

    //    while (true)
    //    {
    //        if (root == null)
    //        {
    //            yield return null;

    //        }
    //        else
    //        {
    //            yield return StartCoroutine(root.Run(this));//aqui eka chama o run passando ela mesmo como parametro para herdar a behaviour tree
    //        }

    //        if (ponto < pontoDeDeslocamento.Length)
    //        {
    //            if (pontoDeDeslocamento[ponto]) distancia = Vector3.Distance(pontoDeDeslocamento[ponto].position, transform.position); // aqui caucula 1x so a distancia do ponto para o do inimigo
    //        }

    //        yield return new WaitForSeconds(0.1f); //aqui aplica um delay pra ficar mais leve
    //    }
    //}


    //public IEnumerator Animacao()
    //{
    //    if (!morreu)
    //    {

    //        if (alvo)//se viu o player
    //        {
    //if (Vector3.Distance(jogador.transform.position, transform.position) > distDoJogador)
    //                {
    //                  //  anim.Play("WalkingGuarda");
    //                    agente.angularSpeed = 150;
    //                    distanceRay = 30;
    //                }
    //                else
    //                {
    //                    //anim.Play("Idle");
    //                    agente.angularSpeed = 30;
    //                    distanceRay = distanciaPadrao;
    //                }

    //            }

    //            /*else if (viuRatin && !visto)
    //            {
    //                Transform rato = GameObject.FindGameObjectWithTag("Rato").transform;
    //                transform.LookAt(rato);
    //                Destroy(rato.gameObject, 5);

    //            }

    //             else if (assobio)//se ouviu o assobio
    //            {
    //                if (!pegaAssobio.activeSelf) pegaAssobio.SetActive(true);
    //                if (Vector3.Distance(assobioPos, transform.position) < 2)
    //                {
    //                   // anim.Play("Idle 0");
    //                    agente.angularSpeed = 30;
    //                    distanceRay = 15;
    //                    yield return new WaitForSeconds(2);
    //                    assobio = false;
    //                }
    //                else
    //                {
    //                  //  anim.Play("WalkingGuarda");
    //                    agente.angularSpeed = 150;
    //                    distanceRay = 30;
    //                }
    //            }*/
    //            else//faz seu movimento
    //            {
    //                if (ponto < pontoDeDeslocamento.Length || patrulhador)
    //                {
    //                   // anim.Play("WalkingGuarda");
    //                    agente.angularSpeed = 150;
    //                    distanceRay = 30;
    //                }
    //                else
    //                {
    //                   // anim.Play("Idle 0");
    //                    agente.angularSpeed = 30;
    //                    distanceRay = distanciaPadrao;
    //                }
    //            }

    //        }
    //        else
    //        {
    //            inimigo.enabled = false;
    //          //  anim.Play("Dead");
    //            GetComponent<CapsuleCollider>().enabled = false;
    //            GetComponent<Rigidbody>().isKinematic = true;
    //            agente.enabled = false;
    //            enabled = false;
    //        }
    //        yield return new WaitForSeconds(0.1f);
    //        StartCoroutine(Animacao());        
    //    }

    //    public void Morto()
    //    {
    //        morreu = true;
    //    }

    //    public void DesAnim()
    //    {
    //       // anim.enabled = false;
    //    }
    //    private void OnTriggerStay(Collider other)
    //    {
    //        //if (other.CompareTag("Assobio"))
    //        //{

    //        //    assobio = true;
    //        //    assobioPos = other.transform.position ;
    //        //    Destroy(other.gameObject);
    //        }
    //    }
    //}
}
