using UnityEngine;
using UnityEngine.UI;

public class syncAlpha : MonoBehaviour
{
    public Image pattern;

    private Image target;

    private void Start()
    {
        target = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        target.color = new Color(pattern.color.r, pattern.color.g, pattern.color.b, pattern.color.a);
    }
}
