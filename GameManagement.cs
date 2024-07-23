using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class GameManagement : MonoBehaviour
{
    [SerializeField] private Image white_screen;
    [SerializeField] private GameObject title_screen, game_options;
    [SerializeField] private Player[] players;
    [SerializeField] private List<Player> players_in_game, player_placings;
    [SerializeField] private Amenity[] all_amenities;
    [SerializeField] private GameObject[] player_scores, target_buttons, resorts, card_holders;
    [SerializeField] private GameObject price_changer, shop, end_turn, reset_game, guest;
    [SerializeField] private int curr_turn, curr_round = 0;
    [SerializeField] private int rounds = 20;
    [SerializeField] private bool select_screen, game_active = false;
    [SerializeField] private TMP_Text hotel_price, num_of_rounds, num_of_guests, num_of_amenities, amenity_list, curr_player_turn, shop_error, rating_shown, capacity_shown, curr_hotel, game_info;
    [SerializeField] private TMP_Text card_description;
    [SerializeField] private Camera hotel_cam;
    [SerializeField] private List<Guest> all_guests;
    [SerializeField] private GameObject info_and_buttons, invisible_screen;

    [SerializeField] private Player target, curr_perspective;
    [SerializeField] private string card_in_play;

    [SerializeField] private GameObject final_results_screen;
    [SerializeField] private TMP_Text final_results_txt;

    bool turn_start = false;
    //player_info_shown shows:
    //How many cards a player has in their hand (not the actual cards themselves if it's not their turn)
    //The amenities of their hotel
    //The price of their hotel
    //Switches the camera

    public void LeaveTitleScreen()
    {
        StartCoroutine(GoToMainScreen());
    }
    IEnumerator GoToMainScreen()
    {
        white_screen.gameObject.SetActive(true);
        float a = 0;
        while(a < 1)
        {
            white_screen.color = new Vector4(1, 1, 1, a);
            yield return new WaitForSeconds(0.005f);
            a += 0.01f;
            //print("1st loop: " + a);
        }
        title_screen.SetActive(false);
        yield return new WaitForSeconds(1.0f);
        while (a > 0)
        {
            white_screen.color = new Vector4(1, 1, 1, a);
            yield return new WaitForSeconds(0.005f);
            a -= 0.01f;
            //print("2nd loop: " + a);
        }
        white_screen.gameObject.SetActive(false);
        game_options.SetActive(true);
        float spd = 1f;
        while (game_options.transform.position.x < 1280)
        {
            game_options.transform.position += new Vector3(spd, 0, 0);
            yield return new WaitForSeconds(0.001f);
            spd += 1f;
        }
        game_options.transform.position = new Vector3(1280, 720, 0);
    }
    public void SelectScreenActive(bool a)
    {
        select_screen = a;
    }
    public bool CharacterSelectionActive()
    {
        return select_screen;
    }

    public void StartGame()
    {

        //This can also be used to reset games
        game_info.text = "";
       
        final_results_screen.SetActive(false);
        if (rounds < 1)
        {
            rounds = 5;
        }
        hotel_cam.enabled = true;
        players_in_game.Clear();
        player_placings.Clear();
        for(int i = 0; i < player_scores.Length; i++)
        {
            player_scores[i].SetActive(false);
        }

        for(int i = 0; i < all_guests.Count; i++)
        {
            Destroy(all_guests[i]);
        }
        all_guests.Clear();

        resorts[2].SetActive(false);
        resorts[3].SetActive(false);
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].gameObject.activeSelf)
            {
                players_in_game.Add(players[i]);
                player_placings.Add(players[i]);
                resorts[i].SetActive(true);
                players[i].ResetGame();
                players[i].GetHotel().ResetGame();
                players[i].GetHotel().AddAmenity(all_amenities[0]);
                players[i].GetHotel().AddAmenity(all_amenities[3]);
                players[i].GetHotel().AddAmenity(all_amenities[5]);
                players[i].GetHotel().AddAmenity(all_amenities[13]);
                players[i].GetHotel().AddAmenity(all_amenities[15]);
            }
        }

        print("There are " + players_in_game.Count + " players in the game");

        game_active = true;
        curr_round = 1;
        num_of_rounds.text = "1/" + rounds.ToString();

        ChangeScoreOrder();
        StartRound();
        
    }
    public void StartRound()
    {
        invisible_screen.SetActive(true);
        curr_turn = 0;
        turn_start = true;
        ChangePerspective(curr_turn, true);
        //players_in_game[curr_turn].SetTurn(true);

        StartCoroutine(PlaceCards(players_in_game[curr_turn], true));
        
        //Show their cards
    }
    public void NextTurn()
    {
        //ChangeScoreOrder(); //I don't think this is needed 7/20/2024 8:14 P.M.
        StartCoroutine(GoToNextTurn());
    }

    IEnumerator GoToNextTurn()
    {

        for (int z = 0; z < players_in_game[curr_turn].GetCardsInHand().Count; z++)
        {
            print("Set previous card inactive of player " + players_in_game[curr_turn].name + " - " + players_in_game[curr_turn].GetCardsInHand()[z].transform.parent.name);
            players_in_game[curr_turn].GetCardsInHand()[z].transform.parent.gameObject.SetActive(false);
        }
        for (int y = 0; y < card_holders.Length; y++)
        {
            card_holders[y].SetActive(false);
        }

        players_in_game[curr_turn].SetTurn(false);
        curr_turn++;
        
        invisible_screen.SetActive(true);
        info_and_buttons.SetActive(false);

        if (curr_turn < players_in_game.Count)
        {

            game_info.transform.position = new Vector3(-3500, 720, 0);
            game_info.text = "NEXT TURN";
            int wait = 0;
            while (game_info.transform.position.x < 3500)
            {
                game_info.transform.position += new Vector3(1, 0, 0);
                wait++;
                if (wait > 24)
                {
                    yield return new WaitForSeconds(0.005f);
                    wait = 0;
                }
            }

            turn_start = true; //This will prevent ChangePerspective from calling an unnecessary second time to PlaceCards
            ChangePerspective(curr_turn, true);

            StartCoroutine(PlaceCards(players_in_game[curr_turn], true));

            //curr_player_turn.text = players_in_game[curr_turn].name;
            //ChangePerspective(curr_turn, true);
            //info_and_buttons.SetActive(true);
        }
        else
        {
            curr_round++;
            if (curr_round > rounds)
            {
                game_active = false;
                
                game_info.text = "Incoming guests...";
                game_info.transform.position = new Vector3(1280, 720, 0);
                StartCoroutine(SpawnGuests());
                print("GameOver!");
            }
            else
            {
                //info_and_buttons.SetActive(false);
                game_info.text = "Incoming guests...";
                game_info.transform.position = new Vector3(1280, 720, 0);
                StartCoroutine(SpawnGuests());

            }

        }
    }

    IEnumerator SpawnGuests()
    {

        for (int z = 0; z < all_guests.Count; z++)
        {
            all_guests[z].CheckHotelAgain();
        }

        for (int i = 0; i < 50; i++) //50 guests spawn at the end of each round
        {
            GameObject new_guest = Instantiate(guest, Vector3.zero, Quaternion.identity);
            new_guest.GetComponent<Guest>().DecideStandards();
            all_guests.Add(new_guest.GetComponent<Guest>());
        }

        print("Guests spawned");
        
        yield return new WaitForSeconds(3.0f); //Instead of 5 seconds, probably something to check that each new guest made a decision

        game_info.text = "Money awarded!";
        print("Guests spawn complete");
        List<Guest> temp = new();
        List<Guest> temp_two = new();
        for (int z = 0; z < all_guests.Count; z++)
        {
            if (all_guests[z].GetCurrHotel() != null)
            {
                temp.Add(all_guests[z]);
            }
            else
            {
                temp_two.Add(all_guests[z]);
            }
        }
        all_guests.Clear();
        for(int w = 0; w < temp_two.Count; w++)
        {
            Destroy(temp_two[w].gameObject);
        }
        temp_two.Clear();
        for (int j = 0; j < temp.Count; j++)
        {
            all_guests.Add(temp[j]);
        }

        yield return new WaitForSeconds(2.0f);

        //Spawn money for each guest
        for (int y = 0; y < players_in_game.Count; y++)
        {
            players_in_game[y].ChangeMoney(players_in_game[y].GetHotel().GetNumOfGuests() * players_in_game[y].GetHotel().GetPrice());
            players_in_game[y].GetHotel().ChangeTempCost(0, true);
            players_in_game[y].GetHotel().ChangeTempRating(0, true);
        }

        print("Money awarded");
        yield return new WaitForSeconds(1.0f);

        if (game_active)
        {
            game_info.transform.position = new Vector3(-3500, 720, 0);
            game_info.text = "NEXT TURN";
            int wait = 0;
            while (game_info.transform.position.x < 3500)
            {
                game_info.transform.position += new Vector3(1, 0, 0);
                wait++;
                if (wait > 24)
                {
                    yield return new WaitForSeconds(0.005f);
                    wait = 0;
                }
            }

            //Also I need to make the screens disappear for calculations

            num_of_rounds.text = curr_round.ToString() + "/" + rounds.ToString();
            ChangeScoreOrder();
            StartRound();
        }
        else
        {
            ChangeScoreOrder();
            game_info.text = "GAMEOVER";
            final_results_txt.text = "";
            for(int i = 0; i < player_placings.Count; i++)
            {
                switch (player_placings[i].GetPlacing())
                {
                    case 1:
                        final_results_txt.text += "<color=green>";
                        break;
                    case 2:
                        final_results_txt.text += "<color=yellow>";
                        break;
                    case 3:
                        final_results_txt.text += "<color=orange>";
                        break;
                    case 4:
                        final_results_txt.text += "<color=red>";
                        break;
                    default:
                        print("Something's wrong");
                        break;
                }
                final_results_txt.text += player_placings[i].GetPlacing() + " Place: " + player_placings[i].name + " - " + player_placings[i].GetMoney()+ "\n";
            }
            final_results_screen.SetActive(true);
            game_info.transform.position = new Vector3(1280, 1080, 0);

            //Show the final results,
        }

    }

    public void CardActivated(string card_name)
    {
        bool card_successful = false;
        card_in_play = card_name;
        switch (card_name)
        {
            case "AmusementPark":
                print("AmusementPark");
                players_in_game[curr_turn].GetHotel().AddAmenity(all_amenities[1]);
                num_of_amenities.text = "Amenities: " + players_in_game[curr_turn].GetHotel().GetNumOfAmenities();
                rating_shown.text = "Rating: " + players_in_game[curr_turn].GetHotel().GetRating().ToString("F1");
                card_successful = true;
                break;
            case "BadReviews":
                print("BadReviews");
                if (players_in_game[curr_turn].GetMoney() > 2000) {
                    if (target != null) {
                        players_in_game[curr_turn].GetMoney();
                        target.GetHotel().ChangeTempRating(-2.0f);
                        card_successful = true;
                    }
                    else
                    {
                        ChooseTarget();
                    }
                }
                else
                {
                    print("This card cannot be activated");
                }

            break;
            case "Beach":
                print("Beach");
                players_in_game[curr_turn].GetHotel().AddAmenity(all_amenities[4]);
                num_of_amenities.text = "Amenities: " + players_in_game[curr_turn].GetHotel().GetNumOfAmenities();
                rating_shown.text = "Rating: " + players_in_game[curr_turn].GetHotel().GetRating().ToString("F1");
                card_successful = true;
                break;
            case "BedBugs":
                print("BedBugs");
                if (target != null) 
                {
                    target.GetHotel().RemoveAmenity(all_amenities[5].name);
                    card_successful = true;
                }
                else
                {
                    ChooseTarget();
                } 
                break;
            case "CutWires":
                print("CutWires");
                if (target != null)
                {
                    target.GetHotel().RemoveAmenity(all_amenities[15].name); //Right now this card is bad unless you go last
                    card_successful = true;
                }
                else
                {
                    ChooseTarget();
                }
                break;
            case "Expenses":
                print("Expenses");
                if (target != null)
                {
                    target.ChangeMoney(-target.GetHotel().GetNumOfGuests() * 50);
                    //target.GetHotel().RemoveAmenity(all_amenities[15].name); //Right now this card is bad unless you go last
                    card_successful = true;
                }
                else
                {
                    ChooseTarget();
                }
                break;
            case "FIRE":
                print("FIRE");
                if (target != null)
                {
                    for(int i = 0; i < 3; i++)
                    {
                        int num = target.GetHotel().GetNumOfAmenities();
                        if (num > 0)
                        {
                            target.GetHotel().RemoveAmenity(target.GetHotel().GetAmenities()[Random.Range(0, num)].name);
                        }
                        else
                        {
                            break;
                        }
                    }
                    card_successful = true;
                }
                else
                {
                    ChooseTarget();
                }
                break;
            case "FreeParking":
                print("Free Parking");
                players_in_game[curr_turn].GetHotel().AddAmenity(all_amenities[9]);
                num_of_amenities.text = "Amenities: " + players_in_game[curr_turn].GetHotel().GetNumOfAmenities();
                rating_shown.text = "Rating: " + players_in_game[curr_turn].GetHotel().GetRating().ToString("F1");
                card_successful = true;
                break;
            case "GetFit":
                print("GetFit");
                players_in_game[curr_turn].GetHotel().AddAmenity(all_amenities[8]);
                num_of_amenities.text = "Amenities: " + players_in_game[curr_turn].GetHotel().GetNumOfAmenities();
                rating_shown.text = "Rating: " + players_in_game[curr_turn].GetHotel().GetRating().ToString("F1");
                card_successful = true;
                break;
            case "GreatReviews":
                print("GreatReviews");
                players_in_game[curr_turn].GetHotel().ChangeTempRating(2.0f);
                card_successful = true;
                break;
            case "ExtraTips":
                print("ExtraTips");
                players_in_game[curr_turn].ChangeMoney(players_in_game[curr_turn].GetHotel().GetNumOfGuests() * 25);
                card_successful = true;
                break;
            case "PizzaShop":
                print("PizzaShop");
                players_in_game[curr_turn].GetHotel().AddAmenity(all_amenities[11]);
                num_of_amenities.text = "Amenities: " + players_in_game[curr_turn].GetHotel().GetNumOfAmenities();
                rating_shown.text = "Rating: " + players_in_game[curr_turn].GetHotel().GetRating().ToString("F1");
                card_successful = true;
                break;
            case "Pool":
                print("Pool");
                players_in_game[curr_turn].GetHotel().AddAmenity(all_amenities[10]);
                num_of_amenities.text = "Amenities: " + players_in_game[curr_turn].GetHotel().GetNumOfAmenities();
                rating_shown.text = "Rating: " + players_in_game[curr_turn].GetHotel().GetRating().ToString("F1");
                card_successful = true;
                break;
            case "PriceDown":
                print("PriceDown");
                if (target != null)
                {
                    target.GetHotel().ChangeTempCost(-1000);
                    card_successful = true;
                }
                else
                {
                    ChooseTarget();
                }
                break;
            case "PriceUp":
                print("PriceUp");
                if (target != null)
                {
                    target.GetHotel().ChangeTempCost(500);
                    card_successful = true;
                }
                else
                {
                    ChooseTarget();
                }
                break;
            case "Robbery":
                print("Robbery");
                if (target != null)
                {
                    int x = Random.Range(0, target.GetCardsInHand().Count);
                    Card stolen_card = target.GetCardsInHand()[x];
                    print("Card being stolen! - " + stolen_card.transform.parent.name);
                    target.GetCardsInHand().RemoveAt(x);
                    for (int i = 0; i < players_in_game[curr_turn].GetCardsInHand().Count; i++)
                    {
                        if (players_in_game[curr_turn].GetCardsInHand()[i].transform.parent.name.Contains(card_name))
                        {
                            players_in_game[curr_turn].GetCardsInHand()[i].Shrink();
                            players_in_game[curr_turn].GetCardsInHand()[i].transform.parent.gameObject.SetActive(false);
                            players_in_game[curr_turn].GetCardsInHand().RemoveAt(i);
                            break;
                        }
                    }
                    players_in_game[curr_turn].AddCard(stolen_card);
                    StartCoroutine(PlaceCards(players_in_game[curr_turn], false));
                    //card_successful = true;
                }
                else
                {
                    ChooseTarget();
                }
                break;
            case "WackyGift":
                print("WackyGift");
                for (int i = 0; i < players_in_game[curr_turn].GetCardsInHand().Count; i++)
                {
                    if (players_in_game[curr_turn].GetCardsInHand()[i].transform.parent.name.Contains(card_name))
                    {
                        players_in_game[curr_turn].GetCardsInHand()[i].Shrink();
                        players_in_game[curr_turn].GetCardsInHand()[i].transform.parent.gameObject.SetActive(false);
                        players_in_game[curr_turn].GetCardsInHand().RemoveAt(i);
                        break;
                    }
                }

                players_in_game[curr_turn].DrawCard();
                players_in_game[curr_turn].DrawCard();

                StartCoroutine(PlaceCards(players_in_game[curr_turn], false));
                break;
            default:
                print("Something's wrong - " + card_name);
                break;
        }


        if (card_successful)
        {
            //print("Test1");
            for(int i = 0; i < players_in_game[curr_turn].GetCardsInHand().Count; i++)
            {
                if (players_in_game[curr_turn].GetCardsInHand()[i].transform.parent.name.Contains(card_name))
                {
                    players_in_game[curr_turn].GetCardsInHand()[i].Shrink();
                    players_in_game[curr_turn].GetCardsInHand()[i].transform.parent.gameObject.SetActive(false);
                    players_in_game[curr_turn].GetCardsInHand().RemoveAt(i);  
                    break;
                }
            }
            //print("Test2");
            //StartCoroutine(PlaceCards(players_in_game[curr_turn], false));
            ChangeScoreOrder();
            ChangePerspective(curr_turn, true);
            
        }
        target = null;
    }

    public void RemoveGuest(Guest guest)
    {
        all_guests.Remove(guest);
    }
    public void ChangeRound(TMP_InputField field) //Instead of an input field, I should probably change this to just be like the price changer, but that's later
    {
        //also, try and catch requires Unity.System, but if I use that it's harder to use Random.Range
        if(field.text != "" && field.text != "-")
        {
            rounds = int.Parse(field.text);
        }

    }
    public void ChangeScoreOrder()
    {
        //Everyone's placing is 1st
        //0 --> 4th
        //1 - 2 (top to bottom) --> 3rd
        //3 - 5 (top to bottom) --> 2nd
        //6 - 9 (top to bottom) --> 1st
        //print("ChangeScoreOrder");
        player_placings.Sort(new MoneyComparer());
        player_placings.Reverse();
        for(int y = 0; y < player_scores.Length; y++)
        {
            player_scores[y].SetActive(false);
        }

        player_placings[0].SetPlacing(1);
        player_scores[6].SetActive(true);
        player_scores[6].transform.GetChild(0).transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = player_placings[0].name;
        player_scores[6].transform.GetChild(0).transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "$" + player_placings[0].GetMoney().ToString("F2");
        
        for (int x = 0; x < 5; x++)
        {
            player_scores[6].transform.GetChild(0).transform.GetChild(2).transform.GetChild(x).GetComponent<Image>().fillAmount = player_placings[0].GetHotel().GetRating() - x ;
        }

        for (int i = 1; i < player_placings.Count; i++)
        {
            if (player_placings[i].GetMoney() == player_placings[i - 1].GetMoney())
            {
                player_placings[i].SetPlacing(player_placings[i - 1].GetPlacing());
            }
            else
            {
                player_placings[i].SetPlacing(i + 1);
            }

            int z;
            if (player_placings[i].GetPlacing() == 1)
            {
                z = 6 + i;
            }
            else if (player_placings[i].GetPlacing() == 2)
            {
                z = 2 + i;
            }
            else if (player_placings[i].GetPlacing() == 3)
            {
                z = i - 1;
            }
            else
            {
                z = 0;
            }

            player_scores[z].SetActive(true);
            player_scores[z].transform.GetChild(0).transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = player_placings[i].name;
            player_scores[z].transform.GetChild(0).transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "$" + player_placings[i].GetMoney().ToString("F2");
            for (int x = 0; x < 5; x++)
            {
                player_scores[z].transform.GetChild(0).transform.GetChild(2).transform.GetChild(x).GetComponent<Image>().fillAmount = player_placings[i].GetHotel().GetRating() - x;
            }
        }

    }

    public IEnumerator PlaceCards(Player player, bool start_of_turn)
    {

        //print(player.name);
        //print(start_of_turn);
        turn_start = false;
        print("PlaceCards");
        //This code doesn't set the player's currents cards inactive before looking, 
        for(int z = 0; z < card_holders.Length; z++)
        {
            card_holders[z].SetActive(false);
        }

        if (start_of_turn)
        {
            players_in_game[curr_turn].DrawCard();

            for (int i = 0; i < player.GetCardsInHand().Count; i++)
            {
                //print("Run : " + player.GetCardsInHand()[i].transform.parent.transform.position + "\n" + card_holders[i].transform.position);
                player.GetCardsInHand()[i].transform.parent.transform.position = card_holders[i].transform.position + new Vector3(0, 2000, 0);
                player.GetCardsInHand()[i].transform.parent.gameObject.SetActive(true);
                
                int wait = 0;
                //print("Card pos y: " + player.GetCardsInHand()[i].transform.parent.transform.position.y);
                //print("Card holder pos y: " + card_holders[i].transform.position.y);
                while (player.GetCardsInHand()[i].transform.parent.transform.position.y > card_holders[i].transform.position.y)
                {
                    //print("Run 1");
                    player.GetCardsInHand()[i].transform.parent.transform.position -= new Vector3(0, 1, 0);
                    wait++;
                    if (wait > 84)
                    {
                        yield return new WaitForSeconds(0.01f);
                        wait = 0;
                    }
                }
                //print("Run 2");
            }
            yield return new WaitForSeconds(0.1f);
            print("Cards were placed properly");
            info_and_buttons.SetActive(true);
            curr_player_turn.text = players_in_game[curr_turn].name;
            if (!players_in_game[curr_turn].IsAI())
            {
                invisible_screen.SetActive(false);
            }
            players_in_game[curr_turn].SetTurn(true);
                 
        }
        else
        {

            if(player != players_in_game[curr_turn]) //No peaking at other player's hands
            {
                for (int y = 0; y < player.GetCardsInHand().Count; y++)
                {
                    card_holders[y].SetActive(true);
                }
            }
            else
            {
                for (int y = 0; y < player.GetCardsInHand().Count; y++)
                {
                    player.GetCardsInHand()[y].transform.parent.gameObject.SetActive(true);
                    player.GetCardsInHand()[y].transform.parent.transform.position = card_holders[y].transform.position;
                }
            }

        }
    }

    public void ChangePButtonCall(int x) {
        ChangePerspective(x);
    }
    public void ChangePerspective(int x, bool stats_change = false)
    {
        //player_placings[x]
        amenity_list.transform.parent.gameObject.SetActive(false);


        if(curr_perspective != null)
        {
            for(int i = 0; i < curr_perspective.GetCardsInHand().Count; i++)
            {
                curr_perspective.GetCardsInHand()[i].transform.parent.gameObject.SetActive(false);
            }
        }
        else
        {
            for (int i = 0; i < players_in_game[0].GetCardsInHand().Count; i++)
            {
                players_in_game[0].GetCardsInHand()[i].transform.parent.gameObject.SetActive(false);
            }
        }

        if (!stats_change)
        {
            //print("Show me " + player_placings[x].name + "'s hotel");
            curr_perspective = player_placings[x];
            hotel_price.text = "$" + player_placings[x].GetHotel().GetPrice().ToString("F2");
            num_of_amenities.text = "Amenities: " + player_placings[x].GetHotel().GetNumOfAmenities();
            num_of_guests.text = "Guests: " + player_placings[x].GetHotel().GetNumOfGuests();
            //print("This hotel has " + player_placings[x].GetHotel().GetNumOfGuests());
            capacity_shown.text = "Capacity: " + player_placings[x].GetHotel().GetCapacity();
            rating_shown.text = "Rating: " + player_placings[x].GetHotel().GetRating().ToString("F1");
            curr_hotel.text = player_placings[x].name + "'s hotel";
            if (!turn_start)
            {
                StartCoroutine(PlaceCards(player_placings[x], false));
            }
            //amenity_list.transform.parent.gameObject.SetActive(!amenity_list.transform.parent.gameObject.activeSelf);
            //amenity_list.text = "";
        }
        else
        {
            //When the price changes, I'm sending the current turn, and the current turn order will not always match the player_placings order
            //print("Show me " + players_in_game[x].name + "'s hotel");
            curr_perspective = players_in_game[x];
            hotel_price.text = "$" + players_in_game[x].GetHotel().GetPrice().ToString("F2");
            num_of_amenities.text = "Amenities: " + players_in_game[x].GetHotel().GetNumOfAmenities();
            num_of_guests.text = "Guests: " + players_in_game[x].GetHotel().GetNumOfGuests();
            //print("This hotel has " + players_in_game[x].GetHotel().GetNumOfGuests());
            capacity_shown.text = "Capacity: " + players_in_game[x].GetHotel().GetCapacity();
            rating_shown.text = "Rating: " + players_in_game[x].GetHotel().GetRating().ToString("F1");
            curr_hotel.text = players_in_game[x].name + "'s hotel";
            if (!turn_start)
            {
                StartCoroutine(PlaceCards(players_in_game[x], false));
            }
            //amenity_list.transform.parent.gameObject.SetActive(!amenity_list.transform.parent.gameObject.activeSelf);
            //amenity_list.text = "";
        }

    }

    public void ShowAmenityList()
    {
        //Change later
        print("Show Amenity List");
        amenity_list.transform.parent.gameObject.SetActive(!amenity_list.transform.parent.gameObject.activeSelf);
        amenity_list.transform.parent.GetComponent<RectTransform>().sizeDelta = new Vector2(700, curr_perspective.GetHotel().GetAmenities().Count * 100f);
        //amenity_list.gameObject.SetActive(!amenity_list.gameObject.activeSelf);
        amenity_list.text = "";
        for(int i = 0; i < curr_perspective.GetHotel().GetAmenities().Count; i++)
        {
            for (int y = 0; y < curr_perspective.GetHotel().GetNumOfAmenityType()[i]; y++)
            {
                amenity_list.text += curr_perspective.GetHotel().GetAmenities()[i].name + "\n";
            }
        }
    }

    public void BuyAmenity(Amenity amenity)
    {
        if (players_in_game[curr_turn].GetMoney() >= amenity.GetPrice())
        {
            shop_error.gameObject.SetActive(false);
            players_in_game[curr_turn].ChangeMoney(-amenity.GetPrice());
            players_in_game[curr_turn].GetHotel().AddAmenity(amenity);
            ChangeScoreOrder();
            //Play some sound effect here
            
            num_of_amenities.text = "Amenities: " + players_in_game[curr_turn].GetHotel().GetNumOfAmenities();
            rating_shown.text = "Rating: " + players_in_game[curr_turn].GetHotel().GetRating().ToString("F1");
        }
        else
        {
            shop_error.gameObject.SetActive(true);
            shop_error.text = "You do not have enough money to buy this!";
        }
        
    }
    public void BuyCardDraw()
    {
        if (players_in_game[curr_turn].GetMoney() >= 500)
        {
            shop_error.gameObject.SetActive(false);
            players_in_game[curr_turn].ChangeMoney(-500);
            ChangeScoreOrder();
            players_in_game[curr_turn].DrawCard();
        }
        else
        {
            shop_error.gameObject.SetActive(true);
            shop_error.text = "You do not have enough money to buy this!";
        }
    }
    public void IncreaseCapacity()
    {
        if (players_in_game[curr_turn].GetMoney() >= 500)
        {
            shop_error.gameObject.SetActive(false);
            players_in_game[curr_turn].ChangeMoney(-500);
            ChangeScoreOrder();
            players_in_game[curr_turn].GetHotel().ChangeCapacity(50);
            capacity_shown.text = "Capacity: " + players_in_game[curr_turn].GetHotel().GetCapacity();
        }
        else
        {
            shop_error.gameObject.SetActive(true);
            shop_error.text = "You do not have enough money to buy this!";
        }
    }
    public void ChangeHotelPrice(bool inc)
    {
        if (inc)
        {
            players_in_game[curr_turn].GetHotel().ChangePrice(1);
        }
        else
        {
            players_in_game[curr_turn].GetHotel().ChangePrice(-1);
        }
        ChangePerspective(curr_turn, true);
    }

    public bool GameActive()
    {
        return game_active;
    }

    public List<Player> GetPlayersInGame()
    {
        return players_in_game;
    }

    public Amenity[] GetAllAmenities()
    {
        return all_amenities;
    }

    public void ChooseTarget()
    {

        print("Choose target");

        price_changer.SetActive(false);
        end_turn.SetActive(false);
        shop.SetActive(false);

        for (int z = 0; z < players_in_game[curr_turn].GetCardsInHand().Count; z++)
        {
            players_in_game[curr_turn].GetCardsInHand()[z].transform.parent.gameObject.SetActive(false);
        }

        for (int i = 0; i < target_buttons.Length; i++)
        {
            if (player_scores[i].activeSelf)
            {
                target_buttons[i].SetActive(true);
            }
        }
        game_info.fontSize = 144;
        game_info.text = "Choose a target -->";
        game_info.transform.position = new Vector3(1280, 1080, 0);
    }

    public void TargetSelected(int x)
    {
        price_changer.SetActive(true);
        end_turn.SetActive(true);
        shop.SetActive(true);

        for (int i = 0; i < target_buttons.Length; i++)
        {
            target_buttons[i].SetActive(false);
            
        }
        game_info.fontSize = 204;
        game_info.transform.position = new Vector3(-3000, 720, 0);
        //print(x);
        target = player_placings[x];
        //print("Target chosen - " + target.name);
        CardActivated(card_in_play);
        
    }

    public TMP_Text GetCardInfo()
    {
        return card_description;
    }

}

public class MoneyComparer: IComparer<Player>
/*This is a custom comparer, this is VERY important for when I order a list in my own way
This compares the class placing, and orders them by their money*/
{
    public int Compare(Player x, Player y)
    {
        return x.GetMoney().CompareTo(y.GetMoney());
    }
}