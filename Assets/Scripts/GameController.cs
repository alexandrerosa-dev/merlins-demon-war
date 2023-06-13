using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    // Voltar ao menu principal
    public void Quit()
    {
        SceneManager.LoadScene(0);
    }

    // Pular o turno
    //TODO Finish this code
    public void SkipTurn()
    {
        Debug.Log("Skip turn");
    }
}
