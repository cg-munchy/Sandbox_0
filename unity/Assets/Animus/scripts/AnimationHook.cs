using MoreMountains.Feedbacks;
using ScriptableObjectArchitecture;
using UnityEngine;

public class AnimationHook : MonoBehaviour
{
    //[SerializeField] private MMF_Player player;
    [SerializeField] private GameEvent leafEvent;
    [SerializeField] private GameEvent singEvent;
    
    public void VFXLeaf()
    {
        leafEvent.Raise();
    } 
    
    public void VFXSing()
    {
        singEvent.Raise();
    }

}
