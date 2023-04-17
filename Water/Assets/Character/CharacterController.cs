using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BehaviourState
{
    public string ID;
    public GameObject Obj;
}
public class CharacterController : MonoBehaviour
{
    [SerializeField] Animator _Anim;

    [SerializeField] WaterBallControll waterBallController;
    [SerializeField] float _TurnSpeed;
    Vector3 waterBallTarget;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            StopAllCoroutines();
            StartCoroutine(Coroutine_WaterBall());
        }
       
    }

    IEnumerator Coroutine_WaterBall()
    {
        while (true)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (!waterBallController.WaterBallCreated())
                {
                    yield return StartCoroutine(Coroutine_Turn());
                    _Anim.SetTrigger("CreateWaterBall");
                }
                else
                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hit;
                    yield return StartCoroutine(Coroutine_Turn());
                    if (Physics.Raycast(ray, out hit))
                    {
                        waterBallTarget = hit.point;
                        _Anim.SetTrigger("ThrowWaterBall");
                    }
                }
            }
            yield return null;
        }
    }

    private void AnimationCallback_CreateWaterBall()
    {
        if (!waterBallController.WaterBallCreated())
        {
            waterBallController.CreateWaterBall();
        }
    }

    private void AnimationCallback_ThrowBall()
    {
        if (waterBallController.WaterBallCreated())
        {
            waterBallController.ThrowWaterBall(waterBallTarget);
        }
    }

   

   
    IEnumerator Coroutine_Turn()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if(Physics.Raycast(ray,out hit))
        {
            Vector3 direction = (hit.point - transform.position);
            direction.y = 0;
            direction = direction.normalized;
            Vector3 startForward = transform.forward;
            float angle = Vector3.Angle(startForward, direction);
            _Anim.SetFloat("Turn", Vector3.Cross(startForward, direction).y);
            float lerp = 0;
            while (lerp < 1)
            {
                transform.forward = Vector3.Slerp(startForward, direction, lerp);
                lerp += Time.deltaTime* _TurnSpeed / angle;
                yield return null;
            }
            _Anim.SetFloat("Turn",0);
        }
    }
}
