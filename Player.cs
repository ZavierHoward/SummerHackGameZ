using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Player : MonoBehaviour
{

    [SerializeField] private GameManagement game_manager;
    [SerializeField] private Card[] deck;
    private List<Card> cards_in_hand = new();
    [SerializeField] private Hotel hotel;
    [SerializeField] private float money;
    [SerializeField] private int placing; // 1 - 4
    [SerializeField] private bool isTurn, ai = false;
    [SerializeField] private bool startTurn = true;

    [SerializeField] private readonly float DEFAULT_MONEY = 1000; //Maybe later on I'll change it so players can decide how much money they wanna start off with, how many cards in hand, etc.
    [SerializeField] private readonly float STARTING_CARDS_IN_HAND = 5;
    [SerializeField] private readonly float MAX_CARDS_IN_HAND = 6;

    private int card_in_deck = 0;

    private void Update()
    {

        if (!ai)
        {
            return;
        }
        if (!isTurn)
        {
            return;
        }
        if (!game_manager.GameActive())
        {
            return;
        }

        if (startTurn)
        {
            StartCoroutine(FullTurn());
            startTurn = false;
        }
        //Determine priorities --> this just determines what card to activate
        //Set price of hotel
        //Choose random card in hand to activate
        //Buy amenities
        //End turn

    }

    IEnumerator FullTurn()
    {

        bool sabotage = false;
        if(Random.Range(0, 100) > 70)
        {
            sabotage = true;
        }
        //For now, the AI is just going to pick a card randomly to activate each turn, and if it fails to activate, end turn

        yield return new WaitForSeconds(0.75f);

        //I can have them RNG it to determine how many cards they can play a turn

        if (sabotage)
        {
            print("AI wants to sabotage a player! The one in first place!");
            for(int i = 0; i < cards_in_hand.Count; i++)
            {
                if (cards_in_hand[i].GetSabotage())
                {
                    print("AI attempts to activate the card - " + cards_in_hand[i].transform.parent.name + "!");
                    game_manager.CardActivated(cards_in_hand[i].transform.parent.name);
                    yield return new WaitForSeconds(1.0f);
                    game_manager.TargetSelected(0); //Always target first place
                    yield return new WaitForSeconds(0.75f);
                    break;
                }
            }
        }
        else
        {
            print("AI wants to help themself!");
            //int x = 0;
            for (int i = 0; i < cards_in_hand.Count; i++)
            {
                if (cards_in_hand[i].GetSelfGain())
                {
                    print("AI attempts to activate the card - " + cards_in_hand[i].transform.parent.name + "!");
                    game_manager.CardActivated(cards_in_hand[i].transform.parent.name);
                    yield return new WaitForSeconds(1.0f);
                    break;
                }
            }
            
        }
        yield return new WaitForSeconds(1.5f);

        //Buy amenities

        if (Random.Range(0, 3) == 2) //33% chance to buy amenities
        {
            print("AI wants to buy an amenity");
            List<Amenity> amenities_that_can_be_bought = new();
            for (int i = 0; i < game_manager.GetAllAmenities().Count(); i++)
            {
                if (game_manager.GetAllAmenities()[i].GetPrice() <= GetMoney())
                {
                    amenities_that_can_be_bought.Add(game_manager.GetAllAmenities()[i]);
                }
            }

            game_manager.BuyAmenity(amenities_that_can_be_bought[Random.Range(0, amenities_that_can_be_bought.Count)]);
            yield return new WaitForSeconds(0.5f);
        }
        else
        {
            print("AI does not want to buy an amenity");
        }

        yield return new WaitForSeconds(1f);

        int new_price = (int)(hotel.GetRating() * hotel.GetNumOfAmenities() * 5);
        int wait = 0;
        if (hotel.GetTempCost() == 0)
        {
            while (hotel.GetPrice() != new_price)
            {

                if (hotel.GetPrice() < new_price)
                {
                    game_manager.ChangeHotelPrice(true);
                }
                else
                {
                    game_manager.ChangeHotelPrice(false);
                }
                wait++;
                if (wait > 48)
                {
                    yield return new WaitForSeconds(0.05f);
                    wait = 0;
                }

            }
        }

        print("AI sets their price (or tries to)");

        yield return new WaitForSeconds(1.0f);
        print("AI wants to end their turn");
        game_manager.NextTurn();
            
    }

    public void BecomeHuman()
    {
        ai = false;
    }
    public void BecomeAI()
    {
        ai = true;
    }
    public void ResetGame()
    {
        money = DEFAULT_MONEY;
        cards_in_hand.Clear();
        card_in_deck = 0;
        ResetDeck();
        for(int i = 0; i < STARTING_CARDS_IN_HAND; i++)
        {
            DrawCard();
        }
    }
    public void ResetDeck()
    {
        ShuffleArray(deck);
    }

    void ShuffleArray<T>(T[] array)
    {
        System.Random random = new();
        for (int i = array.Length - 1; i > 0; i--)
        {
            int j = random.Next(i + 1);
            T temp = array[i];
            array[i] = array[j];
            array[j] = temp;
        }
    }

    public bool DrawCard()
    {
        if(cards_in_hand.Count >= MAX_CARDS_IN_HAND || card_in_deck >= deck.Length)
        {
            print("No draw");
            return false;
        }

        //print("DRAW!");
        cards_in_hand.Add(deck[card_in_deck]);
        card_in_deck++;
        return true;
                
    }

    public void AddCard(Card new_card)
    {
        cards_in_hand.Add(new_card);
    }

    public void ChangeMoney(float change)
    {
        money += change;
    }
    public void SetPlacing(int p)
    {
        placing = p;
    }
    public void SetTurn(bool t)
    {
        isTurn = t;
        if (!t)
        {
            startTurn = true;
        }
    }
    public Card[] GetDeck()
    {
        return deck;
    }
    public List<Card> GetCardsInHand()
    {
        return cards_in_hand;
    }
    public Hotel GetHotel()
    {
        return hotel;
    }
    public float GetMoney()
    {
        return money;
    }
    public int GetPlacing()
    {
        return placing;
    }
    public bool IsTurn()
    {
        return isTurn;
    }
    public bool IsAI()
    {
        return ai;
    }
}
