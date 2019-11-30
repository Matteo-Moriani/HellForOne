using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedUnitEffects : MonoBehaviour
{

    [Header("Border")]
    [SerializeField, Range(1, 3), Tooltip("The initial size of the border.")]
    private float startWidthBorder;

    [SerializeField, Range(1, 3), Tooltip("The final size of the border (set 1 if you don't want border).")]
    private float finalWidthBorder;

    [SerializeField, Min(0), Tooltip("The scaling of border start after this number of seconds.")]
    private float resizeAfter;

    [SerializeField, Min(0), Tooltip("The scaling lasts this number of seconds")]
    private float resizeIn;

    [Header("Circle")]
    [SerializeField, Tooltip("The quad object with the texture of the circle under the unit.")]
    private GameObject circle;

    [SerializeField, Tooltip("The duration of circle animation in seconds.")]
    private float animationDuration;

    private bool highlight;
    private bool confirm;
    private float startHighlighting;
    private float startConfirm;

    private void Update()
    {
        float width;
        float step;
        if (highlight)
        {
            if (Time.time - startHighlighting > resizeAfter)
            {
                width = GetComponent<Renderer>().material.GetFloat("_OutlineWidth");
                if (width != finalWidthBorder)
                {
                    step = (startWidthBorder - finalWidthBorder) / (resizeIn / Time.deltaTime);
                    width -= step;
                    if ((width < finalWidthBorder && startWidthBorder > finalWidthBorder) || (width > finalWidthBorder && startWidthBorder < finalWidthBorder))
                    {
                        width = finalWidthBorder;
                    }
                    GetComponent<Renderer>().material.SetFloat("_OutlineWidth", width);
                }
            }
        }

        if (confirm)
        {
            if (Time.time - startConfirm < animationDuration)
            {
                step = 1 / (animationDuration / Time.deltaTime);
                width = circle.transform.localScale.x;
                width -= step;
                if (width < 0)
                {
                    width = 0;
                }

                circle.transform.localScale = new Vector3(width, 1, width);
            }
            else
            {
                confirm = false;
                circle.SetActive(false);
            }
        }
    }

    /// <summary> 
    /// It enables the borders on the unit. 
    /// </summary> 
    public void HighlightUnit()
    {
        GetComponent<Renderer>().material.SetFloat("_OutlineWidth", startWidthBorder);
        startHighlighting = Time.time;
        highlight = true;
    }

    /// <summary> 
    /// Removes the border on the unit. 
    /// </summary> 
    public void CleanHighlighting()
    {
        GetComponent<Renderer>().material.SetFloat("_OutlineWidth", 1f);
        highlight = false;
    }

    /// <summary> 
    /// Plays one time the particle system, enables the circle under the unit and circle animation. 
    /// </summary> 
    public void ConfirmOrder()
    {
        GetComponentInChildren<ParticleSystem>().Play();
        confirm = true;
        circle.transform.localScale = Vector3.one;
        startConfirm = Time.time;
        circle.SetActive(true);
    }
}
