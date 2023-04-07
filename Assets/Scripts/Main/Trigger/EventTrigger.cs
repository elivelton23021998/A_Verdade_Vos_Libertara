using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class EventTrigger : MonoBehaviour
{
	public UnityEvent mudarCena;
	public UnityEvent reiniciarJogo;
	public UnityEvent fadeIn;

	IEnumerator MudarCena()
    {
		yield return new WaitForSeconds(2.5f);
		mudarCena.Invoke();
    }

	IEnumerator FadeIn()
    {
		yield return new WaitForSeconds(1f);
		fadeIn.Invoke();
    }

	IEnumerator ReiniciarJogo()
	{
		//tempo dos creditos
		yield return new WaitForSeconds(110f);
		reiniciarJogo.Invoke();
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player")
		{
			StartCoroutine(FadeIn());
			StartCoroutine(MudarCena());
			StartCoroutine(ReiniciarJogo());
		}
	}
}
