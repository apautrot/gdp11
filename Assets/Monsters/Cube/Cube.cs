using UnityEngine;
using System.Collections;

public class Cube : Monster
{
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

    GoTween tween;
    AbstractGoTween spriteTween;

    new void Start()
    {
		base.Start ();

        sprite = gameObject.FindChildByName("Sprite");

        StartCoroutine(AnimateCoroutine());
    }

    new void OnDestroy()
    {
		base.OnDestroy ();

        if (tween != null)
            tween.destroy();

        if (spriteTween != null)
            spriteTween.destroy();
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

            if(duration != 2)
            {
                if ((Player.Instance.transform.position - transform.position).magnitude < crushJumpDistanceThreshold)
                {
					if (Player.Instance.EnergyPoints > 0)
						Player.Instance.GetComponent<Rigidbody2D>().AddForce(Vector2.up, ForceMode.Force); 
                }   
            }
        }
    }

    float CrushJump()
    {
        Vector3 velocity = (Player.Instance.transform.position - transform.position).normalized * crushJumpDistance;

        tween = transform.positionTo(crushJumpDuration, velocity, true)
            .eases(crushJumpMoveEase);

        spriteTween = sprite.transform.localPositionTo(crushJumpDuration / 2, new Vector3(0, crushJumpHeight, 0), true)
            .eases(crushJumpSpriteEase)
            .loops(2, GoLoopType.PingPong);

        // Son d'écrasement des ennemis
        Audio.Instance.PlaySound(AllSounds.Instance.CubeBigHit);

        return tween.totalDuration + spriteTween.totalDuration;
    }

    float NormalJump()
    {
        Vector3 velocity = (Player.Instance.transform.position - transform.position).normalized * jumpDistance;

         tween = transform.positionTo(jumpDuration, velocity, true)
            .eases(jumpMoveEase);

        spriteTween = sprite.transform.localPositionTo(jumpDuration / 2, new Vector3(0, jumpHeight, 0), true)
            .eases(jumpSpriteEase)
            .loops(2, GoLoopType.PingPong);

        Audio.Instance.PlaySound(AllSounds.Instance.Cube);

        return tween.totalDuration + spriteTween.totalDuration;
    }
}
