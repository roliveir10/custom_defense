using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscoverButton : MonoBehaviour, IInteractable
{
    [SerializeField] private Vector3Int cellPos = Vector3Int.zero;

    public Vector3Int CellPos
    {
        get { return cellPos; }
        set { cellPos = value; }
    }

    private void AnimDiscoverButton()
    {
        Animator animator = GetComponent<Animator>();
        if (animator)
        {
            bool isClicked = animator.GetBool("click");
            animator.SetBool("click", !isClicked);
        }
    }

    void IInteractable.ActionCanceled()
    {
        AnimDiscoverButton();
    }
    void IInteractable.ActionDown()
    {
        AnimDiscoverButton();
    }
    void IInteractable.ActionUp()
    {
        Discover.Instance.DiscoverPressed(cellPos);
    }
}
