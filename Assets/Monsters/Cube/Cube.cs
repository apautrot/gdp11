using UnityEngine;
using System.Collections;

public class Cube : MonoBehaviour
{
    Rigidbody2D body;
    GameObject sprite;

    [Header("Normal jump")]
    public float jumpDistance = 100;
    public float jumpDuration = 1;
    public float jumpHeight = 50;
    public GoEaseType jumpMoveEase = GoEaseType.QuadInOut;
    public GoEaseType jumpSpriteEase = GoEaseType.QuadInOut;

    [Header("Threshold")]
    public float crushJumpDistanceThreshold = 100;

    [Header("Crush jump")]
    public float crushJumpDistance = 80;
    public float crushJumpDuration = 1;
    public float crushJumpHeight = 100;
    public GoEaseType crushJumpMoveEase = GoEaseType.QuadInOut;
    public GoEaseType crushJumpSpriteEase = GoEaseType.QuadInOut;

    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        sprite = gameObject.FindChildByName("Sprite");

        StartCoroutine(AnimateCoroutine());
    }

    IEnumerator AnimateCoroutine()
    {
        while (true)
        {
            float duration = 0;
            if ((Player.Instance.transform.position - transform.position).magnitude < crushJumpDistanceThreshold)
                duration = CrushJump() + 2; 
            else
                duration = NormalJump();

            yield return new WaitForSeconds(duration);

            if(duration != 1)
            {

                if ((Player.Instance.transform.position - transform.position).magnitude < crushJumpDistanceThreshold)
                {
					if (Player.Instance.EnergyPoints > 0)
						Player.Instance.GetComponent<Rigidbody2D>().AddForce(Vector2.up, ForceMode.Force);

                    // Son d'écrasement des ennemis
                }
            }
        }
    }

    float CrushJump()
    {
        Vector3 velocity = (Player.Instance.transform.position - transform.position).normalized * crushJumpDistance;

        GoTween tween = transform.positionTo(crushJumpDuration, velocity, true)
            .eases(crushJumpMoveEase);

        AbstractGoTween spriteTween = sprite.transform.localPositionTo(crushJumpDuration / 2, new Vector3(0, crushJumpHeight, 0), true)
            .eases(crushJumpSpriteEase)
            .loops(2, GoLoopType.PingPong);

        return tween.totalDuration;
    }

    float NormalJump()
    {
        Vector3 velocity = (Player.Instance.transform.position - transform.position).normalized * jumpDistance;

        GoTween tween = transform.positionTo(jumpDuration, velocity, true)
            .eases(jumpMoveEase);

        AbstractGoTween spriteTween = sprite.transform.localPositionTo(jumpDuration / 2, new Vector3(0, jumpHeight, 0), true)
            .eases(jumpSpriteEase)
            .loops(2, GoLoopType.PingPong);

        return tween.totalDuration;
    }
}
