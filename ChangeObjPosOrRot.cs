using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeObjPosOrRot : MonoBehaviour
{
    [SerializeField] private float change_pos_x, change_pos_y, change_pos_z, change_rot_x, change_rot_y, change_rot_z;
    [SerializeField] private GameObject obj;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += new Vector3(change_pos_x, change_pos_y, change_pos_z);
        transform.Rotate(new Vector3(change_rot_x, change_rot_y, change_rot_z));
        if (name.Contains("cabin"))
        {
            transform.position = obj.transform.position;
        }
    }
}
