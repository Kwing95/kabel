using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldUnit : MonoBehaviour
{

    public List<PartyMember> party;
    public GameObject corpse;
    public ColorIndicator indicator;

    // Start is called before the first frame update
    void Awake()
    {
        //party = new List<PartyMember>();
        // Default
        //party.Add(new PartyMember(3, 3, PartyMember.Real.Real));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void FullHeal()
    {
        for (int i = 0; i < party.Count; ++i)
            party[i].FullHeal();
    }

    // Damages a random unit in the party
    public void TakeDamage(PartyMember attacker, bool damagesFocus = false)
    {
        List<int> validTargets = new List<int>();
        for (int i = 0; i < party.Count; ++i)
            if (PartyMember.Interacts(attacker, party[i]))
                validTargets.Add(i);

        if (validTargets.Count == 0)
            return;
        
        int targetedUnit = validTargets[Random.Range(0, validTargets.Count)];

        if (damagesFocus)
            party[targetedUnit].focus -= 1;
        else
        {
            party[targetedUnit].health -= 1;
            // This needs to be much more complicated
            if (OneDown() && GetComponent<PlayerMover>())
            {
                Instantiate(corpse, transform.position, Quaternion.identity);
                GetComponentInChildren<SpriteRenderer>().enabled = false;
                enabled = false;

                TransitionFader.instance.Transition(0);
            }
            if (AllDown() && GetComponent<AutoMover>())
            {
                Instantiate(corpse, transform.position, Quaternion.identity);
                DestroyImmediate(gameObject);
            }
                
        }

        // indicator.SetColor(health);

    }

    // Returns true if at least one party member is defeated
    public bool OneDown()
    {
        for (int i = 0; i < party.Count; ++i)
            if (party[i].health <= 0)
                return true;
        return false;
    }

    // Returns true if all party members are defeated
    public bool AllDown()
    {
        for (int i = 0; i < party.Count; ++i)
            if (party[i].health > 0)
                return false;
        return true;
    }

    // Returns focus of all party members with at least 1 focus
    public List<int> Foci()
    {
        List<int> returnValue = new List<int>();
        for (int i = 0; i < party.Count; ++i)
            if (party[i].focus > 0)
                returnValue.Add(party[i].focus);
        return returnValue;
    }

}

[System.Serializable]
public class PartyMember
{
    public int maxHealth = 3;
    public int maxFocus = 3;
    public int health = 3;
    public int focus = 3;
    public enum Real { Real, Fake, Manic };
    public Real real;

    public PartyMember(int _maxHealth, int _maxFocus, Real _real)
    {
        maxHealth = _maxHealth;
        maxFocus = _maxFocus;
        real = _real;
        FullHeal();
    }

    public void FullHeal()
    {
        health = maxHealth;
        focus = maxFocus;
    }

    // Characters can interact with each other if both are real, both are fake,
    // or if one character is manic.
    public static bool Interacts(PartyMember a, PartyMember b)
    {
        return a.real == b.real || a.real == Real.Manic || b.real == Real.Manic;
    }
}