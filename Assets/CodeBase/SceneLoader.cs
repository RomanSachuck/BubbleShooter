using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

    public class SceneLoader : MonoBehaviour
    {
        public void Load(string name) =>
            StartCoroutine(LoadScene(name));
        private IEnumerator LoadScene(string nexScene)
        {
            if(SceneManager.GetActiveScene().name == nexScene)
            {
                yield break;
            }

            AsyncOperation waitNextScene = SceneManager.LoadSceneAsync(nexScene);

            while(!waitNextScene.isDone) 
                yield return null;
        } 
    }
