using System.Collections;
using TMPro;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    Vector2 direction;

    bool isKnockbacked;
    public bool isSlowed;

    [SerializeField] Rigidbody2D rb;
    [SerializeField] EnemyConfig enemyConfig;
    [SerializeField] GameObject obj;
    [SerializeField] Canvas canvas;
    [SerializeField] TMP_Text text;
    [SerializeField] Color textColor;
    [SerializeField] private Animator animator;

    Color baseColor;
    [SerializeField] Color damageColor;
    [SerializeField] SpriteRenderer spriteRenderer;

    Coroutine indicator;

    public float health;

    void OnEnable()
    {
        baseColor = spriteRenderer.color;
        canvas.worldCamera = FindFirstObjectByType<Camera>();
        direction = new Vector2(0, transform.position.y) - (Vector2)transform.position;

        if (direction.x > 0)
            obj.transform.localScale = new Vector3(1, 1, 1);
        else
            obj.transform.localScale = new Vector3(-1, 1, 1);
        
        health = enemyConfig.health;
    }

    void FixedUpdate()
    {
        if (isKnockbacked) 
            return;

        if (isSlowed)
            rb.linearVelocity = direction * (enemyConfig.speed / 2) * Time.deltaTime;
        else
            rb.linearVelocity = direction * enemyConfig.speed * Time.deltaTime;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("EnemyFinishLine"))
            EnemyVictorious();
        
    }

    public void ApplyDamage(float damage, float knockback, float knockbackTime)
    {
        spriteRenderer.color = damageColor;
        if (indicator != null) 
            StopCoroutine(indicator);

        indicator = StartCoroutine(ShowDamageIndicator((int)damage));
        health -= damage;

        isKnockbacked = true;

        if (health <= 0)
            EnemyDead();
        
        StartCoroutine(ApplyKnockback(knockback, knockbackTime));
    }

    IEnumerator ApplyKnockback(float knockback, float knockbackTime)
    {
        isKnockbacked = true;
        rb.AddForceX(-direction.x + transform.position.x * knockback * Time.deltaTime, ForceMode2D.Impulse);
        yield return new WaitForSeconds(knockbackTime);
        isKnockbacked = false;
        spriteRenderer.color = baseColor;
    }

    void EnemyDead()
    {
        AscendancyEvents.TriggerOnAddValue(enemyConfig.currencyReward);
        EnemySpawnerEvents.TriggerEnemyDead();
        Destroy(gameObject);
    }

    void EnemyVictorious()
    {
        HealthEvents.OnRemoveValue(enemyConfig.damage);
        EnemySpawnerEvents.TriggerEnemyDead();
        Destroy(gameObject);
    }

    IEnumerator ShowDamageIndicator(float damage)
    {
        text.text = damage.ToString();
        text.color = textColor;

        Color currentColor = textColor;

        float elapsedTime = 0f;

        while (elapsedTime < 1)
        {
            elapsedTime += Time.deltaTime;

            float newAlpha = Mathf.Lerp(1f, 0f, elapsedTime / 1);

            currentColor.a = newAlpha;
            text.color = currentColor;

            yield return null;
        }

        currentColor.a = 0f;
        text.color = currentColor;

        indicator = null;
    }
}