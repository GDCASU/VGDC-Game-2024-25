using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.SceneManagement;

/* -----------------------------------------------------------
 * Author:
 * Matthew Glos
 */// --------------------------------------------------------

/* -----------------------------------------------------------
 * Pupose:
 * Centralize scene loading functionality for all objects in the scene
 */// --------------------------------------------------------
public class Scene_Transition_Script : MonoBehaviour
{


    public Animator colorDip;
    public float transitionTime = 0.2f;


    public void LoadSceneByName(string SceneName, Vector3 target) {
        StartCoroutine(LoadSceneByNameAnimated(SceneName,target));
    
    }
    IEnumerator LoadSceneByNameAnimated(string SceneName, Vector3 target) {
        colorDip.SetTrigger("End");
        yield return new WaitForSeconds(transitionTime);
        AsyncOperation aSync = SceneManager.LoadSceneAsync(SceneName,LoadSceneMode.Single);

        while (!aSync.isDone) {
            yield return null;
        }

        CharacterController cController = GameObject.Find("Player").GetComponent<CharacterController>();
        cController.enabled = false;
        GameObject.Find("Player").GetComponent<Transform>().position = target + new Vector3(0,0,-0.5f);
        cController.enabled = true;


        colorDip.SetTrigger("Start");

    }


}
