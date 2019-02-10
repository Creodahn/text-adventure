using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game State")]
public class State : ScriptableObject {
  [SerializeField] string expectedInputToAccessState;
  [SerializeField] string itemAddedToInventory;
  [SerializeField] string[] inventoryRequirements;
  [SerializeField] State[] possibleNextStates;
  [TextArea(5, 7)] [SerializeField] string badInputResponse = "This is not a valid input. Try again.\n\n";
  [TextArea(10, 14)] [SerializeField] string stateText = "";
  [TextArea(10, 14)] [SerializeField] string visitedText = "";

  private bool visited = false;

  public string GetBadInputResponse() {
    return badInputResponse;
  }

  public string[] GetInventoryRequirements() {
    return inventoryRequirements;
  }

  public string GetItemAddedToInventory() {
    return itemAddedToInventory;
  }

  public string GetText() {
    string text = "";

    if(visited && visitedText.Length > 0) {
      text = visitedText;
    } else {
      text = stateText;
    }

    return text;
  }

  public State[] GetNextStates() {
    return possibleNextStates;
  }

  // match user input to state's required input
  // if there is no required input, just return true
  public bool MatchUserInput(string userInput) {
    bool result = true;

    if(expectedInputToAccessState.Length > 0) {
      result = userInput == expectedInputToAccessState;
    }

    return result;
  }

  public void SetVisited() {
    visited = true;
  }
}
