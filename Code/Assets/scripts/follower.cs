using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class follower : MonoBehaviour
{
	public GameObject gold;
	public Transform[] sites;
	private void OnTriggerEnter2D(Collider2D collision)
	{
		Instantiate(gold, sites[Random.Range(0, sites.Length)].position, Quaternion.identity, collision.transform);
	}
}
