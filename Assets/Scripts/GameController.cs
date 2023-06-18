﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    static public GameController instance = null;

    public Deck playerDeck = new Deck();
    public Deck enemyDeck  = new Deck();

    public Hand playersHand = new Hand();
    public Hand enemysHand  = new Hand();

    public Player player = null;
    public Player enemy  = null;

    public List<CardData> cards = new List<CardData>();

    public Sprite[] healthNumbers = new Sprite[10];
    public Sprite[] damageNumbers = new Sprite[10];

    public GameObject cardPrefab = null;
    public Canvas     canvas     = null;

    public bool isPlayable = false;

    public GameObject effectFromLeftPrefab  = null;
    public GameObject effectFromRightPrefab = null;

    public Sprite fireBallImage       = null;
    public Sprite iceBallImage        = null;
    public Sprite multiFireBallImage  = null;
    public Sprite multiIceBallImage   = null;
    public Sprite fireAndIceBallImage = null;

    public bool playersTurn = true;

    public Text turnText = null;

    public Image enemySkipTurn = null;
    private void Awake()
    {
        instance = this;

        playerDeck.Create();
        enemyDeck.Create();

       StartCoroutine(DealHands());
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
        if (playersTurn && isPlayable)
            NextPlayerTurn();
    }

    internal IEnumerator DealHands()
    {
        yield return new WaitForSeconds(1);
        for (int t = 0; t < 3; t++)
        {
            playerDeck.DealCard(playersHand);
            enemyDeck.DealCard(enemysHand);
            yield return new WaitForSeconds(1);
        }
        isPlayable = true;
    }

    internal bool UseCard(Card card, Player usingOnPlayer, Hand fromHand)
    {
        // RemoveCard
        // DealReplacementCard
        if (!CardValid(card, usingOnPlayer, fromHand))
            return false;

        isPlayable = false;

        CastCard(card, usingOnPlayer, fromHand);

        player.glowImage.gameObject.SetActive(false);
        enemy.glowImage.gameObject.SetActive(false);

        fromHand.RemoveCard(card);

        return false;
    }

    internal bool CardValid(Card cardBeingPlayed, Player usingOnPlayer, Hand fromHand)
    {
        bool valid = false;

        if (cardBeingPlayed == null)
            return false;

        if (fromHand.isPlayers)
        {
            if (cardBeingPlayed.cardData.cost <= player.mana)
            {
                if (usingOnPlayer.isPlayer && cardBeingPlayed.cardData.isDefenseCard)
                    valid = true;
                if (!usingOnPlayer.isPlayer && !cardBeingPlayed.cardData.isDefenseCard)
                    valid = true;
            }
        }
        else  // from enemys / do inimigo
        {
            if (cardBeingPlayed.cardData.cost <= enemy.mana)
            {
                if (!usingOnPlayer.isPlayer && cardBeingPlayed.cardData.isDefenseCard)
                    valid = true;
                if (usingOnPlayer.isPlayer && !cardBeingPlayed.cardData.isDefenseCard)
                    valid = true;
            }
        }
        return valid;
    }

    internal void CastCard(Card card, Player usingOnPlayer, Hand fromHand)
    {
        if (card.cardData.isMirrorCard)
        {
            usingOnPlayer.SetMirror(true);
            NextPlayerTurn();
            isPlayable = true;
        }
        else
        {
            if (card.cardData.isDefenseCard) // health cards / cartas de cura
            {
                usingOnPlayer.health += card.cardData.damage;

                if (usingOnPlayer.health > usingOnPlayer.maxHealth)
                    usingOnPlayer.health = usingOnPlayer.maxHealth;

                UpdateHealths();

                StartCoroutine(CastHealEffect(usingOnPlayer));
            }
            else // Attack card /  carta de ataque
            {
                CastAttackEffect(card, usingOnPlayer);// CastAttackEffect
            }
            // todo Add score
        }
        
        if (fromHand.isPlayers)
        {
            GameController.instance.player.mana -= card.cardData.cost;
            GameController.instance.player.UpdateManaBalls();
        }
        else
        {
            GameController.instance.enemy.mana -= card.cardData.cost;
            GameController.instance.enemy.UpdateManaBalls();
        }
    }

    private IEnumerator CastHealEffect(Player usingOnPLayer)
    {
        yield return new WaitForSeconds(0.5f);
        NextPlayerTurn();
        isPlayable = true;
    }

    internal void CastAttackEffect(Card card, Player usingOnPlayer)
    {
        GameObject effectGO = null;
        if (usingOnPlayer.isPlayer)
            effectGO = Instantiate(effectFromRightPrefab, canvas.gameObject.transform);
        else
            effectGO = Instantiate(effectFromLeftPrefab, canvas.gameObject.transform);

        Effect effect = effectGO.GetComponent<Effect>();
        if (effect)
        {
            effect.targetPlayer = usingOnPlayer;
            effect.sourceCard   = card;

            switch (card.cardData.damageType)
            {
                case CardData.DamageType.Fire:
                    if (card.cardData.isMulti)
                        effect.effectImage.sprite = multiFireBallImage;
                    else
                        effect.effectImage.sprite = fireBallImage;
                break;
                case CardData.DamageType.Ice:
                    if (card.cardData.isMulti)
                        effect.effectImage.sprite = multiIceBallImage;
                    else
                        effect.effectImage.sprite = iceBallImage;
                break;
                case CardData.DamageType.Both:
                    effect.effectImage.sprite = fireAndIceBallImage;
                break;
            }
        }
    }

    internal void UpdateHealths()
    {
        player.UpdateHealth();
        enemy.UpdateHealth();

        if (player.health <= 0)
        {
            // GameOver
        }
        if (enemy.health <= 0)
        {
            // todo new enemy
        }
    }

    private IEnumerator GameOver()
    {
        yield return new WaitForSeconds(1);
    }

    internal void NextPlayerTurn()
    {
        playersTurn = !playersTurn;

        if (playersTurn)
        {
            if (player.mana < 5)
                player.mana++;
        }
        else
        {
            if (enemy.mana < 5)
                enemy.mana++;
        }

        SetTurnText();

        player.UpdateManaBalls();
        enemy.UpdateManaBalls();

        if (!playersTurn)
            MonsterTurn();
    }

    internal void SetTurnText()
    {
        if (playersTurn)
        {
            turnText.text = "Merlin's turn";
        }
        else
        {
            turnText.text = "Enemy's turn";
        }
    }

    private void MonsterTurn()
    {
        Card card = AIChooseCard();
        StartCoroutine(MonsterCastCard(card));
    }

    private Card AIChooseCard()
    {
        List<Card> available = new List<Card>();
        for(int i = 0; i < 3; i++)
        {
            if (CardValid(enemysHand.cards[i], enemy, enemysHand))
                available.Add(enemysHand.cards[i]);
            else if (CardValid(enemysHand.cards[i], player, enemysHand))
                available.Add(enemysHand.cards[i]);
        }

        if (available.Count == 0) //none available / nenhum disponível 
        {
            NextPlayerTurn();
            return null;
        }
        int choice = UnityEngine.Random.Range(0, available.Count);
        return available[choice];
    }

    private IEnumerator MonsterCastCard(Card card)
    {
        yield return new WaitForSeconds(0.5f);

        if (card)
        {
            TurnCard(card);

            yield return new WaitForSeconds(2);

            if (card.cardData.isDefenseCard)
                UseCard(card, enemy, enemysHand);
            else // attack card
                UseCard(card, player, enemysHand);

            yield return new WaitForSeconds(1);

            enemyDeck.DealCard(enemysHand);

            yield return new WaitForSeconds(1);
        }
        else // no card to choose, skip turn / não há cartas para escolher, pular turno
        {
            enemySkipTurn.gameObject.SetActive(true);
            yield return new WaitForSeconds(1);
            enemySkipTurn.gameObject.SetActive(false);
        }
    }

    internal void TurnCard(Card card)
    {
        Animator animator = card.GetComponentInChildren<Animator>();
        if (animator)
        {
            animator.SetTrigger("Flip");
        }
        else
            Debug.LogError("No Animator found");
    }
}
