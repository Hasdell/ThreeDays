using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CSVUtil;

public class EnemyData : MonoBehaviour
{
    private static List<Dictionary<string, object>> CSVData;

    private static Dictionary<short, EnemyInfo> enemyTable;
    public static Dictionary<short, EnemyInfo> EnemyTable { get { return enemyTable; } set { enemyTable = value; } }
    
    public static void ReadAttackData()
    {
        CSVData = null;
        enemyTable = null;
        enemyTable = new Dictionary<short, EnemyInfo>();

        CSVData = ReadCSV("EnemySkillTable");

        foreach(var value in CSVData)
        {
            EnemyInfo enemy = new EnemyInfo();
            enemy.monster_Code = shortParse(value["Monster_Code"]);
            enemy.monster_Damage = floatParse(value["Monster_Damage"]);
            enemy.monster_Speed = floatParse(value["Monster_Speed"]);
            enemy.monster_AttackDelay = floatParse(value["Monster_AttackDelay"]);
            enemy.monster_AttackSpeed = floatParse(value["Monster_AttackSpeed"]);
            enemy.monster_MinDistance = floatParse(value["Monster_MinDistance"]);
            enemy.chase_Range = floatParse(value["Chase_Range"]);
            enemy.attack_Range = floatParse(value["Attack_Range"]);
            enemy.monster_Hp = floatParse(value["Monster_Hp"]);

            enemyTable.Add(enemy.monster_Code, enemy);
        }
    }
    public class EnemyInfo
    {
        public short monster_Code;
        public float monster_Damage;
        public float monster_Speed;
        public float monster_AttackDelay;
        public float monster_AttackSpeed;
        public float monster_MinDistance;
        public float chase_Range;
        public float attack_Range;
        public float monster_Hp;
    }

}
