using UnityEngine;

/* -----------------------------------------------------------
 * Author:
 * Ian Fletcher
 * 
 * Modified By:
 * 
 */// --------------------------------------------------------

/* -----------------------------------------------------------
 * Purpose:
 * Hold a list of custom editor attributes to make programming the
 * game easier
 */// --------------------------------------------------------


/// <summary>
/// Creates a [InspectorReadOnly] Attribute so we can expose values
/// to the inspector while not allowing its editting.
/// </summary>
public class InspectorReadOnly : PropertyAttribute
{

}