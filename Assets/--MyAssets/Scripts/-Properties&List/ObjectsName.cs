using UnityEngine;

public class ObjectsName : MonoBehaviour
{
    [SerializeField] private string Name;

    //Work mobile and pc (active when player click which object with this script attached)
    private void OnMouseDown()
    {
        GeometricObjectList.Instance.SelectedObjectName = Name;
        Debug.Log("You select " + Name + " object.");
    }

}
