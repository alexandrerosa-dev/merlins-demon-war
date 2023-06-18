using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOver : MonoBehaviour
{
    public Text scoreText = null;
    public Text killsText = null;

    private void Awake()
    {
        scoreText.text = "Score: " + GameController.instance.playerScore.ToString();
        killsText.text = "Demons killed: " + GameController.instance.playerKills.ToString();
    }

    // Código para voltar ao menu principal
    public void MainMenu()
    {
        SceneManager.LoadScene(0);
    }
}
