/* This is an example to show how to connect to 2 HM-10 devices
 * that are connected together via their serial pins and send data
 * back and forth between them.
 */

using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Text;
using System;
using UnityEngine.VR;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

public class ArduinoHM10Test : MonoBehaviour
{
    public string DeviceName = "Wahoo SPEED";
    public string ServiceUUID = "1816";
    public string Characteristic = "2A5B";

    public Text statusText;
    public Text BluetoothStatus;
    public GameObject PanelMiddle;
    public Text TextToSend;
    public PlayerController player;

    private bool mutexLock = false;
    private int bleCount = 1;

    enum States
    {
        None,
        Scan,
        Connect,
        Subscribe,
        Unsubscribe,
        Disconnect,
        Communication,
    }

    private bool _workingFoundDevice = true;
    private bool _connected = false;
    private float _timeout = 0f;
    private States _state = States.None;
    private bool _foundID = false;
    private String lastInp = "";

    // this is our hm10 device
    private string _hm10;

    public void OnButton(Button button)
    {
        if (button.name.Contains("Send"))
        {
            if (string.IsNullOrEmpty(TextToSend.text))
            {
                //BluetoothStatus.text = "Enter text to send...";
            }
            else
            {
                SendString(TextToSend.text);
            }
        }
        else if (button.name.Contains("Toggle"))
        {
            SendByte(0x01);
        }
    }

    void Reset()
    {
        _workingFoundDevice = false;    // used to guard against trying to connect to a second device while still connecting to the first
        _connected = false;
        _timeout = 0f;
        _state = States.None;
        _foundID = false;
        _hm10 = null;
        //PanelMiddle.SetActive(false);
    }

    void SetState(States newState, float timeout)
    {
        _state = newState;
        _timeout = timeout;
    }

    void StartProcess()
    {
        //        BluetoothStatus.text = "Initializing...";
        Debug.Log("Start Process");

        Reset();
        BluetoothLEHardwareInterface.Initialize(true, false, () =>
        {

            SetState(States.Scan, 0.1f);
            //BluetoothStatus.text = "Initialized";

        }, (error) =>
        {

            BluetoothLEHardwareInterface.Log("Error: " + error);
        });
    }

    // Use this for initialization
    void Start()
    {
        Debug.Log("Start");
 
        StartProcess();
    }

    void Awake()
    {
        //HM10_Status.text = "";
        Debug.Log("Start BLE");
       // StartProcess();
        DontDestroyOnLoad(this.gameObject);
    }

    // Update is called once per frame
 


