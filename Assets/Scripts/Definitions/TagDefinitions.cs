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
 * Have the string tags for collision checking be defined on a centralized location so we can change the tag names
 * without having to fix every string on the game
 */// --------------------------------------------------------


/// <summary>
/// Collection of tag definitions
/// </summary>
public static class TagDefinitions
{
  public const string PlayerTag = "Player";  
  public const string EnemyTag = "Enemy";
  public const string CollectibleTag = "Collectible";
  public const string InteractableTag = "Interactable";
  public const string ProjectileTag = "Projectile";
  public const string GroundTag = "Ground";
  public const string WallTag = "Wall";
  
}
