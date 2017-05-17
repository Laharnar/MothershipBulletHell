using UnityEngine;
using System.Collections;
using System;

public abstract class Quest {
    public string tag;
    /// <summary>
    /// 0 to 100 percentage
    /// </summary>
    public int completed;
    //public bool Completed { get { return completed == 0 ? false : true; } }

    // on complete, pass itself as parameter
    public Action<Quest> onCompleted;

    public Quest(string tag, Action<Quest> callback) {
        this.tag = tag;
        this.completed = 0;
        onCompleted += callback;
    }

    public void Completed() {
        completed = 100;

        if (onCompleted != null) {
            onCompleted(this);
        }
    }
}

public class DestroyQuest : ScoredQuest {
    MarkedQuestTarget target;

    public DestroyQuest(string tag, Action<Quest> callback, int worth, MarkedQuestTarget target) : base(tag, callback, worth) {
        this.target = target;
        target.Init(this);
    }

}


[System.Obsolete("Not used yet, you can safely remove it.")]
public class ScoredQuest : Quest {
    /// <summary>
    /// How much is this quest worth towards progress, 0-100
    /// </summary>
    internal int weightScore;
    public ScoredQuest(string tag, Action<Quest> callback, int worth) : base(tag, callback) {
        weightScore = worth;
    }
}

[System.Obsolete("Not used yet, you can safely remove it.")]
class ScoredQuests:Quest {
    ScoredQuest[] quests;

    public ScoredQuests(string tag, Action<Quest> callback, params ScoredQuest[] quests) : base(tag, callback) {
        this.quests = quests;
        for (int i = 0; i < this.quests.Length; i++) {
            this.quests[i].onCompleted += OnSubQuestComplete;
        }
    }

    void OnSubQuestComplete(Quest source) {
        if (SumScores(this) >= 100) {
            Completed();
        }
    }

    static int SumScores(ScoredQuests sq) {
        int score = 0;
        for (int i = 0; i < sq.quests.Length; i++) {
            score += sq.quests[i].weightScore;
        }
        return score;
    }
}


/// <summary>
/// Mothership in a shape of a cross. It has attached 4 modules, 2 engine and 2 carrier.
/// </summary>
public class CrossMothership : MothershipBase {


    /** Behaviour:
     * Create carriers that attack the enemy.
     * Four sides can get destroyed by hitting critical point.
     * The ship slowly moves around.
     * If in danger, it teleports away.
     * */
    // quest
    public MarkedQuestTarget boss;
    public DestroyQuest bossQuest;
    public MarkedQuestTarget[] questTargets;
    public DestroyQuest[] partsOfBossQuest;

    // spawning
    SpawnController spawner;

    Targeting targeting;

    // todo: parts of spaceship have different hp.

    /// <summary>
    /// Does Ai want the carriers to come
    /// </summary>
    bool requestBuild = false;

    protected new void Awake() {
        base.Awake();

        AiSetup();

        StartCoroutine(BuildCarriers());

        QuestSetup();
    }

    #region Ai
    void AiSetup() {
        root = new DecisionMaxChoice("",
                new UtilityNode(SendCarriers, 
                    new ArgsFn(NoTarget, -1f), // if hp and shields are low it will still spawn until shields regen
                    new ArgsFn(HpLow),
                    new ArgsFn(ShieldsDamaged)
                ),
                new UtilityNode(Search,
                    new ArgsFn(NoTarget, 1f)
                    )/*,
                new UtilityNode(Clockwise,
                        new ArgsFn(SideTheEnemyIsOn, 1),
                        new ArgsFn(TooClose, 1),

                        new ArgsFn(MostDamagedSide),
                    ),
                new UtilityNode(CounterClockwise
                        new ArgsFn(TooClose, -1),
                        new ArgsFn(SideTheEnemyIsOn, -1)
                    )
                new UtilityNode(BothEnginesLeft
                    new ArgsFn(TooClose, -1),
                    new ArgsFn(SideTheEnemyIsOn, -1)
                )
                new UtilityNode(BothEnginesRight
                    new ArgsFn(TooClose, -1),
                    new ArgsFn(SideTheEnemyIsOn, -1)
                )*/
            );
    }

    float NoTarget(float multiplier) {
        return targeting.target == null ? multiplier : -multiplier;
    }

    float ShieldsDamaged() {
        return hpShields.shield / hpShields.maxShields == 1f ? 0f : 0.2f;
    }

    float HpLow() {
        return 1-hpShields.hp / hpShields.maxHp;
    }

    void Engine1() {
        // fires engine 1, into left or right side
    }

    void Engine2() {
        // fires engine 2, into left or right side
    }

    void Search() {
        targeting.target = SearchManager.ClosestEnemyUnit(info, initAlliance);
    }

    void SendCarriers() {
        requestBuild = true;
    }
    #endregion

    IEnumerator BuildCarriers() {
        while (true) {
            if (requestBuild) {
                // spawns random number of carriers in 2 hangars every 10 secs
                for (int i = 0; i < 2; i+=UnityEngine.Random.Range(1, 2)) {
                    for (int j = 0; j < 1; j++) {
                        SpawnController.AddOrder(spawner.hangars);
                    }
                }
                // this timer makes sure that carriers take a while to appear
                yield return new WaitForSeconds(10);// change this to builder queue type

            } else yield return null;
        }
    }

    #region destroy mothership quest
    void QuestSetup() {
        // quest setup
        bossQuest = new DestroyQuest("Beat the boss", GoalAchieved, 100, boss);
        partsOfBossQuest = new DestroyQuest[4];
        for (int i = 0; i < partsOfBossQuest.Length; i++) {
            partsOfBossQuest[i] = new DestroyQuest("Destroy part of ship", PartOfGoalAchieved, 25, questTargets[i]);
        }
    }

    void GoalAchieved(Quest questDone) {
        // todo: update whole quest ui with green
        Debug.Log("Boss destroyed! Update ui, load win screen or animation.");
    }

    void PartOfGoalAchieved(Quest subQuestDone) {
        Debug.Log("Part of boss destroyed! Update ui, load animation.");
    }
    #endregion

    public override void Destroyed() {
        
        // quest
        // mark quest completed when destroyed
        bossQuest.Completed();
    }
}

public abstract class MothershipBase : SpaceshipBase {
    
    

}
public abstract class SpaceshipBase : AiBase {
    public HpControl hpShields;

    public UnitInfo info;

    public abstract void Destroyed();

    protected new void Awake() {
        base.Awake();
        hpShields.beforeDestroyed += Destroyed;
    }
}