using System;
using System.Collections;
using UnityEngine;

public static class MovementCommands {


    public static void ExecuteRotationTo(this SimpleUnit getSource, float direction)
    {
        if (direction < 0) {
            getSource.ExecuteMove(SimpleUnit.Moves.right);
        } else if (direction > 0) {
            getSource.ExecuteMove(SimpleUnit.Moves.left);
        }
    }

    public static void SmoothRotateTowards(this Transform transform, Vector3 target, float mobility, LerpedSideEngine sideEngine)
    {
        if (sideEngine.restrictRotation) {
            mobility = mobility * 0.5f;// increase time it takes to make the turn
        }

        Quaternion newRotation = Quaternion.LookRotation(transform.position - target, Vector3.forward);
        newRotation.x = 0.0f;
        newRotation.y = 0.0f;
#if realisticRotation
        //transform.rotation = Quaternion.Lerp(transform.rotation, newRotation, rotationCurve.Evaluate(Time.deltaTime * mobility));
#endif

        transform.rotation = Quaternion.Lerp(transform.rotation, newRotation, Time.deltaTime * mobility);
        Debug.DrawLine(transform.position, target);
    }

    /// <summary>
    /// Moves forwards and stops execution after some time
    /// </summary>
    /// <param name="source"></param>
    /// <param name="seconds"></param>
    /// <returns></returns>
    public static IEnumerator SimpleTimedMove(SimpleUnit source, SimpleUnit.Moves move, float seconds, bool idleAtEnd = false)
    {
        source.ExecuteMove(move);
        yield return new WaitForSeconds(seconds);
        if (idleAtEnd)
        {
            source.Idle();
        }
    }

}
