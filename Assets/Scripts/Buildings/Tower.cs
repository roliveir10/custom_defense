using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : Building, IInteractable
{
    void IInteractable.ActionCanceled()
    {
        throw new System.NotImplementedException();
    }

    void IInteractable.ActionDown()
    {
        throw new System.NotImplementedException();
    }

    void IInteractable.ActionUp()
    {
        BuildingPanel.SetActive(!BuildingPanel.activeInHierarchy);
    }
}