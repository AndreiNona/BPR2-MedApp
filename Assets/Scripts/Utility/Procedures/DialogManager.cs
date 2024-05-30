using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour
{
    public TMP_Text dialogText;
    public List<Button> optionButtons;
    
    string[] RequiredForBP = new string[] { "RightTopArm", "RightTopArmLow" };

    private Dictionary<int, DialogNode> dialogNodes;

    void Start()
    {
        InitializeDialogNodes();
        ShowDialogNode(0); // Start from the initial dialog node
    }
    public void ResetToDefault()
    {
        InitializeDialogNodes();
        ShowDialogNode(0);
    }
    void OnDisable()
    {
        ResetToDefault();
    }
    //TODO: Replace manager with singleton
   public void InitializeDialogNodes()
    {
        dialogNodes = new Dictionary<int, DialogNode>
        {
            { 0, new DialogNode("We should record the blood pressure for this patient", new Dictionary<string, Action>
                {
                    { "Clean equipment", () =>
                        {
                            GameManager.instance.PreparePatient("PrepTools");
                            GameManager.instance.isRoomLocked=true;
                        }
                    },
                    { "They are fine, send them away", () => ShowDialogNode(0) },
                    { "Ask the patient to sit down", () =>
                        {
                            FindObjectOfType<GameManager>().PatientPosture("Sit");
                            ShowDialogNode(1);
                            GameManager.instance.isRoomLocked=true;
                        }
                    },
                    { "Make small talk", () => ShowDialogNode(0) }
                })
            },
            { 1, new DialogNode("The patient sits down. What's next?", new Dictionary<string, Action>
                {
                    { "Ask for good posture", () => {
                        GameManager.instance.PatientPosture("GoodPosture");
                        GameManager.instance.PatientPosture("GoodPosture");
                        GameManager.instance.PreparePatient("PreparePatient");
                    }},
                    { "Make small talk", () => ShowDialogNode(1) },
                    { "Ask to stand up", () => {
                        GameManager.instance.PatientPosture("Up");
                        ShowDialogNode(2);
                    }},
                    { "Ask about what they ate", () => ShowDialogNode(1) }
                })
            },
            { 2, new DialogNode("The patient is up. What's next?", new Dictionary<string, Action>
                {
                    { "Ask the patient to sit down", () => {
                        GameManager.instance.PatientPosture("Sit");
                        ShowDialogNode(1);
                    }},
                    { "PLACEHOLDER_1", () => ShowDialogNode(2) },
                    { "PLACEHOLDER_2", () => ShowDialogNode(2) },
                    { "PLACEHOLDER_3", () => ShowDialogNode(2) }
                })
            },
            { 3, new DialogNode("Looks like some tools are dirty", new Dictionary<string, Action>
                {
                    { "Continue when they are clean", () => GameManager.instance.PreparePatient("PrepTools") },
                    { "PLACEHOLDER_1", () => ShowDialogNode(3) },
                    { "PLACEHOLDER_2", () => ShowDialogNode(3) },
                    { "PLACEHOLDER_3", () => ShowDialogNode(3) },
                })
            },
            { 4, new DialogNode("Our tools are properly sanitized. What's next?", new Dictionary<string, Action>
                {
                    { "Ask for good posture", () => {
                        GameManager.instance.PatientPosture("Sit");
                        ShowDialogNode(1);
                    }},
                    { "Ask the patient to lay down", () => ShowDialogNode(1) },
                    { "Talk about the weather", () => ShowDialogNode(1) }
                })
            },
            { 5, new DialogNode("The tools are prepared, but our patient isn't!", new Dictionary<string, Action>
                {
                    { "Ask the patient to sit down", () =>
                        {
                            GameManager.instance.PatientPosture("Sit");
                            ShowDialogNode(1);
                        }
                    },
                    { "PLACEHOLDER_1", () => ShowDialogNode(4) },
                    { "PLACEHOLDER_2", () => ShowDialogNode(4) },
                    { "PLACEHOLDER_3", () => ShowDialogNode(4) },
                })
            },
            { 6, new DialogNode("Our patient is ready, but our tools aren't!", new Dictionary<string, Action>
                {
                    { "Clean equipment", () => GameManager.instance.PreparePatient("PrepTools") },
                    { "PLACEHOLDER_1", () => ShowDialogNode(4) },
                    { "PLACEHOLDER_2", () => ShowDialogNode(4) },
                    { "PLACEHOLDER_3", () => ShowDialogNode(4) },
                })
            },
            { 7, new DialogNode("Our prep is done. What to do next?", new Dictionary<string, Action>
                {
                    { "PLACEHOLDER_1", () => ShowDialogNode(7) },
                    { "PLACEHOLDER_2", () => ShowDialogNode(7) },
                    { "PLACEHOLDER_3", () => ShowDialogNode(7) },
                    { "Place cuff", () => GameManager.instance.ManageAttachable("RightTopArm",true,8,9,10) }
                })
            },
            { 8, new DialogNode("Cuff placed on the patient. What's next?", new Dictionary<string, Action>
                {
                    { "Continue examination", () => ShowDialogNode(12) },
                    { "We are done", () => ShowDialogNode(12) },
                    { "PLACEHOLDER_1", () => ShowDialogNode(12) },
                    { "Remove the cuff", () => GameManager.instance.ManageAttachable("RightTopArm",false,9,8,10) }
                })
            },           
            { 9, new DialogNode("Cuff has been removed. What's next?", new Dictionary<string, Action>
                {
                    { "PLACEHOLDER_1", () => ShowDialogNode(9) },
                    { "Place cuff", () => GameManager.instance.ManageAttachable("RightTopArm",true,8,9,10) }
                })
            },
            { 10, new DialogNode("Cuff has been placed incorrectly. Try again", new Dictionary<string, Action>
                {
                    { "Place cuff", () => GameManager.instance.ManageAttachable("RightTopArm",true,8,9,10) }
                })
            },
            { 11, new DialogNode("You have no equipment to prepare,", new Dictionary<string, Action>
                {
                    { "Clean equipment", () => GameManager.instance.PreparePatient("PrepTools") }
                })
            },
            { 12, new DialogNode("Cuff has been properly placed.", new Dictionary<string, Action>
                {
                    { "Place stethoscope head onto the patient", () => GameManager.instance.ManageAttachable("RightTopArmLow",true,13,12,23) },
                    { "Wait", () => ShowDialogNode(12)  }
                })
            },
            { 13, new DialogNode("Stethoscope has been properly placed.", new Dictionary<string, Action>
                {
                    { "Raise pressure", () => ShowDialogNode(14) },
                    { "INCORRECT_A_1", () => ShowDialogNode(13) },
                    { "INCORRECT_A_2", () => ShowDialogNode(13) },
                    { "INCORRECT_A_3", () => ShowDialogNode(13) }
                })
            },
            { 14, new DialogNode("What pressure do we raise to?", new Dictionary<string, Action>
                {
                    { "Obvious answer", () => ShowDialogNode(15) },
                    { "INCORRECT_A_1", () => ShowDialogNode(14) },
                    { "INCORRECT_A_2", () => ShowDialogNode(14) },
                    { "INCORRECT_A_3", () => ShowDialogNode(14) }
                })
            },
            { 15, new DialogNode("Pressure raised to 'Obvious answer'", new Dictionary<string, Action>
                {
                    { "Let air out ", () => ShowDialogNode(16) },
                    { "INCORRECT_A_1", () => ShowDialogNode(15) },
                    { "INCORRECT_A_2", () => ShowDialogNode(15) },
                    { "INCORRECT_A_3", () => ShowDialogNode(15) }
                })
            },
            { 16, new DialogNode("Letting air out, what do we record?", new Dictionary<string, Action>
                {
                    { "Record pressure on first pulse", () =>
                    {
                        GameManager.instance.GeneratePatientVitals(RequiredForBP);
                        ShowDialogNode(17);
                    }   },
                    { "INCORRECT_A_1", () => ShowDialogNode(16) },
                    { "INCORRECT_A_2", () => ShowDialogNode(16) },
                    { "INCORRECT_A_3", () => ShowDialogNode(16) }
                })
            },
            { 17, new DialogNode("Correct. Letting air out, what do we record?", new Dictionary<string, Action>
                {
                    { "Record diastolic on last pulse", () =>
                    {
                        GameManager.instance.GeneratePatientVitals(RequiredForBP);
                        ShowDialogNode(18);
                    }   },
                    { "INCORRECT_A_1", () => ShowDialogNode(17) },
                    { "INCORRECT_A_2", () => ShowDialogNode(17) },
                    { "INCORRECT_A_3", () => ShowDialogNode(17) }
                })
            },
            { 18, new DialogNode("Correct. What would a good rounding be? ", new Dictionary<string, Action>
                {
                    { "The nearest 2mmHG", () =>
                    {
                        GameManager.instance.GeneratePatientVitals(RequiredForBP);
                        ShowDialogNode(19);
                    }   },
                    { "INCORRECT_A_1", () => ShowDialogNode(18) },
                    { "INCORRECT_A_2", () => ShowDialogNode(18) },
                    { "INCORRECT_A_3", () => ShowDialogNode(18) }
                })
            },
            { 19, new DialogNode("Correct. What are the last measuring steps? ", new Dictionary<string, Action>
                {
                    { "Let remaining air out", () =>
                    {
                        GameManager.instance.GeneratePatientVitals(RequiredForBP);
                        ShowDialogNode(20);
                    }   },
                    { "INCORRECT_A_1", () => ShowDialogNode(19) },
                    { "INCORRECT_A_2", () => ShowDialogNode(19) },
                    { "INCORRECT_A_3", () => ShowDialogNode(19) }
                })
            },
            { 20, new DialogNode("Correct again. What are the last measuring steps? ", new Dictionary<string, Action>
                {
                    { "Note the pressure", () =>
                    {
                        ShowDialogNode(21);
                    }   },
                    { "INCORRECT_A_1", () => ShowDialogNode(20) },
                    { "INCORRECT_A_2", () => ShowDialogNode(20) },
                    { "INCORRECT_A_3", () => ShowDialogNode(20) }
                })
            },
            { 21, new DialogNode("You have successfully preformed the procedure. What follows? ", new Dictionary<string, Action>
                {
                    { "Preform hand hygiene", () =>
                    {
                        GameManager.instance.UnlockAllAttachable();
                        GameManager.instance.isRoomLocked=false;
                        ShowDialogNode(22);
                    }   },
                    { "INCORRECT_A_1", () => ShowDialogNode(21) },
                    { "INCORRECT_A_2", () => ShowDialogNode(21) },
                    { "INCORRECT_A_3", () => ShowDialogNode(21) }
                })
            },
            { 22, new DialogNode("Well done! Feel free to browse other procedures", new Dictionary<string, Action>
                {
                    // Further actions
                })
            },            
            { 23, new DialogNode("Stethoscope has been placed incorrectly. Try again", new Dictionary<string, Action>
                {
                    { "Place stethoscope", () => GameManager.instance.ManageAttachable("RightTopArmLow",true,13,12,23) }
                })
            }
        };
    }

   public void ShowDialogNode(int nodeIndex)
    {
        var node = dialogNodes[nodeIndex];
        dialogText.text = node.Text;

        int optionIndex = 0;
        foreach (var option in node.Options)
        {
            if (optionButtons[optionIndex] != null && optionButtons[optionIndex].gameObject != null) {
                optionButtons[optionIndex].gameObject.SetActive(true);
                optionButtons[optionIndex].GetComponentInChildren<TMP_Text>().text = option.Key;
                Action action = option.Value;

                optionButtons[optionIndex].onClick.RemoveAllListeners();
                optionButtons[optionIndex].onClick.AddListener(() => action.Invoke());
            }
            optionIndex++;
        }

        // Hide unused buttons
        for (; optionIndex < optionButtons.Count; optionIndex++)
        {
            optionButtons[optionIndex].gameObject.SetActive(false);
        }
    }
}

public class DialogNode
{
    public string Text { get; private set; }
    public Dictionary<string, Action> Options { get; private set; }

    public DialogNode(string text, Dictionary<string, Action> options)
    {
        Text = text;
        Options = options;
    }
}
