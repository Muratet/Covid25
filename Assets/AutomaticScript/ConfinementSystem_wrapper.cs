using UnityEngine;
using FYFY;

[ExecuteInEditMode]
public class ConfinementSystem_wrapper : MonoBehaviour
{
	private void Start()
	{
		this.hideFlags = HideFlags.HideInInspector; // Hide this component in Inspector
	}

	public void updateCountryUI()
	{
		MainLoop.callAppropriateSystemMethod ("ConfinementSystem", "updateCountryUI", null);
	}

	public void OnPrimarySchoolChange(System.Boolean newState)
	{
		MainLoop.callAppropriateSystemMethod ("ConfinementSystem", "OnPrimarySchoolChange", newState);
	}

	public void OnSecondarySchoolChange(System.Boolean newState)
	{
		MainLoop.callAppropriateSystemMethod ("ConfinementSystem", "OnSecondarySchoolChange", newState);
	}

	public void OnHighSchoolChange(System.Boolean newState)
	{
		MainLoop.callAppropriateSystemMethod ("ConfinementSystem", "OnHighSchoolChange", newState);
	}

	public void OnUniversityChange(System.Boolean newState)
	{
		MainLoop.callAppropriateSystemMethod ("ConfinementSystem", "OnUniversityChange", newState);
	}

	public void OnCivicismChange(System.Boolean newState)
	{
		MainLoop.callAppropriateSystemMethod ("ConfinementSystem", "OnCivicismChange", newState);
	}

	public void OnShopChange(System.Boolean newState)
	{
		MainLoop.callAppropriateSystemMethod ("ConfinementSystem", "OnShopChange", newState);
	}

	public void OnCertificateChange(System.Boolean newState)
	{
		MainLoop.callAppropriateSystemMethod ("ConfinementSystem", "OnCertificateChange", newState);
	}

	public void OnAgeDependentChange(System.Boolean newState)
	{
		MainLoop.callAppropriateSystemMethod ("ConfinementSystem", "OnAgeDependentChange", newState);
	}

	public void OnAgeMinEndEdit(System.String newAge)
	{
		MainLoop.callAppropriateSystemMethod ("ConfinementSystem", "OnAgeMinEndEdit", newAge);
	}

	public void OnAgeMaxEndEdit(System.String newAge)
	{
		MainLoop.callAppropriateSystemMethod ("ConfinementSystem", "OnAgeMaxEndEdit", newAge);
	}

	public void OnBedsChange(System.Boolean newState)
	{
		MainLoop.callAppropriateSystemMethod ("ConfinementSystem", "OnBedsChange", newState);
	}

}