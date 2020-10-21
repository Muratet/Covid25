using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorManager : MonoBehaviour
{
    public Texture2D cursorRight;
    public Texture2D cursorLeft;

    private int offset = 0;

    // Update is called once per frame
    void Update()
    {
        Vector3 mousePos = Input.mousePosition;

        // recaller la position du tooltip pour qu'il soit dirigé vers le centre de l'écran
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
