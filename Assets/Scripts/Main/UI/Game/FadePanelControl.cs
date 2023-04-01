using UnityEngine;
using UnityEngine.UI;

public class FadePanelControl : MonoBehaviour {

    public Image fadePanel;
    public string FadeIn;
    public string FadeOut;
    public float speed;

    private Animation anim;

    private void Awake()
    {
        if (!fadePanel.GetComponent<Animation>())
        {
            Debug.LogError("O componente de animação não existe");
            return;
        }

        anim = fadePanel.GetComponent<Animation>();
    }

    public void FadeInPanel()
    {
        anim[FadeIn].speed = speed;
        anim.Play(FadeIn);
    }

    public void FadeOutPanel()
    {
        anim[FadeOut].speed = speed;
        anim.Play(FadeOut);
    }

    public bool isFading()
    {
        return anim.isPlaying;
    }
}
