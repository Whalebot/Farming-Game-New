using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Sirenix.OdinInspector;
using UnityEngine.InputSystem;
public class InteractScript : MonoBehaviour
{
    public bool canInteract;
    public bool grid;
    public float gridSize;
    public bool gridOffset;
    public bool placingItem;

    [TabGroup("Debug")] public InteractionType interactionType;
    [TabGroup("Debug")] public bool blocked;
    [TabGroup("Debug")] public bool inSoil;
    [TabGroup("Debug")] public ItemScript lastItem;
    [TabGroup("Debug")] public PlantScript lastPlant;
    [TabGroup("Debug")] public Interactable lastInteractable;
    [TabGroup("Debug")] public Vector3 groundPos;
    [TabGroup("Debug")] public float rayLength;
    [TabGroup("Debug")] public LayerMask groundMask;
    [TabGroup("Debug")] public Transform previewHolder;
    RaycastHit hit;
    Renderer rend;
    Vector3 nearestGrid;


    [TabGroup("Components")] public Movement move;
    [TabGroup("Components")] public Status status;
    [TabGroup("Components")] public GameObject itemPrompt;
    [TabGroup("Components")] public LayerMask blockMask;
    [TabGroup("Components")] public LayerMask interactMask;
    [TabGroup("Components")] public TextMeshProUGUI interactText;
    [TabGroup("Components")] public Transform reference;
    [TabGroup("Components")] public Transform player;
    [TabGroup("Components")] public Material baseMaterial;
    [TabGroup("Components")] public Material pickupMaterial;
    [TabGroup("Components")] public Material failMaterial;
    [TabGroup("Components")] public InventoryScript inventoryScript;
    [TabGroup("Components")] public Skill appraisalSkill;
    [TabGroup("Components")] public AppaisalWindow appaisalWindow;


    // Start is called before the first frame update
    void Start()
    {
        rend = GetComponentInChildren<Renderer>();
        status.animationEvent += DetectCollisions;
        InputManager.Instance.leftInput += RotateLeft;
        InputManager.Instance.rightInput += RotateRight;
        GameManager.Instance.advanceGameState += ExecuteFrame;
    }

    private void OnDisable()
    {
        status.animationEvent -= DetectCollisions;
        InputManager.Instance.leftInput -= RotateLeft;
        InputManager.Instance.rightInput -= RotateRight;
        GameManager.Instance.advanceGameState -= ExecuteFrame;
    }


    void ExecuteFrame()
    {
        if (Physics.Raycast(transform.position + Vector3.up * 0.5F, Vector3.down, out hit, rayLength, groundMask))
            groundPos = hit.point;

        rend.enabled = !move.isMoving && status.currentState == Status.State.Neutral && !GameManager.isPaused;
        if (status.currentState == Status.State.Neutral)
        {
            DetectCollisions();
        }
    }

    void RotateLeft()
    {
        previewHolder.Rotate(previewHolder.up, 45);
    }
    void RotateRight()
    {
        previewHolder.Rotate(previewHolder.up, -45);
    }

    Vector3 FindNearestGrid()
    {
        nearestGrid = reference.position;
        nearestGrid.x = RoundToGridSize(nearestGrid.x);
        nearestGrid.y = Mathf.Round(nearestGrid.y);
        nearestGrid.z = RoundToGridSize(nearestGrid.z);

        return nearestGrid;
    }

    float RoundToGridSize(float f)
    {
        float closest = Mathf.Round(f / gridSize);
        float f1 = closest * gridSize;
        float f2 = closest * gridSize;

        if (gridOffset)
        {
            f1 = closest * gridSize + gridSize / 2;
            f2 = closest * gridSize - gridSize / 2;
        }

        if (Mathf.Abs(f1 - f) > Mathf.Abs(f2 - f))
        {
            return f2;
        }
        else
        {
            return f1;
        }
    }

    void DetectCollisions()
    {
        canInteract = false;
        blocked = false;
        itemPrompt.SetActive(false);
        appaisalWindow.gameObject.SetActive(false);

        if (grid)
            transform.position = FindNearestGrid();
        else { transform.position = reference.position; }

        rend.material = baseMaterial;

        Collider[] col = Physics.OverlapBox(transform.position, Vector3.one * 0.5F, transform.rotation, interactMask);
        foreach (Collider item in col)
        {
            if (item.transform.IsChildOf(status.transform)) {
                Debug.Log("Found child of player");
                continue; }
            
            Interactable temp = item.GetComponentInParent<Interactable>();

            if (temp == null) continue;
            canInteract = true;
            if (item.tag != "Player" && item.tag != "Terrain" && item.tag != "Plant")
            {
                blocked = true;
            }
            if (temp is ItemScript)
            {
                interactionType = InteractionType.Item;
                lastItem = (ItemScript)temp;
                interactText.text = "" + temp.type;
                itemPrompt.SetActive(!GameManager.menuOpen && !TutorialManager.Instance.inTutorial);
            }
            else if (temp is PlantScript)
            {
                lastPlant = (PlantScript)temp;
                if (appraisalSkill.level > 0)
                {
                    appaisalWindow.DisplayUI(lastPlant);
                    appaisalWindow.gameObject.SetActive(true);
                }
                if (!lastPlant.finished || lastPlant.picked) return;

                interactionType = InteractionType.Plant;
                if (lastPlant.type != Interactable.Type.None)
                {
                    interactText.text = "" + temp.type;
                    itemPrompt.SetActive(!GameManager.menuOpen);
                }
            }
            else
            {
                interactionType = InteractionType.Generic;
                lastInteractable = temp;
                interactText.text = "" + temp.type;
                itemPrompt.SetActive(!GameManager.menuOpen);
            }
        }

        if (interactionType != InteractionType.None)
        {
            if (!placingItem)
            {
                rend.material = pickupMaterial;
            }
            else rend.material = failMaterial;
        }
        if (blocked)
        {
            //canInteract = false;
            rend.material = failMaterial;
        }
    }

    public void PickItem()
    {
        if (lastItem != null)
            lastItem.Interact();
    }

    public void PickPlant()
    {
        if (!lastPlant.finished) return;
        lastPlant.Interact();
    }
}

public enum InteractionType
{
    None,
    Generic,
    Item,
    Plant
}