using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Rerode : MonoBehaviour
{
    [SerializeField] Image fillGauge;
    Player p;
    Vector3 playerpos;

    private void Start() {
        p = Player.Instance;
    }

    private void Update() {
        playerpos = p.transform.position;
        transform.position = new Vector3(playerpos.x, playerpos.y + 1);
    }

    public void SetFill(float fillAmount)
    {
        fillGauge.fillAmount = fillAmount;

        fillGauge.gameObject.SetActive(fillAmount > 0f);
    }
}
