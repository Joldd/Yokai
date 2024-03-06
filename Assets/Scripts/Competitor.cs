using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YokaiNoMori.Enumeration;
using YokaiNoMori.Interface;

public class Competitor : MonoBehaviour, ICompetitor
{
	//Interface ICompetitor
	public ECampType GetCamp()
	{
		ECampType campType = (ECampType)0;

		if (gameObject.transform.CompareTag("Player01"))
			campType = (ECampType)1;
		else if (gameObject.transform.CompareTag("Player02"))
			campType = (ECampType)2;

		return campType;
	}

	public string GetName()
	{
		return gameObject.name;
	}

	public void SetCamp(ECampType camp)
	{
		throw new System.NotImplementedException();
	}

	public void StartTurn()
	{
		throw new System.NotImplementedException();
	}

	public void StopTurn()
	{
		throw new System.NotImplementedException();
	}
}
