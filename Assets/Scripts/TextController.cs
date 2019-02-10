using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class TextController : MonoBehaviour {
  [SerializeField] State initialState;

  public InputField input;
  public Text text;
  public Text prompt;

  private bool noFail;
  private bool lastKey;
  private bool printed;
  private bool started;
  private Inventory inventory;

  private State currentState;
  private string textToPrint;
  
  // Use this for initialization
  void Start() {
    StartCoroutine(StateStart());
  }
  
  // Update is called once per frame
  void Update() {
    StateUpdate();
  }

  // figure out which state should be returned
  State ManageState(string userInput) {
    State newState = null;
    State[] nextStates = currentState.GetNextStates();

    // if there's only one possible state, just return it if input is correct
    if(nextStates.Length == 1 && nextStates[0].MatchUserInput(userInput)) {
      newState = nextStates[0];
    } else {
      for (int i = 0; i < nextStates.Length; i++) {
        // match user input to required input for possible state
        if(nextStates[i].MatchUserInput(userInput)) {
          string[] reqs = nextStates[i].GetInventoryRequirements();
          bool hasAllReqs = true;
          string newItem = nextStates[i].GetItemAddedToInventory();

          // this is a little weird, but the item needs to be in the inventory before
          // checking to see if it's required for the transition
          // TODO: find a better solution for this
          // this also causes a problem with picking up the gun, since both transitions are
          // available from the main part of the bathroom
          if(newItem.Length > 0) {
            inventory.pickupItem(newItem);
          }

          // if state has inventory requirements, figure out if item is in inventory
          if (reqs.Length > 0) { 
            for (int j = 0; j < reqs.Length; j++) {
              hasAllReqs = hasAllReqs && inventory.checkInventory(reqs[j]);
            }
          }
          
          if(hasAllReqs) {
            newState = nextStates[i];
            break;
          }
        }
      }
    }


    return newState;
  }

  IEnumerator StateStart() {
    currentState = initialState;
    prompt.enabled = false;
    text.text = "";
    textToPrint = "";
    lastKey = false;
    printed = false;
    noFail = true;
    input.text = "";
    input.DeactivateInputField();
    printed = true;
    inventory = (Inventory)ScriptableObject.CreateInstance("Inventory");
    inventory.Initialize();

    yield return SlowPrint(initialState.GetText());
  }

  void StateUpdate() {
    if (!printed) {
      text.text = textToPrint;

      printed = true;
    }

    if (lastKey) {
      lastKey = false;
      input.text = "";
      input.ActivateInputField();
    }

    if (Input.GetKeyDown(KeyCode.Return)) {
      lastKey = true;

      if (printed) {
        StateTransition(input.text.ToLower());
      }

      if (!started) {
        prompt.enabled = true;
        input.ActivateInputField();
        started = true;
      }
    }
  }

  IEnumerator SlowPrint(string txt) {
    float seconds = 0.0005f;

    for (int i = 0; i < txt.Length; i++) {
      text.text += txt[i];

      if (i % 5 == 0) {
        yield return new WaitForSeconds(seconds);
      }
    }
  }
  
  void StateTransition(string inputText) {
    State nextState = ManageState(inputText);
    Debug.Log(inputText);
    Debug.Log(nextState);

    if (!noFail) {
      textToPrint = "Player: " + inputText + "\n\n";
    } else {
      textToPrint = "";
    }

    if(nextState != null) {
      textToPrint += nextState.GetText();
      currentState = nextState;
      currentState.SetVisited();
    } else {
      textToPrint = currentState.GetBadInputResponse();
    }

    printed = false;
  }
}
