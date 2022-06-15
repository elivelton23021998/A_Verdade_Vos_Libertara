using System;
using UnityEngine;
using AVVL.JsonManager;

[Serializable]
public class SaveLoadScriptable : ScriptableObject
{
    public bool enableEncryption;
    public FilePath filePath = FilePath.GameDataPath;
    public string cipherKey;
}