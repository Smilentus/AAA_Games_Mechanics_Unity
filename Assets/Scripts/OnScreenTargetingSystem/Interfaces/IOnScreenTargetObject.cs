using UnityEngine;


public interface IOnScreenTargetObject
{
    public Transform TargetTransform { get; }


    public void InjectTargetObject();
    public void RemoveTargetObject();
}