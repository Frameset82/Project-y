using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class Healthbar: MonoBehaviour
{
	Transform cam;
	public Slider slider;
	public Gradient gradient;
	public Image fill;

	void Start()
	{
		cam = Camera.main.transform;
	}

	
	void Update()
	{
		transform.LookAt(transform.position + cam.rotation * Vector3.forward, cam.rotation * Vector3.up);
	}

	public void SetMaxHealth(int health)
	{
		slider.maxValue = health;
		slider.value = health;

		fill.color = gradient.Evaluate(1f);
	}


	public void SetHealth(int health)
	{

		slider.value = health;
        fill.color = gradient.Evaluate(slider.normalizedValue);

		if (health <=0)
		{
			//slider.value = 0;
			fill.color = Color.clear;
		}
	}

}
