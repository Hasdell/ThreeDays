using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CSVUtil;

public class EnemyStateProbabilityData : MonoBehaviour
{
    private static List<Dictionary<string, object>> CSVData;

    private static Dictionary<short, EnemyStateProbability> enemyProbabilityTable;
    public static Dictionary<short, EnemyStateProbability> EnemyProbabilityTable { get { return enemyProbabilityTable; } set { enemyProbabilityTable = value; } }

    public static void ReadProbabilityData()
    {
        CSVData = null;
        enemyProbabilityTable = null;
        enemyProbabilityTable = new Dictionary<short, EnemyStateProbability>();

        CSVData = ReadCSV("EnemyStateProbabilityTable");

        foreach (var value in CSVData)
        {
            EnemyStateProbability enemy = new EnemyStateProbability();
            enemy.monster_Code = shortParse(value["monster_Code"]);
            enemy.idleToTraceProbability = floatParse(value["idleToTraceProbability"]);
            enemy.idleToAttackProbability = floatParse(value["idleToAttackProbability"]);
            enemy.idleToEscapeProbability = floatParse(value["idleToEscapeProbability"]);
            enemy.traceToAttackProbability = floatParse(value["traceToAttackProbability"]);
            enemy.traceToIdleProbability = floatParse(value["traceToIdleProbability"]);
            enemy.traceToEscapeProbability = floatParse(value["traceToEscapeProbability"]);
            enemy.attackToIdleProbability = floatParse(value["attackToIdleProbability"]);
            enemy.attackToAttackProbability = floatParse(value["attackToAttackProbability"]);
            enemy.attackToEscapeProbability = floatParse(value["attackToEscapeProbability"]);

            enemyProbabilityTable.Add(enemy.monster_Code, enemy);
        }
    }
    public class EnemyStateProbability
    {
        public short monster_Code;
        public float idleToTraceProbability;
        public float idleToAttackProbability;
        public float idleToEscapeProbability;
        public float traceToAttackProbability;
        public float traceToIdleProbability;
        public float traceToEscapeProbability;
        public float attackToIdleProbability;
        public float attackToAttackProbability;
        public float attackToEscapeProbability;
    }
}
