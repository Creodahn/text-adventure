using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class TextController : MonoBehaviour {
  public InputField input;
  public Text text;
  public Text prompt;

  private bool bullets;
  private bool gun;
  private bool noFail;
  private bool keys;
  private bool lastKey;
  private bool printed; 
  private bool pursued;
  private bool started;

  private enum States {
    start, wake_0, bricks_0, glass_0, rubble_0, tunnel_0, 
    wake_1, bricks_1, glass_1, rubble_1, tunnel_1, escape,
    bricks_2, glass_2, rubble_2, tunnel_2,
    bricks_3, glass_3, rubble_3, tunnel_3,
    bricks_4, rubble_4,
    dark_tunnel_0, fumble, dim_tunnel_0,
    dim_tunnel_1, dim_tunnel_2, right_tunnel_0, right_tunnel_1,
    left_tunnel_0, left_tunnel_1, dim_tunnel_3, l_tunnel_0,
    l_tunnel_1, alcove_0, alcove_1, alcove_2, bathroom_0,
    bathroom_1, stall_0, stall_1, bench_0, bench_1,
    locked_door_0, locked_door_1,
    boss_room_0, boss_room_1, boss_room_2, boss_room_3,
    death, victory
  };

  private int fumbles;
  private States currentState;
  private string textToPrint;
  
  // Use this for initialization
  void Start() {
    StartCoroutine(StateStart());
  }
  
  // Update is called once per frame
  void Update() {
    StateUpdate();
  }

  IEnumerator StateStart() {
    StreamReader reader = new StreamReader("Assets/Text/start.txt");
    string txt = reader.ReadToEnd();
    currentState = States.start;
    prompt.enabled = false;
    text.text = "";
    textToPrint = "";
    lastKey = false;
    printed = false;
    pursued = false;
    bullets = false;
    gun = false;
    fumbles = 0;
    noFail = true;
    keys = false;
    input.text = "";
    input.DeactivateInputField();
    printed = true;
    reader.Close();

    yield return SlowPrint(txt);
  }

  void StateUpdate() {
    if (!printed) {
      string currentState_str = currentState.ToString();
      string path = "Assets/Text/" + currentState + ".txt";
      string suffix = "";

      switch (currentState_str) {
        case "fumble":
          suffix = "_default";

          if (fumbles >= 0 && fumbles < 3) {
            suffix = "_" + fumbles.ToString();
          }

          break;
        case "left_tunnel_0":
        case "right_tunnel_0":
          if (pursued) {
            suffix = "_pursued";
          }
          break;
      }

      path = "Assets/Text/" + currentState + suffix + ".txt";

      StreamReader reader = new StreamReader(path);
      textToPrint += reader.ReadToEnd();
      reader.Close();

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
    if (!noFail) {
      textToPrint = "Player: " + input.text + "\n\n";
    } else {
      textToPrint = "";
    }

    printed = false;
    textToPrint = "";

    switch (currentState) {
      case States.start:
        currentState = States.wake_0;
        noFail = false;
        break;
      case States.wake_0: goto case States.tunnel_0;
      case States.tunnel_0:
        if(inputText == "look at bricks") {
          currentState = States.bricks_0;
        } else if(inputText == "look at glass") {
          currentState = States.glass_0;
        } else if(inputText == "look at rubble") {
          currentState = States.rubble_0;
        } else if(!noFail) {
          textToPrint += "This is not a valid input. Try again.\n\n";
        }
        break;
      case States.bricks_0:
        if(inputText == "return") {
          currentState = States.tunnel_0;
        } else {
          textToPrint += "This is not a valid input. Try again.\n\n";
        }
        break;
      case States.glass_0:
        if(inputText == "return") {
          currentState = States.tunnel_0;
        } else {
          textToPrint += "This is not a valid input. Try again.\n\n";
        }
        break;
      case States.bricks_1:
        if(inputText == "return") {
          currentState = States.tunnel_1;
        } else {
          textToPrint += "This is not a valid input. Try again.\n\n";
        }
        break;
      case States.glass_1:
        if(inputText == "return") {
          currentState = States.tunnel_1;
        } else {
          textToPrint += "This is not a valid input. Try again.\n\n";
        }
        break;
      case States.rubble_0:
        if(inputText == "take pipe") {
          currentState = States.rubble_1;
        } else if(inputText == "return") {
          currentState = States.tunnel_0;
        } else {
          textToPrint += "This is not a valid input. Try again.\n\n";
        }
        break;
      case States.rubble_1: goto case States.rubble_2;
      case States.tunnel_1:
        if(inputText == "look at bricks") {
          currentState = States.bricks_1;
        } else if(inputText == "look at glass") {
          currentState = States.glass_1;
        } else if(inputText == "look at rubble") {
          currentState = States.rubble_1;
        } else {
          textToPrint += "This is not a valid input. Try again.\n\n";
        }
        break;
      case States.rubble_2:
        if(inputText == "use pipe on rubble") {
          currentState = States.escape;
        } else if(inputText == "return") {
          currentState = States.tunnel_1;
        } else {
          textToPrint += "This is not a valid input. Try again.\n\n";
        }
        break;
      case States.escape:
        if(inputText == "restart") {
          noFail = true;
          currentState = States.wake_1;
        } else {
          textToPrint = "Sorry, the game is over. Please type RESTART to play again.";
        }
        break;
      case States.tunnel_2: goto case States.wake_1;
      case States.wake_1:
        noFail = false;
        if(inputText == "look at bricks") {
          currentState = States.bricks_2;
        } else if(inputText == "look at glass") {
          currentState = States.glass_2;
        } else if(inputText == "look at rubble") {
          currentState = States.rubble_3;
        } else {
          textToPrint += "This is not a valid input. Try again.\n\n";
        }        
        break;
      case States.tunnel_3:
        if(inputText == "look at bricks") {
          currentState = States.bricks_4;
        } else if(inputText == "look at glass") {
          currentState = States.glass_3;
        } else if(inputText == "look at rubble") {
          currentState = States.rubble_4;
        } else {
          textToPrint += "This is not a valid input. Try again.\n\n";
        }
        break;
      case States.bricks_2:
        if(inputText == "pull bricks") {
          currentState = States.bricks_3;
        } else if (inputText == "return") {
          currentState = States.tunnel_2;
        } else {
          textToPrint += "This is not a valid input. Try again.\n\n";
        }
        break;
      case States.rubble_3: goto case States.glass_2;
      case States.glass_2:
        if(inputText == "return") {
          currentState = States.tunnel_2;
        } else {
          textToPrint += "This is not a valid input. Try again.\n\n";
        }
        break;
      case States.rubble_4: goto case States.glass_3;
      case States.glass_3:
        if(inputText == "return") {
          currentState = States.tunnel_3;
        } else {
          textToPrint += "This is not a valid input. Try again.\n\n";
        }
        break;
      case States.bricks_3: goto case States.bricks_4;
      case States.bricks_4:
        if(inputText == "enter opening") {
          currentState = States.dark_tunnel_0;
        } else if (inputText == "return") {
          currentState = States.tunnel_3;
        } else {
          textToPrint += "This is not a valid input. Try again\n\n";
        }
        break;
      case States.dark_tunnel_0:
        if(inputText == "fumble around") {
          currentState = States.fumble;
        } else {
          textToPrint += "You can't do that here. It's very, very dark.\n\n";
        }
        break;
      case States.fumble:
        if(inputText == "fumble around") {
          int roll = Random.Range(1, 10);
          if(roll < 3) {
            fumbles++;
            currentState = States.fumble;
          } else {
            currentState = States.dim_tunnel_0;
          }
        } else {
          textToPrint += "You can't do that here. It's very, very dark.\n\n";
        }
        break;
      case States.dim_tunnel_0: 
        if(inputText == "look around") {
          currentState = States.dim_tunnel_1;
        } else {
          textToPrint += "It may be that the beam of the flashlight is too dim, " +
                         "but you can't find whatever that was to do that.\n\n";
        }
        break;
      case States.dim_tunnel_1:
        if(inputText == "go right") {
          currentState = States.right_tunnel_0;
        } else if(inputText == "go left") {
          currentState = States.left_tunnel_0;
        } else if(inputText == "cower") {
          currentState = States.dim_tunnel_2;
        } else {
          textToPrint += "It may be that the beam of the flashlight is too dim, " +
                 "but you can't find whatever that was to do that.\n\n";
        }
        break;
      case States.dim_tunnel_2:
        if(inputText == "go right") {
          currentState = States.right_tunnel_0;
        } else if(inputText == "go left") {
          currentState = States.left_tunnel_0;
        } else if(inputText == "listen sound") {
          currentState = States.dim_tunnel_3;
        } else {
          textToPrint += "It may be that the beam of the flashlight is too dim, " +
                          "but you can't find whatever that was to do that.\n\n";
        }
        break;
      case  States.dim_tunnel_3:
        if(inputText == "go right") {
          currentState = States.right_tunnel_0;
        } else if(inputText == "go left") {
          currentState = States.left_tunnel_0;
        } else {
          textToPrint += "You're not sure if it's the fear, or you just don't know " +
                         "what you were thinking, but you can't figure out how to do that.\n\n";
        }
        break;
      case States.right_tunnel_0: 
        if(inputText == "restart"){
          currentState = States.wake_1;
        } else {
          textToPrint += "Sorry, you're quite dead and unable to do that, or indeed anything else.\n\n" +
                         "Type RESTART to try again.";
        }
        break;
      case States.left_tunnel_0:
        if(inputText == "follow tunnel") {
          currentState = States.l_tunnel_0;
        } else {
          textToPrint += "There's nothing to do here but follow the tunnel.\n\n";
        }
        break;
      case States.alcove_0:
        if(inputText == "take keys") {
          currentState = States.alcove_2;
          keys = true;
        } else if (inputText == "return") {
          currentState =  States.l_tunnel_1;
        } else {
          textToPrint += "You can't do that.\n\n";
        }
        break;
      case States.alcove_1:
        if(inputText == "take keys") {
          currentState = States.alcove_2;
          keys = true;
        } else if (inputText == "return") {
          currentState =  States.l_tunnel_1;
        } else {
          textToPrint += "You can't do that.\n\n";
        }
        break;
      case States.alcove_2:
        if (inputText == "return") {
          currentState = States.l_tunnel_1;
        } else {
          textToPrint += "You can't do that.\n\n";
        }
        break;
      case States.l_tunnel_0:
        if(inputText == "enter alcove") {
          currentState = States.alcove_0;
        } else {
          textToPrint += "On second thought, it's too dark to do that.\n\n";
        }
        break;
      case States.l_tunnel_1:
        if(inputText == "follow tunnel") {
          if(keys) {
            currentState = States.locked_door_1;
          } else {
            currentState = States.locked_door_0;
          }
        } else if (inputText == "enter bathroom") {
          if(gun) {
            currentState = States.bathroom_1;
          } else {
            currentState = States.bathroom_0;
          }
        } else if(inputText == "approach bench") {
          if(bullets) {
            currentState = States.bench_1;
          } else {
            currentState = States.bench_0;
          }
        } else {
          textToPrint += "... huh?";
        }
        break;
      case States.bathroom_0:
        if (inputText == "enter stall") {
          currentState = States.stall_0;
        } else if (inputText == "leave") {
          currentState = States.l_tunnel_1;
        } else {
          textToPrint += "You look around in confusion. What were you trying to do?\n\n";
        }
        break;
      case States.bathroom_1:
        if (inputText == "enter stall") {
          currentState = States.stall_1;
        } else if (inputText == "leave") {
          currentState = States.l_tunnel_1;
        } else {
          textToPrint += "You look around in confusion. What were you trying to do?\n\n";
        }
        break;
      case States.stall_0:
        if (inputText == "take gun") {
          gun = true;
          currentState = States.stall_1;
        } else if (inputText == "return") {
          currentState = States.bathroom_1;
        } else {
          textToPrint += "You can't do that in front of a corpse!\n\n";
        }
        break;
      case States.stall_1:
        if (inputText == "return") {
          currentState = States.bathroom_1;
        } else {
          textToPrint += "You can't do that in front of a corpse!\n\n";
        }
        break;
      case States.bench_0:
        if (inputText == "return") {
          currentState = States.l_tunnel_1;
        } else {
          textToPrint += "... what?\n\n";
        }
        break;
      case States.bench_1:
        if (inputText == "return") {
          currentState = States.l_tunnel_1;
        } else {
          textToPrint += "... what?\n\n";
        }
        break;
      case States.locked_door_0:
        if (inputText == "return") {
          currentState = States.l_tunnel_1;
        } else {
          textToPrint += "... what?\n\n";
        }
        break;
      case States.locked_door_1:
        if(inputText == "open door") {
          if(gun && bullets) {
            currentState = States.boss_room_1;
          } else if(gun && !bullets) {
            currentState = States.boss_room_2;
          } else if(!gun && bullets) {
            currentState = States.boss_room_3;
          } else {
            currentState = States.boss_room_0;
          }
        } else if(inputText == "return") {
          currentState = States.l_tunnel_1;
        } else {
          textToPrint += "... what?\n\n";
        }
        break;
      case States.boss_room_0: goto case States.boss_room_3;
      case States.boss_room_1:
        currentState = States.victory;
        break;
      case States.boss_room_2: goto case States.boss_room_3;
      case States.boss_room_3:
        currentState = States.death;
        break;
      case States.death: goto case States.victory;
      case States.victory:
        if(inputText == "restart") {
          StartCoroutine(Statestart());
        } else {
          textToPrint += "Sorry, the game is really over this time. You can restart by typing RESTART.";
        }
        break;
      default:
        textToPrint = "Hey, idiot. You didn't code that input case.";
        break;
    }
  }
}
