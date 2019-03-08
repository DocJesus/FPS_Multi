using UnityEngine;

public class Util
{
    //set le layer a un objet donné et à ses enfants
    public static void SetLayerRecursiv(GameObject obj, int layer)
    {
        if (obj == null)
            return;

        obj.layer = layer;

        foreach (Transform child in obj.transform)
        {
            SetLayerRecursiv(child.gameObject, layer);
        }
    }
}
