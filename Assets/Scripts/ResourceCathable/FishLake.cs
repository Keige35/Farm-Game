using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(ItemDropper))]
public class FishLake : InterplayObject, ICathable
{
    [SerializeField] private Transform playerPosition;
    [SerializeField] private Transform fishLakePosition;
    private ItemDropper itemDropper;

    public virtual void CathItem()
    {
        itemDropper ??= GetComponent<ItemDropper>();
        itemDropper.DropItem(fishLakePosition, playerPosition);
    }
}
