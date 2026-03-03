using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEditor;
using UnityEngine;

public class Currency : MonoBehaviour
{
	[SerializeField] private int worth;
	[SerializeField] Rigidbody myRB;
	[SerializeField] private float searchDistance = 400;
	[SerializeField] private float waitBeforeSearchTime = 1;
	[SerializeField] private float rotationSpeed = 150;
	[SerializeField] private float maxSpeed = 300;
	[SerializeField] private GameObject collectionEffect;

	[SerializeField] private string targetTag = "Player";
	[SerializeField] private bool foundPlayer = false;
	[SerializeField] private GameObject target;
	
	[SerializeField] private AudioSource source;
	[SerializeField] private bool isCoin;

	private bool paused;
	private Vector3 storedVelocity = Vector3.zero;
	private Vector3 storedAngular = Vector3.zero;
	private void Start()
	{
		myRB = GetComponent<Rigidbody>();
		myRB.angularVelocity = new Vector3(Random.Range(-rotationSpeed, rotationSpeed), Random.Range(-rotationSpeed, rotationSpeed), Random.Range(-rotationSpeed, rotationSpeed));
	}
	private void FixedUpdate()
	{
		if (paused)
		{
			return;
		}
		if (!foundPlayer)
		{
			Collider[] collisions = Physics.OverlapSphere(transform.position, searchDistance);
			for (int i = 0; i < collisions.Length; i++)
			{
				if (collisions[i].gameObject.tag == targetTag)
				{
					target = collisions[i].gameObject;
					foundPlayer = true;
					break;
				}
			}
		}
		else
		{
			if (isCoin)
			{
				float distanceToTarget = Vector3.Distance(target.transform.position, transform.position);
				float speed = maxSpeed * (1 - (distanceToTarget / (searchDistance * 2)));
				transform.position = Vector3.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);
			}
		}
	}
	private void OnTriggerEnter(Collider _other)
	{
		if (_other.CompareTag("Player"))
		{
			if (collectionEffect)
			{
				Instantiate(collectionEffect, _other.GetComponent<Player4Base>().getCoinParent());
			}
			_other.GetComponent<Player4Base>().setCredits(worth);
			Destroy(gameObject);
		}
	}

	void OnEnable()
	{
		EventManager.pauseEvent += pause;
		EventManager.unPauseEvent += unPause;
	}
	public void OnDisable()
	{
		EventManager.pauseEvent -= pause;
		EventManager.unPauseEvent -= unPause;
	}

	private void pause()
	{
		storedVelocity = myRB.velocity;
		storedAngular = myRB.angularVelocity;
		myRB.angularVelocity = Vector3.zero;
		myRB.velocity = Vector3.zero;
	}
	private void unPause()
	{
		myRB.velocity = storedVelocity;
		myRB.angularVelocity = storedAngular;
	}
}
