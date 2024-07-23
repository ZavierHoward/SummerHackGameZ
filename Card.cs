using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Card : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameManagement game_manager;
    [SerializeField] private string description;
    //[SerializeField] private bool in_hand;
    //[SerializeField] private int card_num;
    [SerializeField] private bool self_gain, sabotage, protection;
    bool keep_growing = false;


    void Update()
    {
        if (game_manager == null || !game_manager.GameActive())
        {
            return;
        }
        /*if (keep_growing)
        {
            
        }*/
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        game_manager.GetCardInfo().transform.parent.gameObject.SetActive(true);
        game_manager.GetCardInfo().text = description;
        StartCoroutine(GrowCard(true));
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        StartCoroutine(GrowCard(false));
        game_manager.GetCardInfo().transform.parent.gameObject.SetActive(false);
        game_manager.GetCardInfo().text = "";
    }

    public void Shrink()
    {
        transform.parent.transform.localScale = new Vector3(0.75f, 0.8f, 0);
    }
    IEnumerator GrowCard(bool grow)
    {
        keep_growing = grow;
        if (grow)
        {
            transform.parent.GetComponent<Image>().color = Color.green;
            while(transform.parent.transform.localScale.x < 1.05f && keep_growing)
            {
                transform.parent.transform.localScale += new Vector3(0.05f, 0.05f, 0f);
                yield return new WaitForSeconds(0.005f);
            }
        }
        else
        {
            transform.parent.GetComponent<Image>().color = Color.red;
            while (transform.parent.transform.localScale.x > 0.75f)
            {
                transform.parent.transform.localScale -= new Vector3(0.05f, 0.05f, 0f);
                yield return new WaitForSeconds(0.005f);
            }
        }
    }
    public string GetDescription()
    {
        return description;
    }
    public bool GetSelfGain()
    {
        return self_gain;
    }
    public bool GetSabotage()
    {
        return sabotage;
    }
    public bool GetProtection()
    {
        return protection;
    }
}
