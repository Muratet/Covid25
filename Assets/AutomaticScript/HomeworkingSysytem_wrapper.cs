using UnityEngine;
using FYFY;

[ExecuteInEditMode]
public class HomeworkingSysytem_wrapper : MonoBehaviour
{
	private void Start()
	{
		this.hideFlags = HideFlags.HideInInspector; // Hide this component in Inspector
	}

	public void OnHomeworkingChange(System.Boolean newState)
	{
		MainLoop.callAppropriateSystemMethod (null, "OnHomeworkingChange", newState);
	}

}