    void Update()
    {


        Debug.Log("BLE Still Exist");
        if (_timeout > 0f)
        {
            _timeout -= Time.deltaTime;
            if (_timeout <= 0f)
            {
                _timeout = 0f;

                switch (_state)
                {
                    case States.None:
                        break;

                    case States.Scan:

                        statusText = GameObject.Find("Status Text").GetComponent<Text>();
                        statusText.text = "Scanning";
                        //BluetoothStatus.text = "Scanning for Wahoo SPEED devices...";
                        Debug.Log("Scanning");
                        BluetoothLEHardwareInterface.ScanForPeripheralsWithServices(null, (address, name) =>
                        {

                            // we only want to look at devices that have the name we are looking for
                            // this is the best way to filter out devices

                            DeviceName = "Wahoo SPEED";
                            Debug.Log("Name:" + DeviceName);
                            Debug.Log("NameName:" + name);
                        if (name.Contains(DeviceName) && bleCount <= 3 )
                            {
                                _workingFoundDevice = true;
                                statusText.text = "Found " + name;
                                // it is always a good idea to stop scanning while you connect to a device
                                // and get things set up
                                //BluetoothStatus.text = "";

                                // add it to the list and set to connect to it
                                _hm10 = address;

                                Debug.Log(_hm10);

                                //HM10_Status.text = "Found Wahoo SPEED";
                                GameObject button = GameObject.Find("BLE"+bleCount);
                                Button butt = button.GetComponent<Button>();
                                butt.onClick.AddListener(delegate { Connect(address); });
                                button.GetComponentInChildren<Text>().text = name;
                                bleCount++;

                                //SetState(States.Connect, 0.5f);

                                _workingFoundDevice = false;
                            }

                        }, null, false, false);
                        break;

                    case States.Connect:
                        // set these flags
                        _foundID = false;
                        statusText.text = "Connecting";
                        //HM10_Status.text = "Connecting to Wahoo SPEED";
                        Debug.Log("Beep Boop Connect mode Avtivated!!");
                        // note that the first parameter is the address, not the name. I have not fixed this because
                        // of backwards compatiblity.
                        // also note that I am note using the first 2 callbacks. If you are not looking for specific characteristics you can use one of
                        // the first 2, but keep in mind that the device will enumerate everything and so you will want to have a timeout
                        // large enough that it will be finished enumerating before you try to subscribe or do any other operations.
                        BluetoothLEHardwareInterface.ConnectToPeripheral(_hm10, (name) =>
                        {
                            Debug.Log("Connected to " + name);
                        }, (str1, str2) =>
                        {
                            Debug.Log("What this 1? :" + str1);
                            Debug.Log("What this 2? :" + str2);
                        }, (address, serviceUUID, characteristicUUID) =>
                        {
                            ServiceUUID = "1816";
                            Characteristic = "2A5B";
                            Debug.Log("Service" + ServiceUUID);
                            Debug.Log("CharacteristicUUID" + Characteristic);
                            if (IsEqual(serviceUUID, ServiceUUID))
                            {
                                // if we have found the characteristic that we are waiting for
                                // set the state. make sure there is enough timeout that if the
                                // device is still enumerating other characteristics it finishes
                                // before we try to subscribe
                                if (IsEqual(characteristicUUID, Characteristic))
                                {
                                    _connected = true;
                                    SetState(States.Subscribe, 2f);
                                    statusText.text = "Connected";

                                   // HM10_Status.text = "Connected to Wahoo SPEED";
                                }
                            }
                        }, (disconnectedAddress) =>
                        {
                            BluetoothLEHardwareInterface.Log("Device disconnected: " + disconnectedAddress);
                           // HM10_Status.text = "Disconnected";
                        });
                        break;

                    case States.Subscribe:
                        //HM10_Status.text = "Subscribing to Wahoo SPEED CSC service";
                        statusText.text = "Subscribing";
                        Debug.Log("BLE Working");
                        BluetoothLEHardwareInterface.SubscribeCharacteristicWithDeviceAddress(_hm10, ServiceUUID, Characteristic, null, (address, characteristicUUID, bytes) =>
                        {

                                String inp = BitConverter.ToString(bytes);
                                Debug.Log(inp);
                                Debug.Log("Active Scene" + SceneManager.GetActiveScene().name);
                                //HM10_Status.text = BitConverter.ToString(bytes);
                                if (SceneManager.GetActiveScene().name == "ArduinoHM10Test")
                                {
                                    //GetComponent<BezierWalker>().enabled = false;
                                    SceneManager.LoadScene("Start");
                                    print("Load Start");


                                }

                                

                                if (SceneManager.GetActiveScene().name == "Game")
                                {
                                    if (inp != lastInp)
                                    {
                                        player.SetSpeed(1f);
                                        lastInp = inp;
                                    }
                                    else
                                    {
                                        player.SetSpeed(0f);
                                    }


                                }
                               




                        });

                        // set to the none state and the user can start sending and receiving data
                        //_state = States.None;
                        //HM10_Status.text = "Waiting...";

                        //PanelMiddle.SetActive(true);
                        break;

                    case States.Unsubscribe:
                        BluetoothLEHardwareInterface.UnSubscribeCharacteristic(_hm10, ServiceUUID, Characteristic, null);
                        SetState(States.Disconnect, 4f);
                        break;

                    case States.Disconnect:
                        if (_connected)
                        {
                            BluetoothLEHardwareInterface.DisconnectPeripheral(_hm10, (address) =>
                            {
                                BluetoothLEHardwareInterface.DeInitialize(() =>
                                {

                                    _connected = false;
                                    _state = States.None;
                                });
                            });
                        }
                        else
                        {
                            BluetoothLEHardwareInterface.DeInitialize(() =>
                            {

                                _state = States.None;
                            });
                        }
                        break;
                }
            }
        }
    }

    string FullUUID(string uuid)
    {
        return "0000" + uuid + "-0000-1000-8000-00805F9B34FB";
    }

    bool IsEqual(string uuid1, string uuid2)
    {
        if (uuid1.Length == 4)
            uuid1 = FullUUID(uuid1);
        if (uuid2.Length == 4)
            uuid2 = FullUUID(uuid2);

        return (uuid1.ToUpper().Equals(uuid2.ToUpper()));
    }

    void SendString(string value)
    {
        var data = Encoding.UTF8.GetBytes(value);
        // notice that the 6th parameter is false. this is because the HM10 doesn't support withResponse writing to its characteristic.
        // some devices do support this setting and it is prefered when they do so that you can know for sure the data was received by 
        // the device
        BluetoothLEHardwareInterface.WriteCharacteristic(_hm10, ServiceUUID, Characteristic, data, data.Length, false, (characteristicUUID) =>
        {

            BluetoothLEHardwareInterface.Log("Write Succeeded");
        });
    }

    void SendByte(byte value)
    {
        byte[] data = new byte[] { value };
        // notice that the 6th parameter is false. this is because the HM10 doesn't support withResponse writing to its characteristic.
        // some devices do support this setting and it is prefered when they do so that you can know for sure the data was received by 
        // the device
        BluetoothLEHardwareInterface.WriteCharacteristic(_hm10, ServiceUUID, Characteristic, data, data.Length, false, (characteristicUUID) =>
        {

            BluetoothLEHardwareInterface.Log("Write Succeeded");
        });
    }

    void Connect(string addr)
    {
        _hm10 = addr;
        BluetoothLEHardwareInterface.StopScan();
        SetState(States.Connect, 0.5f);

    }
}
