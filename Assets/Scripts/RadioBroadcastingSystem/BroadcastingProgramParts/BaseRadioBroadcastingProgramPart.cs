using UnityEngine;


//[CreateAssetMenu(fileName = "RadioBroadcastingProgramPart", menuName = "RadioBroadcastingSystem/RadioBroadcastingProgramPart")]
public abstract class BaseRadioBroadcastingProgramPart : ScriptableObject
{
    [TextArea(2, 5)]
    [SerializeField]
    protected string m_programPartTitle;
    public string ProgramPartTitle => m_programPartTitle;


    public abstract float GetProgramPartSecondsLength();
}