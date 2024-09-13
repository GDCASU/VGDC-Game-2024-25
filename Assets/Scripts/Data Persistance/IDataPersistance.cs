using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* -----------------------------------------------------------
 * Author:
 * Ian Fletcher
 * 
 * Modified By:
 * 
 */// --------------------------------------------------------

/* -----------------------------------------------------------
 * Pupose:
 * Interfaces to be implemented by scripts that need to save
 * or load data from different play sessions
 */// --------------------------------------------------------

public interface IDataPersistance
{
    /// <summary>
    /// Load data from file
    /// </summary>
    void LoadData();

    /// <summary>
    /// Save Data to file
    void SaveData();
}
