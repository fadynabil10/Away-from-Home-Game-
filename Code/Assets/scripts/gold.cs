using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gold : MonoBehaviour
{
	private void OnTriggerEnter2D(Collider2D collision)
	{
		collision.gameObject.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
	}
}
