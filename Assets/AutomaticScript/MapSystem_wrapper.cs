using UnityEngine;
using FYFY;

[ExecuteInEditMode]
public class MapSystem_wrapper : MonoBehaviour
{
	private void Start()
	{
		this.hideFlags = HideFlags.HideInInspector; // Hide this component in Inspector
	}

	public void selectTerritory(TerritoryData newTerritory)
	{
		MainLoop.callAppropriateSystemMethod ("MapSystem", "selectTerritory", newTerritory);
	}

}