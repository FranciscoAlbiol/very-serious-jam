using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using TMPro;

public class quatro_manager : MonoBehaviour
{
    public quatro_card_SO[] deck;

    //player hand game object
    public GameObject p_hand;
    public GameObject t_hand;
    
    public Camera mainCamera;

    //everyone's hands + table cards
    private List<quatro_card_SO> NPC1_hand = new List<quatro_card_SO>();
    private List<quatro_card_SO> NPC2_hand = new List<quatro_card_SO>();
    private List<quatro_card_SO> player_hand = new List<quatro_card_SO>();

    private Queue<quatro_card_SO> table_deck = new Queue<quatro_card_SO>();
    private List<quatro_card_SO> table_hand = new List<quatro_card_SO>();

    private quatro_card_SO last_card_played;

    private int turn_index = 0;
    private bool is_match_over = false;
    private bool waiting_player = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void start_game()
    {
        suffle_deck();
        prepare_deck_queue();

        //give cards to players:
        //NPC 1
        for (int i = 0; i < 4; i++) {
            give_card(NPC1_hand);
        }

        //NPC 2
        for (int i = 0; i < 4; i++) {
            give_card(NPC2_hand);
        }

        //PLAYER
        player_hand_manager pHand_manager = p_hand.GetComponent<player_hand_manager>();
        for (int i = 0; i < 4; i++) {
            give_card(player_hand);
            pHand_manager.AddCardToHand(player_hand[i]);
        }


        //FIRST CARD IN TABLE
        quatro_card_SO current_card = table_deck.Peek(); table_deck.Dequeue();
        table_hand.Add(current_card);

        play_card(current_card);

        StartCoroutine(quatro_flow_maganer());  
        
    }

    void Update()
    {
        if (waiting_player && Input.GetMouseButtonDown(0))
        {
            player_select_card();
        }
    }

    IEnumerator quatro_flow_maganer()
    {
        while (!is_match_over)
        {
            if (turn_index >= 3)
            {
                turn_index = 0;
            }

            if (turn_index == 1)
            {
                // Player's turn
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
                // NPC's turn
                yield return new WaitForSeconds(1.0f); 
                npc_turn();
                
            }

            turn_index++;
        }

    }

    void player_turn()
    {
        
    }

    void player_select_card() {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
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

    // Call this method when the real player clicks a card or draws
    public void end_player_turn()
    {
        if (turn_index == 1 && waiting_player)
        {
            waiting_player = false; // This breaks the inner while loop and advances the game
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

        card_view cardScript = t_hand.GetComponent<card_view>();
        cardScript.Initialize(card);
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
}
