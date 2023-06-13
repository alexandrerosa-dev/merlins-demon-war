using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    // Código para voltar ao menu principal
    public void MainMenu()
    {
        SceneManager.LoadScene(0);
    }
}
