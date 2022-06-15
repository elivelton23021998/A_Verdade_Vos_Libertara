using Diagnostics = System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEditor;
using AVVL.JsonManager;

public class SaveGameMenu : EditorWindow
{
    private const string menuPath = "Tools/AVVL/SaveGame/";
    private const string filePath = "Assets/AVVL_Package/AVVL Assets/Scriptables/Game Settings/";

    [MenuItem(menuPath + "Delete SavedGames")]
    static void DeleteSavedGame()
    {
        if (Directory.Exists(GetPath()))
        {
            string[] files = Directory.GetFiles(GetPath(), "Save?.sav");
            if (files.Length > 0)
            {
                for (int i = 0; i < files.Length; i++)
                {
                    File.Delete(files[i]);
                }

                EditorUtility.DisplayDialog("SaveGame Deleted", "Deleting SavedGame is completed.", "Okay");
            }
            else
            {
                EditorUtility.DisplayDialog("Directory empty", "Folder is empty.", "Okay");
            }
        }
        else
        {
            EditorUtility.DisplayDialog("Directory not found", "Failed to find Directory:  " + GetPath(), "Okay");
        }
    }

    [MenuItem(menuPath + "Saveables Manager")]
    static void SavedGameManager()
    {
        if (SaveGameHandler.Instance != null)
        {
            EditorWindow window = GetWindow<SaveGameMenu>(true, "Saveables Editor", true);
            window.minSize = new Vector2(500, 130);
            window.Show();
        }
        else
        {
            Debug.LogError("[SaveableManager] não foi possível encontrar um script SaveGameHandler!");
        }
    }

    void OnGUI()
    {
        SaveGameHandler handler = SaveGameHandler.Instance;
        SerializedObject serializedObject = new SerializedObject(handler);
        SerializedProperty list = serializedObject.FindProperty("saveableDataPairs");

        GUIStyle boxStyle = GUI.skin.GetStyle("HelpBox");
        boxStyle.fontSize = 10;
        boxStyle.alignment = TextAnchor.MiddleCenter;

        int count = handler.saveableDataPairs.Length;
        string warning = "";
        MessageType messageType = MessageType.None;

        if (count > 0 && handler.saveableDataPairs.All(pair => pair.Instance != null))
        {
            warning = "SaveGame Handler foi configurado com sucesso!";
            messageType = MessageType.Info;
        }
        else if (count > 0 && handler.saveableDataPairs.Any(pair => pair.Instance == null))
        {
            warning = "Algumas das instâncias salváveis ​​estão faltando! Por favor, encontre a cena salvável novamente!";
            messageType = MessageType.Error;
        }
        else if(count < 1)
        {
            warning = "Para usar o recurso SaveGame em sua cena, você deve primeiro encontrar salváveis!";
            messageType = MessageType.Warning;
        }

        EditorGUI.HelpBox(new Rect(1, 0, EditorGUIUtility.currentViewWidth - 2, 40), warning, messageType);
        EditorGUI.HelpBox(new Rect(1, 40, EditorGUIUtility.currentViewWidth - 2, 30), "Salváveis ​​Encontrados: " + count, MessageType.None);

        GUIContent btnTxt = new GUIContent("Encontrar Graváveis");
        var rt = GUILayoutUtility.GetRect(btnTxt, GUI.skin.button, GUILayout.Width(150), GUILayout.Height(30));
        rt.center = new Vector2(EditorGUIUtility.currentViewWidth / 2, rt.center.y);
        rt.y = 80;

        if (GUI.Button(rt, btnTxt, GUI.skin.button))
        {
            SetupSaveGame();
        }
    }

    void OnInspectorUpdate()
    {
        Repaint();
    }

    static void SetupSaveGame()
    {
        SaveGameHandler handler = SaveGameHandler.Instance;

        if (handler != null)
        {
            Diagnostics.Stopwatch stopwatch = new Diagnostics.Stopwatch();
            stopwatch.Reset();
            stopwatch.Start();

            var saveablesQuery = from Instance in Tools.FindAllSceneObjects<MonoBehaviour>()
                                 where typeof(ISaveable).IsAssignableFrom(Instance.GetType()) && !Instance.GetType().IsInterface && !Instance.GetType().IsAbstract
                                 let key = string.Format("{0}_{1}", Instance.GetType().Name, System.Guid.NewGuid().ToString("N"))
                                 select new SaveableDataPair(SaveableDataPair.DataBlockType.ISaveable, key, Instance, new string[0]);

            var attributesQuery = from Instance in Tools.FindAllSceneObjects<MonoBehaviour>()
                                  let attr = Instance.GetType().GetFields().Where(field => field.GetCustomAttributes(typeof(SaveableField), false).Count() > 0 && !field.IsLiteral && field.IsPublic).Select(fls => fls.Name).ToArray()
                                  let key = string.Format("{0}_{1}", Instance.GetType().Name, System.Guid.NewGuid().ToString("N"))
                                  where attr.Count() > 0
                                  select new SaveableDataPair(SaveableDataPair.DataBlockType.Attribute, key, Instance, attr);

            SaveableDataPair[] pairs = saveablesQuery.Union(attributesQuery).ToArray();
            stopwatch.Stop();

            handler.saveableDataPairs = pairs;
            EditorUtility.SetDirty(handler);

            Debug.Log("<color=green>[Setup SaveGame Successful]</color> Objetos salváveis ​​encontrados:: " + pairs.Length + ", Tempo decorrido: " + stopwatch.ElapsedMilliseconds + "ms");
        }
        else
        {
            Debug.LogError("[Setup SaveGame Error] Para configurar o SaveGame você precisa configurar sua cena primeiro.");
        }
    }

    private static string GetPath()
    {
        if (Directory.Exists(filePath))
        {
            if (Directory.GetFiles(filePath).Length > 0)
            {
                return JsonManager.GetFilePath((AVVL.JsonManager.FilePath)AssetDatabase.LoadAssetAtPath<SaveLoadScriptable>(filePath + "SaveLoadSettings.asset").filePath);
            }
            return JsonManager.GetFilePath((AVVL.JsonManager.FilePath)FilePath.GameDataPath);
        }
        else
        {
            return JsonManager.GetFilePath((AVVL.JsonManager.FilePath)FilePath.GameDataPath);
        }
    }
}
