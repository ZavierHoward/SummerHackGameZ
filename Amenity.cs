using UnityEngine;

public class Amenity : MonoBehaviour
{

    [SerializeField] private float price = 0.0f;
    [SerializeField] private float improvement_rating = 0.0f;
    [SerializeField] private GameObject building;
    
    public float GetPrice()
    {
        return price;
    }
    public float GetImprovementRating()
    {
        return improvement_rating;
    }
    public GameObject GetBuilding()
    {
        return building;
    }    
}
