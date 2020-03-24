using UnityEngine;
using System.Collections;

public class Loader : MonoBehaviour
{
    public GameObject gameManager;

    public void Click_play()
    {
        if (Game_manager.gameInstance == null)
            Instantiate(gameManager);
    }
}
