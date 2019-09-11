using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyUnit : FieldUnit
{

    private List<int> partyHealth;
    private List<int> partyFocus;
    private List<bool> partyReal;

    // Start is called before the first frame update
    void Awake()
    {
        partyHealth = new List<int>();
        partyFocus = new List<int>();
        partyReal = new List<bool>();

        partyHealth.Add(3);
        partyHealth.Add(3);
        partyHealth.Add(3);

        partyFocus.Add(3);
        partyFocus.Add(3);
        partyFocus.Add(3);

        FullHeal();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void FullHeal()
    {
        for (int i = 0; i < partyHealth.Count; ++i)
        {
            partyHealth[i] = maxHealth;
            partyFocus[i] = maxFocus;
        }
    }

    public override void ResetAP()
    {
        ap = maxAP - (3 - Mathf.Min(partyHealth.ToArray()));
    }

    public override int GetHealth()
    {
        return Mathf.Min(Mathf.Min(partyHealth.ToArray()));
    }

    public override int GetFocus()
    {
        int total = 0;
        for (int i = 0; i < partyFocus.Count; ++i)
            total += partyFocus[i];

        return total;
    }
}
