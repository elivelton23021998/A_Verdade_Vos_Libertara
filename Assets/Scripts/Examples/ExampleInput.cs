using UnityEngine;
public class ExampleInput : MonoBehaviour
{
    public ConfigHandler configHandler;

    private KeyCode useKey;
    private bool isSet = false;
    private bool isPressed = false;

    void Update()
    {
        if (configHandler.GetKeysCount() > 0 && !isSet)
        {
            useKey = Parser.Convert<KeyCode>(configHandler.Deserialize("Input", "Usar"));
            isSet = true;
        }

        if (Input.GetKeyDown(useKey) && !isPressed)
        {
            Debug.Log("Tecla 'Usar' Pressionada!");
            isPressed = true;
        }
        else if (isPressed)
        {
            isPressed = false;
        }
    }
}