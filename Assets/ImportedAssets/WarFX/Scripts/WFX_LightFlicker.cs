using UnityEngine;
using System.Collections;

/**
 *	Rapidly sets a light on/off.
 *	
 *	(c) 2015, Jean Moreno
**/

[RequireComponent(typeof(Light))]
public class WFX_LightFlicker : MonoBehaviour
{	
	void Start ()
	{
		//timer = time;
		//StartCoroutine("Flicker");
		GetComponent<Light>().enabled = false;
	}

	public void LightCoroutine()
    {
		GetComponent<Light>().enabled = !GetComponent<Light>().enabled;

		Invoke("LightOff", 0.1f);
	}

	public void LightOff()
    {
		GetComponent<Light>().enabled = !GetComponent<Light>().enabled;
	}

	//IEnumerator Flicker()
	//{
	//
	//	while(true)
	//	{
	//		GetComponent<Light>().enabled = !GetComponent<Light>().enabled;
	//		
	//		do
	//		{
	//			timer -= Time.deltaTime;
	//			yield return null;
	//		}
	//		while(timer > 0);
	//		timer = time;
	//	}
	//}
}
