﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAi : MonoBehaviour {

    public GameObject[] allUnits;
    public GameObject moveToUnit;
    public UnitConfig unitConfig;
    public MapConfig mapConfig;
    int damage;

    private List<UnitConfig> playerUnits;

    private void Start()
    {
        mapConfig = GameObject.FindGameObjectWithTag("Map").GetComponent<MapConfig>();
    }
    void Update ()
    {
        //HACK: AI Movement?
        transform.GetChild(0).localEulerAngles = new Vector3(0, Camera.main.transform.root.GetChild(0).rotation.eulerAngles.y, 0);
        if (!mapConfig.turnSystem.playerTurn && unitConfig.actionPoints.actions > 0 && unitConfig.isMoving == false)
        {
            FindClosestPlayerUnit();
            UnitConfig closestUnit = moveToUnit.GetComponent<UnitConfig>();
            mapConfig.tileMap.GeneratePathTo(closestUnit.tileX, closestUnit.tileY, gameObject.GetComponent<UnitConfig>());
            //HACK: zombie attack?
            if (unitConfig.currentPath.Count < 3)
            {
                closestUnit.health.TakeDamage(damage);
                unitConfig.actionPoints.actions = 0;
            }
            //HACK: move?
            else
            {
                unitConfig.EnemyMoveNextTile();
            }
        }
    }

    public void FindClosestPlayerUnit()
    {
        List<UnitConfig> listOfPlayers = new List<UnitConfig>();
        int[] listOfPlayersNumber = new int[mapConfig.turnSystem.playerUnits.Count];//an array with lenght of have many players currently exist
        float distance = Mathf.Infinity;
        Vector3 position = transform.position;
        
        for (int i = 0; i < mapConfig.turnSystem.playerUnits.Count; i++)
        {

            Vector3 diff = mapConfig.turnSystem.playerUnits[i].transform.position - position;
            float curDistance = diff.sqrMagnitude;
            if (curDistance < distance)
            {
                distance = curDistance;
                moveToUnit = mapConfig.turnSystem.playerUnits[i].gameObject;
            }
        }
    }
}
