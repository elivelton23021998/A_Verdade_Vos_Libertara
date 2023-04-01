using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class UIObjectInfo : MonoBehaviour {

    public string objectTitle;

    [Header("Principal")]
    public MonoBehaviour Script;
    public string FieldName;

    [Header("Títulos")]
    public string useText = "Usar";
    public string trueTitle = "Fechar";
    public string falseTitle = "Abrir";

    [Header("Configs")]
    public bool changeUseText = false;
    public bool isUppercased;

    private FieldInfo field;

    void Start()
    {
        if (!string.IsNullOrEmpty(objectTitle) || changeUseText) return;

        if (!Script)
        {
            Debug.LogError("Por favor, atribua qual script você deseja usar!");
        }
        else if (string.IsNullOrEmpty(FieldName))
        {
            Debug.LogError("Por favor, atribua TitleParameter!");
        }

        field = Script.GetType().GetFields().SingleOrDefault(fls => fls.Name == FieldName && fls.IsPublic);

        if (field == null)
        {
            Debug.LogError("[" + gameObject.GameObjectPath() + "] Não foi possível localizar o parâmetro de título ou o campo de parâmetro de título é privado!");
        }
    }

    void Update()
    {
        if (field != null)
        {
            try
            {
                bool fieldValue = Parser.Convert<bool>(Script.GetType().InvokeMember(FieldName, BindingFlags.GetField, null, Script, null).ToString());

                if (fieldValue)
                {
                    objectTitle = trueTitle;
                }
                else
                {
                    objectTitle = falseTitle;
                }
            }
            catch(Exception ex)
            {
                Debug.Log(ex.Message);
            }
        }

        if (isUppercased && !string.IsNullOrEmpty(objectTitle))
        {
            objectTitle = objectTitle.ToUpper();
        }
    }
}
