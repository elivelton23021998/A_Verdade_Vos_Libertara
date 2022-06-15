using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using AVVL.JsonManager;
using UnityEngine;

[Serializable]
public class SaveableDataPair
{
    public enum DataBlockType { ISaveable, Attribute }

    public DataBlockType BlockType = DataBlockType.ISaveable;
    public string BlockKey;
    public MonoBehaviour Instance;
    public string[] FieldData;

    public SaveableDataPair(DataBlockType type, string key, MonoBehaviour instance, string[] fileds)
    {
        BlockType = type;
        BlockKey = key;
        Instance = instance;
        FieldData = fileds;
    }
}

/// <summary>
/// Script principal para salvar/carregar.
/// </summary>
public class SaveGameHandler : Singleton<SaveGameHandler> {

    public SaveLoadScriptable SaveLoadSettings;

    [Tooltip("Serialize os dados do jogador entre as cenas.")]
    public bool dataBetweenScenes;

    [Tooltip("Não é necessário, se você não quiser Fade quando a cena começar ou mudar, deixe em branco.")]
    public UIFadePanel fadeControl;

    public SaveableDataPair[] saveableDataPairs;

    private ItemSwitcher switcher;
    private Inventory inventory;
    private ObjectiveManager objectives;
    private GameObject player;
    private bool loadGame;

    [HideInInspector]
    public string lastSave;

