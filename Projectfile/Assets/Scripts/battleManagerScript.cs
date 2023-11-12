using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class battleManagerScript : MonoBehaviour
{
    public camShakeScript CSS;
    public gameManagerScript GMS;

    private bool battleStatus;
    
    public void battle(GameObject initiator, GameObject recipient)
    {
        battleStatus = true;
        var initiatorUnit = initiator.GetComponent<UnitScript>();
        var recipientUnit = recipient.GetComponent<UnitScript>();
        int initiatorAtt = initiatorUnit.attackDamage;
        int recipientAtt = recipientUnit.attackDamage;
        if (initiatorUnit.attackRange == recipientUnit.attackRange)
        {
            GameObject tempParticle = Instantiate( recipientUnit.GetComponent<UnitScript>().damagedParticle,recipient.transform.position, recipient.transform.rotation);
            Destroy(tempParticle, 2f);
            recipientUnit.dealDamage(initiatorAtt);
            if (checkIfDead(recipient))
            {
                recipient.transform.parent = null;
                recipientUnit.unitDie();
                battleStatus = false;
                GMS.checkIfUnitsRemain(initiator, recipient);
                return;
            }
            initiatorUnit.dealDamage(recipientAtt);
            if (checkIfDead(initiator))
            {
                initiator.transform.parent = null;
                initiatorUnit.unitDie();
                battleStatus = false;
                GMS.checkIfUnitsRemain(initiator, recipient);
                return;
            }
        }
        else
        {
            GameObject tempParticle = Instantiate(recipientUnit.GetComponent<UnitScript>().damagedParticle, recipient.transform.position, recipient.transform.rotation);
            Destroy(tempParticle, 2f);
            recipientUnit.dealDamage(initiatorAtt);
            if (checkIfDead(recipient))
            {
                recipient.transform.parent = null;
                recipientUnit.unitDie();
                battleStatus = false;
                GMS.checkIfUnitsRemain(initiator, recipient);
                return;
            }
        }
        battleStatus = false;
    }
    public bool checkIfDead(GameObject unitToCheck)
    {
        if (unitToCheck.GetComponent<UnitScript>().currentHealthPoints <= 0)
        {
            return true;
        }
        return false;
    }

    public void destroyObject(GameObject unitToDestroy)
    {
        Destroy(unitToDestroy);
    }

    public IEnumerator attack(GameObject unit, GameObject enemy)
    {
        battleStatus = true;
        float elapsedTime = 0;
        Vector3 startingPos = unit.transform.position;
        Vector3 endingPos = enemy.transform.position;
        unit.GetComponent<UnitScript>().setWalkingAnimation();
        while (elapsedTime < .25f)
        {
           
            unit.transform.position = Vector3.Lerp(startingPos, startingPos+((((endingPos - startingPos) / (endingPos - startingPos).magnitude)).normalized*.5f), (elapsedTime / .25f));
            elapsedTime += Time.deltaTime;
            
            yield return new WaitForEndOfFrame();
        }
        while (battleStatus)
        {
            StartCoroutine(CSS.camShake(.2f,unit.GetComponent<UnitScript>().attackDamage,getDirection(unit,enemy)));
            if(unit.GetComponent<UnitScript>().attackRange == enemy.GetComponent<UnitScript>().attackRange && enemy.GetComponent<UnitScript>().currentHealthPoints - unit.GetComponent<UnitScript>().attackDamage > 0)
            {
                StartCoroutine(unit.GetComponent<UnitScript>().displayDamageEnum(enemy.GetComponent<UnitScript>().attackDamage));
                StartCoroutine(enemy.GetComponent<UnitScript>().displayDamageEnum(unit.GetComponent<UnitScript>().attackDamage));
            }
            else
            {
                StartCoroutine(enemy.GetComponent<UnitScript>().displayDamageEnum(unit.GetComponent<UnitScript>().attackDamage));
            }
            battle(unit, enemy);
            yield return new WaitForEndOfFrame();
        }
        if (unit != null)
        {
            StartCoroutine(returnAfterAttack(unit, startingPos));
        }
    }
    public IEnumerator returnAfterAttack(GameObject unit, Vector3 endPoint) {
        float elapsedTime = 0;
        while (elapsedTime < .30f)
        {
            unit.transform.position = Vector3.Lerp(unit.transform.position, endPoint, (elapsedTime / .25f));
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        unit.GetComponent<UnitScript>().setWaitIdleAnimation();
        unit.GetComponent<UnitScript>().wait();
    }
    public Vector3 getDirection(GameObject unit, GameObject enemy)
    {
        Vector3 startingPos = unit.transform.position;
        Vector3 endingPos = enemy.transform.position;
        return (((endingPos - startingPos) / (endingPos - startingPos).magnitude)).normalized;
    }

}
