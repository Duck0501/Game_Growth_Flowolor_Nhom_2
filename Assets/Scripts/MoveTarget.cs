using UnityEngine;

public class MoveTarget : MonoBehaviour
{
    void OnMouseDown()
    {
        GameManagerMove.Instance.SwapWithSlot(transform.parent);
    }
}
