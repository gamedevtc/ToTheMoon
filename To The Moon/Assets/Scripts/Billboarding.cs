using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Billboarding : MonoBehaviour
{
	[SerializeField]private int enemyHealth;
	[SerializeField]private Image enemyFill;
	[SerializeField]private Slider enemySlider;

	// Update is called once per frame
	void Update()
	{
		UpdateHealthBar();
	}

	public void UpdateHealthBar()
	{
		if (enemySlider.value <= enemySlider.minValue)
		{
			enemyFill.enabled = false;
		}

		else if (enemySlider.value >= enemySlider.minValue)
		{
			enemyFill.enabled = true;
		}

		float enemyHPValue = transform.GetComponent<AIController>().getHealth()/ transform.GetComponent<AIController>().getMaxHealth();
		enemySlider.value = enemyHPValue;
		if (transform.gameObject.CompareTag("Enemy"))
			transform.LookAt(Camera.main.transform);
		//transform.Rotate(0, 0, 0);
	}
}
