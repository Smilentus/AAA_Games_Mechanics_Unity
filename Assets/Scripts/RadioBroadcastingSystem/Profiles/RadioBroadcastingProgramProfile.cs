using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "RadioBroadcastingProgramProfile", menuName = "RadioBroadcastingSystem/RadioBroadcastingProgramProfile")]
public class RadioBroadcastingProgramProfile : ScriptableObject
{
    [SerializeField]
    protected string m_programName;
    public string ProgramName => m_programName;


    [TextArea(5, 10)]
    [SerializeField]
    protected string m_programDescription;
    public string ProgramDescription => m_programDescription;


    [SerializeField]
    protected List<BaseRadioBroadcastingProgramPart> m_programParts = new List<BaseRadioBroadcastingProgramPart>();
    public List<BaseRadioBroadcastingProgramPart> ProgramParts => m_programParts;


    public float ProgramLength
    {
        get
        {
            float output = 0;

            foreach (BaseRadioBroadcastingProgramPart part in m_programParts)
            {
                output += part.GetProgramPartSecondsLength();
            }

            return output;
        }
    }
}