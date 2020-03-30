using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    void ActionDown();

    void ActionUp();

    void ActionCanceled();
}
