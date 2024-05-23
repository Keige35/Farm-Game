using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SaveHelper : MonoBehaviour
{
    public string key;

    [SerializeField] private bool saveKeyRandomizer;

    [SerializeField] private bool isSavePosition;
    [SerializeField] private bool isSaveInventory;
    [SerializeField] private bool isSaveHealth;
    [SerializeField] private bool isSaveNPC;
    [SerializeField] private bool isSavePercent;

    [HideIf("isSavePosition", false)] public SavePositionConfiguration SavePositionConfiguration;
    [HideIf("isSaveInventory", false)] public InventorySaveHelper InventorySaveHelper;
    [HideIf("isSaveHealth", false)] public SaveHealthHelper SaveHealthHelper;
    [HideIf("isSavePercent", false)] public SavePercentHelper SavePercentHelper;
    [HideIf("isSaveNPC", false)] public NPCSpawnSaveHelper NPCSpawnSaveHelper;


    private SaveData saveData;
    private Bootstrap bootstrap;
    private bool isSubscribe;

    public void LoadFromKey(string newKey)
    {
        key = newKey;
        Load();
    }

    private void Start()
    {
        if (saveKeyRandomizer)
        {
            key = key + Random.Range(0, int.MaxValue);
        }

        bootstrap = ServiceLocator.GetService<Bootstrap>();

        bootstrap.OnSave += Save;
        bootstrap.OnLoad += Load;
        isSubscribe = true;

    }

    private void Load()
    {
        var rigidBody = GetComponent<Rigidbody>();
        if (rigidBody)
        {
            rigidBody.isKinematic = true;
            StartCoroutine(EnableBody(rigidBody));
        }

        saveData ??= ServiceLocator.GetService<SaveData>();
        var saveDataFix = saveData.GetStorageByKey(key);
        if (saveDataFix != null)
        {
            if (isSavePosition)
            {
                var savePositionConfiguration = saveDataFix.SaveConfiguration.SavePositionConfiguration;
                transform.position = savePositionConfiguration.Position;
                transform.rotation = savePositionConfiguration.Rotation;
                SavePositionConfiguration = savePositionConfiguration;
            }

            if (isSaveInventory)
            {
                var itemSpawner = ServiceLocator.GetService<ItemSpawner>();
                var saveInventroy = saveDataFix.SaveConfiguration.InventorySaveHelper;
                var inventory = InventorySaveHelper.characterInventory;
                foreach (var saveItem in saveInventroy.savedItem)
                {
                    inventory.AddItem(itemSpawner.GetItemByType(saveItem));
                }

                var handItem = saveInventroy.HandItemType;
                inventory.AddNewHandItem(itemSpawner.GetHandItemByType(handItem));
            }

            if (isSaveHealth)
            {
                SaveHealthHelper.Damageable.UpdateCurrentHealth(saveDataFix.SaveConfiguration.SaveHealthHelper.SaveHealth);
                SaveHealthHelper.SaveHealth = saveDataFix.SaveConfiguration.SaveHealthHelper.SaveHealth;
            }

            if (isSavePercent)
            {
                SavePercentHelper.PuduNpcStateMachine.SetDamnagePercent(saveDataFix.SaveConfiguration.SavePercentHelper.SavePercentList);
                SavePercentHelper.PuduNpcStateMachine.SetCurrentPercent(saveDataFix.SaveConfiguration.SavePercentHelper.SavePercent);
            }
        }
    }

    private IEnumerator EnableBody(Rigidbody rigidbody)
    {
        yield return null;
        rigidbody.isKinematic = false;
    }

    private void Save()
    {
        saveData ??= ServiceLocator.GetService<SaveData>();
        var isSaved = true;
        var saveDataFix = saveData.GetStorageByKey(key);
        if (saveDataFix == null)
        {
            isSaved = false;
            saveData.SaveStorages.Add(new SaveStorage() { Key = key, SaveConfiguration = new GameSaveConfiguration() });
        }

        saveDataFix = saveData.GetStorageByKey(key);
        if (isSavePosition)
        {
            if (isSaved == false)
            {
                saveDataFix.SaveConfiguration.SavePositionConfiguration = new SavePositionConfiguration();
            }

            saveDataFix.SaveConfiguration.SavePositionConfiguration.Position = transform.position;
            saveDataFix.SaveConfiguration.SavePositionConfiguration.Rotation = transform.rotation;
        }

        if (isSaveInventory)
        {
            saveDataFix.SaveConfiguration.InventorySaveHelper = new InventorySaveHelper();
            foreach (var item in InventorySaveHelper.characterInventory.InventoryItem)
            {
                saveDataFix.SaveConfiguration.InventorySaveHelper.savedItem.Add(item.ItemType);
            }

            saveDataFix.SaveConfiguration.InventorySaveHelper.HandItemType =
                InventorySaveHelper.characterInventory.CurrentHandItem.HandItemType;
        }

        if (isSaveHealth)
        {
            saveDataFix.SaveConfiguration.SaveHealthHelper = new SaveHealthHelper();
            saveDataFix.SaveConfiguration.SaveHealthHelper.SaveHealth = SaveHealthHelper.Damageable.CurrentHealth;
        }

        if (isSavePercent)
        {
           saveDataFix.SaveConfiguration.SavePercentHelper = new SavePercentHelper();
            foreach (var percent in SavePercentHelper.PuduNpcStateMachine.DamagePercent)
            {
                saveDataFix.SaveConfiguration.SavePercentHelper.SavePercentList.Add(percent);
            }

            saveDataFix.SaveConfiguration.SavePercentHelper.SavePercent =
            SavePercentHelper.PuduNpcStateMachine.CurrentPercent;
        }
        if (isSaveNPC)
        {
            saveDataFix.SaveConfiguration.NPCSpawnSaveHelper = new NPCSpawnSaveHelper();
            saveDataFix.SaveConfiguration.NPCSpawnSaveHelper.DamageableType = DamageableType.Pudu;
            saveDataFix.SaveConfiguration.IsNPC = true;
        }

        Debug.Log("Savehelper");
    }
}

[Serializable]
public class NPCSpawnSaveHelper
{
    [field: SerializeField] public DamageableType NPCType { get; set; }
    public DamageableType DamageableType;
}

[Serializable]
public class InventorySaveHelper
{
    [field: SerializeField] public CharacterInvetory characterInventory { get; set; }

    public List<ItemType> savedItem = new List<ItemType>();
    public HandItemType HandItemType;
}

[Serializable]
public class SaveHealthHelper
{
    [field: SerializeField] public Damageable Damageable { get; set; }
    public int SaveHealth;
}

[Serializable]
public class SavePercentHelper
{
    [field: SerializeField] public PuduNpcStateMachine PuduNpcStateMachine { get; set; }
    public List<float> SavePercentList = new List<float>();
    public float SavePercent;
}