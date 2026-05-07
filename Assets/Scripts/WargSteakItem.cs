using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WargSteakItem : Item
{
    public override void Pickup()
    {
        SoundEffectManager.Play("victory");
        ScoreManager.Calculate(FindObjectOfType<InventoryController>());
        LoadWin();
        base.Pickup();


    }

    private void LoadWin()
    {
        Debug.Log("loading now");
        GameObject.FindAnyObjectByType<SceneTransition>().InvokeWin();
    }
}
