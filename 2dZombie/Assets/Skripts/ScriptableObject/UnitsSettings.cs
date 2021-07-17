using UnityEngine;

[CreateAssetMenu(fileName = "UnitsSettings", menuName = "UnitsSettings", order = 51)]

public class UnitsSettings :ScriptableObject
    {
        [SerializeField] private int _hp;
        [SerializeField] private int _armor;

        public int Hp
        {
            get
            {
                return _hp;
            }
        }

        public int Armor
        {
            get
            {
                return _armor;
            }
        }
    }