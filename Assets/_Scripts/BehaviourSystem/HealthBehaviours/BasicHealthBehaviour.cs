using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/ProgramableBehaviours/BasicHealthBehaviour")]
public class BasicHealthBehaviour : AHealthBehaviourSO
{
    [SerializeField] private float maxHealth;
    [SerializeField] private int scoreGainOnDeath;
    
    public override float CurrentHealth { get; protected set; }

    public override float MaxHealth => maxHealth;

    public override bool IsDead { get; protected set; }

    public override void OnSetup(BehaviourController controller)
    {
        ResetHealth();
    }
    
    private void ResetHealth()
    {
        CurrentHealth = maxHealth;
    }

    public override void TakeDamage(float damage)
    {
        if (IsDead) return;
        CurrentHealth -= damage;
        if (CurrentHealth <= 0) Die();
    }

    private void Die()
    {
        if(GameManager.Instance.CurrentLevel.TryGetBehaviour<ScoreCounter>(out var scoreCounter)) 
            scoreCounter.ScoreGained(scoreGainOnDeath);
        OnDeath.Invoke(this);
        OnDeath.RemoveAllListeners();
        IsDead = true;
        CurrentHealth = 0;
        Controller.PoolObject.GoToPool();
    }
}
