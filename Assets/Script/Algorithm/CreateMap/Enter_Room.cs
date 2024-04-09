using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enter_Room : MonoBehaviour
{
    [Header("Map")]
    [SerializeField] private Map map;
    [SerializeField] private MapTree mapTree;
    [SerializeField] private List<EnemyList> enemyList = new List<EnemyList>();
    [SerializeField] private GameObject WARNING;
    [SerializeField] private bool isHallWay;
    List<Enemy_Base> curEnemy = new List<Enemy_Base>();
    private BoxCollider2D box;
    private GameObject fade;
    private Vector3Int plusPos;

    public void Setting(Vector2 _size, Vector3Int _pos, MapTree _mapTree, GameObject _fade, bool _isHallWay)
    {
        box = GetComponent<BoxCollider2D>();

        mapTree = _mapTree;
        box.size = _size;
        isHallWay = _isHallWay;
        fade = _fade;

        if (isHallWay)
        {
            plusPos = mapTree.hallWayPos;
            map = mapTree.hallWay;
        }
        else
        {
            plusPos = mapTree.mapPos;
            enemyList = mapTree.map.enemyList;
            map = mapTree.map;
        }

    }

    IEnumerator SpawnRoutine(int wavecount)
    {
        curEnemy.Clear();

        for (int i = 0; i < enemyList.Count; i++)
        {
            if (enemyList[i].wavecount != wavecount) continue;
            StartCoroutine(Spawn(i));
        }
        yield return new WaitForSeconds(1.5f);
        while (curEnemy.Count != 0)
        {
            for (int i = 0; i < curEnemy.Count; i++)
                if (curEnemy[i] == null)
                    curEnemy.Remove(curEnemy[i]);

            yield return null;
        }
    }

    IEnumerator Spawn(int i){
        var list = enemyList[i];

        yield return new WaitForSeconds(list.waitTime);
        Destroy(Instantiate(WARNING, new Vector3(list.spawnPos.x + plusPos.x, list.spawnPos.y + plusPos.y),
                   Quaternion.identity, this.transform), 1);
        yield return new WaitForSeconds(1);

        var temp = Instantiate(list.spawnEnemy,
                   new Vector3(list.spawnPos.x + plusPos.x, list.spawnPos.y + plusPos.y),
                   Quaternion.identity, this.transform);
        if (temp.TryGetComponent<RandomMove>(out var ranMove))
        {
            ranMove.room_LeftDown = map.leftDown + (Vector2Int)plusPos;
            ranMove.room_RightUp = map.rightUp + (Vector2Int)plusPos;
        }

        if (!temp.TryGetComponent<IItme>(out var isItem)) curEnemy.Add(temp);
    }

    IEnumerator StageRoutine()
    {
        if (enemyList.Count == 0) yield break;
        GameManager.Instance.DoorOnOff(true);
        yield return new WaitForSeconds(1);
        for (int i = 0; i <= enemyList[enemyList.Count - 1].wavecount; i++)
        {
            yield return StartCoroutine(SpawnRoutine(i));
            yield return new WaitForSeconds(1);
        }
        GameManager.Instance.DoorOnOff(false);
        yield break;
    }

    public IEnumerator FadeOut(GameObject fade, float sec)
    {
        float timer = 0f;

        while (timer <= sec)
        {
            fade.GetComponent<SpriteRenderer>().color = new Color(0.145098f, 0.07450981f, 0.1019608f, 1 - timer / sec);
            timer += Time.deltaTime;

            yield return null;
        }
        Destroy(fade);

        yield break;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Destroy(GetComponent<BoxCollider2D>());
            StartCoroutine(FadeOut(fade, 0.5f));
            mapTree.ForOfWar(mapTree, isHallWay);

            if (!isHallWay) StartCoroutine(StageRoutine());
        }
    }
}
