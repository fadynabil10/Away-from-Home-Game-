using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coins : MonoBehaviour
{
	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.CompareTag("Player"))
		{
			if(collision.GetComponent<Charachter>().health <= collision.GetComponent<Charachter>().startingHealth)
			collision.GetComponent<Charachter>().health += 7;
			AudioManager.Play(AudioClipName.CoinCollect);
			Destroy(gameObject);
		}
	}

	IEnumerator sound()
	{
		GameAudioSource.audioSource.volume *= 20;
		yield return new WaitForSeconds(0.1f);
		GameAudioSource.audioSource.volume /= 20;
	}
}
