using System.Collections.Generic;
using UnityEngine;

public class Guest : MonoBehaviour
{

    [SerializeField] private GameManagement game_manager;
    [SerializeField] private Hotel curr_hotel = null;
    [SerializeField] private float min_rating, max_pay;
    //How low of rated hotel will this guest go to
    //How much is that guest willing to pay
    [SerializeField] private List<Amenity> preferred_amenities; //duplicates can be involved
    //[SerializeField] private List<Hotel> hotels_to_consider = new();
    //[SerializeField] private List<int> points_for_each_hotel = new();

    [SerializeField] private Dictionary<Hotel, int> hotels_to_consider = new();

    [SerializeField] private readonly int MIN_ACCEPTABLE_RATING = 1;
    [SerializeField] private readonly int MAX_ACCEPTABLE_RATING = 5;
    [SerializeField] private readonly int MIN_WILLING_TO_PAY = 50;
    [SerializeField] private readonly int MAX_WILLING_TO_PAY = 1000;

    private int pts_req;

    public void DecideStandards()
    {
        int x = Random.Range(1, 101);
        if(x < 5)
        {
            min_rating = Random.Range(MIN_ACCEPTABLE_RATING, MAX_ACCEPTABLE_RATING + 1);
        }
        else if(x < 40)
        {
            min_rating = Random.Range(2, MAX_ACCEPTABLE_RATING + 1);
        }
        else
        {
            min_rating = Random.Range(3, MAX_ACCEPTABLE_RATING + 1);
        }
        if (min_rating < 5)
        {
            min_rating += 0.1f * Random.Range(0, 10); //1 - 10, in increments of 0.1, are all possible
                                                      //However it's more likely that customers will want to be at a higher rated hotel
        }

        x = Random.Range(1, 101);
        if (x < 85)
        {
            max_pay = Random.Range(MIN_WILLING_TO_PAY, MAX_WILLING_TO_PAY / 2);
        }
        else
        {
            max_pay = Random.Range(MIN_WILLING_TO_PAY, MAX_WILLING_TO_PAY);
        }

        for(int i = 0; i < game_manager.GetAllAmenities().Length; i++)
        {
            x = Random.Range(0, 2);
            if(x == 0)
            {
                preferred_amenities.Add(game_manager.GetAllAmenities()[i]);
            }
        }

        pts_req = Random.Range(10, 20);

        DecideHotel();
    }

    public void DecideHotel()
    {

        List<Hotel> all_hotels = new();
        //List<int> pts = new();
        for(int i = 0; i < game_manager.GetPlayersInGame().Count; i++)
        {
            all_hotels.Add(game_manager.GetPlayersInGame()[i].GetHotel());
            //points_for_each_hotel.Add(0);

            int pts = 0;
            if (game_manager.GetPlayersInGame()[i].GetHotel().GetPrice() <= max_pay)
            {
                pts += 10;
            }
            if (game_manager.GetPlayersInGame()[i].GetHotel().GetRating() >= min_rating)
            {
                pts += 10;
            }
            for (int z = 0; z < preferred_amenities.Count; z++)
            {
                if (game_manager.GetPlayersInGame()[i].GetHotel().GetAmenities().Contains(preferred_amenities[z]))
                {
                    //print("This hotel has an amenity I like!");
                    pts += 1;
                }
            }
            hotels_to_consider[game_manager.GetPlayersInGame()[i].GetHotel()] = pts;
        }


        List<Hotel> final_hotels = new();
        for(int i = 0; i < hotels_to_consider.Count; i++)
        {
            bool highest = true;
            for (int z = 0; z < hotels_to_consider.Count; z++)
            {
                if (hotels_to_consider[all_hotels[i]] < hotels_to_consider[all_hotels[z]])
                {
                    highest = false;
                    break;
                }
            }
            if (highest && hotels_to_consider[all_hotels[i]] > pts_req) //It has at least 10 points
            {
                final_hotels.Add(all_hotels[i]);
            }
        }

        if(final_hotels.Count > 1)
        {
            int decide = Random.Range(0, 2);
            //0 --> decides by ratings
            //1 --> decides by pricing
            
            if(decide == 0)
            {
                //print("I like ratings");

                final_hotels.Sort(new RatingComparer());
                final_hotels.Reverse();
                for(int i = 1; i == final_hotels.Count; i++)
                {
                    if (final_hotels[i].GetRating() < final_hotels[0].GetRating())
                    {
                        final_hotels.RemoveAt(i);
                    }
                }
            }
            else
            {
                //print("I'm cheap");
                final_hotels.Sort(new PriceComparer());
                for (int i = 1; i == final_hotels.Count; i++)
                {
                    if (final_hotels[i].GetPrice() > final_hotels[0].GetPrice())
                    {
                        final_hotels.RemoveAt(i);
                    }
                }
            }
        } 
        
        if(final_hotels.Count == 1)
        {
            //print("Hotel chosen");
            EnterHotel(final_hotels[0]);
        }
        else if(final_hotels.Count > 1)
        {
            //print("Choose hotel randomly");
            EnterHotel(final_hotels[Random.Range(0, final_hotels.Count)]);
        }
        else
        {
            print("There's no hotel for me :(");
        }


    }
    public void EnterHotel(Hotel hotel)
    {
        curr_hotel = hotel;
        hotel.AddGuest(this);
    }
    public void CheckHotelAgain()
    {
        int pts = 0;
        if (curr_hotel.GetPrice() <= max_pay)
        {
            pts += 10;
        }
        if (curr_hotel.GetRating() >= min_rating)
        {
            pts += 10;
        }
        for (int z = 0; z < preferred_amenities.Count; z++)
        {
            if (curr_hotel.GetAmenities().Contains(preferred_amenities[z]))
            {
                //print("This hotel has an amenity I like!");
                pts += 1;
            }
        }
        if(pts < pts_req)
        {
            LeaveHotel();
        }
    }
    public void LeaveHotel()
    {
        print("I'M LEAVING!");
        curr_hotel.RemoveGuest(this);
        curr_hotel = null;
    }


    /*public void TurnOver()
    {
        print("Run");
        if(curr_hotel == null)
        {
            //print("Guest destroyed");
            game_manager.RemoveGuest(this);
            Destroy(gameObject);
        }
    }*/
    public Hotel GetCurrHotel()
    {
        return curr_hotel;
    }
    
}

public class RatingComparer: IComparer<Hotel>
{
    public int Compare(Hotel x, Hotel y)
    {
        return x.GetRating().CompareTo(y.GetRating());
    }
}

public class PriceComparer : IComparer<Hotel>
{
    public int Compare(Hotel x, Hotel y)
    {
        return x.GetPrice().CompareTo(y.GetPrice());
    }
}