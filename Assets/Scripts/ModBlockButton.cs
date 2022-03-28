using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ModBlockButton : MonoBehaviour
{
	
	public ModBlock modBlock;
	public Text text;
	
	public void destroy()
	{
		Destroy(gameObject);
	}

	public void Awake()
	{
		StartCoroutine(updateName());
	}

	public IEnumerator updateName()
	{
		while (true)
		{
			yield return new WaitForSeconds(0.5f);
			text.text = modBlock.name;
		}
	}
	
}
