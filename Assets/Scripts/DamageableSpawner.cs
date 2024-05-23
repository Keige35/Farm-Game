using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageableSpawner : MonoBehaviour
{
    [SerializeField] private int amount;
    [SerializeField] private DamageableType damageableType;
    private ItemSpawner itemSpawner;

    private void Start()
    {
        if (ServiceLocator.GetService<SaveData>().IsFirstLaunch == false)
        {
            itemSpawner ??= ServiceLocator.GetService<ItemSpawner>();
            for (int i = 0; i < amount; i++)
            {
                itemSpawner.SpawnNewDamageableItem(damageableType, transform.position, transform.rotation);
            }

            ServiceLocator.GetService<SaveData>().IsFirstLaunch = true;
        }
    }

    public void Spawn()
    {
        itemSpawner ??= ServiceLocator.GetService<ItemSpawner>();
        itemSpawner.SpawnNewDamageableItem(damageableType, transform.position, transform.rotation);
    }
}