using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Sirenix.OdinInspector;

public class RandomItemSpawner : MonoBehaviour
{
    public bool autoSpawn;
    public Vector2 size;
    public DropItem[] items;
    public int chance;
    public float startHeight;
    public float rayLength = 5F;
    public LayerMask mask;
   public List<int> itemRNGList;
    Vector3 offset;
    // Start is called before the first frame update
    void Awake()
    {

        offset = transform.position;
        SetupItemRNG();
        TimeManager.Instance.hubDayEvent += SpawnStuff;
    }

    private void Start()
    {
        if (autoSpawn) SpawnStuff();
    }

    [Button("Setup RNG")]
    void SetupItemRNG()
    {
        offset = transform.position;
        itemRNGList = new List<int>();
        for (int z = 0; z < items.Length; z++)
        {
            for (int v = 0; v < items[z].chance; v++)
            {
                itemRNGList.Add(z);
            }
        }
    }

    [Button("Spawn Stuff")]
    public void SpawnStuff()
    {


        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                int RNG = Random.Range(0, 10000);
                if (RNG < chance)
                {

                    RaycastHit hit;

                    Vector3 startPos = new Vector3(x * 0.98F, startHeight, y * 0.98F);

                    if (Physics.Raycast(startPos + offset, Vector3.down, out hit, rayLength, mask))
                    {
                        if (hit.collider.CompareTag("Terrain"))
                        {
                            int itemRNG = Random.Range(0, itemRNGList.Count);


                            Instantiate(items[itemRNGList[itemRNG]].GO, hit.point, Quaternion.Euler(0, Random.Range(0, 360), 0), transform);
                        }
                    }
                }
            }
        }
    }

    [Button("Delete children")]
    void DeleteChildren() {
        //foreach (Transform child in transform)
        //{
        //    DestroyImmediate(child.gameObject);
        //}
        int children = transform.childCount;
        for (int i = children - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }
    }

    private void OnDrawGizmos()
    {

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position + new Vector3(size.x / 2, 0, size.y / 2), new Vector3(size.x, Mathf.Abs(startHeight), size.y));

    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.digit9Key.wasPressedThisFrame) SpawnStuff();
    }
}

[System.Serializable]
public class DropItem
{
    public GameObject GO;
    public ItemSO so;
    public int quantity;
    public int quality = 20;
    public int guarenteed;
    public int chance;
}