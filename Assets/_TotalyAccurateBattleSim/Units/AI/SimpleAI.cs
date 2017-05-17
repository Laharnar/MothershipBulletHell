using UnityEngine;
using System.Collections;

/// <summary>
/// Test class for simple unit class
/// </summary>
public class SimpleAI : MonoBehaviour {

    SimpleUnit source;
    Transform target;

    // Use this for initialization
    void Start()
    {
        source = GetComponent<SimpleUnit>();
        StartCoroutine(SimplestAI());
    }

    void OnTriggerEnter2D(Collider2D collider2d)
    {
        target = collider2d.transform;
    }



    IEnumerator SimplestAI()
    {
        
        while (Time.time < 5)
        {
            source.ExecuteMove(SimpleUnit.Moves.forward);
            yield return null;
        }
        Debug.Log("pass");
        while (Time.time < 10)
        {
            source.ExecuteMove(SimpleUnit.Moves.back);
            yield return null;
        }
        source.Idle(true, true);
        while (Time.time < 15)
        {
            //source.ExecuteMove(SimpleUnit.Moves.left);
            source.ExecuteMove(SimpleUnit.Moves.forward);
            yield return null;
        }
        while (Time.time < 20)
        {
            source.ExecuteMove(SimpleUnit.Moves.right);
            yield return null;
        }
        source.Idle();
    }
}
