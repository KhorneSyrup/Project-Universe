﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;



public class TilingController : MonoBehaviour
{

    public Camera Cam;
    public GameObject Tile;
    private MeshFilter mesh;
    public bool AllowBuilding;
    private bool SnapToGrid;

    private Vector3 Coords;
    private Vector3 debugrayend;

    private bool sw = false;

    //  public BuildMenu buildMenu;

    public GameObject TileContainer;
    public GameObject BuildingMenu;
    public GameObject Button;
    private GameObject temp;
    private string FilePath;

    private GameObject newButton;
    private SelectTileButton but;

    //  public TileCollection tileContainer = TileCollection.Load(Path.Combine(Application.dataPath, "Tiles.xml"));

    // Start is called before the first frame update
    void Start()
    {
        // buildMenu = GameObject.Find("EventSystem").GetComponent<BuildMenu>();
        SnapToGrid = true;

        ReadXMLTiles();

        /* SAVING XML FILE FOR TESTING - THIS WILL GO FOR EXTERIOR EDITOR
          if (sw == false)
         {
             SaveXMLTiles();
             sw = true;
          }
        */

        // Mesh model = (Mesh)Resources.Load("Models/Tiles/Floors/nukeguard/mesh1", typeof(Mesh));
        // Mesh model = Resources.LoadAll<Mesh>("Models/Tiles/Floors/nukeguard/mesh1")[5];


    }


    // Update is called once per frame
    void Update()
    {



        if (AllowBuilding)
        {

            Tile.SetActive(true);

            RaycastHit hit;
            Ray ray = Cam.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                if (Physics.Raycast(ray))
                {
                    //this is for debug on Scene, draw line for raycasting
                    Debug.DrawLine(hit.point, debugrayend, Color.red);
                    //   Debug.Log(hit.point);

                    if (hit.transform.gameObject.tag != "TilingGhost")  // prevents from raycasting tile

                        //tile snapping into 1x1x1 grid
                        if (SnapToGrid)
                        {
                            Coords.x = Mathf.Round(hit.point.x);
                            Coords.y = Mathf.Round(hit.point.y);
                            Coords.z = Mathf.Round(hit.point.z);
                        }
                    Tile.transform.position = Coords; //following tile
                }
            }
        }
        else
            Tile.SetActive(false);

    }


    public LODGroup group;

    //READING / SAVING XML FILE

    public void ReadXMLTiles()
    {
        var tileContainer = TileCollection.Load(Path.Combine(Application.dataPath, "Tiles.xml"));
        // var xmlData = @"<TileCollection><tiles><Tiles name=""a""><model_path></model_path>x<material_path>y</material_path></Tiles></tiles></TileCollection>";
        // var tileContainer = TileCollection.LoadFromText(xmlData);
        // Debug.Log("Number of tiles in database: " + tileContainer.tiles.Length);
        //  Debug.Log(tileContainer.tiles[0].model_path);

        // AddButton(tileContainer.tiles.Length);  //ADD BUTTONS TO UI


        var parentDatabase = new GameObject();
        parentDatabase.name = "Tile_Database";
        parentDatabase.layer = 10;
        for (int c = 0; c < tileContainer.tiles.Length; c++)
        {
            FilePath = tileContainer.tiles[c].model_path;
            Debug.Log(tileContainer.tiles[c].model_path);
            //  FilePath = "Models/Tiles/Floors/nukeguard/mesh1";
            GameObject model = Resources.Load<GameObject>(FilePath);
            GameObject obj = (GameObject)Instantiate(model);

            //  obj.AddComponent(typeof(LODGroup));


            foreach (Transform child in obj.transform)
                child.gameObject.layer = 10;

            // obj.SetActive(false);
            obj.transform.SetParent(parentDatabase.transform);
            // obj.name = tileContainer.tiles[c].Name;
            obj.name = c.ToString();


            newButton = Instantiate(Button);
            but = newButton.GetComponent<SelectTileButton>();
            but.TileID = c;
            newButton.GetComponentInChildren<Text>().text = tileContainer.tiles[c].Name;
            newButton.transform.SetParent(BuildingMenu.transform);

            // TO REMOVE WHEN ALL TILES IS UNIFIED
            if (obj.transform.Find("Camera") != null)
            {
                obj.transform.Find("Camera").GetComponent<Camera>().enabled = false;
            }
            // END REMOVE


            //obj.GetComponent<LODGroup>().
            // /*
            if (obj.transform.Find("model") != null)
            {

                group = obj.AddComponent<LODGroup>();

                // Add 4 LOD levels
                LOD[] lods = new LOD[4];

                for (int i = 0; i < 4; i++)
                {
                    GameObject primType = obj.transform.Find("model").gameObject;
                    switch (i)
                    {
                        case 1:
                            primType = obj.transform.Find("lod1").gameObject;
                            break;
                        case 2:
                            primType = obj.transform.Find("lod2").gameObject;
                            break;
                        case 3:
                            primType = obj.transform.Find("lod3").gameObject;
                            break;
                    }

                    Renderer[] renderers = new Renderer[1];
                    renderers[0] = primType.GetComponentInChildren<Renderer>();
                    lods[i] = new LOD(1.0F / (i + 1.2f), renderers);
                }
                group.SetLODs(lods);
                group.RecalculateBounds();
            }
        }
    }

    public void SaveXMLTiles()
    {
        var tileContainer = TileCollection.Load(Path.Combine(Application.dataPath, "Tiles.xml"));
        tileContainer.Save(Path.Combine(Application.persistentDataPath, "Tiles.xml"));
    }

    //---------------------BUILDING UI--------------------------

    public void AddButton(int i)
    {

        for (int c = 0; c < i; c++)
        {
            var newButton = Instantiate(Button);
            newButton.transform.SetParent(BuildingMenu.transform);
        }

    }
}

