using UnityEngine;
public class Jumpscare : MonoBehaviour {

	private JumpscareEffects effects;

	[Header("Jumpscare Condiguração")]
	public Animation AnimationObject;
	public AudioClip AnimationSound;
	public float SoundVolume = 0.5f;

	[Tooltip("O valor define por quanto tempo o jogador ficará com medo")]
	public float ScareLevelSec = 33f;

    [SaveableField, HideInInspector]
	public bool isPlayed;

	void Start()
	{
		effects = ScriptManager.Instance.gameObject.GetComponent<JumpscareEffects> ();
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player" && !isPlayed) {
			AnimationObject.Play ();
			if(AnimationSound){Tools.PlayOneShot2D(transform.position, AnimationSound, SoundVolume);}
			effects.Scare (ScareLevelSec);
			isPlayed = true;
		}
	}
}
