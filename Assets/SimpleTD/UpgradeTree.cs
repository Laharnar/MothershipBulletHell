using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UpgradeTree : MonoBehaviour {

    public bool isstartingTree;
    public bool isendTree;
    public int curLevel= 0;
    public int levelLimit= 1;
    public Requirement[] requiresBeforeUnlocked;
    
    [System.Serializable]public class Requirement
    {
        public int requiredLevel=1;
        public UpgradeTree upgrade;
    }

    public bool requiermentsFullfiled;// they are fulfiled if you can reach starting upgrade by any path
    public bool bought { get { return curLevel > 0; } }
     Button btn;

	// Use this for initialization
	void Start () {

        btn = GetComponent<Button>();
        btn.interactable = requiermentsFullfiled;

    }

    // Update is called once per frame
    void Update () {
        if (!requiermentsFullfiled && AreRequirmentFulfilled())
        {
            requiermentsFullfiled = true;
            btn.interactable = requiermentsFullfiled;
        }

    }
    
    /// <summary>
    /// Search is done backwards from item that called for it
    /// </summary>
    /// <returns></returns>
    bool AreRequirmentFulfilled()
    {
        if (requiresBeforeUnlocked.Length == 0)
        {
            return true;
        }
        foreach (var requierment in requiresBeforeUnlocked)
        {
            if (requierment.upgrade.bought && requierment.upgrade.curLevel >= requierment.requiredLevel)
            {
                bool pathClear = requierment.upgrade.AreRequirmentFulfilled();
                if (pathClear)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public void Upgrade()
    {
        print("dsds");
        curLevel = Mathf.Clamp(curLevel+1, 0, levelLimit);

        if (curLevel < levelLimit)
        {
            return;
        }
        btn.interactable = false;
    }
}
