﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GenerateButtons : MonoBehaviour
{

    AbilityInfoObject characterClass;
    [SerializeField] AbilityButton abilityButtonPrefab;
    [SerializeField] Text abilityTooltip;
    AbilityButtonFunctions abilityButtonFunctions;

    public void GenerateCurrentButtons(AbilityInfoObject characterClass)
    {

        foreach (AbilityInfo ability in characterClass.abilities)
        {
            //Create gameobject with veriables from Class/Abilities
            AbilityButton newButton = GameObject.Instantiate(abilityButtonPrefab, transform);
            newButton.abilityName.text = ability.name;
            newButton.abilityButton.GetComponent<AbilityButtonFunctions>().useAbility = ability.keybind;
            newButton.abilityIcon.sprite = ability.icon;
            newButton.abilityKeybind = ability.keybind;
            newButton.abilityTooltip = ability.tooltip;
            newButton.abilityButton.onClick.AddListener(() => { ability.callbackFunction.Invoke(); });
        }
    }
    public void ClearCurrentButtons()
    {
        foreach (Transform child in gameObject.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }
}
