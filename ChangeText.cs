using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChangeText : MonoBehaviour
{
    [SerializeField] private GameManagement game_manager;
    [SerializeField] private Player player;
    [SerializeField] private TMP_Text txt;
    private void Update()
    {
        if (player != null) //to tell players if they've selected a human or AI
        {
            if (player.IsAI())
            {
                txt.text = "<color=red>[AI]";
            }
            else
            {
                txt.text = "<color=yellow>[Human]";
            }
        }

    }
}
