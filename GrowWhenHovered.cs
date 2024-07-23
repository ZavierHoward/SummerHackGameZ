using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GrowWhenHovered : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    //Used to grow UI briefly
    public float growth = 0.2f;
    public float original_size_x = 0;
    bool keep_growing = false;
    void Start()
    {
        original_size_x = transform.localScale.x;
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        StartCoroutine(GrowCard(true));
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        StartCoroutine(GrowCard(false));
    }

    IEnumerator GrowCard(bool grow)
    {
        keep_growing = grow;
        if (grow)
        {
            while (transform.localScale.x < original_size_x + growth && keep_growing)
            {
                transform.localScale += new Vector3(0.05f, 0.05f, 0f);
                yield return new WaitForSeconds(0.002f);
            }
        }
        else
        {
            while (transform.localScale.x > original_size_x)
            {
                transform.localScale -= new Vector3(0.05f, 0.05f, 0f);
                yield return new WaitForSeconds(0.002f);
            }
        }
    }
}
