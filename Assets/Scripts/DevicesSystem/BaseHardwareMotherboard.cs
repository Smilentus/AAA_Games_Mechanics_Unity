using System;
using System.Collections.Generic;
using UnityEngine;


public class BaseHardwareMotherboard : MonoBehaviour
{
    [SerializeField]
    private List<BaseHardwareBlock> m_installedHardwareBlocks = new List<BaseHardwareBlock>();
    public IEnumerable<BaseHardwareBlock> InstalledHardwareBlocks => m_installedHardwareBlocks;


    public T GetHardwareBlockType<T>() where T : BaseHardwareBlock
    {
        foreach (BaseHardwareBlock baseHardwareBlock in m_installedHardwareBlocks)
        {
            if (baseHardwareBlock.GetType().Equals(typeof(T)))
            {
                return (T)baseHardwareBlock;
            }
        }

        return default(T);
    }


    public void InstallHardwareBlock(BaseHardwareBlock baseHardwareBlock)
    {
        if (m_installedHardwareBlocks.Contains(baseHardwareBlock)) return;

        m_installedHardwareBlocks.Add(baseHardwareBlock);
    }

    public void RemoveHardwareBlock(BaseHardwareBlock baseHardwareBlock)
    {
        if (!m_installedHardwareBlocks.Contains(baseHardwareBlock)) return;

        m_installedHardwareBlocks.Remove(baseHardwareBlock);
    }


    public bool IsHardwareBlockTypeInstalled<T>() where T : BaseHardwareBlock
    {
        foreach (BaseHardwareBlock baseHardwareBlock in m_installedHardwareBlocks)
        {
            if (baseHardwareBlock.GetType().Equals(typeof(T)))
            {
                return true;
            }
        }

        return false;
    }


    public virtual bool IsMinimalVitalBlocksInstalled()
    {
        return IsHardwareBlockTypeInstalled<CpuHardwareBlock>() &&
               IsHardwareBlockTypeInstalled<GraphicsHardwareBlock>() &&
               IsHardwareBlockTypeInstalled<MemoryHardwareBlock>() &&
               IsHardwareBlockTypeInstalled<BatteryHardwareBlock>();
    }
}