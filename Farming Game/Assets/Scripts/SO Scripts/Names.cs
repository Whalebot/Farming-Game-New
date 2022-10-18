using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Character Names", menuName = "ScriptableObjects/Names")]
public class Names : ScriptableObject
{
    public List<string> prefixes;
    public List<string> names;
    public List<string> affixes;

    public string GenerateName()
    {
        string affix = "", name = "", prefix = "";

        if (prefixes.Count > 0)
        {
            prefix = prefixes[Random.Range(0, prefixes.Count)];
        }
        if (affixes.Count > 0)
        {
            affix = affixes[Random.Range(0, affixes.Count)];
        }
        if (names.Count > 0)
        {
            name = names[Random.Range(0, names.Count)];
        }
        return prefix + " " + name + " " + affix;
    }
}
