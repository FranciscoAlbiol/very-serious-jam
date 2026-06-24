using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class quatro_manager : MonoBehaviour
{
    public PlayerInteraction p_interaction;
    public GameObject quatro_scene;

    public quatro_card_SO[] deck;

    public GameObject p_hand;
    public GameObject t_hand;
    
    public Camera mainCamera;

    private List<quatro_card_SO> NPC1_hand = new List<quatro_card_SO>();
    private List<quatro_card_SO> NPC2_hand = new List<quatro_card_SO>();
    private List<quatro_card_SO> player_hand = new List<quatro_card_SO>();

    private Queue<quatro_card_SO> table_deck = new Queue<quatro_card_SO>();
    private List<quatro_card_SO> table_hand = new List<quatro_card_SO>();

    private quatro_card_SO last_card_played;

    public GameObject action_buttons;
    public GameObject raise_bet_HUD;
    public GameObject poker_scene;

    private int turn_index = 0;
    private bool is_match_over = false;
    private bool waiting_player = false;

    private int current_bet = 0;
    private int raise_bet = 0;
    private int mult = 1; //to check for reverse turns

    public static quatro_manager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void start_quatro()
    {
        player_hand_manager pHand_manager = p_hand.GetComponent<player_hand_manager>();

        Debug.Log("Playing a quatro match");
        suffle_deck();
        prepare_deck_queue();

        //make sure their hands are empty before starting a match
        NPC1_hand.Clear();
        NPC2_hand.Clear();

        pHand_manager.EliminateAllCards();
        player_hand.Clear();


        //give npc cards
        for (int i = 0; i < 4; i++) {
            give_card(NPC1_hand);
        }

        for (int i = 0; i < 4; i++) {
            give_card(NPC2_hand);
        }

        //give player cards
        
        for (int i = 0; i < 4; i++) {
            give_card(player_hand);
            pHand_manager.AddCardToHand(player_hand[i]);
        }

        //set the first card on the table
        quatro_card_SO current_card = table_deck.Peek(); table_deck.Dequeue();
        table_hand.Add(current_card);

        play_card(current_card);

        StartCoroutine(quatro_flow_maganer());
        
    }

    void Update()
    {
        if (waiting_player && Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            player_select_card();
        }
    }

    IEnumerator quatro_flow_maganer()
    {
        yield return new WaitForSeconds(1.0f); 
        quatro_scene.SetActive(true);

        while (!is_match_over)
        {
            if (turn_index >= 3)
            {
                turn_index = 0;
            }

            if (turn_index == 1)
            {
                Debug.Log("Player turn started.");
                waiting_player = true;
                player_turn();

                while (waiting_player)
                {
                    yield return null;
                }
            }
            else
            {
                yield return new WaitForSeconds(1.0f); 
                npc_turn();
                
            }

            turn_index = turn_index + (1 * mult);
        }

    }

    void player_turn()
    {
        action_buttons.SetActive(true);
    }

    void player_select_card() {
        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            card_view clickedCard = hit.collider.GetComponent<card_view>();

            if (clickedCard != null)
            {
                check_if_card_is_valid(clickedCard);
            }
        }
    }

    void check_if_card_is_valid(card_view card) {
        quatro_card_SO cardData = card.card_data;

        if (cardData.number == last_card_played.number || cardData.color == last_card_played.color) {
            {
                play_card(cardData);

                player_hand.Remove(cardData);
                player_hand_manager pHand_manager = p_hand.GetComponent<player_hand_manager>();
                pHand_manager.EliminateCard(card);
            }
        }
    }

    public void end_player_turn()
    {
        Debug.Log("finishing turn !!");
        if (turn_index == 1 && waiting_player)
        {
            waiting_player = false;
            action_buttons.SetActive(false);
        }
    }

    void npc_turn()
    {
        List<quatro_card_SO> current_hand = (turn_index == 0) ? NPC1_hand : NPC2_hand;
        bool playedCard = false;

        for (int i = 0; i < current_hand.Count; i++)
        {
            if (current_hand[i].number == last_card_played.number || current_hand[i].color == last_card_played.color)
            {
                Debug.Log($"NPC {turn_index} plays a card!!");
                play_card(current_hand[i]);
                playedCard = true;
                current_hand.RemoveAt(i);

                break;
            }
        }

        if (!playedCard)
        {
            Debug.Log($"NPC {turn_index} can't play and draws a card :(");
            give_card(current_hand);
        }

    }

    void suffle_deck() {
        for (int i = deck.Length - 1; i > 0; i--)
        {
            int randomIndex = UnityEngine.Random.Range(0, i + 1);

            quatro_card_SO temp = deck[i];
            deck[i] = deck[randomIndex];
            deck[randomIndex] = temp;
        }
    }

    void prepare_deck_queue() {
        for (int i = 0; i < deck.Length; i++)
        {
            table_deck.Enqueue(deck[i]);
        }
    }

    void play_card(quatro_card_SO card) {
        last_card_played = card;

        if (card.number > 9) {
            if (card.number == 10) {
                List<quatro_card_SO> current_hand;
                //10 : +4
                if (turn_index == 0) {
                    //give cards to player
                    for (int i = 0; i < 4; i++) {
                        give_card_to_player();
                    }
                    
                }

                else {
                    if (turn_index == 1) {
                    //give cards to npc2
                    current_hand = NPC2_hand;
                    }
                    else {
                        //give cards to npc1
                        current_hand = NPC1_hand;
                    }

                    for (int i = 0; i < 4; i++) {
                        give_card(current_hand);
                    }
                }
                
            }
            
            else if (card.number == 11) {
                //11 : block
                turn_index = turn_index + (1 * mult);
            }
            
            else {
                //12 : reverse
                mult = mult * -1;
            }

            
        }

        card_view cardScript = t_hand.GetComponent<card_view>();
        cardScript.Initialize(card);
        if(turn_index == 1) //player turn
                    end_player_turn();
    }

    public void give_card(List<quatro_card_SO> hand) {
        if (table_deck.Count != 0) {
            quatro_card_SO current_card = table_deck.Peek(); table_deck.Dequeue();
            hand.Add(current_card);
        }

        else
            Debug.Log("No more cards in the deck!");
    }

    public void give_card_to_player() {
        give_card(player_hand);
        player_hand_manager pHand_manager = p_hand.GetComponent<player_hand_manager>();
        pHand_manager.AddCardToHand(player_hand[player_hand.Count-1]);

        end_player_turn();
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

    public void change_bet(int amount) {
        raise_bet += amount;

        TMP_Text bet_text = raise_bet_HUD.GetComponentInChildren<TMP_Text>();        
        bet_text.text = raise_bet.ToString();
    }

    public void player_fold() {
        StopAllCoroutines();
        action_buttons.SetActive(false);
        poker_scene.SetActive(false);
        p_interaction.EndInteraction();
    }
}