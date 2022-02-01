using UnityEngine;
using FYFY;

public class BarsSystem_wrapper : BaseWrapper
{
	public UnityEngine.GameObject barsContainer;
	public UnityEngine.GameObject barPrefab;
	public UnityEngine.Transform xAxis;
	public UnityEngine.GameObject countrySimData;
	private void Start()
	{
		this.hideFlags = HideFlags.NotEditable;
		MainLoop.initAppropriateSystemField (system, "barsContainer", barsContainer);
		MainLoop.initAppropriateSystemField (system, "barPrefab", barPrefab);
		MainLoop.initAppropriateSystemField (system, "xAxis", xAxis);
		MainLoop.initAppropriateSystemField (system, "countrySimData", countrySimData);
	}

}
