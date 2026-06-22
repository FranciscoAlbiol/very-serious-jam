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
    
    public card_SO[] deck;

    //player cards (objects)
    public GameObject p_card1;
    public GameObject p_card2;

    //table cards (objects)
    public GameObject[] t_cards;

    //everyone's hands + table cards
    private card_SO[] NPC1_hand = new card_SO[2];
    private card_SO[] NPC2_hand = new card_SO[2];
    private card_SO[] player_hand = new card_SO[2];
    private card_SO[] table_cards = new card_SO[6];

    //ui
    public GameObject action_buttons;
    public GameObject raise_bet_HUD;

    //turn checks
    private bool waiting_player = false;
    private int turn_index = 0;
    private int current_bet = 0; //keeps track of money spent this game
    private int raise_bet = 0;

    public static poker_manager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }


    public void start_poker()
    {
        suffle_deck();
        create_round();

        change_bet(5);

        StartCoroutine(poker_flow_manager());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator poker_flow_manager() {
        //NOTE: CHECK IF AT ANY POINT ANY OF THE NPCS FOLD AND JUST. DON'T MAKE THEM HAVE A TURN
        //IF THE PLAYER IS THE ONLY ONE THAT DIDN'T FOLD, GIVE THEM A WIN

        //1. BET
        yield return StartCoroutine(npc_turn1(0)); 
        yield return StartCoroutine(player_turn()); 
        yield return StartCoroutine(npc_turn1(2));

        //2. SHOW 3 CARDS FROM TABLE
        Debug.Log("Flop");
        show_table_cards(0, 2);
        yield return new WaitForSeconds(1f);

        //3. BET AGAIN
        yield return StartCoroutine(npc_turn2(0, 3)); 
        yield return StartCoroutine(player_turn()); 
        yield return StartCoroutine(npc_turn2(2, 3));

        //4. SHOW 2 CARDS FROM TABLE
        Debug.Log("Turn");
        show_table_cards(3, 4);
        yield return new WaitForSeconds(1f);

        //5. BET AGAIN
        yield return StartCoroutine(npc_turn2(0, 5)); 
        yield return StartCoroutine(player_turn()); 
        yield return StartCoroutine(npc_turn2(2, 5));

        //6. SHOW FINAL CARD FROM TABLE
        Debug.Log("River");
        show_table_cards(5, 5);
        yield return new WaitForSeconds(1f);

        //7. FINAL BET
        yield return StartCoroutine(npc_turn2(0, 6)); 
        yield return StartCoroutine(player_turn()); 
        yield return StartCoroutine(npc_turn2(2, 6));

        //8. SHOW CARDS
        //not doing it here
        //add 2 cards in the table in front of each player
        //hide the held cards; show the cards in the table

        //9. ASSIGN WINNER
        int pointsNPC1 = calculate_points(NPC1_hand); Debug.Log(pointsNPC1);
        int pointsNPC2 = calculate_points(NPC1_hand); Debug.Log(pointsNPC2);
        int pointsPlayer = calculate_points(player_hand); Debug.Log(pointsPlayer);

        //here's where you'd give the player money. if they win or not
        //player_reward();
    }

    IEnumerator player_turn() {
        waiting_player = true;
        action_buttons.SetActive(true);
        yield return new WaitUntil(() => !waiting_player);
        action_buttons.SetActive(false);
    }

    IEnumerator npc_turn1(int npcIndex) {
        
        card_SO[] currentNPC;
        if(npcIndex == 0) currentNPC = NPC1_hand;
        else currentNPC = NPC2_hand;

        //npc will fold if: 
        // - sum of cards is less than 10 (only number cards (ignoring the aces rn))
        // - they are not a pair or of the same suit
        if(currentNPC[0].number + currentNPC[1].number < 10
           && (currentNPC[0].number != currentNPC[1].number
               || currentNPC[0].suit != currentNPC[1].suit)) {

            //fold
            Debug.Log("I fold!");
        }
        
        else {
            Debug.Log("I keep playing!");
        }

        yield return new WaitForSeconds(1f);
    }

    IEnumerator npc_turn2(int npcIndex, int cards_in_table) {
        
        card_SO[] currentNPC;
        if(npcIndex == 0) currentNPC = NPC1_hand;
        else currentNPC = NPC2_hand;

        HandEvaluation hand = EvaluateHand(currentNPC, table_cards, cards_in_table);

        
        //npc will raise if:
        // - They see a flush
        // - They see a four of a kind
        // - They see a full house
        // - 30% they just raise

        //while testing with a single suit, they would always see a flush
        //line to add to check for flush: hand.hasFlush

        float bluff = UnityEngine.Random.Range(0f, 100f); 
        if (hand.hasFourOfAKind 
            || hand.hasFullHouse || bluff <= 10f) {
           //raise
           Debug.Log("I raise my bet!");
        }

        //npc will fold if: 
        // - they don't see a:
        //   -- pair
        //   -- two pair
        else if (!hand.hasPair && !hand.hasTwoPair) {
           //fold
           Debug.Log("I stop playing!");
        }

        //else: check
        else {
            //check
            Debug.Log("I keep playing");
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
        //give each player the first 6 cards
        NPC1_hand[0] = deck[0];
        NPC1_hand[1] = deck[1];

        NPC2_hand[0] = deck[2];
        NPC2_hand[1] = deck[3];

        player_hand[0] = deck[4];
        player_hand[1] = deck[5];

        //give the next 6 cards to the table
        table_cards[0] = deck[6];
        table_cards[1] = deck[7];
        table_cards[2] = deck[8];
        table_cards[3] = deck[9];
        table_cards[4] = deck[10];
        table_cards[5] = deck[11];

        //show player hand on screen
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

        //Combine hand and active table cards
        List<card_SO> allCards = new List<card_SO>(hand);
        for (int i = 0; i < cardsInTable; i++) {
            allCards.Add(tableCards[i]);
        }

        //Calculate groups
        var numberGroups = allCards.GroupBy(c => c.number).Select(g => g.Count()).ToList();
        var suitGroups = allCards.GroupBy(c => c.suit).ToList();

        int pairsCount = numberGroups.Count(count => count == 2);
        int tripsCount = numberGroups.Count(count => count == 3);

        //Flush / Straight
        bool hasFlush = suitGroups.Any(g => g.Count() >= 5);
        bool hasStraight = CheckForStraight(allCards);

        //Straight Flush
        bool hasStraightFlush = false;
        if (hasFlush) {
            // Find the suit that has 5 or more cards, and check if those specific cards form a straight
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
        // Get card number and sort them from small to big
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

    //actions
    public void player_fold() {
        //exits the minigame straigh up
    }

    public void player_check() {
        //goes to the next guy turn and does nothing else
        waiting_player = false;
    }

    public void player_raise() {
        action_buttons.SetActive(false);
        raise_bet_HUD.SetActive(true);
        raise_bet = 0;

        TMP_Text bet_text = raise_bet_HUD.GetComponentInChildren<TMP_Text>();        
        bet_text.text = "0";
    }

    public void player_finish_raise() {
        current_bet += raise_bet;
        raise_bet_HUD.SetActive(false);
        waiting_player = false;
    }

    //raising bet
    public void change_bet(int amount) {
        raise_bet += amount;

        TMP_Text bet_text = raise_bet_HUD.GetComponentInChildren<TMP_Text>();        
        bet_text.text = raise_bet.ToString();
    }
}

