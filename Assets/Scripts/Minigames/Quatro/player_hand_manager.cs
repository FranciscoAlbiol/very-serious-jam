using System.Collections.Generic;
using UnityEngine;

public class player_hand_manager : MonoBehaviour
{
    [Header("Settings")]
    public GameObject cardPrefab; 
    public float cardSpacing = .5f; // Distance between each card's center
    public quatro_card_SO card_data; //placeholder in place of sending the actual cards through the manager

    private List<GameObject> spawnedCards = new List<GameObject>();


    public void AddCardToHand(quatro_card_SO cardData)
    {
        GameObject newCard = Instantiate(cardPrefab, transform);

        card_view cardScript = newCard.GetComponent<card_view>();

        if (cardScript != null)
        {
            cardScript.Initialize(cardData);
        }

        spawnedCards.Add(newCard);
        

        ArrangeCards();
    }

    public void EliminateAllCards() {
        int cardCount = spawnedCards.Count;

        Debug.Log("Going to delete every card in phand");

        foreach (GameObject card in spawnedCards)
        {
            if (card != null) 
            {
                Destroy(card);
            }
        }
        spawnedCards.Clear();
    }

    public void EliminateCard(card_view card) {
        spawnedCards.Remove(card.gameObject);
        Destroy(card.gameObject);
        
        ArrangeCards();
    }


    private void ArrangeCards()
    {
        int cardCount = spawnedCards.Count;
        if (cardCount == 0) return;

        float totalWidth = (cardCount - 1) * cardSpacing;
        float startX = -totalWidth / 2f;

        for (int i = 0; i < cardCount; i++)
        {
            float localX = startX + (i * cardSpacing);

            spawnedCards[i].transform.localPosition = new Vector3(localX, 0f, 0f);
            spawnedCards[i].transform.localRotation = Quaternion.identity;
        }
    }


}