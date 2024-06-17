using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CatchItems : MonoBehaviour
{
    [SerializeField] private float radius;
    [SerializeField] private Transform catcher;
    
    public void ItemsCatch()
    {

        var foundItem = Physics.OverlapSphere(catcher.position, radius).Where(t => t.GetComponent<ICathable>() != null).ToList();
        foreach (var t in foundItem)
        {
            t.GetComponent<ICathable>().CathItem();
        }

        Debug.Log("work");
    }
    
}
