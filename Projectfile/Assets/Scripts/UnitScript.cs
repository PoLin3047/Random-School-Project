using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UnitScript : MonoBehaviour
{
    public int teamNum;
    public int x;
    public int y;

    public bool coroutineRunning;

    public Queue<int> movementQueue;
    public Queue<int> combatQueue;

    public float visualMovementSpeed = .15f;

    public Animator animator;


    public GameObject tileBeingOccupied;

    public GameObject damagedParticle;

    public string unitName;
    public int moveSpeed = 2;    
    public int attackRange = 1;
    public int attackDamage = 1;
    public int maxHealthPoints = 5;
    public int currentHealthPoints;
    public Sprite unitSprite;

    [Header("UI Elements")]

    public Canvas healthBarCanvas;
    public TMP_Text hitPointsText;
    public Image healthBar;

    public Canvas damagePopupCanvas;
    public TMP_Text damagePopupText;
    public Image damageBackdrop;

    public tileMapScript map;

    public Transform startPoint;
    public Transform endPoint;
    public float moveSpeedTime = 1f;
    
    public GameObject holder2D;

    private float journeyLength;

    public bool unitInMovement;

    public enum movementStates
    {
        Unselected,
        Selected,
        Moved,
        Wait
    }
    public movementStates unitMoveState;

    public List<Node> path = null;

    public List<Node> pathForMovement = null;
    public bool completedMovement = false;

    private void Awake()
    {
        animator = holder2D.GetComponent<Animator>();
        movementQueue = new Queue<int>();
        combatQueue = new Queue<int>();
        x = (int)transform.position.x;
        y = (int)transform.position.z;
        unitMoveState = movementStates.Unselected;
        currentHealthPoints = maxHealthPoints;
        hitPointsText.SetText(currentHealthPoints.ToString());
    }

    public void LateUpdate()
    {
        healthBarCanvas.transform.forward = Camera.main.transform.forward;
        holder2D.transform.forward = Camera.main.transform.forward;
    }

    public void MoveNextTile()
    {
        if (path.Count == 0)
        {
            return;
        }
        else
        {
            StartCoroutine(moveOverSeconds(transform.gameObject, path[path.Count - 1]));
        }
     }

   
    public void moveAgain()
    {
        path = null;
        setMovementState(0);
        completedMovement = false;
        gameObject.GetComponentInChildren<SpriteRenderer>().color = Color.white;
        setIdleAnimation();
    }
    public movementStates getMovementStateEnum(int i)
    {
        if (i == 0)
        {
            return movementStates.Unselected;
        }
        else if (i == 1)
        {
            return movementStates.Selected;
        }
        else if (i == 2)
        {
            return movementStates.Moved;
        }
        else if (i == 3)
        {
            return movementStates.Wait;
        }
        return movementStates.Unselected;
    }
    public void setMovementState(int i)
    {
        if (i == 0)
        {
            unitMoveState =  movementStates.Unselected;
        }
        else if (i == 1)
        {
            unitMoveState = movementStates.Selected;
        }
        else if (i == 2)
        {
            unitMoveState = movementStates.Moved;
        }
        else if (i == 3)
        {
            unitMoveState = movementStates.Wait;
        }
    }
    public void updateHealthUI()
    {
        healthBar.fillAmount = (float)currentHealthPoints / maxHealthPoints;
        hitPointsText.SetText(currentHealthPoints.ToString());
    }
    public void dealDamage(int x)
    {
        currentHealthPoints = currentHealthPoints - x;
        updateHealthUI();
    }
    public void wait()
    {
        gameObject.GetComponentInChildren<SpriteRenderer>().color = Color.gray;
    }
    public void changeHealthBarColour(int i)
    {
        if (i == 0)
        {
            healthBar.color = Color.blue;
        }
        else if (i == 1)
        {
            healthBar.color = Color.red;
        }
    }
    public void unitDie()
    {
        if (holder2D.activeSelf)
        {
            StartCoroutine(fadeOut());
            StartCoroutine(checkIfRoutinesRunning());
        }
    } 
    public IEnumerator checkIfRoutinesRunning()
    {
        while (combatQueue.Count>0)
        {
            yield return new WaitForEndOfFrame();
        }
        Destroy(gameObject);
    }    
    public IEnumerator fadeOut()
    {
        combatQueue.Enqueue(1);
        Renderer rend = GetComponentInChildren<SpriteRenderer>();
        
        for (float f = 1f; f >= .05; f -= 0.01f)
        {
            Color c = rend.material.color;
            c.a = f;
            rend.material.color = c;
            yield return new WaitForEndOfFrame();
        }
        combatQueue.Dequeue();
    }
    public IEnumerator moveOverSeconds(GameObject objectToMove,Node endNode)
    {
        movementQueue.Enqueue(1);
        path.RemoveAt(0);
        while (path.Count != 0)
        {
            Vector3 endPos = map.tileCoordToWorldCoord(path[0].x, path[0].y);
            objectToMove.transform.position = Vector3.Lerp(transform.position, endPos, visualMovementSpeed);
            if ((transform.position - endPos).sqrMagnitude < 0.001)
            {

                path.RemoveAt(0);
              
            }
            yield return new WaitForEndOfFrame();
        }
        visualMovementSpeed = 0.15f;
        transform.position = map.tileCoordToWorldCoord(endNode.x, endNode.y);
        x = endNode.x;
        y = endNode.y;
        tileBeingOccupied.GetComponent<ClickableTileScript>().unitOnTile = null;
        tileBeingOccupied = map.tilesOnMap[x, y];
        movementQueue.Dequeue();
        /*gameManagerScript CallCam = GameObject.FindObjectOfType<gameManagerScript>();
        gameManagerScript.ToggleCameraTarget(isOnTarget1);*/
    }

    public IEnumerator displayDamageEnum(int damageTaken)
    {
        combatQueue.Enqueue(1);
        damagePopupText.SetText(damageTaken.ToString());
        damagePopupCanvas.enabled = true;
        for (float f = 1f; f >=-0.01f; f -= 0.01f)
        {
            Color backDrop = damageBackdrop.GetComponent<Image>().color;
            Color damageValue = damagePopupText.color;
            backDrop.a = f;
            damageValue.a = f;
            damageBackdrop.GetComponent<Image>().color = backDrop;
            damagePopupText.color = damageValue;
            yield return new WaitForEndOfFrame();
        }
        combatQueue.Dequeue();
    }
    public void resetPath()
    {
        path = null;
        completedMovement = false;
    }
    public void displayDamage(int damageTaken)
    {
        damagePopupCanvas.enabled = true;
        damagePopupText.SetText(damageTaken.ToString());
    }
    public void disableDisplayDamage()
    {
        damagePopupCanvas.enabled = false;
    }
    public void setSelectedAnimation()
    {
        animator.SetTrigger("toSelected");
    }
    public void setIdleAnimation()
    {        
        animator.SetTrigger("toIdle");
    }
    public void setWalkingAnimation()
    {
        animator.SetTrigger("toWalking");
    }
    public void setAttackAnimation()
    {
       animator.SetTrigger("toAttacking");
    }
    public void setWaitIdleAnimation()
    {
        animator.SetTrigger("toIdleWait");
    }
    public void setDieAnimation()
    {
        animator.SetTrigger("dieTrigger");
    }
}
