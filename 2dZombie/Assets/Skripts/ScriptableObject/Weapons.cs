using UnityEngine;

[CreateAssetMenu(fileName = "Weapons", menuName = "Weapons", order = 51)]

public class Weapons :ScriptableObject
{
    [SerializeField] private int _dmg;
    
    public int Dmg
    {
        get
        {
            return _dmg;
        }
    }

}