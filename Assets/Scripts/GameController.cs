using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    static public GameController instance = null;

    public Deck playerDeck = new Deck();
    public Deck enemyDeck  = new Deck();

    public Hand playersHand = new Hand();
    public Hand enemysHand = new Hand();

    public List<CardData> cards = new List<CardData>();

    public Sprite[] healthNumbers = new Sprite[0];
    public Sprite[] damageNumbers = new Sprite[0];

    private void Awake()
    {
        instance = this;

        playerDeck.Create();
        enemyDeck.Create();
    }

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
