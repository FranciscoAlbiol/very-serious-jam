using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using TMPro;

public class HandEvaluation {
    public bool hasStraightFlush;
    public bool hasFourOfAKind;
    public bool hasFullHouse;
    public bool hasFlush;
    public bool hasStraight;
    public bool hasThreeOfAKind;
    public bool hasTwoPair;
    public bool hasPair;
}

public class poker_manager : MonoBehaviour
{

    public PlayerInteraction p_interaction;
    
    public card_SO[] deck;

    public GameObject p_card1;
    public GameObject p_card2;

    public GameObject NPC1_cards;
    public GameObject NPC2_cards;

    public GameObject[] t_cards;

    private card_SO[] NPC1_hand = new card_SO[2];
    private card_SO[] NPC2_hand = new card_SO[2];
    private card_SO[] player_hand = new card_SO[2];
    private card_SO[] table_cards = new card_SO[6];

    public GameObject action_buttons;
    public GameObject raise_bet_HUD;
    public GameObject poker_scene;
    public Sprite back_card_sprite;

    public GameObject indicator_speech_bubble;
    public TMP_Text indicator_text;

    private bool waiting_player = false;
    private int turn_index = 0;
    private int current_bet = 0; 
    private int raise_bet = 0;

    private bool NPC1_folded = false;
    private bool NPC2_folded = false;

    private int global_bet = 0; 
    private int npc1_bet = 0;
    private int npc2_bet = 0;

    private int current_min_bet = 5;
    private int player_bet = 0;

