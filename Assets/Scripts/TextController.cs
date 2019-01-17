using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class TextController : MonoBehaviour {
	public Text text;
	public InputField input;
	public Text prompt;

  private bool bullets;
  private bool gun;
  private bool noFail;
  private bool keys;
  private bool lastKey;
  private bool printed; 
  private bool pursued;
  private bool stopPrinting;
  private bool started;

	private enum states {
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
  private states current_state;
  private string text_to_print;
	
	// Use this for initialization
	void Start() {
		StartCoroutine(state_start());
	}
	
	// Update is called once per frame
	void Update() {
    state_update();
	}

  IEnumerator state_start() {
    StreamReader reader = new StreamReader("Assets/Text/start.txt");
    string txt = reader.ReadToEnd();
    current_state = states.start;
    prompt.enabled = false;
    text.text = "";
    text_to_print = "";
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

    yield return slowPrint(txt);
	}

  void state_update() {
    if (!printed) {
      string current_state_str = current_state.ToString();
      string path = "Assets/Text/" + current_state + ".txt";
      string suffix = "";

      switch (current_state_str) {
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

      path = "Assets/Text/" + current_state + suffix + ".txt";

      StreamReader reader = new StreamReader(path);
      text_to_print += reader.ReadToEnd();
      reader.Close();

      text.text = text_to_print;

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
        state_transition(input.text.ToLower());
      }

      if (!started) {
        prompt.enabled = true;
        input.ActivateInputField();
        started = true;
      }
    }

  }

  IEnumerator slowPrint(string txt) {
    float seconds = 0.0005f;

    for (int i = 0; i < txt.Length; i++) {
      text.text += txt[i];

      if (i % 5 == 0) {
        yield return new WaitForSeconds(seconds);
      }
    }
  }
	
	void state_transition(string input_text) {
		if (!noFail) {
			text_to_print = "Player: " + input.text + "\n\n";
		} else {
			text_to_print = "";
		}
		//Debugging log
		/* print("Transitioning state: " +
		      "\ncurrent_state = " + current_state +
		      "\ninput_text = " + input_text +
		      "\nnoFail = " + noFail.ToString());
		 */
		printed = false;
    text_to_print = "";

    switch (current_state) {
			case states.start:
				current_state = states.wake_0;
				noFail = false;
				break;
			case states.wake_0: goto case states.tunnel_0;
			case states.tunnel_0:
				if(input_text == "look at bricks") {
					current_state = states.bricks_0;
				} else if(input_text == "look at glass") {
					current_state = states.glass_0;
				} else if(input_text == "look at rubble") {
					current_state = states.rubble_0;
				} else if(!noFail) {
					text_to_print += "This is not a valid input. Try again.\n\n";
				}
				break;
			case states.bricks_0:
				if(input_text == "return") {
					current_state = states.tunnel_0;
				} else {
          text_to_print += "This is not a valid input. Try again.\n\n";
				}
				break;
			case states.glass_0:
				if(input_text == "return") {
					current_state = states.tunnel_0;
				} else {
          text_to_print += "This is not a valid input. Try again.\n\n";
				}
				break;
			case states.bricks_1:
				if(input_text == "return") {
					current_state = states.tunnel_1;
				} else {
          text_to_print += "This is not a valid input. Try again.\n\n";
				}
				break;
			case states.glass_1:
				if(input_text == "return") {
					current_state = states.tunnel_1;
				} else {
          text_to_print += "This is not a valid input. Try again.\n\n";
				}
				break;
			case states.rubble_0:
				if(input_text == "take pipe") {
					current_state = states.rubble_1;
				} else if(input_text == "return") {
					current_state = states.tunnel_0;
				} else {
          text_to_print += "This is not a valid input. Try again.\n\n";
				}
				break;
			case states.rubble_1: goto case states.rubble_2;
			case states.tunnel_1:
				if(input_text == "look at bricks") {
					current_state = states.bricks_1;
				} else if(input_text == "look at glass") {
					current_state = states.glass_1;
				} else if(input_text == "look at rubble") {
					current_state = states.rubble_1;
				} else {
          text_to_print += "This is not a valid input. Try again.\n\n";
				}
				break;
			case states.rubble_2:
				if(input_text == "use pipe on rubble") {
					current_state = states.escape;
				} else if(input_text == "return") {
					current_state = states.tunnel_1;
				} else {
          text_to_print += "This is not a valid input. Try again.\n\n";
				}
				break;
			case states.escape:
				if(input_text == "restart") {
					noFail = true;
					current_state = states.wake_1;
				} else {
          text_to_print = "Sorry, the game is over. Please type RESTART to play again.";
				}
				break;
			case states.tunnel_2: goto case states.wake_1;
			case states.wake_1:
				noFail = false;
				if(input_text == "look at bricks") {
					current_state = states.bricks_2;
				} else if(input_text == "look at glass") {
					current_state = states.glass_2;
				} else if(input_text == "look at rubble") {
					current_state = states.rubble_3;
				} else {
          text_to_print += "This is not a valid input. Try again.\n\n";
				}				
				break;
			case states.tunnel_3:
				if(input_text == "look at bricks") {
					current_state = states.bricks_4;
				} else if(input_text == "look at glass") {
					current_state = states.glass_3;
				} else if(input_text == "look at rubble") {
					current_state = states.rubble_4;
				} else {
          text_to_print += "This is not a valid input. Try again.\n\n";
				}
				break;
			case states.bricks_2:
				if(input_text == "pull bricks") {
					current_state = states.bricks_3;
				} else if (input_text == "return") {
					current_state = states.tunnel_2;
				} else {
          text_to_print += "This is not a valid input. Try again.\n\n";
				}
				break;
			case states.rubble_3: goto case states.glass_2;
			case states.glass_2:
				if(input_text == "return") {
					current_state = states.tunnel_2;
				} else {
          text_to_print += "This is not a valid input. Try again.\n\n";
				}
				break;
			case states.rubble_4: goto case states.glass_3;
			case states.glass_3:
				if(input_text == "return") {
					current_state = states.tunnel_3;
				} else {
          text_to_print += "This is not a valid input. Try again.\n\n";
				}
				break;
			case states.bricks_3: goto case states.bricks_4;
			case states.bricks_4:
				if(input_text == "enter opening") {
					current_state = states.dark_tunnel_0;
				} else if (input_text == "return") {
					current_state = states.tunnel_3;
				} else {
          text_to_print += "This is not a valid input. Try again\n\n";
				}
				break;
			case states.dark_tunnel_0:
				if(input_text == "fumble around") {
					current_state = states.fumble;
				} else {
          text_to_print += "You can't do that here. It's very, very dark.\n\n";
				}
				break;
			case states.fumble:
				if(input_text == "fumble around") {
					int roll = Random.Range(1, 10);
					if(roll < 3) {
						fumbles++;
						current_state = states.fumble;
					} else {
						current_state = states.dim_tunnel_0;
					}
				} else {
          text_to_print += "You can't do that here. It's very, very dark.\n\n";
				}
				break;
			case states.dim_tunnel_0: 
				if(input_text == "look around") {
					current_state = states.dim_tunnel_1;
				} else {
          text_to_print += "It may be that the beam of the flashlight is too dim, " +
					             "but you can't find whatever that was to do that.\n\n";
				}
				break;
			case states.dim_tunnel_1:
				if(input_text == "go right") {
					current_state = states.right_tunnel_0;
				} else if(input_text == "go left") {
					current_state = states.left_tunnel_0;
				} else if(input_text == "cower") {
					current_state = states.dim_tunnel_2;
				} else {
          text_to_print += "It may be that the beam of the flashlight is too dim, " +
								 "but you can't find whatever that was to do that.\n\n";
				}
				break;
			case states.dim_tunnel_2:
				if(input_text == "go right") {
					current_state = states.right_tunnel_0;
				} else if(input_text == "go left") {
					current_state = states.left_tunnel_0;
				} else if(input_text == "listen sound") {
					current_state = states.dim_tunnel_3;
				} else {
          text_to_print += "It may be that the beam of the flashlight is too dim, " +
					 			 "but you can't find whatever that was to do that.\n\n";
				}
				break;
			case  states.dim_tunnel_3:
				if(input_text == "go right") {
					current_state = states.right_tunnel_0;
				} else if(input_text == "go left") {
					current_state = states.left_tunnel_0;
				} else {
          text_to_print += "You're not sure if it's the fear, or you just don't know " +
                                 "what you were thinking, but you can't figure out how to do that.\n\n";
				}
				break;
			case states.right_tunnel_0: 
				if(input_text == "restart"){
					current_state = states.wake_1;
				} else {
          text_to_print += "Sorry, you're quite dead and unable to do that, or indeed anything else.\n\n" +
	                             "Type RESTART to try again.";
				}
				break;
			case states.left_tunnel_0:
				if(input_text == "follow tunnel") {
					current_state = states.l_tunnel_0;
				} else {
          text_to_print += "There's nothing to do here but follow the tunnel.\n\n";
				}
				break;
			case states.alcove_0:
				if(input_text == "take keys") {
					current_state = states.alcove_2;
					keys = true;
				} else if (input_text == "return") {
					current_state =  states.l_tunnel_1;
				} else {
          text_to_print += "You can't do that.\n\n";
				}
				break;
			case states.alcove_1:
				if(input_text == "take keys") {
					current_state = states.alcove_2;
					keys = true;
				} else if (input_text == "return") {
					current_state =  states.l_tunnel_1;
				} else {
          text_to_print += "You can't do that.\n\n";
				}
				break;
			case states.alcove_2:
				if (input_text == "return") {
					current_state = states.l_tunnel_1;
				} else {
          text_to_print += "You can't do that.\n\n";
				}
				break;
			case states.l_tunnel_0:
				if(input_text == "enter alcove") {
					current_state = states.alcove_0;
				} else {
          text_to_print += "On second thought, it's too dark to do that.\n\n";
				}
				break;
			case states.l_tunnel_1:
				if(input_text == "follow tunnel") {
					if(keys) {
						current_state = states.locked_door_1;
					} else {
						current_state = states.locked_door_0;
					}
				} else if (input_text == "enter bathroom") {
					if(gun) {
						current_state = states.bathroom_1;
					} else {
						current_state = states.bathroom_0;
					}
				} else if(input_text == "approach bench") {
					if(bullets) {
						current_state = states.bench_1;
					} else {
						current_state = states.bench_0;
					}
				} else {
          text_to_print += "... huh?";
				}
				break;
			case states.bathroom_0:
				if (input_text == "enter stall") {
					current_state = states.stall_0;
				} else if (input_text == "leave") {
					current_state = states.l_tunnel_1;
				} else {
          text_to_print += "You look around in confusion. What were you trying to do?\n\n";
				}
				break;
			case states.bathroom_1:
				if (input_text == "enter stall") {
					current_state = states.stall_1;
				} else if (input_text == "leave") {
					current_state = states.l_tunnel_1;
				} else {
          text_to_print += "You look around in confusion. What were you trying to do?\n\n";
				}
				break;
			case states.stall_0:
				if (input_text == "take gun") {
					gun = true;
					current_state = states.stall_1;
				} else if (input_text == "return") {
					current_state = states.bathroom_1;
				} else {
          text_to_print += "You can't do that in front of a corpse!\n\n";
				}
				break;
			case states.stall_1:
				if (input_text == "return") {
					current_state = states.bathroom_1;
				} else {
          text_to_print += "You can't do that in front of a corpse!\n\n";
				}
				break;
			case states.bench_0:
				if (input_text == "return") {
					current_state = states.l_tunnel_1;
				} else {
          text_to_print += "... what?\n\n";
				}
				break;
			case states.bench_1:
				if (input_text == "return") {
					current_state = states.l_tunnel_1;
				} else {
          text_to_print += "... what?\n\n";
				}
				break;
			case states.locked_door_0:
				if (input_text == "return") {
					current_state = states.l_tunnel_1;
				} else {
          text_to_print += "... what?\n\n";
				}
				break;
			case states.locked_door_1:
				if(input_text == "open door") {
					if(gun && bullets) {
						current_state = states.boss_room_1;
					} else if(gun && !bullets) {
						current_state = states.boss_room_2;
					} else if(!gun && bullets) {
						current_state = states.boss_room_3;
					} else {
						current_state = states.boss_room_0;
					}
				} else if(input_text == "return") {
					current_state = states.l_tunnel_1;
				} else {
          text_to_print += "... what?\n\n";
				}
				break;
			case states.boss_room_0: goto case states.boss_room_3;
			case states.boss_room_1:
				current_state = states.victory;
				break;
			case states.boss_room_2: goto case states.boss_room_3;
			case states.boss_room_3:
				current_state = states.death;
				break;
			case states.death: goto case states.victory;
			case states.victory:
				if(input_text == "restart") {
					StartCoroutine(state_start());
				} else {
          text_to_print += "Sorry, the game is really over this time. You can restart by typing RESTART.";
				}
				break;
			default:
        text_to_print = "Hey, idiot. You didn't code that input case.";
				break;
		}
	}
}