    void Start()
    {
        inventory = GetComponent<Inventory>();
        objectives = GetComponent<ObjectiveManager>();
        player = GetComponent<AVVL_GameManager>().Player;
        switcher = player.GetComponentInChildren<ScriptManager>().GetScript<ItemSwitcher>();

        JsonManager.Settings(SaveLoadSettings, true);

        if (saveableDataPairs.Any(pair => pair.Instance == null))
        {
            Debug.LogError("[SaveGameHandler] Algumas das instâncias salváveis ​​estão ausentes ou foram destruídas. Por favor, selecione Setup SaveGame novamente no menu Tools!");
            return;
        }

        if (PlayerPrefs.HasKey("LoadGame"))
        {
            loadGame = Convert.ToBoolean(PlayerPrefs.GetInt("LoadGame"));

            if (loadGame && PlayerPrefs.HasKey("LoadSaveName"))
            {
                string filename = PlayerPrefs.GetString("LoadSaveName");

                if (File.Exists(JsonManager.GetFilePath(AVVL.JsonManager.FilePath.GameDataPath) + filename))
                {
                    JsonManager.DeserializeData(filename);
                    string loadScene = (string)JsonManager.Json()["scene"];
                    lastSave = filename;

                    if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == loadScene)
                    {
                        LoadSavedSceneData(true);
                    }
                }
                else
                {
                    Debug.Log("<color=yellow>[SaveGameHandler]</color> Não foi possível encontrar o arquivo de carregamento: " + filename);
                    PlayerPrefs.SetInt("LoadGame", 0);
                    loadGame = false;
                }
            }

            if(!loadGame && dataBetweenScenes)
            {
                JsonManager.ClearArray();

                if (File.Exists(JsonManager.GetFilePath(AVVL.JsonManager.FilePath.GameDataPath) + "_nextSceneData.dat"))
                {
                    JsonManager.DeserializeData(AVVL.JsonManager.FilePath.GameDataPath, "_nextSceneData.dat");
                    LoadSavedSceneData(false);
                }
            }
        }
    }

    /* SALVAR JOGO */
    public void SaveGame(bool allData)
    {
        JsonManager.ClearArray();
        Dictionary<string, object> playerData = new Dictionary<string, object>();
        Dictionary<string, object> slotData = new Dictionary<string, object>();
        Dictionary<string, object> shortcutData = new Dictionary<string, object>();
        Dictionary<string, object> objectivesData = new Dictionary<string, object>();

        /* PLAYER PARES */
        if (allData)
        {
            JsonManager.AddPair("scene", UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
            JsonManager.AddPair("dateTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

            playerData.Add("playerPosition", player.transform.position);
            playerData.Add("cameraRotation", player.GetComponentInChildren<MouseLook>().GetRotation());
        }

        playerData.Add("playerHealth", player.GetComponent<HealthManager>().Health);
        /* FIM PLAYER PARES */

        /* ITEMSWITCHER PARES */
        Dictionary<string, object> switcherData = new Dictionary<string, object>
        {
            { "switcherActiveItem", switcher.currentItem },
            { "switcherLightObject", switcher.currentLightObject },
            { "switcherWeaponItem", switcher.weaponItem }
        };

        foreach (var Item in switcher.ItemList)
        {
            Dictionary<string, object> ItemInstances = new Dictionary<string, object>();

            foreach (var Instance in Item.GetComponents<MonoBehaviour>().Where(x => typeof(ISaveableArmsItem).IsAssignableFrom(x.GetType())).ToArray())
            {
                ItemInstances.Add(Instance.GetType().Name.Replace(" ", "_"), (Instance as ISaveableArmsItem).OnSave());
                switcherData.Add("switcher_item_" + Item.name.ToLower().Replace(" ", "_"), ItemInstances);
            }
        }
        /* FIM ITEMSWITCHER PARES */

        /* INVENTARIO PARES */
        foreach (var slot in inventory.Slots)
        {
            if (slot.GetComponent<InventorySlot>().itemData != null)
            {
                InventoryItemData itemData = slot.GetComponent<InventorySlot>().itemData;
                Dictionary<string, object> itemDataArray = new Dictionary<string, object>
                {
                    { "slotID", itemData.slotID },
                    { "itemID", itemData.item.ID },
                    { "itemAmount", itemData.m_amount }
                };

                slotData.Add("inv_slot_" + inventory.Slots.IndexOf(slot), itemDataArray);
            }
            else
            {
                slotData.Add("inv_slot_" + inventory.Slots.IndexOf(slot), "null");
            }
        }

        Dictionary<string, object> inventoryData = new Dictionary<string, object>
        {
            { "inv_slots_count", inventory.Slots.Count },
            { "slotsData", slotData }
        };

        /* INVENTARIO ATALHO PARES */
        if (inventory.Shortcuts.Count > 0)
        {
            inventoryData.Add("shortcutsData", inventory.Shortcuts.ToDictionary(x => x.item.ID, x => x.shortcutKey.ToString()));
        }

        /* INVENTARIO FIXED CONTAINER PARES */
        if (inventory.FixedContainerData.Count > 0)
        {
            inventoryData.Add("fixedContainerData", inventory.GetFixedContainerData());
        }
        /* FIM INVENTARIO PARES */

        /* OBJETIVO PARES */
        if (objectives.objectiveCache.Count > 0)
        {
            foreach (var obj in objectives.objectiveCache)
            {
                Dictionary<string, object> objectiveData = new Dictionary<string, object>
                {
                    { "toComplete", obj.toComplete },
                    { "isCompleted", obj.isCompleted }
                };

                objectivesData.Add(obj.identifier.ToString(), objectiveData);
            }
        }
        /* FIM OBJETIVO PARES */

        //Adicionar pares de dados ao buffer de serialização
        JsonManager.AddPair("playerData", playerData);
        JsonManager.AddPair("itemSwitcherData", switcherData);
        JsonManager.AddPair("inventoryData", inventoryData);
        JsonManager.AddPair("objectivesData", objectivesData);

        //Adicionar todos os salváveis
        if (allData && saveableDataPairs.Length > 0)
        {
            foreach (var Pair in saveableDataPairs)
            {
                if(Pair.BlockType == SaveableDataPair.DataBlockType.ISaveable)
                {
                    var data = (Pair.Instance as ISaveable).OnSave();
                    if (data != null)
                    {
                        JsonManager.AddPair(Pair.BlockKey, data);
                    }
                }
                else if (Pair.BlockType == SaveableDataPair.DataBlockType.Attribute)
                {
                    Dictionary<string, object> attributeFieldPairs = new Dictionary<string, object>();

                    if (Pair.FieldData.Length > 0)
                    {
                        foreach (var Field in Pair.FieldData)
                        {
                            FieldInfo fieldInfo = Pair.Instance.GetType().GetField(Field);

                            if (fieldInfo.FieldType == typeof(Color) || fieldInfo.FieldType == typeof(KeyCode))
                            {
                                if (fieldInfo.FieldType == typeof(Color))
                                {
                                    attributeFieldPairs.Add(GetAttributeKey(fieldInfo), string.Format("#{0}", ColorUtility.ToHtmlStringRGBA((Color)Pair.Instance.GetType().InvokeMember(Field, BindingFlags.GetField, null, Pair.Instance, null))));
                                }
                                else
                                {
                                    attributeFieldPairs.Add(GetAttributeKey(fieldInfo), Pair.Instance.GetType().InvokeMember(Field, BindingFlags.GetField, null, Pair.Instance, null).ToString());
                                }
                            }
                            else
                            {
                                attributeFieldPairs.Add(GetAttributeKey(fieldInfo), Pair.Instance.GetType().InvokeMember(Field, BindingFlags.GetField, null, Pair.Instance, null));
                            }
                        }
                    }
                    else
                    {
                        Debug.LogError("Empty Fields Data: " + Pair.BlockKey);
                    }

                    JsonManager.AddPair(Pair.BlockKey, attributeFieldPairs);
                }
            }
        }

        //Serialize todos os pares do buffer
        SerializeSaveData(!allData);
    }

    /* CARREGAR JOGO */
    void LoadSavedSceneData(bool allData)
    {
        if (allData)
        {
            var posToken = JsonManager.Json()["playerData"]["playerPosition"];
            player.transform.position = posToken.ToObject<Vector3>();

            var rotToken = JsonManager.Json()["playerData"]["cameraRotation"];
            player.GetComponentInChildren<MouseLook>().SetRotation(rotToken.ToObject<Vector2>());
        }

        var healthToken = JsonManager.Json()["playerData"]["playerHealth"];
        player.GetComponent<HealthManager>().Health = (float)healthToken;

        switcher.currentLightObject = (int)JsonManager.Json()["itemSwitcherData"]["switcherLightObject"];
        switcher.weaponItem = (int)JsonManager.Json()["itemSwitcherData"]["switcherWeaponItem"];

        //Desserializar os dados do item do switcher
        foreach (var Item in switcher.ItemList)
        {
            JToken ItemToken = JsonManager.Json()["itemSwitcherData"]["switcher_item_" + Item.name.ToLower().Replace(" ", "_")];

            foreach (var Instance in Item.GetComponents<MonoBehaviour>().Where(x => typeof(ISaveableArmsItem).IsAssignableFrom(x.GetType())).ToArray())
            {
                (Instance as ISaveableArmsItem).OnLoad(ItemToken[Instance.GetType().Name.Replace(" ", "_")]);
            }
        }

        //Desserializar ItemSwitcher ActiveItem
        int switchID = (int)JsonManager.Json()["itemSwitcherData"]["switcherActiveItem"];
        if (switchID != -1)
        {
            switcher.ActivateItem(switchID);
        }

        //Desserializar dados de inventário
        StartCoroutine(DeserializeInventory(JsonManager.Json()["inventoryData"]));

        //Desserializar objetivos
        if (JsonManager.HasKey("objectivesData"))
        {
            Dictionary<int, Dictionary<string, string>> objectivesData = JsonManager.Json<Dictionary<int, Dictionary<string, string>>>(JsonManager.Json()["objectivesData"].ToString());

            foreach (var obj in objectivesData)
            {
                objectives.AddObjectiveModel(new ObjectiveModel(obj.Key, int.Parse(obj.Value["toComplete"]), bool.Parse(obj.Value["isCompleted"])));
            }
        }

        //Desserializar salváveis
        if (allData)
        {
            foreach (var Pair in saveableDataPairs)
            {
                JToken token = JsonManager.Json()[Pair.BlockKey];

                if (token == null) continue;

                if (Pair.BlockType == SaveableDataPair.DataBlockType.ISaveable)
                {
                    if (Pair.Instance.GetType() == typeof(SaveObject) && (Pair.Instance as SaveObject).saveType == SaveObject.SaveType.ObjectActive)
                    {
                        bool enabled = token["obj_enabled"].ToObject<bool>();
                        Pair.Instance.gameObject.SetActive(enabled);
                    }
                    else
                    {
                        (Pair.Instance as ISaveable).OnLoad(token);
                    }
                }
                else if (Pair.BlockType == SaveableDataPair.DataBlockType.Attribute)
                {
                    foreach (var Field in Pair.FieldData)
                    {
                        SetValue(Pair.Instance, Pair.Instance.GetType().GetField(Field), JsonManager.Json()[Pair.BlockKey][GetAttributeKey(Pair.Instance.GetType().GetField(Field))]);
                    }
                }
            }
        }
        else
        {
            File.Delete(JsonManager.GetFilePath(AVVL.JsonManager.FilePath.GameDataPath) + "_nextSceneData.dat");
        }
    }

    /* CARREGAR INVENTÁRIO */
    private IEnumerator DeserializeInventory(JToken token)
    {
        yield return new WaitUntil(() => inventory.Slots.Count > 0);

        int slotsCount = (int)token["inv_slots_count"];
        int neededSlots = slotsCount - inventory.Slots.Count;

        if(neededSlots != 0)
        {
            inventory.ExpandSlots(neededSlots);
        }

        for (int i = 0; i < inventory.Slots.Count; i++)
        {
            JToken slotToken = token["slotsData"]["inv_slot_" + i];
            string slotString = slotToken.ToString();

            if (slotString != "null")
            {
                inventory.AddItemSlot((int)slotToken["slotID"], (int)slotToken["itemID"], (int)slotToken["itemAmount"]);
            }
        }

        //Desserializar atalhos
        if (token["shortcutsData"] != null && token["shortcutsData"].HasValues)
        {
            Dictionary<int, KeyCode> shortcutsData = token["shortcutsData"].ToObject<Dictionary<int, KeyCode>>();

            foreach (var shortcut in shortcutsData)
            {
                inventory.ShortcutBind(inventory.GetItem(shortcut.Key), shortcut.Value);
            }
        }

        //Desserializar FixedContainer
        if (token["fixedContainerData"] != null && token["fixedContainerData"].HasValues)
        {
            Dictionary<int, int> fixedContainerData = token["fixedContainerData"].ToObject<Dictionary<int, int>>();

            foreach (var item in fixedContainerData)
            {
                inventory.FixedContainerData.Add(new ContainerItemData(inventory.GetItem(item.Key), item.Value));
            }
        }
    }

    string GetAttributeKey(FieldInfo Field)
    {
        SaveableField saveableAttr = Field.GetCustomAttributes(typeof(SaveableField), false).Cast<SaveableField>().SingleOrDefault();

        if (string.IsNullOrEmpty(saveableAttr.CustomKey))
        {
            return Field.Name.Replace(" ", string.Empty);
        }
        else
        {
            return saveableAttr.CustomKey;
        }
    }

    void SetValue(object instance, FieldInfo fInfo, JToken token)
    {
        Type type = fInfo.FieldType;
        string value = token.ToString();
        if (type == typeof(string)) fInfo.SetValue(instance, value);
        if (type == typeof(int)) fInfo.SetValue(instance, int.Parse(value));
        if (type == typeof(uint)) fInfo.SetValue(instance, uint.Parse(value));
        if (type == typeof(long)) fInfo.SetValue(instance, long.Parse(value));
        if (type == typeof(ulong)) fInfo.SetValue(instance, ulong.Parse(value));
        if (type == typeof(float)) fInfo.SetValue(instance, float.Parse(value));
        if (type == typeof(double)) fInfo.SetValue(instance, double.Parse(value));
        if (type == typeof(bool)) fInfo.SetValue(instance, bool.Parse(value));
        if (type == typeof(char)) fInfo.SetValue(instance, char.Parse(value));
        if (type == typeof(short)) fInfo.SetValue(instance, short.Parse(value));
        if (type == typeof(byte)) fInfo.SetValue(instance, byte.Parse(value));
        if (type == typeof(Vector2)) fInfo.SetValue(instance, token.ToObject(type));
        if (type == typeof(Vector3)) fInfo.SetValue(instance, token.ToObject(type));
        if (type == typeof(Vector4)) fInfo.SetValue(instance, token.ToObject(type));
        if (type == typeof(Quaternion)) fInfo.SetValue(instance, token.ToObject(type));
        if (type == typeof(KeyCode)) fInfo.SetValue(instance, Parser.Convert<KeyCode>(value));
        if (type == typeof(Color)) fInfo.SetValue(instance, Parser.Convert<Color>(value));
    }

    public void SaveNextSceneData()
    {
        JsonManager.ClearArray();
        SaveGame(false);
    }

    async void SerializeSaveData(bool betweenScenes)
    {
        string filepath = JsonManager.GetFilePath(AVVL.JsonManager.FilePath.GameSavesPath);
        GetComponent<AVVL_GameManager>().ShowSaveNotification(1);

        if (!betweenScenes)
        {
            if (Directory.Exists(filepath))
            {
                DirectoryInfo di = new DirectoryInfo(filepath);
                FileInfo[] fi = di.GetFiles("Save?.sav");

                if (fi.Length > 0)
                {
                    string SaveName = "Save" + fi.Length;
                    lastSave = SaveName + ".sav";
                    FileStream file = new FileStream(JsonManager.GetCurrentPath() + SaveName + ".sav", FileMode.OpenOrCreate);
                    await Task.Run(() => JsonManager.SerializeJsonDataAsync(file));
                }
                else
                {
                    lastSave = "Save0.sav";
                    FileStream file = new FileStream(JsonManager.GetCurrentPath() + "Save0.sav", FileMode.OpenOrCreate);
                    await Task.Run(() => JsonManager.SerializeJsonDataAsync(file));
                }
            }
            else
            {
                Directory.CreateDirectory(JsonManager.GetCurrentPath());

                lastSave = "Save0.sav";
                FileStream file = new FileStream(JsonManager.GetCurrentPath() + "Save0.sav", FileMode.OpenOrCreate);
                await Task.Run(() => JsonManager.SerializeJsonDataAsync(file));
            }
        }
        else
        {
            FileStream file = new FileStream(JsonManager.GetFilePath(AVVL.JsonManager.FilePath.GameDataPath) + "_nextSceneData.dat", FileMode.OpenOrCreate);
            await Task.Run(() => JsonManager.SerializeJsonDataAsync(file, true));
        }
    }
}