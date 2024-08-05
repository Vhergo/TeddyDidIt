using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnpauseScene : MonoBehaviour
{
    public void Unpause()
    {
        MySceneManager.Instance.PauseGameToggle();
    }
}
