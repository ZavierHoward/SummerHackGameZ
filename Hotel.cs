using System.Collections.Generic;
using UnityEngine;

public class Hotel : MonoBehaviour
{
    private readonly float MIN_RATING = 0.0f;
    private readonly float MAX_RATING = 5.0f;
    private readonly int MIN_CAPACITY = 50;
    private readonly int MAX_CAPACITY = 10000;
    private readonly float MIN_PRICE = 10;
    private readonly float MAX_PRICE = 1000;

    [SerializeField] private float temp_rating_change = 0.0f;
    [SerializeField] private float temp_cost = 0.0f;

    //[SerializeField] private GameObject building;
    [SerializeField] private int capacity;
    [SerializeField] private float rating, price;
    
    [SerializeField] private List<Amenity> amenities;
    [SerializeField] private List<int> num_of_amenity_type;

    [SerializeField] private List<Guest> guests;
    [SerializeField] private Player owner;

    [SerializeField] private readonly Vector3 DEFAULT_BUILDING_SIZE = new(1.0f, 1.0f, 1.0f);
    [SerializeField] private readonly float DEFAULT_RATING = 0.0f;
    [SerializeField] private readonly int DEFAULT_CAPACITY = 100;
    [SerializeField] private readonly float DEFAULT_PRICE = 100;
    
    public void ResetGame()
    {
        gameObject.transform.localScale = DEFAULT_BUILDING_SIZE;
        rating = DEFAULT_RATING;
        capacity = DEFAULT_CAPACITY;
        price = DEFAULT_PRICE;
        /*for(int i = 0; i < amenities.Count; i++)
        {
            amenities[i].;
        }*/
        amenities.Clear();
        num_of_amenity_type.Clear();
        guests.Clear();

    }

    public void ChangeRating(float change)
    {
        //print("Before: " + GetRating());
        rating += change;
        if(rating < MIN_RATING)
        {
            rating = MIN_RATING;
        }else if(rating > MAX_RATING)
        {
            rating = MAX_RATING;
        }
        //print("After: " + GetRating());
    }

    public void ChangeCapacity(int inc)
    {
        capacity += inc;
        if (capacity < MIN_CAPACITY)
        {
            capacity = MIN_CAPACITY;
        }
        else if (capacity > MAX_CAPACITY)
        {
            capacity = MAX_CAPACITY;
        }
    }

    public void ChangePrice(int inc)
    {
        if (temp_cost == 0)
        {
            price += inc;
            if (price < MIN_PRICE)
            {
                price = MIN_PRICE;
            }
            else if (price > MAX_PRICE)
            {
                price = MAX_PRICE;
            }
        }
    }
    public void AddGuest(Guest customer)
    {
        guests.Add(customer);
    }
    public void RemoveGuest(Guest customer)
    {
        guests.Remove(customer);
    }
    public void AddAmenity(Amenity amenity)
    {

        if (!amenities.Contains(amenity))
        {
            amenities.Add(amenity);
            num_of_amenity_type.Add(1);
            ChangeRating(amenity.GetImprovementRating());
        }
        else
        {
            for(int i = 0; i < amenities.Count; i++)
            {
                if (amenities[i] == amenity)
                {
                    num_of_amenity_type[i]++;
                    ChangeRating(amenity.GetImprovementRating() / 10);
                    print("Duplicate");
                    break;
                }
            }
        }
        //print("Amenity added - " + amenity.name);
    }
    public void RemoveAmenity(string amenity)
    {
        for(int i = 0; i < amenities.Count; i++)
        {
            if (amenities[i].name == amenity)
            {
                if (num_of_amenity_type[i] > 1)
                {
                    ChangeRating(-amenities[i].GetImprovementRating() / 10);
                }
                else
                {
                    ChangeRating(-amenities[i].GetImprovementRating());
                    amenities.RemoveAt(i);
                    num_of_amenity_type.RemoveAt(i);
                }  
                //print("Amenity removed - " + amenity);
                break;
            }
        }
        
    }
    /*public void AwardOwnerMoney()
    {
        for (int i = 0; i < guests.Count; i++)
        {

        }
    }*/
    public int GetCapacity()
    {
        return capacity;
    }
    public float GetRating()
    {
        if (rating + temp_rating_change > 5)
        {
            return 5;
        }
        else if(rating + temp_rating_change < 0)
        {
            return 0;
        }
        return rating + temp_rating_change;
    }
    public float GetPrice()
    {
        if(temp_cost != 0)
        {
            if(price + temp_cost > 1000)
            {
                return 1000;
            }
            else if(price + temp_cost < 10)
            {
                return 10;
            }
            return price + temp_cost;
        }
        return price;
    }
    public int GetNumOfGuests()
    {
        return guests.Count;
    }
    public int GetNumOfAmenities()
    {
        int cnt = 0;
        for(int i = 0; i < num_of_amenity_type.Count; i++)
        {
            cnt += num_of_amenity_type[i];
        }
        return cnt;
    }
    public List<Amenity> GetAmenities()
    {
        return amenities;
    }
    public List<int> GetNumOfAmenityType()
    {
        return num_of_amenity_type;
    }

    public void ChangeTempRating(float change, bool reset = false)
    {
        if (!reset)
        {
            temp_rating_change += change;
        }
        else
        {
            temp_rating_change = 0;
        }
    }

    public void ChangeTempCost(int change, bool reset = false)
    {
        if (!reset)
        {
            temp_cost += change;
        }
        else
        {
            temp_cost = 0;
        }
    }

    public float GetTempCost() 
    { 
        return temp_cost;
    }

}
