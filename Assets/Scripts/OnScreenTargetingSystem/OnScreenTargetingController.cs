using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class OnScreenTargetingController : MonoBehaviour
{
    private static OnScreenTargetingController instance;
    public static OnScreenTargetingController Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<OnScreenTargetingController>(true);
            }

            return instance;
        }
    }


    public event Action<IOnScreenTargetObject> onTargetObjectAdded;
    public event Action<IOnScreenTargetObject> onTargetObjectRemoved;


    private List<IOnScreenTargetObject> targetObjects = new List<IOnScreenTargetObject>();
    public List<IOnScreenTargetObject> TargetObjects => targetObjects;


    [SerializeField]
    private Transform playerTransform;
    public Transform PlayerTransform => playerTransform;


    public void SetPlayerTransform(Transform _playerTransform)
    {
        playerTransform = _playerTransform;
    }


    public void SortNullTargets()
    {
        targetObjects = targetObjects.Where(x => x != null).ToList();
    }


    public void AddTargetObject(IOnScreenTargetObject targetObject)
    {
        targetObjects.Add(targetObject);

        onTargetObjectAdded?.Invoke(targetObject);
    }

    public void RemoveTargetObject(IOnScreenTargetObject targetObject)
    {
        targetObjects.Remove(targetObject);

        onTargetObjectRemoved?.Invoke(targetObject);
    }
}