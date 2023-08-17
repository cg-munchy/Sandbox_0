using System.Collections;
using ScriptableObjectArchitecture;
using UnityEngine;

namespace CustomScripts
{
    public class ExtraControls : MonoBehaviour
    {
        [SerializeField] private string danceTrigger;
        [SerializeField] private string victory;
        [SerializeField] private Animator animator;
        [SerializeField] private BoolReference isBusy;

        private void Awake()
        {
            isBusy.Value = false;
        }

        private void Update()
        {
            if (isBusy.Value)
            {
                if(Mathf.Abs(Input.GetAxis("Horizontal"))>0 || Mathf.Abs(Input.GetAxis("Vertical")) > 0)
                    Interrupt();
                return;
            }
            if (Input.GetButtonDown("Fire1"))
            {
                StartCoroutine(PlayAnimation(danceTrigger));
            }
            else if (Input.GetButtonDown("Fire2"))
            {
                StartCoroutine(PlayAnimation(victory));
            }
            
        }

        public void Interrupt()
        {
            StopAllCoroutines();
            isBusy.Value = false;
        }

        private IEnumerator PlayAnimation(string trigger)
        {
            
            animator.SetTrigger(trigger);
            Debug.Log("trigerring");
            isBusy.Value = true;
            yield return null;
            var time = animator.GetCurrentAnimatorStateInfo(0).length;
            yield return new WaitForSeconds(time);
            isBusy.Value = false;
            animator.ResetTrigger(trigger);
        }
    }
}