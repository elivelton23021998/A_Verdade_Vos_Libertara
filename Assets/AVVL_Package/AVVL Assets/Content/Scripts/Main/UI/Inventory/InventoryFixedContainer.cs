using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryFixedContainer : MonoBehaviour
{
    public string ContainerName;

    public void UseObject()
    {
        Inventory.Instance.ShowFixedInventoryContainer(ContainerName);
    }
}