using UnityEngine;
using FYFY;

public class MapSystem_wrapper : BaseWrapper
{
	public TMPro.TMP_Text territoryName;
	public TimeScale time;
	public UnityEngine.TextAsset rawContent;
	private void Start()
	{
		this.hideFlags = HideFlags.NotEditable;
		MainLoop.initAppropriateSystemField (system, "territoryName", territoryName);
		MainLoop.initAppropriateSystemField (system, "time", time);
		MainLoop.initAppropriateSystemField (system, "rawContent", rawContent);
	}

	public void selectTerritory(TerritoryData newTerritory)
	{
		MainLoop.callAppropriateSystemMethod (system, "selectTerritory", newTerritory);
	}

}
