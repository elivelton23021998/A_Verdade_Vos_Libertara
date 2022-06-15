using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Events;

public class KeycardLock : MonoBehaviour, ISaveable
{
    private Inventory inventory;
    private MeshRenderer textRenderer;

    [Header("Configuração")]
    [Tooltip("ID do inventário do cartão-chave")]
    public int keycardID;

    [Tooltip("Remova o cartão-chave após o acesso concedido")]
    public bool removeCard;
    public TextMesh resultText;

    [Header("Cores")]
    public Color NormalColor = Color.white;
    public Color GrantedColor = Color.green;
    public Color DeniedColor = Color.red;

    [Header("Texto")]
    public string NormalText;
    public string GrantedText;
    public string DeniedText;

    [Header("Sons")]
    public AudioClip accessGranted;
    public AudioClip accessDenied;
    public float volume = 1f;

    [Header("Eventos")]
    public UnityEvent OnAccessGranted;
    public UnityEvent OnAccessDenied;

    private bool granted;
    private bool denied;

    void Start()
    {
        inventory = GameObject.Find("GAMEMANAGER").GetComponent<Inventory>();
        textRenderer = resultText.gameObject.GetComponent<MeshRenderer>();

        textRenderer.material.SetColor("_Color", NormalColor);
        resultText.text = NormalText;
    }

    public void UseObject()
    {
        if (!denied && !granted)
        {
            if (inventory.CheckItemInventory(keycardID))
            {
                if (accessGranted) { AudioSource.PlayClipAtPoint(accessGranted, transform.position, volume); }
                textRenderer.material.SetColor("_Color", GrantedColor);
                resultText.text = GrantedText;
                OnAccessGranted.Invoke();

                if (removeCard)
                {
                    inventory.RemoveItem(keycardID);
                }

                granted = true;
                denied = false;
            }
            else
            {
                if (accessDenied) { AudioSource.PlayClipAtPoint(accessDenied, transform.position, volume); }
                textRenderer.material.SetColor("_Color", DeniedColor);
                resultText.text = DeniedText;
                OnAccessDenied.Invoke();
                StartCoroutine(AccessDenied());
                denied = true;
            }
        }
    }

    IEnumerator AccessDenied()
    {
        yield return new WaitForSeconds(2);
        textRenderer.material.SetColor("_Color", NormalColor);
        resultText.text = NormalText;
        denied = false;
    }

    public Dictionary<string, object> OnSave()
    {
        return new Dictionary<string, object>
        {
            {"Garantido", granted }
        };
    }

    public void OnLoad(JToken token)
    {
        granted = (bool)token["Garantido"];

        if (granted)
        {
            textRenderer.material.SetColor("_Color", GrantedColor);
            resultText.text = GrantedText;
            OnAccessGranted.Invoke();
        }
    }
}
