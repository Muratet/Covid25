using UnityEngine;

/// <summary>
/// This component manage the mouse cursor in order the finger is oriented toward the center of the screen
/// </summary>
public class CursorManager : MonoBehaviour
{
    /// <summary></summary>
    public Texture2D cursorRight;
    /// <summary></summary>
    public Texture2D cursorLeft;

    private int offset = 0;

    // Update is called once per frame
    void Update()
    {
        Vector3 mousePos = Input.mousePosition;

        // set tooltip positionin order it is oriented toward the center of the screen
        if (mousePos.x + offset > Screen.width / 2)
        {
            offset = 0;
            Cursor.SetCursor(cursorRight, new Vector2(offset, 0), CursorMode.ForceSoftware);
        }
        if (mousePos.x + offset < Screen.width / 2)
        {
            offset = 32;
            Cursor.SetCursor(cursorLeft, new Vector2(offset, 0), CursorMode.ForceSoftware);
        }
    }
}
