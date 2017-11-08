﻿
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using UnityEditor;

public class UnitConfig : MonoBehaviour
{
    public string unitName;
    public Color[] unitColor;
    //public Transform dmgStartPos;
    //public GameObject floatingDmg;

    //Data from scriptable objects
    public WeaponInfoObject unitWeapon;
    public ClassStatsObject unitClassStats;
    public AbilityInfoObject unitAbilities;

    //Script references, internal
    [HideInInspector]public ActionPoints actionPoints;
    [HideInInspector]public Health health;
    [HideInInspector]public UnitMovement movement;
    public generateButtons generateButtons;
    //Script References, external
    [HideInInspector]public MapConfig mapConfig;

    //Unit//
    [HideInInspector] public bool isSelected = false;
    public bool isFriendly;
    public GameObject modelController;
    //Unit Position
    public int tileX;
    public int tileY;

    //grid Reference
    public List<Node> currentPath = null;



    public int movePoints;
    [SerializeField]float animaitionSpeed = 0.05f;
    public bool isMoving = false;
    public bool isSprinting = false;
    public bool isShooting = false;
    public bool isDead = false;
    public bool isHighlighted = false;

    public SoldierAnimation animatorS;
    public ZombieAnimation animatorZ;

    int pathIndex = 0;
    public float pathProgress;
    LineRenderer line;
    public EnemyAi enemyAi;
    Color currentColor;
    [HideInInspector]public Animator animatorHealthbar;
    Vector3 cameraStartPosition;

    //BaseUnitCopy
    void Start()
    {
        GameObject classModel = Instantiate(unitClassStats.classModel, modelController.transform);

        GameObject weaponModel = Instantiate(unitWeapon.weaponModel, classModel.GetComponent<WeaponPosition>().hand);

        //Initiate Variables//
        //////////////////////
        //Get Unit movement points
        movePoints = unitClassStats.maxUnitMovePoints;
        //get unit tile coordinates

        //Add the map incase its missing
        mapConfig = GameObject.FindGameObjectWithTag("Map").GetComponent<MapConfig>();
        animatorHealthbar = GetComponentInChildren<Animator>();

        if (enemyAi == null)
            InitializeEnemy();

        Vector3 tileCoords = mapConfig.tileMap.WorldCoordToTileCoord((int)transform.position.x, (int)transform.position.z);

        //Set unit position on grid
        tileX = (int)tileCoords.x;
        tileY = (int)tileCoords.z;
        
        line = GetComponent<LineRenderer>();

        if(isFriendly)
            animatorS = GetComponentInChildren<SoldierAnimation>();
        else
            animatorZ = GetComponentInChildren<ZombieAnimation>();

        //Make sure scriptable objects are assigned, if not, assign defaults and send message
        /*if (unitWeapon == null)
        {
            unitWeapon = AssetDatabase.LoadAssetAtPath<WeaponInfoObject>("Assets/Scriptable Object/Pistol.asset");
            Debug.LogWarning("Couldn't find weapon, using default weapon");
        }
        if (unitClassStats == null)
        {
            unitClassStats = AssetDatabase.LoadAssetAtPath<ClassStatsObject>("Assets/Scriptable Object/StatsRookie.asset");
            Debug.LogWarning("Couldn't find Class, using default class");
        }
        if (unitWeapon == null)
        {
            unitAbilities = AssetDatabase.LoadAssetAtPath<AbilityInfoObject>("Assets/Scriptable Object/AbilityRookie.asset");
            Debug.LogWarning("Couldn't find abilities, using default abilities");
        }*/
        actionPoints = GetComponent<ActionPoints>();
        health = GetComponent<Health>();
        movement = GetComponent<UnitMovement>();
    }

