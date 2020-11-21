using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile
{
    public GameObject tileObj;
    public string type;

    public Tile (GameObject obj, string t)
    {
        tileObj = obj;
        type = t;
    }
}

public class CreateGame : MonoBehaviour
{
    // Tile pressed on mouse
    GameObject tile1 = null;
    // Tile released by mouse
    GameObject tile2 = null;

    public GameObject[] tile;
    private List<GameObject> tileList = new List<GameObject>();

    bool renewBoard = false;
    public static int rows = 10;
    public static int cols = 13;

    Tile[,] tiles = new Tile[cols, rows];


    void ShuffleList()
    {
        System.Random rand = new System.Random();
        int r = tileList.Count;
        while (r > 1)
        {
            r--;
            int n = rand.Next(r + 1);
            GameObject val = tileList[n];
            tileList[n] = tileList[r];
            tileList[r] = val;
        }
    }


    private void Start()
    {

        // Create a list of tiles
        int numCopies = (rows * cols) / 3;
        for (int i = 0; i < numCopies; i++)
        {
            for (int j = 0; j < tile.Length; j++)
            {
                GameObject obj = (GameObject)Instantiate(tile[j], new Vector3(-10, 10, 0), tile[j].transform.rotation);
                obj.SetActive(false);
                tileList.Add(obj);
            }
        }

        ShuffleList();

        // Initialise tile grid
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                Vector3 tilePos = new Vector3(col, row, 0);

                //GameObject obj = (GameObject)Instantiate(tiles, tilePos, tiles.transform.rotation);
                //tiles[col, row] = new Tile(obj, obj.name);

                for (int n = 0; n < tileList.Count; n++)
                {
                    GameObject obj = tileList[n];
                    if (!obj.activeSelf)
                    {
                        obj.transform.position = new Vector3(tilePos.x, tilePos.y, tilePos.z);
                        obj.SetActive(true);
                        tiles[col, row] = new Tile(obj, obj.name);
                        n = tileList.Count + 1;
                    }
                }
            }
        }
    }

    private void Update()
    {
        CheckGrid();

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.GetRayIntersection(ray, 1000);

            if (hit)
            {
                tile1 = hit.collider.gameObject;
            }
        }
        else if (Input.GetMouseButtonUp(0) && tile1)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.GetRayIntersection(ray, 1000);

            if (hit)
            {
                tile2 = hit.collider.gameObject;
            }

            if (tile1 && tile2)
            {

                int horDistance = (int)Mathf.Abs(tile1.transform.position.x - tile2.transform.position.x);
                int verDistance = (int)Mathf.Abs(tile1.transform.position.y - tile2.transform.position.y);

                // XOR
                if (horDistance == 1 ^ verDistance == 1)
                {
                    // Swap tiles posotion

                    Tile temp = tiles[(int)tile1.transform.position.x,
                                      (int)tile1.transform.position.y];
                    tiles[(int)tile1.transform.position.x, (int)tile1.transform.position.y] =
                        tiles[(int)tile2.transform.position.x, (int)tile2.transform.position.y];
                    tiles[(int)tile2.transform.position.x, (int)tile2.transform.position.y] = temp;

                    Vector3 tempPos = tile1.transform.position;
                    tile1.transform.position = tile2.transform.position;
                    tile2.transform.position = tempPos;

                    // Reset the touches tiles
                    tile1 = null;
                    tile2 = null;
                }
                else
                {
                    //GetComponent<AudioSource>().Play();
                    Debug.Log("CANT SWAP");
                }
            }
        }
    }

    void CheckGrid()
    {
        int counter = 1;

        for (int row = 0; row < rows; row++)
        {
            counter = 1;

            for (int col = 1; col < cols; col++)
            {
                if (tiles[col, row] != null && tiles[col - 1, row] != null)
                {
                    if (tiles[col,row].type == tiles[col - 1, row].type)
                    {
                        counter++;
                    }
                    else
                    {
                        counter = 1;
                    }

                    if (counter == 3)
                    {
                        if (tiles[col, row] != null)
                        {
                            tiles[col, row].tileObj.SetActive(false);
                        }
                        if (tiles[col - 1, row] != null)
                        {
                            tiles[col - 1, row].tileObj.SetActive(false);
                        }
                        if (tiles[col - 2, row] != null)
                        {
                            tiles[col - 2, row].tileObj.SetActive(false);
                        }

                        tiles[col, row] = null;
                        tiles[col - 1, row] = null;
                        tiles[col - 2, row] = null;
                        renewBoard = true;
                    }
                }
            }
        }

        for (int col = 0; col < cols; col++)
        {
            counter = 1;

            for (int row = 1; row < rows; row++)
            {
                if (tiles[col, row] != null && tiles[col, row - 1] != null)
                {
                    if (tiles[col, row].type == tiles[col, row - 1].type)
                    {
                        counter++;
                    }
                    else
                    {
                        counter = 1;
                    }

                    if (counter == 3)
                    {
                        if (tiles[col, row] != null)
                        {
                            tiles[col, row].tileObj.SetActive(false);
                        }
                        if (tiles[col, row - 1] != null)
                        {
                            tiles[col, row - 1].tileObj.SetActive(false);
                        }
                        if (tiles[col, row - 2] != null)
                        {
                            tiles[col, row - 2].tileObj.SetActive(false);
                        }

                        tiles[col, row] = null;
                        tiles[col, row - 1] = null;
                        tiles[col, row - 2] = null;
                        renewBoard = true;
                    }
                }
            }
        }

        if (renewBoard)
        {
            RenewGrid();
            renewBoard = false;
        }
    }

    void RenewGrid()
    {
        bool anyMoved = false;
        ShuffleList();

        for (int row = 1; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                if (row == rows - 1 && tiles[col, row] == null)
                {
                    Vector3 tilePos = new Vector3(col, row, 0);
                    for (int n = 0; n < tileList.Count; n++)
                    {
                        GameObject obj = tileList[n];

                        if (!obj.activeSelf)
                        {
                            obj.transform.position = new Vector3(tilePos.x, tilePos.y, tilePos.z);
                            obj.SetActive(true);
                            tiles[col, row] = new Tile(obj, obj.name);
                            n = tileList.Count + 1;
                        }
                    }
                }

                if (tiles[col, row] != null)
                {
                    if (tiles[col, row - 1] == null)
                    {
                        tiles[col, row - 1] = tiles[col, row];
                        tiles[col, row - 1].tileObj.transform.position = new Vector3(col, row - 1, 0);
                        tiles[col, row] = null;
                        anyMoved = true;
                    }
                }
            }
        }
        if (anyMoved)
        {
            Invoke("RenewGrid", .5f);
        }
    }
}
