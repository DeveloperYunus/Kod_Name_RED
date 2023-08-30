using UnityEngine;

public class ObjectProduce : MonoBehaviour
{
    private void OnMouseDown()
    {
        GeometricObjectList.Instance.ProduceObject();
    }
}
