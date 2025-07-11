using UnityEngine;
using FYFY;

public class MapSystem_wrapper : BaseWrapper
{
	public TMPro.TMP_Text territoryName;
	public UnityEngine.GameObject simulationData;
	public UnityEngine.TextAsset rawPopulationData;
	public UnityEngine.TextAsset rawCountryData;
	public UnityEngine.GameObject territoryPrefab;
	public UnityEngine.Transform territoriesParent;
	public System.Boolean isDragging;
	private void Start()
	{
		this.hideFlags = HideFlags.NotEditable;
		MainLoop.initAppropriateSystemField (system, "territoryName", territoryName);
		MainLoop.initAppropriateSystemField (system, "simulationData", simulationData);
		MainLoop.initAppropriateSystemField (system, "rawPopulationData", rawPopulationData);
		MainLoop.initAppropriateSystemField (system, "rawCountryData", rawCountryData);
		MainLoop.initAppropriateSystemField (system, "territoryPrefab", territoryPrefab);
		MainLoop.initAppropriateSystemField (system, "territoriesParent", territoriesParent);
		MainLoop.initAppropriateSystemField (system, "isDragging", isDragging);
	}

	public void onNewTerritory(UnityEngine.GameObject newTerritory)
	{
		MainLoop.callAppropriateSystemMethod (system, "onNewTerritory", newTerritory);
	}

	public void selectTerritory(TerritoryData newTerritory)
	{
		MainLoop.callAppropriateSystemMethod (system, "selectTerritory", newTerritory);
	}

	public void onScroll(UnityEngine.EventSystems.BaseEventData data)
	{
		MainLoop.callAppropriateSystemMethod (system, "onScroll", data);
	}

	public void onDrag(UnityEngine.EventSystems.BaseEventData data)
	{
		MainLoop.callAppropriateSystemMethod (system, "onDrag", data);
	}

	public void onBeginDrag()
	{
		MainLoop.callAppropriateSystemMethod (system, "onBeginDrag", null);
	}

	public void onEndDrag()
	{
		MainLoop.callAppropriateSystemMethod (system, "onEndDrag", null);
	}

	public void zoom(System.Single amount)
	{
		MainLoop.callAppropriateSystemMethod (system, "zoom", amount);
	}

}
