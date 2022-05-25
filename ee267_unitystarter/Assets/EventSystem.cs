using Leap.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class EventSystem : MonoBehaviour
{
    public GameObject numWrongObject;
    private int numWrong = 0;

    public GameObject numRightObject;
    private int numRight = 0;

    public GameObject letterObject;
    private string receivedLetter = "";

    public LeapServiceProvider lp;

    public GameObject quad;
    public Material baseBg;
    public Material subBg;

    private const string validChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    private const KeyCode ACTION = KeyCode.A;

    enum State
    {
        Idle = 0,
        Sending = 1,
        Processing = 2,
        PostSubmit = 3,
    }

    State state = State.Idle;

    /**
     * Generate a random new character to text the user on!
     */
    private string GenerateNextTurn()
    {
        int index = (int)(Random.value * (validChars.Length + 1));
        return "" + validChars[index];
    }
    
    /**
     * Use Course Notes 3 appendix to convert a quaternion back into Euler 
     * angles (through rotation matrix elements) to pass into model.
     */
    private Vector3 RotationVecFromQuat(Leap.LeapQuaternion q)
    {
        Vector3 vec = new Vector3();
        vec[0] = Mathf.Asin(2 * q.y * q.z + 2 * q.w * q.x);
        vec[1] = Mathf.Atan2(-2 * q.x * q.z + 2 * q.w * q.y, q.w * q.w - q.x * q.x - q.y * q.y + q.z * q.z);
        vec[2] = Mathf.Atan2(-2 * q.x * q.y + 2 * q.w * q.z, q.w * q.w - q.x * q.x + q.y * q.y - q.z * q.z);
        return vec;
    }

    /**
     * Generate JSON body for REST POST request that will be sent to the classifier
     * web server.
     */
     
    private string CreateFormFromFrame(Leap.Frame f)
    {
        string jsonText = "{\"data\":{";
        // initial fields
        jsonText += "\"id\": " + 0 + ",\n";
        jsonText += "\"label\": " + 0 + ",\n";

        // hand rotation fields
        Vector3 handDir = RotationVecFromQuat(f.Hands[0].Rotation);
        jsonText += "\"hand_dir_1\": " + handDir[0] + ",\n";
        jsonText += "\"hand_dir_2\": " + handDir[1] + ",\n";
        jsonText += "\"hand_dir_3\": " + handDir[2] + ",\n";

        // thumb rotation and bone position fields
        string[] fingers = { "thumb", "index", "middle", "ring", "pinkie" };
        for (int i = 0; i < 5; i++)
        {
            // for each finger,
            Leap.Finger fing;
            if (i == 0)
            {
                fing = f.Hands[0].GetThumb();
            }
            else if (i == 1)
            {
                fing = f.Hands[0].GetIndex();
            }
            else if (i == 2)
            {
                fing = f.Hands[0].GetMiddle();
            }
            else if (i == 3)
            {
                fing = f.Hands[0].GetRing();
            }
            else
            {
                fing = f.Hands[0].GetPinky();
            }

            // append bones and directions to json
            jsonText += "\"" + fingers[i] + "_dir_1\": " + fing.Direction[0] + ",\n";
            jsonText += "\"" + fingers[i] + "_dir_2\": " + fing.Direction[1] + ",\n";
            jsonText += "\"" + fingers[i] + "_dir_3\": " + fing.Direction[2] + ",\n";
            jsonText += "\"" + fingers[i] + "_meta_center_1\": " + fing.Bone(Leap.Bone.BoneType.TYPE_METACARPAL).Center[0] + ",\n";
            jsonText += "\"" + fingers[i] + "_meta_center_2\": " + fing.Bone(Leap.Bone.BoneType.TYPE_METACARPAL).Center[1] + ",\n";
            jsonText += "\"" + fingers[i] + "_meta_center_3\": " + fing.Bone(Leap.Bone.BoneType.TYPE_METACARPAL).Center[2] + ",\n";
            jsonText += "\"" + fingers[i] + "_meta_dir_1\": " + fing.Bone(Leap.Bone.BoneType.TYPE_METACARPAL).Direction[0] + ",\n";
            jsonText += "\"" + fingers[i] + "_meta_dir_2\": " + fing.Bone(Leap.Bone.BoneType.TYPE_METACARPAL).Direction[1] + ",\n";
            jsonText += "\"" + fingers[i] + "_meta_dir_3\": " + fing.Bone(Leap.Bone.BoneType.TYPE_METACARPAL).Direction[2] + ",\n";
            jsonText += "\"" + fingers[i] + "_prox_center_1\": " + fing.Bone(Leap.Bone.BoneType.TYPE_PROXIMAL).Center[0] + ",\n";
            jsonText += "\"" + fingers[i] + "_prox_center_2\": " + fing.Bone(Leap.Bone.BoneType.TYPE_PROXIMAL).Center[1] + ",\n";
            jsonText += "\"" + fingers[i] + "_prox_center_3\": " + fing.Bone(Leap.Bone.BoneType.TYPE_PROXIMAL).Center[2] + ",\n";
            jsonText += "\"" + fingers[i] + "_prox_dir_1\": " + fing.Bone(Leap.Bone.BoneType.TYPE_PROXIMAL).Direction[0] + ",\n";
            jsonText += "\"" + fingers[i] + "_prox_dir_2\": " + fing.Bone(Leap.Bone.BoneType.TYPE_PROXIMAL).Direction[1] + ",\n";
            jsonText += "\"" + fingers[i] + "_prox_dir_3\": " + fing.Bone(Leap.Bone.BoneType.TYPE_PROXIMAL).Direction[2] + ",\n";
            jsonText += "\"" + fingers[i] + "_int_center_1\": " + fing.Bone(Leap.Bone.BoneType.TYPE_INTERMEDIATE).Center[0] + ",\n";
            jsonText += "\"" + fingers[i] + "_int_center_2\": " + fing.Bone(Leap.Bone.BoneType.TYPE_INTERMEDIATE).Center[1] + ",\n";
            jsonText += "\"" + fingers[i] + "_int_center_3\": " + fing.Bone(Leap.Bone.BoneType.TYPE_INTERMEDIATE).Center[2] + ",\n";
            jsonText += "\"" + fingers[i] + "_int_dir_1\": " + fing.Bone(Leap.Bone.BoneType.TYPE_INTERMEDIATE).Direction[0] + ",\n";
            jsonText += "\"" + fingers[i] + "_int_dir_2\": " + fing.Bone(Leap.Bone.BoneType.TYPE_INTERMEDIATE).Direction[1] + ",\n";
            jsonText += "\"" + fingers[i] + "_int_dir_3\": " + fing.Bone(Leap.Bone.BoneType.TYPE_INTERMEDIATE).Direction[2] + ",\n";
            jsonText += "\"" + fingers[i] + "_dist_center_1\": " + fing.Bone(Leap.Bone.BoneType.TYPE_DISTAL).Center[0] + ",\n";
            jsonText += "\"" + fingers[i] + "_dist_center_2\": " + fing.Bone(Leap.Bone.BoneType.TYPE_DISTAL).Center[1] + ",\n";
            jsonText += "\"" + fingers[i] + "_dist_center_3\": " + fing.Bone(Leap.Bone.BoneType.TYPE_DISTAL).Center[2] + ",\n";
            jsonText += "\"" + fingers[i] + "_dist_dir_1\": " + fing.Bone(Leap.Bone.BoneType.TYPE_DISTAL).Direction[0] + ",\n";
            jsonText += "\"" + fingers[i] + "_dist_dir_2\": " + fing.Bone(Leap.Bone.BoneType.TYPE_DISTAL).Direction[1] + ",\n";
            jsonText += "\"" + fingers[i] + "_dist_dir_3\": " + fing.Bone(Leap.Bone.BoneType.TYPE_DISTAL).Direction[2] + ",\n";
            jsonText += "\"" + fingers[i] + "_tip_pos_1\": " + fing.TipPosition[0] + ",\n";
            jsonText += "\"" + fingers[i] + "_tip_pos_2\": " + fing.TipPosition[1] + ",\n";
            jsonText += "\"" + fingers[i] + "_tip_pos_3\": " + fing.TipPosition[2] + ",\n";
        }

        // close off syntax and return
        jsonText += "},\n}";
        return jsonText;
    }

    private IEnumerator MakeRequest()
    {
        Debug.Log("Making Request");
        Debug.Log(lp.CurrentFrame.Hands[0]);
        string form = CreateFormFromFrame(lp.CurrentFrame);

        UnityWebRequest www = UnityWebRequest.Post("http://127.0.0.1:5000/classify", form);
        www.SetRequestHeader("Content-Type", "application/json");
        www.downloadHandler = new DownloadHandlerBuffer();

        yield return www.SendWebRequest();

        if (www.responseCode != 200)
        {
            Debug.Log("something went wrong with API call");

            state = State.Idle;
        }
        else
        {
            Debug.Log("Successfully obtained classifier prediction!");
            receivedLetter = www.downloadHandler.text;
            state = State.Processing;
        }
        state = State.Processing;

    }

    // Update is called once per frame
    void Update()
    {
        // FSM
        if (state == State.Idle)
        {
            if (Input.GetKeyDown(ACTION))
            {
                state = State.Sending;
                StartCoroutine(MakeRequest());
            }
        }
        // only drops into this state once request coroutine has finished;
        // only stays in this state for 1 frame.
        else if (state == State.Processing)
        {
            Debug.Log("Entering Processing State");
            // set background
            quad.GetComponent<MeshRenderer>().material = subBg;

            // update numRight and numWrong
            if (receivedLetter == letterObject.GetComponent<TextMesh>().text)
            {
                numRight++;
                numRightObject.GetComponent<TextMesh>().text = "" + numRight;
            }
            else
            {
                numWrong++;
                numWrongObject.GetComponent<TextMesh>().text = "" + numWrong;
            }

            // set letter
            letterObject.GetComponent<TextMesh>().text = receivedLetter.ToUpper();
            receivedLetter = "";

            // set state
            state = State.PostSubmit;
        }
        // post-submission reflection screen.
        else if (state == State.PostSubmit) {
            // cycle back to idle once the user hits continue!
            Debug.Log("Entering Post Submit State");
            if (Input.GetKeyDown(ACTION))
            {
                // reset background
                quad.GetComponent<MeshRenderer>().material = baseBg;

                // obtain new letter
                string next = GenerateNextTurn();
                letterObject.GetComponent<TextMesh>().text = next;

                // set state
                state = State.Idle;
            }
        }
    }
}
