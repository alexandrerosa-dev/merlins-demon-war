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
        Debug.Log("Skip turn");
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
        // CastCard
        // RemoveCard
        // DealReplacementCard
        if (!CardValid(card, usingOnPlayer, fromHand))
            return false;

        isPlayable = false;

        CastCard(card, usingOnPlayer, fromHand);

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
        }
        else
        {
            if (card.cardData.isDefenseCard)
            {

            }
            else // Attack card /  carta de ataque
            {
                CastAttackEffect(card, usingOnPlayer);// CastAttackEffect
            }
            // todo Add score
        }
        // todo update PLayer mana
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
}
