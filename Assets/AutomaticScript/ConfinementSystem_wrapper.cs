using UnityEngine;
using FYFY;

public class ConfinementSystem_wrapper : BaseWrapper
{
	public UnityEngine.GameObject countrySimData;
	public UnityEngine.Sprite defaultMark;
	public UnityEngine.Sprite customMark;
	public Localization localization;
	private void Start()
	{
		this.hideFlags = HideFlags.NotEditable;
		MainLoop.initAppropriateSystemField (system, "countrySimData", countrySimData);
		MainLoop.initAppropriateSystemField (system, "defaultMark", defaultMark);
		MainLoop.initAppropriateSystemField (system, "customMark", customMark);
		MainLoop.initAppropriateSystemField (system, "localization", localization);
	}

	public void updateCountryUI()
	{
		MainLoop.callAppropriateSystemMethod (system, "updateCountryUI", null);
	}

	public void updateUI()
	{
		MainLoop.callAppropriateSystemMethod (system, "updateUI", null);
	}

	public void OnPrimarySchoolChange(System.Boolean newState)
	{
		MainLoop.callAppropriateSystemMethod (system, "OnPrimarySchoolChange", newState);
	}

	public void OnSecondarySchoolChange(System.Boolean newState)
	{
		MainLoop.callAppropriateSystemMethod (system, "OnSecondarySchoolChange", newState);
	}

	public void OnHighSchoolChange(System.Boolean newState)
	{
		MainLoop.callAppropriateSystemMethod (system, "OnHighSchoolChange", newState);
	}

	public void OnUniversityChange(System.Boolean newState)
	{
		MainLoop.callAppropriateSystemMethod (system, "OnUniversityChange", newState);
	}

	public void OnCivicismChange(System.Boolean newState)
	{
		MainLoop.callAppropriateSystemMethod (system, "OnCivicismChange", newState);
	}

	public void OnShopChange(System.Boolean newState)
	{
		MainLoop.callAppropriateSystemMethod (system, "OnShopChange", newState);
	}

	public void OnCertificateChange(System.Boolean newState)
	{
		MainLoop.callAppropriateSystemMethod (system, "OnCertificateChange", newState);
	}

	public void OnAgeDependentChange(System.Boolean newState)
	{
		MainLoop.callAppropriateSystemMethod (system, "OnAgeDependentChange", newState);
	}

	public void OnAgeMinEndEdit(System.String newAge)
	{
		MainLoop.callAppropriateSystemMethod (system, "OnAgeMinEndEdit", newAge);
	}

	public void OnAgeMaxEndEdit(System.String newAge)
	{
		MainLoop.callAppropriateSystemMethod (system, "OnAgeMaxEndEdit", newAge);
	}

	public void OnBedsChange(System.Boolean newState)
	{
		MainLoop.callAppropriateSystemMethod (system, "OnBedsChange", newState);
	}

}
