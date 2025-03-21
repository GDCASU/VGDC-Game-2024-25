using UnityEngine;

/* -----------------------------------------------------------
* Author:
* Chandler Van
* 
* Modified By:
*/// --------------------------------------------------------

/* -----------------------------------------------------------
 * Purpose:
 * This is just an example implementation and not meant to be seen in production builds
*/// --------------------------------------------------------

public class TestNPCIan : NPC
{
    public TextAsset scaredConversation;
    private bool scaredByPlayer = false;

    public override void OnDialogEnd()
    {
        DropItems();

        base.OnDialogEnd();
    }

    public override void OnDialogStart()
    {
        if(!scaredByPlayer)
            DropItems();
        base.OnDialogStart();
    }

    public override void OnPlayerAttack(float distance, bool canSeePlayer)
    {
        if(canSeePlayer && distance < 5 && !scaredByPlayer)
        {
            DropItems();
            scaredByPlayer = true;
            dialog.SetDialogScript(scaredConversation);
        }
    }
}
