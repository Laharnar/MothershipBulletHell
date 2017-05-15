
using UnityEngine;

static class TransformHelper
{
    public static Transform[] GetChildrenWithoutParent(this Transform transform)
    {
        Transform[] children = transform.GetComponentsInChildren<Transform>();
        // filter parent
        Transform[] t = new Transform[children.Length - 1];
        int c = 0;
        for (int i = 0; i < children.Length; i++)
        {
            if (children[i] == transform)
            {
                continue;
            }
            t[c] = children[i];
            c++;
        }
        return t;
    }
}