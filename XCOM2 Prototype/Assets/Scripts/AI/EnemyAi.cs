﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAi : MonoBehaviour {

    public GameObject[] allUnits;
    public UnitConfig moveToUnit;
    public UnitConfig unitConfig;

    public bool isAttacking;
    public bool isMyTurn = false;
    bool canMove = false;

    int damage;
    public MapConfig mapConfig;
    int posLeftOrRight;
    int posUPOrDown;

    int distance = int.MaxValue;
    private void Start()
    {
        unitConfig = gameObject.GetComponent<UnitConfig>();
        mapConfig = FindObjectOfType<MapConfig>();
    }
    void Update ()
    {
        //Rotate health UI to camera rotation
        transform.GetChild(0).localEulerAngles = new Vector3(0, Camera.main.transform.root.GetChild(0).rotation.eulerAngles.y, 0);

        //HACK: AI Movement?
        if (isMyTurn && !mapConfig.turnSystem.playerTurn && unitConfig.actionPoints.CheckAvailableActions(unitConfig.unitClassStats.moveCost) && unitConfig.CheckUnitState(UnitConfig.UnitState.Idle))
        {
            
            foreach (UnitConfig unit in unitConfig.mapConfig.turnSystem.playerUnits)
            {
                IsPlayerNextToMe(unit.tileX, unit.tileY);
                if(unitConfig.actionPoints.CheckAvailableActions(unitConfig.unitClassStats.moveCost))
                {
                    break;
                }
            }
            
            if (!isAttacking)
                FindClosestPlayerUnit();
            
            unitConfig.EnemyMoveNextTile();
            if (unitConfig.actionPoints.CheckAvailableActions(2))
            {
                unitConfig.actionPoints.SubtractAllActions();
            }
        }
        if (isMyTurn && !unitConfig.actionPoints.CheckAvailableActions(1) && unitConfig.CheckUnitState(UnitConfig.UnitState.Idle))
        {
            if (mapConfig.turnSystem.enemyUnits.Count > mapConfig.turnSystem.enemyIndex)
            {
                
                isAttacking = false;
                isMyTurn = false;
                mapConfig.turnSystem.StartNextEnemy();
            }
        }
    }

    public void FindClosestPlayerUnit()
    {
        distance = int.MaxValue;
        canMove = false;
        foreach (var unit in unitConfig.mapConfig.turnSystem.playerUnits)
        {            
            FindClosestPlayerLocation(unit);
        }
        if (!canMove)
        {
            return;
        }
        unitConfig.mapConfig.tileMap.GeneratePathTo(moveToUnit.tileX + posLeftOrRight, moveToUnit.tileY + posUPOrDown, unitConfig);//make path to the closest position
    }

    private void FindClosestPlayerLocation(UnitConfig unit)
    {
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if ((x != 0 && y != 0) || (x == 0 && y == 0))//if it is looking for a diagonal pos skip to next
                    continue;

                unitConfig.mapConfig.tileMap.GeneratePathTo(unit.tileX + x, unit.tileY + y, unitConfig);
                if (unitConfig.currentPath != null)
                {
                    if (distance > unitConfig.currentPath.Count)
                    {
                        posLeftOrRight = x;
                        posUPOrDown = y;
                        distance = unitConfig.currentPath.Count;
                        moveToUnit = unit;
                        canMove = true;
                    }
                }
            }
        }
    }
    public void IsPlayerNextToMe(int tileX,int tileY)
    {
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 || y == 0)
                {
                    if (tileX == (unitConfig.tileX + x) && tileY == (unitConfig.tileY + y))
                    {
                        foreach (var unit in mapConfig.turnSystem.playerUnits)
                        {
                            if (unit.tileX == tileX && unit.tileY == tileY)
                            {
                                TurnSystem.selectedTarget = unit;
                                break;
                            }
                        }
                        if(!isAttacking)
                            unitConfig.mapConfig.turnSystem.cameraControl.SetCameraTime(0);

                        unitConfig.mapConfig.turnSystem.cameraControl.MoveToTarget(unitConfig.transform.position, true);
                        unitConfig.MeleeAttack(TurnSystem.selectedUnit,TurnSystem.selectedTarget);
                        unitConfig.actionPoints.SubtractAllActions();
                        isAttacking = true;
                        unitConfig.Attack(); //Play attack animation
                    }
                }
            }
        }
    }
}
