using UnityEngine;

[CreateAssetMenu]
public class InputData : ScriptableObject
{
    [Header("Weapon")]
    [SerializeField] KeyCode fireButton = KeyCode.Mouse0;
    [SerializeField] KeyCode aimButton = KeyCode.Mouse1;
    [SerializeField] KeyCode reloadButton = KeyCode.R;
    [SerializeField] KeyCode meleeAttack = KeyCode.F;
    [SerializeField] KeyCode throwWeapon = KeyCode.Q;
    [SerializeField] KeyCode mainWeaponSwitch = KeyCode.Alpha1;
    [SerializeField] KeyCode meleeSwitch = KeyCode.Alpha2;

    [Header("World")]
    [SerializeField] KeyCode interactionButton = KeyCode.E;

    [Header("UI")]
    [SerializeField] KeyCode characterScreenOpen = KeyCode.C;
    [SerializeField] KeyCode menuButton = KeyCode.Escape;

    public KeyCode FireButton { get { return fireButton; } }
    public KeyCode AimButton { get { return aimButton; } }
    public KeyCode ReloadButton { get { return reloadButton; } }
    public KeyCode MeleeAttack { get { return meleeAttack; } }
    public KeyCode ThrowWeapon { get { return throwWeapon; } }
    public KeyCode MainWeaponSwitch { get { return mainWeaponSwitch; } }
    public KeyCode MeleeSwitch { get { return meleeSwitch; } }

    public KeyCode InteractionButton { get { return interactionButton; } }
    
    public KeyCode CharacterScreenOpen { get { return characterScreenOpen; } }
    public KeyCode MenuButton { get { return menuButton; } }
}
