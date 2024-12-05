using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    public class EnemyBattle : MonoBehaviour
    {
        // ダメージを与えた敵キャラクターのIDを格納するリスト
        private List<int> damageEnemyIDs = new List<int>();

        /// <summary>
        /// プレイヤーにダメージを与えた敵のIDを取得
        /// </summary>
        /// <param name="enemyID">ダメージを与えた敵のID</param>
        public void RecordDamage(int enemyID)
        {
            if (!damageEnemyIDs.Contains(enemyID))
            {
                damageEnemyIDs.Add(enemyID);
                Debug.Log($"Enemy ID {enemyID} dealt damage.");
            }
        }

        /// <summary>
        /// 全てのダメージを与えた敵IDのリストを取得します
        /// </summary>
        /// <returns>ダメージを与えた敵IDのリスト</returns>
        public List<int> GetDamageEnemyIDs()
        {
            return damageEnemyIDs;
        }

        /// <summary>
        /// 指定した敵IDがプレイヤーにダメージを与えたか確認
        /// </summary>
        /// <param name="enemyID">確認したい敵ID</param>
        /// <returns>ダメージを与えた場合はtrue、そうでない場合はfalse</returns>
        public bool DidEnemyDealDamage(int enemyID)
        {
            return damageEnemyIDs.Contains(enemyID);
        }

        /// <summary>
        /// ダメージ情報をクリア
        /// </summary>
        public void ClearDamageInfo()//ダメージ情報が一時的に必要なら使う　与えたダメージの敵IDの情報がずっと必要であるのであれば消して

        {
            damageEnemyIDs.Clear();　
        }
        
    }
}

