using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class OneHand : Gun_Base
{
    protected override void Spin()
    {
        dir_gun = new Vector3(target.x, target.y - 0.5f) - me.transform.position;
        float z = Mathf.Atan2(dir_gun.y, dir_gun.x) * Mathf.Rad2Deg;
        rot = z - 90f;
        float angle = Vector2.SignedAngle(Vector2.right, dir_gun);

        if (isRight)
        {
            transform.position = right.position;
            startpos.localPosition = _startpos;
            me.eulerAngles = new Vector3(0, 0, angle);
            me.GetComponent<SpriteRenderer>().flipX = false;

            if (me.rotation.z >= 0.85f || me.rotation.z <= -0.9f)
                isRight = false;
        }
        else
        {
            transform.position = left.position;
            startpos.localPosition = new Vector3(-_startpos.x, _startpos.y);
            me.eulerAngles = new Vector3(0, 0, angle + 180);
            me.GetComponent<SpriteRenderer>().flipX = true;

            if (me.rotation.z >= 0.9f || me.rotation.z <= -0.85f)
                isRight = true;
        }
    }
}
