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
		if(!printed) {
      StreamReader reader = new StreamReader("Assets/Text/" + current_state + ".txt");
      text_to_print += reader.ReadToEnd();
      text.text = text_to_print;
      reader.Close();
      printed = true;
      text_to_print = "";
		}
		
		if (lastKey) {
			lastKey = false;
			input.text = "";
			input.ActivateInputField ();
		}
		
		if (Input.GetKeyDown (KeyCode.Return)) {
			lastKey = true;
			
			if(printed) {
				state_transition(input.text.ToLower());
			}

            if(!started) {
                prompt.enabled = true;
                input.ActivateInputField();
                started = true;
            }
        }
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
    //lighted = false;
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

  IEnumerator slowPrint(string txt) {
    float seconds = 0.0005f;

    for (int i = 0; i < txt.Length; i++) {
      text.text += txt[i];

      if (i % 5 == 0) {
        yield return new WaitForSeconds(seconds);
      }
    }
  }
	
	void state_dark_tunnel0() {
		text.text += "As you crawl through the opening in the pile of bricks, " +
		             "chunks of mortar fall on you. At first, you are too " +
		             "distracted by picking your way through the small space to " +
		             "notice, but as you continue and more and more detritus falls " +
		             "on you, it is impossible to ignore. Where before you had plenty " +
		             "of space,	you are now beginning to feel crushed by the weight " +
		             "of the fallen bricks. You crawl through as fast as you can, and " +
		             "manage to get to the other side of the pile of bricks just as " +
		             "the whole thing collapses. You are now trapped on this side of the " +
		             "pile.\n\n" +
		             "You can FUMBLE AROUND in the dark to see if you can find anything.";
		printed = true;
	}
	
	void state_fumbling() {
		switch(fumbles) {
			case 1:
				text.text += "You think that you find something, but it's just a chunk of brick.\n\n" +
							 "You can FUMBLE AROUND in the dark some more to see if you can find anything.";
				break;
			case 2:
				text.text += "You slowly inch forward in the darkness. You feel something warm and rubbery, " +
				         	 "but it pulls away from your touch. You pull back in surprise.\n\n" +
							 "You can FUMBLE AROUND in the dark some more to see if you can find anything.";
				break;
			case 3:
				text.text += "You find your way to a wall and begin to crawl along it, hoping it will " +
				             "lead you somewhere with some light. Suddenly, the wall ends and you realize " +
				             "that is was just an outcropping of debris that had fallen into the tunnel.\n\n" +
							 "You can FUMBLE AROUND in the dark some more to see if you can find anything.";
				break;
			default:
				text.text += "You feel around in the dark, but are unable to find anything.\n\n" +
						 	 "You can FUMBLE AROUND in the dark some more to see if you can find anything.";
				break;
		}
		printed = true;
	}
	
	void state_dim_tunnel0() {
		text.text += "In the dark, your hand brushes something plastic and cylindrical. You " +
		             "pick it up. The cylinder widens toward one end. You feel a rectangular " +
		             "protrusion on the side. As you try to figure out what it is, the flashlight " +
		             "turns on, blinding you. It seems that you flicked the switch without realizing it.\n\n" +
		             "You can LOOK AROUND to see where you are.";
		printed = true;
	}
	
	void state_dim_tunnel1() {
		text.text += "You blink to clear your eyes. As you adjust to the glow of the flashlight, " +
		             "you examine your surroundings. You recognize the section of the tunnel from " +
		             "before whatever happened that caused the collapse. You are no longer sure " +
		             "it was an earthquake. In fact, you can't quite recall what you were doing in " +
		             "the tunnel in the first place. Perhaps if you retrace your steps you'll " +
		             "be able to figure it out?\n\n" +
		             "The tunnel forks in front of you. Damage from the collapse is evident in both " +
		             "directions. To the right, you see some wires hanging down, sparking occassionally, " +
		             "but that branch is otherwise undamaged. To the left, the walls have collapsed " +
		             "inward, leaving a small vertical space that you think you can squeeze through.\n\n" +
		             "You can GO LEFT to try squeezing through the collapsed walls.\n" +
		             "You can GO RIGHT to try sneaking through the sparking wires.\n" +
		             "You can COWER in fear.";
		printed = true;
	}
	
	void state_dim_tunnel2() {
		text.text += "After an indeterminate amount of time, you stop cowering. " +
		             "Cowering in fear has not changed your situation much. The tunnel still branches " +
		             "to the right and left.\n\n" +
		             "You notice that you have been hearing a new sound for the past few minutes. " +
		             "You are unsure of what it is, or where it is coming from.\n\n" +
					 "You can GO LEFT to try squeezing through the collapsed walls.\n" +
					 "You can GO RIGHT to try sneaking through the sparking wires.\n" +
					 "You can LISTEN to the SOUND.";
		printed = true;
	}
	
	void state_dim_tunnel3() {
		pursued = true;
		text.text += "You listen intently to the sound. As you do so, you pan " +
		             "your flashlight across the tunnel to see if you can find " +
		             "its source. Gradually, you begin to tell it is coming from " +
		             "the lefthand branch of the tunnel. As you look, a shadow " +
		             "oozes from the crack between the collapsed walls. At first " +
		             "it does not appear threatening, but it grows larger as you " +
		             "watch. The shadow takes on hard, pointed edges, and you realize " +
		             "that the sound is its inhuman growling.\n\n" +
		             "The shadow begins creeping closer, now appearing as some " +
		             "great multi-legged beast. You look around yourself, trying " +
		             "to find some way out.\n\n" +
		             "You can try to GO LEFT and get around the creature through the " +
		             "crack it came from.\n" +
		             "You can GO RIGHT and try to escape it through the sparking wires.";
		printed = true;
	}
	
	void state_left_tunnel0() {
		if(pursued) {
			text.text += "You dash past the creature, narrowly avoiding a great swipe that " +
			             "takes a new chunk out of the walls. You manage to squeeze into " +
			             "the crack just before the shadow beast can grab you. You can " +
			             "hear it scrabbling at the opening, trying to pursue you." +
						 "As you reach the end of the narrow space, the beast grabs " +
						 "the cuff of your pants. You pull as hard as you can to get away. " +
						 "For a long moment, it seems that you are stuck fast, but then " +
						 "your pants cuff gives way with a loud rip and you stumble out into " +
						 "the open tunnel beyond the collapsed walls. As you do, you collide " +
						 "with a large timber that had been keeping the passage open. " +
						 "The impact of your fall knocks it to the ground, causing the " +
						 "space between the walls to collapse completely. You shiver, " +
						 "hoping that it will be the last you see of the beast.\n\n" +
						 "You pick yourself up and dust yourself off, surveying your " +
						 "surroundings.\n\n" +
						 "You can FOLLOW the TUNNEL.\n";
		} else {
			text.text += "You squeeze through the crack between the collapsed walls. " +
						 "As you go, you hear something scrabbling at the opening where " +
			             "you entered. You quicken your pace as much as possible. " +
			             "The scratching noise seems to be getting closer and closer. " +
			             "You have the distinct feeling that you are not alone. " +
			             "As you reach the end of the narrow space, something grabs " +
			             "the cuff of your pants. You pull as hard as you can to get away. " +
			             "For a long moment, it seems that you are stuck fast, but then " +
			             "your pants cuff gives way with a loud rip and you stumble out into " +
			             "the open tunnel beyond the collapsed walls. As you do, you collide " +
			             "with a large timber that had been keeping the passage open. " +
			             "The impact of your fall knocks it to the ground, causing the " +
			             "space between the walls to collapse completely.\n\n" +
			             "You pick yourself up and dust yourself off, surveying your " +
			             "surroundings.\n\n" +
			             "You can FOLLOW the TUNNEL.\n";
		}
		printed = true;
		pursued = false;
	}
	
	void state_left_tunnel1() {
		text.text += "";
		printed = true;
	}

	void state_alcove0() {
		text.text += "As you enter the alcove, you nearly knock over a potted plant that you " +
		             "could not see in the dark. Serendipitously, this causes you to put out " +
		             "your hand to steady yourself against the wall, and you hit the light switch. " +
		             "The tunnel floods with light, and you are nearly blinded in the sudden brightness. " +
		             "After taking a few moments to adjust, you look around the alcove.\n\n" + 
		             "On a small table, you see some keys.\n\n" +
		             "You can TAKE the KEYS.\n" +
		             "You can RETURN to the main part of the tunnel.";
		printed = true;
	}
	
	void state_alcove1() {
		text.text += "The keys still sit on the table where they were before.\n\n" +
		             "You can TAKE the KEYs.\n" +
		             "You can RETURN to the main part of the tunnel.";
		printed = true;
	}
	
	void state_alcove2() {
		text.text += "Since you have taken the keys from the table, there doesn't appear to be " +
		             "anything else to do in the alcove.\n\n" +
		             "You can RETURN to the main part of the tunnel.";
		printed = true;
	}
	
	void state_l_tunnel0() {
		text.text += "As you follow the tunnel, it seems that the damage is less obvious. " +
		             "In fact, the only odd thing about this section of the tunnel is that " +
		             "the lights are out.\n\n" +
		             "As you walk along, your flashlight begins to dim. It seems that the " +
		             "batteries have almost reached their limit. You spot an alcove to your " +
		             "left just as the flashlight dims to nothing. You are pretty sure that " +
		             "you saw a light switch in the alcove.\n\n" +
		             "You can ENTER the ALCOVE to try to find the light switch.";
		
		printed = true;
	}
	
	void state_l_tunnel1() {
		text.text += "The alcove where you turned on the lights is to your left.\n\n" +
 					 "Now that the lights are on, you can see that there is a bathroom " +
					 "to the right. Outside of the bathroom, there is a bench. It seems " +
					 "that there is a pile of clothing on the bench." +
					 "You can ENTER the ALCOVE.\n" +
					 "You can ENTER the BATHROOM.\n" +
					 "You can APPROACH the BENCH.\n" +
					 "You can FOLLOW the TUNNEL.";
		printed = true;
	}
	
	void state_bathroom0() {
		text.text += "Inside the bathroom, it looks as though there was a fight. " + 
		             "The mirrors are cracked and broken, several of the sinks have " +
		             "large chunks taken out of them, and water seeps from under the door " +
		             "of more than one stall.\n\n" +
		             "All of the stall doors are locked shut except for one. It yawns open, " +
		             "as if beckoning you to look inside.\n\n" + 
		             "You can ENTER the STALL.\n" +
		             "You can LEAVE the bathroom.";
		printed = true;
	}

	void state_bathroom1() {
		text.text += "The bathroom remains in the same state of disarray that you found it, " +
					 "with perhaps slightly more foul-smelling water on the floor.\n\n" +
					 "You can ENTER the open STALL.\n" +
					 "You can LEAVE the bathroom.";
		printed = true;
	}

	void state_stall0() {
		text.text += "You aren't sure what you expected to find in the stall, but " +
		             "it certainly wasn't a dead security guard! He appears to have been " +
		             "beaten mercilessly. Perhaps that accounts for the state of the " +
		             "bathroom?\n\n" +
		             "In any case, he is beyond help at this point. As you examined the " +
		             "body to see if he was still breathing, you found his sidearm in its " +
		             "holster.\n\n" + 
		             "You can TAKE the GUN.\n" +
		             "You can RETURN to the bathroom.";
		printed = true;
	}
	
	void state_stall1() {
		text.text += "Now that you have taken the security guard's gun, there is " +
		             "nothing more you can do here. You offer a silent promise " +
		             "to the dead guard that you will let someone know his body " +
		             "is here.\n\n" +
		             "You can RETURN to the bathroom.";
		printed = true;
	}
	
	void state_bench0() {
		text.text += "As you approach the bench, you can tell that the pile of clothing " +
		             "is a security guard's jacket that has been balled up and tossed " +
		             "onto the bench. Rifling through its pockets for anything useful, you find " +
		             "a clip for the security guard's sidearm. You take it.\n\n" +
		             "You can RETURN to the main part of the tunnel.";
		printed = true;
		bullets = true;
	}
	
	void state_bench1() {
		text.text += "There is nothing else useful on the bench.\n\n" +
		             "You can RETURN to the main part of the tunnel.";
		printed = true;
	}
	
	void state_locked_door0() {
		text.text += "Around a bend in the tunnel, you come to a door. It looks rather imposing, " +
			         "with several large signs saying things like 'DO NOT ENTER' in various languages. " +
			         "Some signs also have helpful graphics of stick figures being variously maimed " +
			         "and otherwise abused.\n\n" +
			         "You walk up to the door in the tunnel. Trying the handle, you discover that it is " +
			         "locked, and you don't have the key. You try forcing the door, but it is far too sturdy" +
			         "for the likes of you to force it open.\n\n" +
		             "You can RETURN to the previous section of tunnel.";
		printed = true;
	}
	
	void state_locked_door1() {
		text.text += "Around a bend in the tunnel, you come to a door. You try the handle. It is locked. " +
                     "Fortunately, you found some keys that you can try in the lock. They seem to work. " +
                     "The door is now unlocked.\n\n" +
					 "You can OPEN the DOOR.\n" +
					 "You can RETURN to the previous section of tunnel.";
		printed = true;
	}
	
	void state_boss_room0() {
		//no gun, no bullets
		text.text += "As you enter the room, you are set upon by a vast number of shadows. Somehow, " +
                     "they are solid, and all of them have hard, sharp edges. After a very short " +
                     "time, you are completely overwhelmed by the attack.\n\n" +
                     "You can try to WITHSTAND the attack.";
		printed = true;
	}
	
	void state_boss_room1() {
		//gun and bullets
		text.text += "As you enter the room, you are set upon by a vast number of shadows. Thankfully, " +
                     "you found a viable weapon, and you are able to make use of the dead guard's fully " +
                     "loaded pistol. The gun feels like an extension of your arm, with every shot " +
                     "finding its target.\n\n" +
					 "You can try to WITHSTAND the attack";
		printed = true;
	}
	
	void state_boss_room2() {
		//gun, no bullets
		text.text += "As you enter the room, you are set upon by a vast number of living shadows. " +
                     "You have a moment of hope as you remember the gun that you found. You are able to " +
                     "pull out the gun and pull the trigger.\n\n" +
					 "The click of the hammer falling on an empty chamber consumes your entire world.\n\n" +
					 "You can try to WITHSTAND the attack.";
		printed = true;
	}

	void state_boss_room3() {
		//bullets, no gun
		text.text += "As you enter the room, you are set upon by a vast number of living shadows. " +
                     "Your thoughts go to the bullets in your pocket, but you don't have a gun " +
					 "to put them in. You have a moment for regret before you are overwhelmed.\n\n" +
					 "You can try to WITHSTAND the attack.";
		printed = true;
	}

	void state_death() {
		text.text += "As the onslaught ends, you lie on the ground, bleeding from numerous wounds. " +
                     "You try to get up, but find that you can't. Everything around you is hard to make " +
                     "out, as though you are viewing the world through a sheet of gauze. As you lay there, " +
                     "you realize that you are no longer breathing. You can do nothing as the life slowly " +
                     "ebbs from your body.\n\n" +
				     "YOU ARE DEAD. Sorry, you lose.\n\n" +
					 "You can type RESTART to begin the game again.";
		printed = true;
	}
	
	void state_victory() {
		text.text += "Your breathing is heavy, and you feel beaten both physically and mentally, " +
					 "but you survived. At the back of the room, a ladder leads upwards to a hatch " +
					 "that you push open after several tries. After a few moments of rest, you climb " +
  					 "out into the shining sun.\n\n" +
					 "CONGRATULATIONS! You've escaped the tunnel! No, really!\n\n" +
					 "You can type RESTART to start over.";
		printed = true;
	}
	
	void state_right_tunnel0() {
		if(pursued) {
			text.text += "As you run toward the right branch of the tunnel, the beast howls " +
			             "and charges you. It slams into the backs of your legs, sending you " +
			             "flying directly into the hanging wires. Stunned by an electrical shock, " +
			             "you don't stand a chance as the beast descends upon you.\n\n" +
						 "The sights, sounds, and smells that follow are best left undescribed.\n\n" +
						 "CONGRATULATIONS? You're... well, no longer in the tunnel, anyway.\n\n" +
						 "Type RESTART to play again.";
		} else {
			text.text += "As you approach the sparking wires, you trip on an unseen bit of " +
			             "debri, falling directly into the wires. The sights, sounds, and " +
			             "smells that follow are best left undescribed.\n\n" +
			             "CONGRATULATIONS? You're... well, no longer in the tunnel, anyway.\n\n" +
			             "Type RESTART to play again.";
		}
		printed = true;
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
		switch(current_state) {
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
