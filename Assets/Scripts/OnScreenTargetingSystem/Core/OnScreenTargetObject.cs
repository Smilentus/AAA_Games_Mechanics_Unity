using UnityEngine;


public class OnScreenTargetObject : MonoBehaviour, IOnScreenTargetObject
{
    public Transform TargetTransform => this.transform;


    private void Start()
    {
        InjectTargetObject();
    }

    private void OnDestroy()
    {
        RemoveTargetObject();
    }


    public void InjectTargetObject()
    {
        OnScreenTargetingController.Instance?.AddTargetObject(this);
    }

    public void RemoveTargetObject()
    {
        OnScreenTargetingController.Instance?.RemoveTargetObject(this);
    }
}