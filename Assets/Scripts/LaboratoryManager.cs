using System;
using Unity.RemoteConfig;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000017 RID: 23
public class LaboratoryManager : MonoBehaviour
{
	
	[SerializeField] private MenuManager menuManager;
	[SerializeField] private InputField gunField;
	
	public struct userAttributes { }
	public struct appAttributes { }
	
	private void Start()
	{
		ConfigManager.FetchConfigs(new userAttributes(), new appAttributes());
		if (PlayerPrefs.HasKey("ModCode") && PlayerPrefs.GetString("ModCode") == "Working")
		{
			gunField.gameObject.SetActive(false);
		}
	}
	
	public void createGun()
	{
		if (gunField.text == ConfigManager.appConfig.GetString("CreateMod", "") || (PlayerPrefs.HasKey("ModCode") && PlayerPrefs.GetString("ModCode") == "Working"))
		{
			menuManager.LoadScene(2);
			PlayerPrefs.SetString("ModCode", "Working");
			Debug.Log("{GameLog} => [LaboratoryManager] <color=green>Code working</color>");
		}
		PlayerPrefs.SetString("ModCode", "NotWorking");
		Debug.Log("{GameLog} => [LaboratoryManager] <color=red>Code not working</color>");
		Debug.Log("{GameLog} => [LaboratoryManager] Code is: " + ConfigManager.appConfig.GetString("CreateMod", ""));
	}
}
