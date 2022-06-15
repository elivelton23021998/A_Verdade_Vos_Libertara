using UnityEngine;
using UnityEngine.UI;

public class ExampleSerialize : MonoBehaviour {

	[Tooltip("Nome do arquivo de configuração")]
	public string ConfigName = "GameConfig"; 

	public bool showDebug;

	[Tooltip("Defina como true se quiser usar PlayerPrefs para salvar e carregar o caminho de configuração.")]
	public bool usePlayerPrefs;

	public InputField Section;
	public InputField Key;
	public InputField Value;

	void Start () {
		string configName = PlayerPrefs.GetString("GameConfig");

        if (usePlayerPrefs)
        {
            ConfigManager.SetFilename(configName);
        }
        else
        {
            ConfigManager.SetFilename("Config");
        }
	}
	
	public void SerializeValue()
	{
		string m_section = Section.text;
		string m_key = Key.text;
		string m_value = Value.text;
        ConfigManager.Serialize(m_section, m_key, m_value);
	}
	
	public void DeserializeValue()
	{
		string m_section = Section.text;
		string m_key = Key.text;
		Value.text = ConfigManager.Deserialize(m_section, m_key);
	}
	
	public void RemoveKey()
	{
		string m_section = Section.text;
		string m_key = Key.text;
        ConfigManager.RemoveSectionKey(m_section, m_key);
	}
	
	public void RemoveSection()
	{
		string m_section = Section.text;
        ConfigManager.RemoveSection(m_section);
	}
	
	public void RemoveFile()
	{
        ConfigManager.RemoveFile(FilePath.GameDataPath, ConfigName);
	}
}
