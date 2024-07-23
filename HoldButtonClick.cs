using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HoldButtonClick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private GameManagement game_manager;
    [SerializeField] private bool change, clicked;
    [SerializeField] private float wait, wait_time = 0.25f;

    void Update()
    {
        if (!clicked)
        {
            return;
        }
        if (wait <= 0)// && wait < 0)
        {
            game_manager.ChangeHotelPrice(change);
            wait_time /= 2;
            wait = wait_time;
        }
        else
        {
            wait -= Time.deltaTime;
        }
        //else if(clicked)
        //{
        //    wait -= Time.deltaTime;
        //}
    }
    public void OnPointerDown(PointerEventData pointerEventData)
    {
        //Output the name of the GameObject that is being clicked
        //Debug.Log(name + "Game Object Click in Progress");
        game_manager.ChangeHotelPrice(change);
        wait = wait_time;
        clicked = true;

    }

    //Detect if clicks are no longer registering
    public void OnPointerUp(PointerEventData pointerEventData)
    {
        //Debug.Log(name + "No longer being clicked");
        clicked = false;
        wait_time = 0.25f;
    }

}
