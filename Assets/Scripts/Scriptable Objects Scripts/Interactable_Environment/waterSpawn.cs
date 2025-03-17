using UnityEngine;
using UnityEngine.Tilemaps; 

public class waterSpawn : MonoBehaviour
{
    [SerializeField] private GameObject water;

    public Tilemap tilemap = null;
    public Vector3Int spawnCenter; // The location of the object being destroyed
    public int radius = 8; // Is used for the size perimeter of the circle spawn
    [SerializeField] public TileBase blockedTile; // The hazard tiles 
    public GameObject waterPrefab; // The water Tile
    public bool isblocked = true;
        
   



  /*  private void Start()
    {
        tilemap = GetComponent<Tilemap>();
        for (int x = 0; x < radius; x++)
        {
            for (int y = 0; y < radius; y++)
            {
                //Check to see if tile is avaible then create while loop until water tile is placed
                Vector3Int tilePosition = spawnCenter + new Vector3Int(x, y, 0);

                /*if (tilemap.GetTile(tilePosition)!= blockedTile)
                {
                   
                }          }
        }


    }*/
    private void Start()
    {
        tilemap = GameObject.Find("Game Tilemap").GetComponent<Tilemap>();
        Instantiate(waterPrefab);
        InitateWater();
    }
    public void InitateWater()
    {
        for (int x = 0; x < radius; x++)
        {
            Debug.Log("Loop numx " + x);
            for (int y = 0; y < radius; y++)
            {
                Debug.Log("Loop y " + y);
                Instantiate(waterPrefab);
                Vector3Int tilePos = new Vector3Int(x, y, 0);
                if (tilemap.GetTile(tilePos).Equals(blockedTile))
                {
                    Debug.Log("Blocked");
                    /*isblocked = true;
                    while (isblocked)
                    {

                    }*/
               }
                else
                {
                    Debug.Log("water");
                    Instantiate(waterPrefab);
                }
                
             }
            }
        }
    }

   
    /*Uprivate void SpawnWater(int x, int z)
    {
        Vector3 pos = tilemap[x, z].Position;
    }*/






