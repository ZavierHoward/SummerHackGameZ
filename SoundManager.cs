using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioSource[] bg_sounds, sfx_sounds;
    [SerializeField] private GameManagement game_manager;

    // Update is called once per frame
    void Update()
    {
        if (game_manager.GameActive())
        {
            bg_sounds[0].enabled = true;
            bg_sounds[1].enabled = false;
        }
        else
        {
            bg_sounds[0].enabled = false;
            bg_sounds[1].enabled = true;
        }
    }
}
