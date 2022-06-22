using UnityEngine;
using System.Collections;

public class WFX_Demo_DeleteAfterDelay : MonoBehaviour
{
	public float delay = 2.0f;
	private float time = 2.0f;
	
	void Update ()
	{
		delay -= Time.deltaTime;
		if (delay < 0.0f)
        {
			this.gameObject.SetActive(false); 
			delay = time;
		}
	}
}