    void Update()
    {
        if (!isSelected && isFriendly)
        {
            currentPath = null;
            line.positionCount = 0;
        }
        if (isShooting)
        {
            //Vector3.RotateTowards(rotation, )
        }

        if (isMoving == true)
        {
            mapConfig.turnSystem.cameraControl.MoveToTarget(transform.position, cameraStartPosition, true);
            if (currentPath != null && pathIndex < (currentPath.Count - 1))
            {

                Vector3 previousPosition = mapConfig.tileMap.TileCoordToWorldCoord(currentPath[pathIndex].x, currentPath[pathIndex].y);
                Vector3 nextPosition = mapConfig.tileMap.TileCoordToWorldCoord(currentPath[pathIndex + 1].x, currentPath[pathIndex + 1].y);

                pathProgress += Time.deltaTime * animaitionSpeed;
                transform.position = Vector3.Lerp(previousPosition, nextPosition, pathProgress);
                //if unit have reached the end of path reset pathprogress and increase pathindex
                if (pathProgress >= 1.0)
                {

                    pathProgress = 0.0f;
                    pathIndex++;
                }
                //set unit tile postition
                tileX = currentPath[pathIndex].x;
                tileY = currentPath[pathIndex].y;

                if (mapConfig.turnSystem.playerTurn && isFriendly)
                    line.positionCount = 0;
            }

            else//when unit reach location reset special stats
            {
                isMoving = false;
                mapConfig.tileMap.UnitMapData(tileX, tileY);
             
                isSprinting = false;
                currentPath = null;
                pathIndex = 0;
                mapConfig.turnSystem.MoveMarker(mapConfig.turnSystem.unitMarker, transform.position);
                if (mapConfig.turnSystem.playerTurn)
                    mapConfig.turnSystem.cameraControl.MoveToTarget(mapConfig.turnSystem.selectedUnit.transform.position);

                if (actionPoints.actions <= 0)
                {
                    mapConfig.turnSystem.SelectNextUnit();
                }
                else if(actionPoints.actions > 0 && isFriendly && mapConfig.turnSystem.playerTurn)
                {
                    mapConfig.tileMap.ChangeGridColor(movePoints, actionPoints.actions, this);
                }
            }
        }
        //draw line need to be fixed cant be seen in the built version
        if (currentPath != null && isFriendly && !isMoving)//1 long path
        {

            if (currentPath.Count < movePoints + 2 && actionPoints.actions > 1)//Walk
            {
                currentColor = mapConfig.turnSystem.lineColors[0];
            }
            else//dash
            {
                currentColor = mapConfig.turnSystem.lineColors[1];
            }

            for (int i = 0; i < mapConfig.turnSystem.markerImage.Length; i++)
            {
                mapConfig.turnSystem.markerImage[i].color = currentColor;
            }
            line.startColor = currentColor;
            line.endColor = currentColor;

            int currNode = 0;
            while (currNode < currentPath.Count - 1 && currNode < movePoints * actionPoints.actions)
            {
                Vector3 start = mapConfig.tileMap.TileCoordToWorldCoord(currentPath[currNode].x, currentPath[currNode].y);
                Vector3 end = mapConfig.tileMap.TileCoordToWorldCoord(currentPath[currNode + 1].x, currentPath[currNode + 1].y);
                line.positionCount = currNode + 1;
                if (currentPath.Count == 2)
                {
                    line.positionCount = 2;
                    line.SetPosition(0, new Vector3(transform.position.x, 0.1f, transform.position.z));
                    line.SetPosition(1, new Vector3(end.x, 0.1f, end.z));
                }
                if (currNode > 0)
                {
                    line.SetPosition(currNode, new Vector3(end.x, 0.1f, end.z));
                }
                else
                {
                    line.SetPosition(currNode, new Vector3(start.x, 0.1f, start.z));
                }

                currNode++;

                if (line.positionCount > 0 && mapConfig.turnSystem.cursorMarker.position != end)
                {
                    mapConfig.turnSystem.MoveMarker(mapConfig.turnSystem.cursorMarker, end);
                }
            }
        }
    }
    public void InitializeEnemy()
    {
        mapConfig = GameObject.FindGameObjectWithTag("Map").GetComponent<MapConfig>();
        Vector3 tileCoords = mapConfig.tileMap.WorldCoordToTileCoord((int)transform.position.x, (int)transform.position.z);
        enemyAi = GetComponent<EnemyAi>();
        tileX = (int)tileCoords.x;
        tileY = (int)tileCoords.z;
        mapConfig.tileMap.UnitMapData(tileX, tileY);
        actionPoints.unitConfig = this;
    }
    //HACK: Finish this code block when abilities work!
    public void attackUnit(UnitConfig target)
    {
        //Checks if it is the players turn
        if (mapConfig.turnSystem.playerTurn) 
        {
            //Checks if the unit has enough action points
            if (actionPoints.actions > 0) 
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    //Checks if the unit clicked on an enemy
                    if (hit.collider.GetComponent<UnitConfig>()) 
                    {
                        target = hit.collider.GetComponent<UnitConfig>();
                        //Checks if the unit hit is not friendly
                        if (!target.isFriendly) 
                        {
                            //Uses current weapon
                            CalculationManager.HitCheck(unitWeapon, mapConfig.turnSystem.distance);
                            target.health.TakeDamage(CalculationManager.damage, unitWeapon);

                            //Spend Actions
                            mapConfig.turnSystem.totalActions -= target.actionPoints.actions;
                            actionPoints.SubtractAllActions();
                            //Move camera to next unit
                            mapConfig.turnSystem.SelectNextUnit();
                        }
                    }
                }
            }
        }
    }

    public void ShootTarget(UnitConfig target)
    {
        isShooting = true;
        if (isFriendly)
            animatorS.target = target;
        else
            animatorZ.target = target;
    }

    public void MoveNextTile()//start to try to move unit
    {
        if (currentPath == null || isShooting)// if there is no path (or unit shoots) leave function
        {
            return;
        }

        
        
        
        int remainingMovement = movePoints * 2;
        int moveTo = currentPath.Count - 1;
        for (int cost = 1; cost < moveTo; cost++)//is the path possible
        {
            remainingMovement -= (int)mapConfig.tileMap.CostToEnterTile(currentPath[cost].x, currentPath[cost].y, currentPath[1 + cost].x, currentPath[1 + cost].y);
        }
        if (remainingMovement > movePoints)//can you move the unit 
        {
            mapConfig.turnSystem.cameraControl.SetCameraTime(0);
            cameraStartPosition = mapConfig.turnSystem.cameraControl.GetCameraPosition();
            isMoving = true;//start moving in the update
            mapConfig.tileMap.ResetColorGrid();
            mapConfig.tileMap.removeUnitMapData(tileX, tileY);
            animaitionSpeed = 2;
            actionPoints.actions--;
            mapConfig.turnSystem.totalActions--;
            return;
        }
        if (remainingMovement > 0 && actionPoints.actions > 1)//can you move the unit 
        {
            mapConfig.turnSystem.cameraControl.SetCameraTime(0);
            cameraStartPosition = mapConfig.turnSystem.cameraControl.GetCameraPosition();
            isSprinting = true;
            isMoving = true;//start moving in the update
            mapConfig.tileMap.ResetColorGrid();
            mapConfig.tileMap.removeUnitMapData(tileX, tileY);
            animaitionSpeed = 4;
            actionPoints.actions = 0;
            mapConfig.turnSystem.totalActions--;
            return;
        }
        else//is too far away do not move
        {
            return;
        }
        
    }
    public void EnemyMoveNextTile()//start to try to move unit
    {

        if (currentPath == null)// if there is no path leave funktion
        {
            return;
        }
        mapConfig.tileMap.removeUnitMapData(tileX, tileY);
        int remainingMovement = movePoints;
        int moveTo = currentPath.Count - 1;
        for (int cost = 0; cost < moveTo; cost++)//is the path posseble
        {
            remainingMovement -= (int)mapConfig.tileMap.CostToEnterTile(currentPath[cost].x, currentPath[cost].y, currentPath[1 + cost].x, currentPath[1 + cost].y);
        }

        if (remainingMovement > 0)//can you move the unit 
        {
            mapConfig.turnSystem.cameraControl.SetCameraTime(0);
            cameraStartPosition = mapConfig.turnSystem.cameraControl.GetCameraPosition();
            isMoving = true;//start moving in the update
            actionPoints.SubtractActions(1);
            return;
        }

        else//is too far away do not move
        {

            remainingMovement = movePoints * 2;

            for (int i = currentPath.Count - 1; i > remainingMovement; i--)
            {
                currentPath.RemoveAt(i);
            }
            if (currentPath != null)
            {
                mapConfig.turnSystem.cameraControl.SetCameraTime(0);
                cameraStartPosition = mapConfig.turnSystem.cameraControl.GetCameraPosition();
                isMoving = true;
                actionPoints.SubtractActions(2);
            }
            return;
        }
        
    }
    public void Die()//
    {
        isDead = true;
    }

    public void Attack()//
    {
        isShooting = true;
    }

}
