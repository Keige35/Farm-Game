using UnityEngine;

public class Bonfire : InterplayObject
{
    [SerializeField] private ParticleSystem particleSystemBonfire;
    public override void IterplayObject(CharacterInvetory characterInvetory)
    {
        var item = characterInvetory.GetHandItemByType(HandItemType.FullBucket);
        if (item)
        {
            item.ReturnToPool();
            var newBucket = ItemSpawner.Instance.GetHandItemByType(HandItemType.Bucket);
            characterInvetory.AddNewHandItem(newBucket);
            ChokeFire();
        }
        else 
        {
            LightFire();
        }
    }

    void LightFire()
    {
        particleSystemBonfire.gameObject.SetActive(true);
    }

    void ChokeFire()
    {
        particleSystemBonfire.gameObject.SetActive(false);
    }
}
