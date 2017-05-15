using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SimpleEngineUnit : SimpleUnit {


    internal SceneDependantAI followAi;
    public SceneDependantAI followAiChild;

    public SpriteRenderer mainEngine;
    public SpriteRenderer rEngine;
    public SpriteRenderer lEngine;

    public FlameVfx[] vfxs;// 3 vfxs, 1 for every engine, main, left, right

    public SimpleUnit tempSource;
    public SimpleUnit tempSourceChild;

    Transform tempChild;

    FlyState lastFlyState;
    public FlyState flyingState = FlyState.NoEngine;
    public enum FlyState
    {
        NoEngine,
        LeftSideEngine,
        RightSideEngine,
        MainLeftSideEngine,
        MainRightSideEngine,
        MainEngine,
        Stop,
        Undefined
    }
    
    internal bool useMainEngine { get; private set; }
    internal bool useLeftEngine { get; private set; }
    internal bool useRightEngine { get; private set; }

    new void Awake()
    {
        base.Awake();

        followAi = GetComponent<SceneDependantAI>();

        StartCoroutine(Flying());
    }

    IEnumerator Flying()
    {
        while (true)
        {
            useMainEngine = flyingState == FlyState.MainEngine ||
                flyingState == FlyState.MainLeftSideEngine ||
                flyingState == FlyState.MainRightSideEngine;
            useLeftEngine = flyingState == FlyState.LeftSideEngine ||
                flyingState == FlyState.MainLeftSideEngine;
            useRightEngine = flyingState == FlyState.RightSideEngine ||
                flyingState == FlyState.MainRightSideEngine;

            useMainEngine = engines["goingForward"];
            useLeftEngine = engines["goingLeft"];
            useRightEngine = engines["goingRight"];

            BurnoutEffects(useMainEngine, useLeftEngine, useRightEngine);

            yield return null;
        }
    }

    void BurnoutEffects(bool useMengine, bool useLengine, bool useRengine)
    {
        float modifier = lastSteering == steering ? 1 : 0.5f;
        if (useMengine)
        {
            float moveModifier = engines["acceleratingForward"] ? 1 : 0.75f;
            vfxs[0].Burn(useMengine, moveModifier);
        }
        // flame effect on all engines that are on
        vfxs[1].Burn(useLengine, modifier);
        vfxs[2].Burn(useRengine, modifier);
    }
}