    public static poker_manager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        if (wheelAudioSource == null) {
            wheelAudioSource = gameObject.AddComponent<AudioSource>();
        }
    }


    public void start_poker()
    {
        Debug.Log("starting poker");
        poker_scene.SetActive(true);
        NPC1_cards.SetActive(true);
        NPC2_cards.SetActive(true);

        waiting_player = false;
        turn_index = 0;
        current_bet = 0; 
        raise_bet = 0;
        NPC1_folded = false;
        NPC2_folded = false;

        if (indicator_speech_bubble != null) indicator_speech_bubble.SetActive(false);

        current_min_bet = 5;
        player_bet = 5;
        npc1_bet = 5;
        npc2_bet = 5;
        global_bet = 15;
        GameManager.Instance.current_money -= 5;

        suffle_deck();
        for(int i = 0; i < table_cards.Length; i++) {
            t_cards[i].GetComponent<SpriteRenderer>().sprite = back_card_sprite;    

        }

        create_round();

        change_bet(5);

        StartCoroutine(poker_flow_manager());
    }

    void Update()
    {
        
    }

    IEnumerator poker_flow_manager() {

        yield return StartCoroutine(npc_turn1(0)); 
        if (NPC1_folded) NPC1_cards.SetActive(false);

        yield return StartCoroutine(player_turn()); 
        yield return StartCoroutine(npc_turn1(2));
        if (NPC2_folded) NPC2_cards.SetActive(false);
        
        if (NPC1_folded && NPC2_folded)  {
            Debug.Log("Player wins");
            GameManager.Instance.current_money += global_bet;
            player_fold();
        }

        Debug.Log("Flop");
        show_table_cards(0, 2);
        yield return new WaitForSeconds(1f);

        if (!NPC1_folded) {
            yield return StartCoroutine(npc_turn2(0, 3)); 
            if (NPC1_folded) NPC1_cards.SetActive(false);
        }

        yield return StartCoroutine(player_turn()); 

        if (!NPC2_folded) {
            yield return StartCoroutine(npc_turn2(2, 3));
            if (NPC2_folded) NPC2_cards.SetActive(false);
        }

        if (NPC1_folded && NPC2_folded)  {
            Debug.Log("Player wins");
            GameManager.Instance.current_money += global_bet;
            player_fold();
        }

        Debug.Log("Turn");
        show_table_cards(3, 4);
        yield return new WaitForSeconds(1f);

        if (!NPC1_folded) {
            yield return StartCoroutine(npc_turn2(0, 5)); 
            if (NPC1_folded) NPC1_cards.SetActive(false);
        }

        yield return StartCoroutine(player_turn()); 

        if (!NPC2_folded) {
            yield return StartCoroutine(npc_turn2(2, 5));
            if (NPC2_folded) NPC2_cards.SetActive(false);
        }

        if (NPC1_folded && NPC2_folded)  {
            Debug.Log("Player wins");
            GameManager.Instance.current_money += global_bet;
            player_fold();
        }

        Debug.Log("River");
        show_table_cards(5, 5);
        yield return new WaitForSeconds(1f);

        if (!NPC1_folded) {
            yield return StartCoroutine(npc_turn2(0, 6)); 
            if (NPC2_folded) NPC2_cards.SetActive(false);
        }

        yield return StartCoroutine(player_turn());

        if (!NPC1_folded) { 
            yield return StartCoroutine(npc_turn2(2, 6));
            if (NPC2_folded) NPC2_cards.SetActive(false);
        }

        if (NPC1_folded && NPC2_folded)  {
            Debug.Log("Player wins");
            yield return new WaitForSeconds(1f);
            GameManager.Instance.current_money += global_bet;
            player_fold();
        }

        int pointsNPC1 = calculate_points(NPC1_hand); Debug.Log(pointsNPC1);
        int pointsNPC2 = calculate_points(NPC2_hand); Debug.Log(pointsNPC2);
        int pointsPlayer = calculate_points(player_hand); Debug.Log(pointsPlayer);

        if ((NPC1_folded || pointsPlayer >= pointsNPC1) && (NPC2_folded || pointsPlayer >= pointsNPC2)) {
            Debug.Log("player wins");
            GameManager.Instance.current_money += global_bet;
        }

        else {
            Debug.Log("plyer loses");
        }

        yield return new WaitForSeconds(1f);
        player_fold();
    }

    IEnumerator player_turn() {
        if ((NPC1_folded && turn_index == 0) || (NPC2_folded && turn_index == 2)) {
            yield break;
        }

        waiting_player = true;
        action_buttons.SetActive(true);
        yield return new WaitUntil(() => !waiting_player);
        action_buttons.SetActive(false);
    }

    IEnumerator npc_turn1(int npcIndex) {
        
        card_SO[] currentNPC;
        if(npcIndex == 0) currentNPC = NPC1_hand;
        else currentNPC = NPC2_hand;

        SpriteRenderer bubbleSprite = indicator_speech_bubble.GetComponent<SpriteRenderer>();
        if (bubbleSprite != null) {
            bubbleSprite.flipY = (npcIndex != 0);
        }

        indicator_speech_bubble.SetActive(true);

        if(currentNPC[0].number + currentNPC[1].number < 10
           && (currentNPC[0].number != currentNPC[1].number
               || currentNPC[0].suit != currentNPC[1].suit)) {

            Debug.Log("I fold!");
            indicator_text.text = "I fold.";
            yield return new WaitForSeconds(1.5f);
            indicator_speech_bubble.SetActive(false);

            if(npcIndex == 0) NPC1_folded = true;
            else NPC2_folded = true;
        }
        
        else {
            Debug.Log("I keep playing!");
            indicator_text.text = "I call.";
            yield return new WaitForSeconds(1.5f);
            indicator_speech_bubble.SetActive(false);

            if(npcIndex == 0) { npc1_bet += 5; global_bet += 5; }
            else { npc2_bet += 5; global_bet += 5; }
        }

        yield return new WaitForSeconds(1f);
    }

    IEnumerator npc_turn2(int npcIndex, int cards_in_table) {
        
        card_SO[] currentNPC;
        int npcCurrentBet;

        if (npcIndex == 0) {
            currentNPC = NPC1_hand;
            npcCurrentBet = npc1_bet;
        } else {
            currentNPC = NPC2_hand;
            npcCurrentBet = npc2_bet;
        }

        SpriteRenderer bubbleSprite = indicator_speech_bubble.GetComponent<SpriteRenderer>();
        if (bubbleSprite != null) {
            bubbleSprite.flipY = (npcIndex != 0);
        }

        indicator_speech_bubble.SetActive(true);

        float pressureFoldChance = (current_min_bet / 100) * 5f;
        if (UnityEngine.Random.Range(0f, 100f) <= pressureFoldChance) {
            Debug.Log($"NPC {npcIndex} folds due to high bet pressure!");
            indicator_text.text = "Too rich for my blood. Fold.";
            yield return new WaitForSeconds(1.5f);
            indicator_speech_bubble.SetActive(false);

            if (npcIndex == 0) NPC1_folded = true;
            else NPC2_folded = true;
            yield return new WaitForSeconds(1f);
            yield break;
        }

        HandEvaluation hand = EvaluateHand(currentNPC, table_cards, cards_in_table);

        float bluff = UnityEngine.Random.Range(0f, 100f); 
        if (hand.hasFourOfAKind 
            || hand.hasFullHouse || bluff <= 10f) {
           Debug.Log("I raise my bet!");
           indicator_text.text = "I raise by 10!";
           yield return new WaitForSeconds(1.5f);
           indicator_speech_bubble.SetActive(false);

           int npcRaiseAmount = (current_min_bet - npcCurrentBet) + 10;
           current_min_bet += 10;

           if(npcIndex == 0) { npc1_bet += npcRaiseAmount; global_bet += npcRaiseAmount; }
           else { npc2_bet += npcRaiseAmount; global_bet += npcRaiseAmount; }
        }

        else if (!hand.hasPair && !hand.hasTwoPair) {
           Debug.Log("I stop playing!");
           indicator_text.text = "I'm out. Fold.";
           yield return new WaitForSeconds(1.5f);
           indicator_speech_bubble.SetActive(false);

           if(npcIndex == 0) NPC1_folded = true;
           else NPC2_folded = true;
        }

        else {
            int npcCallAmount = current_min_bet - npcCurrentBet;
            if (npcCallAmount == 0) {
                Debug.Log("I check");
                indicator_text.text = "Check.";
            } else {
                Debug.Log("I call");
                indicator_text.text = "I'll call.";
            }
            yield return new WaitForSeconds(1.5f);
            indicator_speech_bubble.SetActive(false);

            if(npcIndex == 0) { npc1_bet += npcCallAmount; global_bet += npcCallAmount; }
            else { npc2_bet += npcCallAmount; global_bet += npcCallAmount; }
        }
            

        yield return new WaitForSeconds(1f);
    }

    void suffle_deck() {
        for (int i = deck.Length - 1; i > 0; i--)
        {
            int randomIndex = UnityEngine.Random.Range(0, i + 1);

            card_SO temp = deck[i];
            deck[i] = deck[randomIndex];
            deck[randomIndex] = temp;
        }
    }

    void create_round() {
        NPC1_hand[0] = deck[0];
        NPC1_hand[1] = deck[1];

        NPC2_hand[0] = deck[2];
        NPC2_hand[1] = deck[3];

        player_hand[0] = deck[4];
        player_hand[1] = deck[5];

        table_cards[0] = deck[6];
        table_cards[1] = deck[7];
        table_cards[2] = deck[8];
        table_cards[3] = deck[9];
        table_cards[4] = deck[10];
        table_cards[5] = deck[11];

        p_card1.GetComponent<SpriteRenderer>().sprite = player_hand[0].card_sprite;    
        p_card2.GetComponent<SpriteRenderer>().sprite = player_hand[1].card_sprite;    
    }

    void show_table_cards(int start_pos, int last_pos) {
        for (int i = start_pos; i <= last_pos; i++) {
            t_cards[i].SetActive(true);
            t_cards[i].GetComponent<SpriteRenderer>().sprite = table_cards[i].card_sprite;    
        }

    }

    HandEvaluation EvaluateHand(card_SO[] hand, card_SO[] tableCards, int cardsInTable) {
        HandEvaluation result = new HandEvaluation();

        List<card_SO> allCards = new List<card_SO>(hand);
        for (int i = 0; i < cardsInTable; i++) {
            allCards.Add(tableCards[i]);
        }

        var numberGroups = allCards.GroupBy(c => c.number).Select(g => g.Count()).ToList();
        var suitGroups = allCards.GroupBy(c => c.suit).ToList();

        int pairsCount = numberGroups.Count(count => count == 2);
        int tripsCount = numberGroups.Count(count => count == 3);

        bool hasFlush = suitGroups.Any(g => g.Count() >= 5);
        bool hasStraight = CheckForStraight(allCards);

        bool hasStraightFlush = false;
        if (hasFlush) {
            var flushSuitCards = suitGroups.First(g => g.Count() >= 5).ToList();
            hasStraightFlush = CheckForStraight(flushSuitCards);
        }

        result.hasStraightFlush = hasStraightFlush;
        result.hasFourOfAKind = numberGroups.Contains(4);
        result.hasFullHouse = (tripsCount >= 1 && pairsCount >= 1) || (tripsCount >= 2);
        result.hasFlush = hasFlush;
        result.hasStraight = hasStraight;
        result.hasThreeOfAKind = numberGroups.Contains(3);
        result.hasTwoPair = pairsCount >= 2;
        result.hasPair = pairsCount >= 1;    

        return result;
    }

    bool CheckForStraight(List<card_SO> cards) {
        var distinctnumbers = cards.Select(c => c.number).Distinct().OrderBy(r => r).ToList();
        if (distinctnumbers.Count < 5) return false;

        int consecutiveCount = 1;
        for (int i = 0; i < distinctnumbers.Count - 1; i++) {
            if (distinctnumbers[i + 1] == distinctnumbers[i] + 1) {
                consecutiveCount++;
                if (consecutiveCount >= 5) return true;
            } else {
                consecutiveCount = 1;
            }
        }

        return false;
    }

    int calculate_points(card_SO[] hand) {
        HandEvaluation current_hand = EvaluateHand(hand, table_cards, 6);
        
        List<card_SO> allCards = new List<card_SO>(hand);
        for (int i = 0; i < 6; i++) {
            allCards.Add(table_cards[i]);
        }

        var numberGroups = allCards.GroupBy(c => c.number).Select(g => g.Count()).ToList();
        var suitGroups = allCards.GroupBy(c => c.suit).ToList();

        int score;

        if (current_hand.hasStraightFlush) {
            var sfCards = suitGroups.First(g => g.Count() >= 5).ToList();
            score = SetHandScore(100, 8, sfCards);
        }
        else if (current_hand.hasFourOfAKind) {
            var fourCards = allCards.GroupBy(c => c.number).First(g => g.Count() == 4).ToList();
            score = SetHandScore(60, 7, fourCards);
        }
        else if (current_hand.hasFullHouse) {
            var fhCards = allCards.GroupBy(c => c.number).Where(g => g.Count() == 3 || g.Count() == 2).SelectMany(g => g).ToList();
            score = SetHandScore(40, 4, fhCards);
        }
        else if (current_hand.hasFlush) {
            var flushCards = suitGroups.First(g => g.Count() >= 5).Take(5).ToList();
            score = SetHandScore(35, 4, flushCards);
        }
        else if (current_hand.hasThreeOfAKind) {
            var tripsCards = allCards.GroupBy(c => c.number).First(g => g.Count() == 3).ToList();
            score = SetHandScore(30, 3, tripsCards);
        }
        else if (current_hand.hasTwoPair) {
            var twoPairCards = allCards.GroupBy(c => c.number).Where(g => g.Count() == 2).SelectMany(g => g).ToList();
            score = SetHandScore(20, 2, twoPairCards);
        }
        else if (current_hand.hasPair) {
            var pairCards = allCards.GroupBy(c => c.number).First(g => g.Count() == 2).ToList();
            score = SetHandScore(10, 2, pairCards);
        }
        else {
            var highestCard = new List<card_SO> { allCards.OrderByDescending(c => c.number).First() };
            score = SetHandScore(5, 1, highestCard);
        }

        return score;
    }

    int SetHandScore(int chips, int mult, List<card_SO> card_list) {
        int bonus_chips = card_list.Sum(card => card.number);
        return (chips + bonus_chips)*mult;
    }

    public void player_fold() {
        StopAllCoroutines();
        action_buttons.SetActive(false);
        poker_scene.SetActive(false);
        p_interaction.EndInteraction();
    }

    public void player_check() {
        int callAmount = current_min_bet - player_bet;
        if (callAmount > 0) {
            if (GameManager.Instance.current_money >= callAmount) {
                player_bet += callAmount;
                global_bet += callAmount;
                GameManager.Instance.current_money -= callAmount;
            } else {
                player_fold();
                return;
            }
        }
        waiting_player = false;
    }

    public void player_raise() {
        action_buttons.SetActive(false);
        raise_bet_HUD.SetActive(true);
        raise_bet = current_min_bet - player_bet + 5;

        TMP_Text bet_text = raise_bet_HUD.GetComponentInChildren<TMP_Text>();        
        bet_text.text = raise_bet.ToString();
    }

    public void player_finish_raise() {
        current_min_bet = player_bet + raise_bet;
        global_bet += raise_bet;
        GameManager.Instance.current_money -= raise_bet; 
        player_bet += raise_bet;
        raise_bet_HUD.SetActive(false);
        waiting_player = false;
    }

    public void change_bet(int amount) {
        int minimumRequiredRaise = current_min_bet - player_bet + 5;
        int target_bet = raise_bet + amount;

        if (target_bet < minimumRequiredRaise) {
            target_bet = minimumRequiredRaise;
        }

        if (target_bet > GameManager.Instance.current_money) {
            target_bet = GameManager.Instance.current_money;
        }

        raise_bet = target_bet;

        TMP_Text bet_text = raise_bet_HUD.GetComponentInChildren<TMP_Text>();        
        bet_text.text = raise_bet.ToString();
    }
}