using KitchenLib.Utils;
using UnityEngine;
using KitchenDeconstructor;
using System.Collections;
namespace DeconstructorMod.Components
{
    public class BlinkingLED : MonoBehaviour
    {
        private Material[] m_StartMat;
        private Material[] m_BlinkMat;
        private bool m_BlinkIsOn;
        private GameObject m_LED;
        private Coroutine m_BlinkCoroutine;
        private void Awake()
        {
            m_LED = gameObject.GetChild("Deconstructor/PrintedLED");
            m_StartMat = new Material[] { MaterialUtils.GetExistingMaterial("Plastic - Dark Green") };
            m_BlinkMat = new Material[] { MaterialUtils.GetExistingMaterial("Paper - Postit Green") };

            m_BlinkCoroutine = StartCoroutine(BlinkLED());
        }
        private IEnumerator BlinkLED()
        {
            while (true)
            {
                yield return new WaitForSeconds(2);
                if (m_BlinkIsOn)
                {
                    m_LED.ApplyMaterial(m_BlinkMat);
                    m_BlinkIsOn = false;

                }
                else
                {
                    m_LED.ApplyMaterial(m_StartMat);
                    m_BlinkIsOn = true;
                }
            }
        }
        public void SetBlink(bool pBlink)
        {
            if (pBlink)
            {
                if (m_BlinkCoroutine == null)
                {
                    m_BlinkCoroutine = StartCoroutine(BlinkLED());
                }
            }
            else
            {
                if (m_BlinkCoroutine != null)
                {
                    StopCoroutine(m_BlinkCoroutine);
                    m_LED.ApplyMaterial(m_StartMat);
                    m_BlinkIsOn = true;
                    m_BlinkCoroutine = null;
                }
            }
        }
    }
}
