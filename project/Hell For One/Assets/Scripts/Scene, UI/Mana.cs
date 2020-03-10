using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Mana : MonoBehaviour
{
    private float manaPool;
    private float maxMana = 100f;
    private Image manaBarIn;
    private float chargeRate = 5f;
    private bool isCharging = false;

    public void ChargeMana()
    {
        Debug.Log( "Charging mana" );
        isCharging = true;
    }

    public void StopChargeMana()
    {
        Debug.Log( "Stopped charging mana" );
        isCharging = false;
    }

    void Awake()
    {
        manaBarIn = transform.GetChild( 0 ).GetComponent<Image>();
    }

    void Start()
    {
        manaPool = 50f;
    }

    void Update()
    {
        manaBarIn.fillAmount = manaPool / maxMana;

        if ( isCharging )
        {
            if (manaPool <= maxMana )
            {
                manaPool += chargeRate * Time.deltaTime;
            }
        }
    }
}
