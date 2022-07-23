using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBoardManager : MonoBehaviour
{
    public GameObject wallHorizontal;
    public GameObject arrow;
    public GameObject wallVertical;
    public GameObject scores;
    public GameObject player;
    public GameObject food;
    public GameObject door;
    public GameObject lightBeam;
    public GameObject enemy;
    public GameObject trap;
    public GameObject statue;
    public GameObject mushroom;
    public GameObject barrel;
    public GameObject chest;
    public GameObject web;
    public GameObject cross;
    public GameObject gameOver;
    private Vector3 euler;
    private bool survey = false;
    private bool randomGenerated = false;
    private int doorXCord;
    private int doorYCord;
    public int labirinthWidth;
    public int labirinthHeight;
    bool verticalActive;
    Vector3 position;
    //List<AssemblyCSharp.WallCoordinates> wallCoordinates;
    // Use this for initialization
    float chance;
    void Start()
    {
        SetupGame();
    }

    void SetupGame()
    {
        string loadParameter = CrossSceneInformation.CrossSceneInfo;
        if(loadParameter == "arrow")
        {
            Debug.Log("Arrow");
            lightBeam.SetActive(false);
            if(!randomGenerated)
                InstantiateArrowScenario();
        }
        else
        {
            Debug.Log("Lightbeam");
            arrow.SetActive(false);
            if (!randomGenerated)
                InstantiateBeamScenario();
        }
        //wallCoordinates = new List<Vector2>();
        if (!survey) { 
            for (int i = -4; i <= 5; i++)
            {
                for (int j = -4; j <= 5; j++)
                {
                    if (randomGenerated) {
                        verticalActive = false;
                        chance = Random.Range(0f, 100f);
                        if (chance > 40f)
                        {
                            position = new Vector3(i * 10 - 5, 0, j * 10);
                            Instantiate(wallHorizontal, position, Quaternion.identity);
                            verticalActive = true;
                        }
                        if (verticalActive && Random.Range(0f, 100f) > 75f)
                        {
                            Quaternion spawnRotation = Quaternion.Euler(0, 90, 0);
                            position = new Vector3(i * 10 - 5, 0f, j * 10 - 5);
                            Instantiate(wallVertical, position, spawnRotation);
                        }
                        else if (!verticalActive && Random.Range(0f, 100f) > 40f)
                        {
                            Quaternion spawnRotation = Quaternion.Euler(0, 90, 0);
                            position = new Vector3(i * 10 - 5, 0f, j * 10 - 5);
                            Instantiate(wallVertical, position, spawnRotation);
                        }
                    }

                    if (Random.Range(0f, 100f) > 85f)
                    {
                        position = new Vector3(i * 10 + Random.Range(-10f, 10f), 0.1f, j * 10 + Random.Range(-10f, 10f));
                        Instantiate(trap, position, Quaternion.identity);
                        //wallCoordinates.Add(new AssemblyCSharp.WallCoordinates(i, j, "vertical", true));
                    }
                    if (Random.Range(0f, 100f) > 60f)
                    {
                        position = new Vector3(i * 10 + Random.Range(-10f, 10f), 0.5f, j * 10 + Random.Range(-10f, 10f));
                        Instantiate(scores, position, Quaternion.identity);
                    }
                    
                    if (Random.Range(0f, 100f) > 70f)
                    {
                        position = new Vector3(i * 10 + Random.Range(-10f, 10f), 1f, j * 10 + Random.Range(-10f, 10f));
                        Instantiate(enemy, position, Quaternion.identity);
                    }
                    
                    if (Random.Range(0f, 100f) > 85f)
                    {
                        position = new Vector3(i * 10 + Random.Range(-10f, 10f), 0.5f, j * 10 + Random.Range(-10f, 10f));
                        Instantiate(food, position, Quaternion.Euler(45 + Random.Range(-10f, 10f), 0.5f, 45 + Random.Range(-10f, 10f)));
                    }


                }
            }
        }
        //float DoorPosition = Random.Range(0f, 100f);
        //Vector3 euler = new Vector3(0, 0, 0);
        //if (DoorPosition <= 25f)
        //{
        //    doorXCord = labirinthWidth / 2 - 1;
        //    doorYCord = (int)Random.Range(-labirinthHeight / 2, labirinthHeight / 2);
        //    euler = new Vector3(0, 0, 0);
        //}
        //else if (DoorPosition <= 50f)
        //{
        //    doorXCord = -labirinthWidth / 2 + 1;
        //    doorYCord = (int)Random.Range(-labirinthHeight / 2, labirinthHeight / 2);
        //    euler = new Vector3(0, 0, 0);
        //    //Door on Right Wall
        //}
        //else if (DoorPosition <= 75f)
        //{
        //    doorYCord = labirinthHeight / 2 - 1;
        //    doorXCord = (int)Random.Range(-labirinthWidth / 2, labirinthWidth / 2);
        //    euler = new Vector3(0, 90, 0);
        //}
        //else
        //{
        //    doorYCord = -labirinthHeight / 2 + 1;
        //    doorXCord = (int)Random.Range(-labirinthWidth / 2, labirinthWidth / 2);
        //    euler = new Vector3(0, -90, 0);
        //}
        //print("Door2: " + door.transform.position);
        //door.transform.rotation = Quaternion.Euler(euler);
        //sdoor.transform.localRotation = Quaternion.Euler(euler);
        //door.SetActive(false);
        //Instantiate(lightBeam, new Vector3(doorXCord, 20f, doorYCord), Quaternion.Euler(euler));
        //GameObject door_m =  Instantiate(door, new Vector3(doorXCord, 1.7f, doorYCord), Quaternion.Euler(euler));

        //arrow.GetComponent<GiantArrowCompass>().SetTarget(door_m);

        gameOver.SetActive(false);
        //position = new Vector3(Random.Range(labirinthHeight / 3, -labirinthHeight / 3), 0, Random.Range(labirinthWidth / 3, -labirinthWidth / 3));
        //player.transform.localPosition = position;
        
        //position = new Vector3(doorXCord, 1.7f, doorYCord);
        print("Player: "+ player.transform.localPosition);
        //door.transform.position = new Vector3(doorXCord, 1.7f, doorYCord);
        //door.GetComponent<Target>().Move(position);

    }
    void InstantiateBeamScenario()
    {
        Quaternion spawnRotation = Quaternion.Euler(0, 90, 0);
        Instantiate(wallVertical, new Vector3(-10, 0f, -45), spawnRotation);
        Instantiate(wallVertical, new Vector3(20, 0f, -45), spawnRotation);
        Instantiate(wallVertical, new Vector3(-30, 0f, -35), spawnRotation);
        Instantiate(wallVertical, new Vector3(-20, 0f, -35), spawnRotation);
        Instantiate(wallVertical, new Vector3(0, 0f, -35), spawnRotation);
        Instantiate(wallVertical, new Vector3(40, 0f, -35), spawnRotation);
        Instantiate(wallVertical, new Vector3(-10, 0f, -25), spawnRotation);
        Instantiate(wallVertical, new Vector3(20, 0f, -25), spawnRotation);
        Instantiate(wallVertical, new Vector3(-30, 0f, -15), spawnRotation);
        Instantiate(wallVertical, new Vector3(-20, 0f, -15), spawnRotation);
        Instantiate(wallVertical, new Vector3(0, 0f, -15), spawnRotation);
        Instantiate(wallVertical, new Vector3(30, 0f, -15), spawnRotation);
        Instantiate(wallVertical, new Vector3(-20, 0f, -5), spawnRotation);
        Instantiate(wallVertical, new Vector3(40, 0f, -5), spawnRotation);
        Instantiate(wallVertical, new Vector3(-20, 0f, 5), spawnRotation);
        Instantiate(wallVertical, new Vector3(-10, 0f, 5), spawnRotation);
        Instantiate(wallVertical, new Vector3(20, 0f, 5), spawnRotation);
        Instantiate(wallVertical, new Vector3(30, 0f, 5), spawnRotation);
        Instantiate(wallVertical, new Vector3(40, 0f, 5), spawnRotation);
        Instantiate(wallVertical, new Vector3(-10, 0f, 15), spawnRotation);
        Instantiate(wallVertical, new Vector3(10, 0f, 15), spawnRotation);
        Instantiate(wallVertical, new Vector3(-30, 0f, 25), spawnRotation);
        Instantiate(wallVertical, new Vector3(-20, 0f, 25), spawnRotation);
        Instantiate(wallVertical, new Vector3(0, 0f, 25), spawnRotation);
        Instantiate(wallVertical, new Vector3(20, 0f, 25), spawnRotation);
        Instantiate(wallVertical, new Vector3(40, 0f, 25), spawnRotation);
        Instantiate(wallVertical, new Vector3(-30, 0f, 35), spawnRotation);
        Instantiate(wallVertical, new Vector3(-10, 0f, 35), spawnRotation);
        Instantiate(wallVertical, new Vector3(30, 0f, 35), spawnRotation);
        Instantiate(wallVertical, new Vector3(20, 0f, 35), spawnRotation);

        //Vertical
        Instantiate(wallVertical, new Vector3(-45, 0f, -40), Quaternion.identity);
        Instantiate(wallVertical, new Vector3(-25, 0f, -40), Quaternion.identity);
        Instantiate(wallVertical, new Vector3(-5, 0f, -40), Quaternion.identity);
        Instantiate(wallVertical, new Vector3(-35, 0f, -30), Quaternion.identity);
        Instantiate(wallVertical, new Vector3(15, 0f, -30), Quaternion.identity);
        Instantiate(wallVertical, new Vector3(35, 0f, -30), Quaternion.identity);
        Instantiate(wallVertical, new Vector3(-45, 0f, -20), Quaternion.identity);
        Instantiate(wallVertical, new Vector3(-25, 0f, -20), Quaternion.identity);
        Instantiate(wallVertical, new Vector3(-5, 0f, -20), Quaternion.identity);
        Instantiate(wallVertical, new Vector3(25, 0f, -20), Quaternion.identity);
        Instantiate(wallVertical, new Vector3(-35, 0f, -10), Quaternion.identity);
        Instantiate(wallVertical, new Vector3(-15, 0f, -10), Quaternion.identity);
        Instantiate(wallVertical, new Vector3(5, 0f, -10), Quaternion.identity);
        Instantiate(wallVertical, new Vector3(35, 0f, -10), Quaternion.identity);
        Instantiate(wallVertical, new Vector3(-35, 0f, 0), Quaternion.identity);
        Instantiate(wallVertical, new Vector3(25, 0f, 0), Quaternion.identity);
        Instantiate(wallVertical, new Vector3(35, 0f, 0), Quaternion.identity);
        Instantiate(wallVertical, new Vector3(-25, 0f, 10), Quaternion.identity);
        Instantiate(wallVertical, new Vector3(-5, 0f, 10), Quaternion.identity);
        Instantiate(wallVertical, new Vector3(15, 0f, 10), Quaternion.identity);
        Instantiate(wallVertical, new Vector3(-45, 0f, 20), Quaternion.identity);
        Instantiate(wallVertical, new Vector3(35, 0f, 20), Quaternion.identity);
        Instantiate(wallVertical, new Vector3(-35, 0f, 30), Quaternion.identity);
        Instantiate(wallVertical, new Vector3(-15, 0f, 30), Quaternion.identity);
        Instantiate(wallVertical, new Vector3(15, 0f, 30), Quaternion.identity);
        Instantiate(wallVertical, new Vector3(35, 0f, 30), Quaternion.identity);
        Instantiate(wallVertical, new Vector3(-25, 0f, 40), Quaternion.identity);
        Instantiate(wallVertical, new Vector3(15, 0f, 40), Quaternion.identity);
        Instantiate(wallVertical, new Vector3(25, 0f, 40), Quaternion.identity);
        Instantiate(wallVertical, new Vector3(45, 0f, 40), Quaternion.identity);

        Instantiate(mushroom, new Vector3(-13, 0f, 20), Quaternion.identity);
        Instantiate(statue, new Vector3(5, 0f, 0), Quaternion.identity);
        Instantiate(cross, new Vector3(47, 1f, 10), Quaternion.Euler(-90, 0, 0));
        Instantiate(chest, new Vector3(-45, 0f, -45), Quaternion.identity);
        Instantiate(web, new Vector3(-15, 0f, -9), Quaternion.identity);
        Instantiate(web, new Vector3(37, 0f, 0), Quaternion.Euler(0, -90, 0));
        Instantiate(barrel, new Vector3(0, 0f, 30), Quaternion.identity);

        euler = new Vector3(0, 0, 0);
        Instantiate(lightBeam, new Vector3(49, 20f, 0), Quaternion.Euler(euler));
        GameObject door_m = Instantiate(door, new Vector3(49, 1.7f, 0), Quaternion.Euler(euler));

        arrow.GetComponent<GiantArrowCompass>().SetTarget(door_m);

        position = new Vector3(-47, 0, 15);
        player.transform.localPosition = position;
        player.transform.position = position;

    }
    void InstantiateArrowScenario()
    {
        Quaternion spawnRotation = Quaternion.Euler(0, 90, 0);
        Instantiate(wallVertical, new Vector3(20, 0f, -45), spawnRotation);
        Instantiate(wallVertical, new Vector3(30, 0f, -35), spawnRotation);
        Instantiate(wallVertical, new Vector3(-40, 0f, -35), spawnRotation);
        Instantiate(wallVertical, new Vector3(-20, 0f, -35), spawnRotation);
        Instantiate(wallVertical, new Vector3(-40, 0f, -25), spawnRotation);
        Instantiate(wallVertical, new Vector3(0, 0f, -25), spawnRotation);
        Instantiate(wallVertical, new Vector3(30, 0f, -25), spawnRotation);
        Instantiate(wallVertical, new Vector3(-10, 0f, -15), spawnRotation);
        Instantiate(wallVertical, new Vector3(0, 0f, -15), spawnRotation);
        Instantiate(wallVertical, new Vector3(20, 0f, -15), spawnRotation);
        Instantiate(wallVertical, new Vector3(-40, 0f, -5), spawnRotation);
        Instantiate(wallVertical, new Vector3(-30, 0f, -5), spawnRotation);
        Instantiate(wallVertical, new Vector3(30, 0f, -5), spawnRotation);
        Instantiate(wallVertical, new Vector3(40, 0f, -5), spawnRotation);
        Instantiate(wallVertical, new Vector3(-20, 0f, 5), spawnRotation);
        Instantiate(wallVertical, new Vector3(10, 0f, 5), spawnRotation);
        Instantiate(wallVertical, new Vector3(20, 0f, 5), spawnRotation);
        Instantiate(wallVertical, new Vector3(40, 0f, 5), spawnRotation);
        Instantiate(wallVertical, new Vector3(-30, 0f, 15), spawnRotation);
        Instantiate(wallVertical, new Vector3(-10, 0f, 15), spawnRotation);
        Instantiate(wallVertical, new Vector3(10, 0f, 15), spawnRotation);
        Instantiate(wallVertical, new Vector3(30, 0f, 15), spawnRotation);
        Instantiate(wallVertical, new Vector3(-40, 0f, 25), spawnRotation);
        Instantiate(wallVertical, new Vector3(-10, 0f, 25), spawnRotation);
        Instantiate(wallVertical, new Vector3(10, 0f, 25), spawnRotation);
        Instantiate(wallVertical, new Vector3(-40, 0f, 35), spawnRotation);
        Instantiate(wallVertical, new Vector3(-30, 0f, 35), spawnRotation);
        Instantiate(wallVertical, new Vector3(-20, 0f, 35), spawnRotation);
        Instantiate(wallVertical, new Vector3(0, 0f, 35), spawnRotation);
        Instantiate(wallVertical, new Vector3(20, 0f, 35), spawnRotation);
        Instantiate(wallVertical, new Vector3(30, 0f, 35), spawnRotation);
        Instantiate(wallVertical, new Vector3(0, 0f, 45), spawnRotation);

        //Vertical
        Instantiate(wallVertical, new Vector3(-35, 0f, -40), Quaternion.identity);
        Instantiate(wallVertical, new Vector3(-25, 0f, -40), Quaternion.identity);
        Instantiate(wallVertical, new Vector3(-15, 0f, -40), Quaternion.identity);
        Instantiate(wallVertical, new Vector3(5, 0f, -40), Quaternion.identity);
        Instantiate(wallVertical, new Vector3(15, 0f, -40), Quaternion.identity);
        Instantiate(wallVertical, new Vector3(-25, 0f, -30), Quaternion.identity);
        Instantiate(wallVertical, new Vector3(5, 0f, -30), Quaternion.identity);
        Instantiate(wallVertical, new Vector3(35, 0f, -30), Quaternion.identity);
        Instantiate(wallVertical, new Vector3(-25, 0f, -20), Quaternion.identity);
        Instantiate(wallVertical, new Vector3(-15, 0f, -20), Quaternion.identity);
        Instantiate(wallVertical, new Vector3(25, 0f, -20), Quaternion.identity);
        Instantiate(wallVertical, new Vector3(-5, 0f, -10), Quaternion.identity);
        Instantiate(wallVertical, new Vector3(25, 0f, -10), Quaternion.identity);
        Instantiate(wallVertical, new Vector3(-25, 0f, 0), Quaternion.identity);
        Instantiate(wallVertical, new Vector3(-5, 0f, 0), Quaternion.identity);
        Instantiate(wallVertical, new Vector3(5, 0f, 0), Quaternion.identity);
        Instantiate(wallVertical, new Vector3(-45, 0f, 10), Quaternion.identity);
        Instantiate(wallVertical, new Vector3(-25, 0f, 10), Quaternion.identity);
        Instantiate(wallVertical, new Vector3(15, 0f, 10), Quaternion.identity);
        Instantiate(wallVertical, new Vector3(25, 0f, 10), Quaternion.identity);
        Instantiate(wallVertical, new Vector3(35, 0f, 10), Quaternion.identity);
        Instantiate(wallVertical, new Vector3(-35, 0f, 20), Quaternion.identity);
        Instantiate(wallVertical, new Vector3(5, 0f, 20), Quaternion.identity);
        Instantiate(wallVertical, new Vector3(-35, 0f, 30), Quaternion.identity);
        Instantiate(wallVertical, new Vector3(-15, 0f, 30), Quaternion.identity);
        Instantiate(wallVertical, new Vector3(15, 0f, 30), Quaternion.identity);
        Instantiate(wallVertical, new Vector3(35, 0f, 30), Quaternion.identity);
        Instantiate(wallVertical, new Vector3(45, 0f, 30), Quaternion.identity);
        Instantiate(wallVertical, new Vector3(-45, 0f, 40), Quaternion.identity);
        Instantiate(wallVertical, new Vector3(-5, 0f, 40), Quaternion.identity);

        Instantiate(mushroom, new Vector3(-45, 0f, -15), Quaternion.identity);
        Instantiate(statue, new Vector3(-20, 0f, -15), Quaternion.identity);
        Instantiate(cross, new Vector3(-5, 1, 2), Quaternion.Euler(-90,0,0));
        Instantiate(chest, new Vector3(-20, 0f, 48), Quaternion.identity);
        Instantiate(web, new Vector3(49, 0f, 20), Quaternion.identity);
        Instantiate(web, new Vector3(-49, 0f, -19), Quaternion.identity);
        Instantiate(barrel, new Vector3(10, 0f, -20), Quaternion.identity);

        euler = new Vector3(0, 90, 0);
        Instantiate(lightBeam, new Vector3(-35, 20f, -49), Quaternion.Euler(euler));
        GameObject door_m = Instantiate(door, new Vector3(-35, 1.7f, -49), Quaternion.Euler(euler));

        arrow.GetComponent<GiantArrowCompass>().SetTarget(door_m);

        position = new Vector3(26,0,35);
        player.transform.localPosition = position;
        player.transform.position = position;

    }

}
