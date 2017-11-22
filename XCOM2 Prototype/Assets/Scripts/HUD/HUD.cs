﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class HUD : MonoBehaviour {
    public GameObject alienUI;
    public GameObject playerUI;
    public GameObject endButton;
    Animator alienAnim;
    public Text turnCounter;
    public Text warningText;
    public Text victoryText;
    public Color playerColor;
    public Color enemyColor;
    public Color victoryColor;
    public Color defeatColor;
    public GameObject warning;
    public victoryCheck victoryScript;

    [HideInInspector]public int amountTurns;
    int maxTurns;
    public bool isPlayerTurn;
    
    public MapConfig mapConfig;

    void Start () {
        mapConfig = FindObjectOfType<MapConfig>();
        amountTurns = 1;
        isPlayerTurn = true;
        maxTurns = mapConfig.turnSystem.getCurrentTurn(amountTurns); //Sets max turns and prints it out
        turnCounter.text = amountTurns + "/" + maxTurns;
        alienAnim = alienUI.GetComponent<Animator>();
    }


    public void pressEnd(bool forceEnd)
    {
        warning.SetActive(false);
        if (TurnSystem.selectedUnit != null && !TurnSystem.selectedUnit.CheckUnitState(UnitConfig.UnitState.Idle))
        {
            return;
        }
        if (TurnSystem.totalActions <= 0 || !isPlayerTurn || forceEnd) //If player has used all actions he is taken to the next turn
        {
            isPlayerTurn = !isPlayerTurn;
            mapConfig.turnSystem.ToggleMarkers(isPlayerTurn);

            if (!isPlayerTurn)
            {
                playerUI.SetActive(false);
                alienUI.SetActive(true);
                alienAnim.Play("AlienActivityOn");
                mapConfig.turnSystem.enemyIndex = 0;
                if(mapConfig.turnSystem.playerUnits.Count > 0)
                    mapConfig.turnSystem.spawnEnemy();

                endButton.SetActive(false);
                mapConfig.tileMap.ResetColorGrid();
                mapConfig.turnSystem.className.gameObject.SetActive(false);

                if (mapConfig.turnSystem.playerUnits.Count < 1)
                {
                    victoryScript.winCheck(false);
                }
            }
            else
            {
                if(mapConfig.turnSystem.playerUnits.Count < 1)
                {
                    victoryScript.winCheck(false);
                }
                playerUI.SetActive(true);
                alienUI.SetActive(false);
                endButton.SetActive(true);
                mapConfig.turnSystem.className.gameObject.SetActive(true);
                mapConfig.turnSystem.cameraControl.playerMovedCamera = false;
                mapConfig.turnSystem.SelectUnit(mapConfig.turnSystem.playerUnits[0]);
            }
            //Add all functionality here, END TURN
            mapConfig.turnSystem.ResetActions(isPlayerTurn);
            
            
            if (isPlayerTurn && TurnSystem.selectedUnit != null)
            {
                mapConfig.turnSystem.KeyboardSelect(true, mapConfig.turnSystem.playerUnits, TurnSystem.selectedUnit);
            }

            if (isPlayerTurn)
                amountTurns++;

            maxTurns = mapConfig.turnSystem.getCurrentTurn(amountTurns); //Sets max turns and sends current turn to turn system
            


            if (amountTurns <= maxTurns) //Displays VICTORY instead of the turn if the player won
                turnCounter.text = amountTurns + "/" + maxTurns;
            else
            {
                victoryText.text = "VICTORY";                                                                                                                               
                victoryText.color = mapConfig.turnSystem.victoryColor;
            }
        }
        else //Show warning if player has more than 0 actions
        {
            warning.SetActive(true);
            warningText.text = "Are you sure? You still have " + TurnSystem.totalActions + " actions left";
        }
    }
    public void abortPress()
    {
        if (warning.activeSelf)
        {
            warning.SetActive(false);
        }
    }
}