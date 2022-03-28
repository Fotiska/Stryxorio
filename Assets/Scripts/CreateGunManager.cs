using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200000E RID: 14
public class CreateGunManager : MonoBehaviour
{
	
	[Header("Objects")]
	public Turret turret;
	
	public Slider[] sliders;
	
	public InputField[] inputFields;
	
	public Dropdown type;
	
	public Dictionary<string, float> floatStats = new Dictionary<string, float>();
	
	public Dictionary<string, int> intStats = new Dictionary<string, int>();
	
	public void Awake()
	{
		floatStats.Add("MaxRange", this.turret.turret.maxRange);
		floatStats.Add("BulletTime", this.turret.turret.bulletTime);
		floatStats.Add("Cooldown", this.turret.turret.speed);
		floatStats.Add("Damage", this.turret.turret.damage);
		floatStats.Add("MaxSpread", this.turret.turret.maxSpread);
		floatStats.Add("Spread", this.turret.turret.spread);
		intStats.Add("BulletsInShoot", this.turret.turret.bulletsInShoot);
	}

	public void Balance(string key)
	{
		string[] array = key.Split(Path.DirectorySeparatorChar);
		int.TryParse(array[0], out int val);
		bool flag = !bool.Parse(array[1]);
		inputFields[val].text = Mathf.RoundToInt(sliders[val].value).ToString();
		if (flag)
		{
			floatStats[array[2]] = (float) Math.Round(sliders[val].value, 2);
			return;
		}
		intStats[array[2]] = Mathf.RoundToInt(sliders[val].value);
	}

	public void Text(string key)
	{
		string[] keys = key.Split(Path.DirectorySeparatorChar);
		int.TryParse(keys[0], out int val);
		Boolean.TryParse(keys[1], out bool fori);
		float value;
		float.TryParse(inputFields[val].text, out value);
		if (inputFields[val].text != value.ToString())
		{
			inputFields[val].text = sliders[val].value.ToString();
			if (fori)
			{
				floatStats[keys[2]] = (float) Math.Round(sliders[val].value, 2);
				return;
			}
			intStats[keys[2]] = Mathf.RoundToInt(sliders[val].value);
		}
		else
		{
			sliders[val].value = value;
			if (fori)
			{
				floatStats[keys[2]] = (float) Math.Round(sliders[val].value, 2);
				return;
			}
			intStats[keys[2]] = Mathf.RoundToInt(sliders[val].value);
		}
	}
	
	public void typeDropdown()
	{
		switch (this.type.value)
		{
		case 0:
			turret.turret.shootType = TurretStruct.ShootType.Basic;
			return;
		case 1: 
			turret.turret.shootType = TurretStruct.ShootType.Blast;
			return;
		case 2:
			turret.turret.shootType = TurretStruct.ShootType.MiniGun;
			return;
		default:
			return;
		}
	}
}
