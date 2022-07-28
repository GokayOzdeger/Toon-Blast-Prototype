using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllGridEntities 
{
    private static BasicGridEntityTypeDefinition[] allEntityTypeDefinitions;
    
    public static BasicGridEntityTypeDefinition[] AllEntityTypeDefinition
    {
        get
        {
            if (allEntityTypeDefinitions != null) return allEntityTypeDefinitions;
            allEntityTypeDefinitions = Resources.LoadAll<BasicGridEntityTypeDefinition>("ScriptableObjects/GridEntityDefinitions");
            return allEntityTypeDefinitions;
        }
    }

    public static BasicGridEntityTypeDefinition GetEntityTypeByName(string name)
    {
        foreach (var entityTypeDefinition in AllEntityTypeDefinition)
        {
            if (entityTypeDefinition.GridEntityTypeName == name)
            {
                return entityTypeDefinition;
            }
        }
        Debug.LogError("Could not find entity type with name: " + name);
        return null;
    }
}
