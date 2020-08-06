using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum ItemType {Coin, MedPack, Ammo, PowerUp }
public class CollectableItem : MonoBehaviour
{
    public ItemType itemType;

    //Constantly rotate the collectable in mid air.
    void Start()
    {
        if(itemType != ItemType.Coin)
        transform.Translate(new Vector3(0, 1, 0), Space.World);
    }

    private void Update()
    {
        transform.Rotate(new Vector3(0, 90, 0) * Time.deltaTime);
    }
}
