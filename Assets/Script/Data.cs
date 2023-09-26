using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class Data : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField]
    private GameObject prefabMonster;
    [SerializeField]
    private GameObject prefabItemGround;
    [SerializeField]
    private GameObject prefabBullet;

    [Header("Other")]
    [SerializeField]
    private GameObject player;
    [SerializeField]
    private float timeSave = 5f;
    [SerializeField]
    private int countMonster = 3;
    public int CountMonster
    {
        get { return countMonster; }
        set { countMonster = value; }
    }

    /// <summary>
    /// list monsters on the map
    /// </summary>
    private GameObject[] monsters;     
    
    private float lastTimeSave = 0f;
    private bool isloadGame = true;

    /// <summary>
    /// save by timer
    /// </summary>
    private void Update()
    {
        if (Time.time - lastTimeSave >= timeSave)
        {
            SaveGame();
            lastTimeSave = Time.time;
        }
    }
    /// <summary>
    /// save(player, monsters, item, bullet) in a file
    /// </summary>
    private void SaveGame()
    {
        SaveData saveData = new SaveData();

        //+ item ground +
        GameObject[] itemsGO = GameObject.FindGameObjectsWithTag("ItemGround");
        Item[] items = new Item[itemsGO.Length];
        for (int i = 0; i < items.Length; i++) 
        {
            items[i].posX = itemsGO[i].transform.position.x;
            items[i].posY = itemsGO[i].transform.position.y;
            items[i].id = itemsGO[i].GetComponent<ItemGround>().Id;
            items[i].count = itemsGO[i].GetComponent<ItemGround>().Count;
        }
        saveData.items = items;
        //- item ground -

        //+ bullet +
        GameObject[] bulletGO = GameObject.FindGameObjectsWithTag("Bullet");
        Bullet[] bullets = new Bullet[bulletGO.Length];
        for (int i = 0; i < bullets.Length; i++) 
        {
            bullets[i].posX = bulletGO[i].transform.position.x;
            bullets[i].posY = bulletGO[i].transform.position.y;
            bullets[i].targetPosX = bulletGO[i].GetComponent<BulletControl>().TargetPos.x;
            bullets[i].targetPosY = bulletGO[i].GetComponent<BulletControl>().TargetPos.y;
            bullets[i].damage = bulletGO[i].GetComponent<BulletControl>().Damage;
        }
        saveData.Bullets = bullets;
        //- bullet -

        //+ monster +
        Monster[] monstersSave = new Monster[monsters.Length];
        for (int i = 0; i < monsters.Length; i++) 
        {
            monstersSave[i].posX = monsters[i].transform.position.x;
            monstersSave[i].posY = monsters[i].transform.position.y;
            monstersSave[i].hp = monsters[i].GetComponent<MonsterControl>().Hp;
            monstersSave[i].isDie = monsters[i].GetComponent<MonsterControl>().IsDead;
        }
        saveData.Monsters = monstersSave;
        //- monster -

        //+ player +
        Player plr = new Player();
        plr.posX = player.transform.position.x;
        plr.posY = player.transform.position.y;
        plr.hp = player.GetComponent<PlayerControl>().Hp;
        plr.idGun = player.GetComponent<PlayerControl>().IdGun;
        plr.isDead = player.GetComponent<PlayerControl>().IsDead;
        InventoryItem[] inventoryItems = player.GetComponent<Backpack>().InventoryItems;
        plr.itemCount = new int[inventoryItems.Length];
        plr.itemId = new int[inventoryItems.Length];
        for (int i = 0; i < inventoryItems.Length; i++) 
        {
            plr.itemId[i] = inventoryItems[i].Id;
            plr.itemCount[i] = inventoryItems[i].Count;
        }
        saveData.player = plr;
        //- player -

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/SaveData.dat");
        bf.Serialize(file, saveData);
        file.Close();
        Debug.Log("Game data saved!");
    }

    /// <summary>
    /// load(player, monsters, item, bullet) from a file
    /// </summary>
    /// <returns>true = successfully</returns>
    private bool LoadGame()
    {
        if (File.Exists(Application.persistentDataPath + "/SaveData.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/SaveData.dat", FileMode.Open);
            SaveData saveData = (SaveData)bf.Deserialize(file);
            file.Close();

            //+ player +
            Player plr = saveData.player;
            if (plr.isDead)
                return false;
            player.GetComponent<PlayerControl>().IsDead = plr.isDead;
            player.transform.position = new Vector3(plr.posX, plr.posY, 0f);
            player.GetComponent<PlayerControl>().Hp = plr.hp;
            InventoryItem[] inventoryItems = new InventoryItem[plr.itemCount.Length];
            player.GetComponent<Backpack>().InitInventoruItem();
            for (int i = 0; i < inventoryItems.Length; i++)
            {
                player.GetComponent<Backpack>().AddItem(plr.itemId[i], plr.itemCount[i]);
                inventoryItems[i].Id = plr.itemId[i];
                inventoryItems[i].Count = plr.itemCount[i];
            }
            player.GetComponent<PlayerControl>().CamAlig();
            player.GetComponent<PlayerControl>().Damage(0);
            //- player -

            //+ item ground +
            Item[] items = saveData.items;
            if (items != null) 
            {
                foreach (Item item in items)
                {
                    GameObject itemGO = Instantiate(prefabItemGround);
                    itemGO.transform.position = new Vector3(item.posX, item.posY, 0f);
                    itemGO.GetComponent<ItemGround>().Init(item.id, item.count);
                }
            }
            //- item ground -

            //+ bullet +
            Bullet[] bullets = saveData.Bullets;
            if (bullets != null) 
            {
                foreach (Bullet bullet in bullets)
                {
                    GameObject bulletGO = Instantiate(prefabBullet);
                    bulletGO.GetComponent<BulletControl>().Init(new Vector2(bullet.posX, bullet.posY), new Vector2(bullet.targetPosX, bullet.targetPosY), bullet.damage, player);
                }
            }
            //- bullet -

            //+ monster +
            Monster[] monstersSave = saveData.Monsters;
            if (monstersSave != null) 
            {
                monsters = new GameObject[monstersSave.Length];
                for (int i = 0; i < monsters.Length; i++)
                {
                    monsters[i] = Instantiate(prefabMonster);
                    monsters[i].transform.position = new Vector3(monstersSave[i].posX, monstersSave[i].posY, 0f);
                    monsters[i].GetComponent<MonsterControl>().IsDead = monstersSave[i].isDie;
                    monsters[i].GetComponent<MonsterControl>().Hp = monstersSave[i].hp;
                    monsters[i].GetComponent<MonsterControl>().Damage(0, false);
                }
            }
            //- monster -
        }
        else
            return false;
        return true;
    }
    /// <summary>
    /// spawn item on map
    /// </summary>
    /// <param name="pos">position on map</param>
    /// <param name="id">id item</param>
    /// <param name="count">count item</param>
    public void AddItemGround(Vector2 pos, int id, int count)
    {
        GameObject item = Instantiate(prefabItemGround);
        item.transform.position = pos;
        item.GetComponent<ItemGround>().Init(id, count);
    }
    /// <summary>
    /// spawn monsters from countMonster
    /// </summary>
    private void SpawnMonsters()
    {
        monsters = new GameObject[countMonster];
        for (int i = 0; i < monsters.Length; i++)
        {
            monsters[i] = Instantiate(prefabMonster);
        }
    }
    /// <summary>
    /// if no load, standart spawn monsters
    /// </summary>
    private void Awake()
    {
        if (!LoadGame())
        {
            isloadGame = false;
            SpawnMonsters();
        }
    }
    /// <summary>
    /// if no load game, give ak and ammo
    /// </summary>
    private void Start()
    {
        if (!isloadGame)
        {
            //add items Player
            player.GetComponent<Backpack>().AddItem(0, 1);//ak
            player.GetComponent<Backpack>().AddItem(1, 10);//ammo ak         
        }
    }
}


[Serializable]
public class SaveData
{
    public Item[] items;
    public Bullet[] Bullets;
    public Monster[] Monsters;
    public Player player;
}

[Serializable]
public struct Player
{
    public float posX;
    public float posY;
    public float hp;
    public int idGun;
    public bool isDead;
    public int[] itemId;
    public int[] itemCount;
}

[Serializable]
public struct Monster
{
    public float posX;
    public float posY;
    public float hp;
    public bool isDie;
}

[Serializable]
public struct Bullet
{
    public float posX;
    public float posY;
    public float targetPosX;
    public float targetPosY;
    public float damage;
}

[Serializable]
public struct Item
{
    public float posX;
    public float posY;
    public int id;
    public int count;
}